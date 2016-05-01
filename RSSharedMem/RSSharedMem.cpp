#include <opencv2/core/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <sys/mman.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <unistd.h>
#include <stdio.h>
#include <errno.h>
#include <semaphore.h>

using namespace cv;

typedef struct
{
	int fd;
	sem_t *sem;
	cv::Size size;
	int type;
	int data_size;
	void *data;
} SharedImageHeader;

#define LEFT_CAM_NAME "robosub_left"
#define RIGHT_CAM_NAME "robosub_right"

extern "C"
{
    bool initialized[2] = {false, false};
    int fd[2] = {0, 0};
    //Ptr<SharedImageHeader> sim = new SharedImageHeader();
    SharedImageHeader *sim[2];
    void *mem[2];

    void DeinitSharedMemory(int cam_id)
    {
        if(cam_id == 0)
        {
            shm_unlink(LEFT_CAM_NAME);
            sem_unlink(LEFT_CAM_NAME);
            delete sim[0];
        }
        else
        {
            shm_unlink(RIGHT_CAM_NAME);
            sem_unlink(RIGHT_CAM_NAME);
            delete sim[1];
        }
    }

    // This is not used but it was nice for debugging and it won't harm anything
    // to leave it in
    int ShowImage(int cam_id, unsigned char *img_bytes, int img_bytes_len)
    {
        std::vector<char> data(img_bytes, img_bytes + img_bytes_len);
        Mat img_data(data);
        Mat img = imdecode(img_data, CV_LOAD_IMAGE_COLOR);

        if(cam_id == 0)
            imshow(LEFT_CAM_NAME, img);
        else
            imshow(RIGHT_CAM_NAME, img);

        cv::waitKey(20);

        return 0;
    }

    int InitSharedMemory(int cam_id, int data_size, Mat img)
    {
        char name[32] = "/";
        if(cam_id == 0)
            strcat(name, LEFT_CAM_NAME);
        else if(cam_id == 1)
            strcat(name, RIGHT_CAM_NAME);

        sim[cam_id] = new SharedImageHeader();

        // create semaphore
        sem_t *sem = sem_open(name, O_CREAT, S_IRWXU, 1);
        if(sem <= 0)
        {
            return -1;
            //ERROR(strerror(errno));
            //EXIT("Failed to create sem for object: " + cam);
        }
        sim[cam_id]->sem = sem;

        // Create shared image header
        fd[cam_id] = shm_open(name, O_RDWR | O_CREAT, S_IRWXU);
        if (fd <= 0)
        {
            printf("Failed to create shm\n");
            return -1;
        }

        sim[cam_id]->fd = fd[cam_id];
        sim[cam_id]->size = img.size();
        sim[cam_id]->type = img.type();
        sim[cam_id]->data_size = data_size;

        // Allocate memory for image
        int total_size = sizeof(SharedImageHeader) + data_size;
        ftruncate(fd[cam_id], total_size);

        // Put frame into memory
        mem[cam_id] = mmap(0, total_size, PROT_READ | PROT_WRITE, MAP_SHARED, fd[cam_id], 0); // OS auto creates buffer
        if (mem[cam_id] <= 0)
        {
            printf("Failed to allocate memory for image\n");
            return -1;
        }

        initialized[cam_id] = true;

        return 0;
    }

    int PlacePNGInSharedMemory(int cam_id, unsigned char *img_bytes, int img_bytes_len)
    {
        // Copy array of bytes (as png or other image format) to mat array
        Mat bytes_mat = Mat(1, img_bytes_len, CV_8UC1, img_bytes);
        // Create mat from encoded bytes
        Mat img = imdecode(Mat(bytes_mat), 1);

        unsigned long data_size = img.total() * img.elemSize();

        if(!initialized[cam_id])
        {
            int retval = InitSharedMemory(cam_id, data_size, img);
            if(retval == -1)
                return -1;
        }

        sim[cam_id]->data = (char*)mem[cam_id] + sizeof(SharedImageHeader);
        sem_wait(sim[cam_id]->sem); // Lock memory
        memcpy(mem[cam_id], sim[cam_id], sizeof(SharedImageHeader)); // Copy Image Header
        memcpy(sim[cam_id]->data, img.data, data_size); // Copy Data
        sem_post(sim[cam_id]->sem); // Unlock memory

        return 0;
    }
}
