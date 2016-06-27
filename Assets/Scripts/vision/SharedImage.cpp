#include "SharedImage.h"

extern "C"
{
    int GetID(char *name)
    {
        for(int i = 0; i < NCAMERAS; ++i)
        {
            if(strcmp(name, names[i]) == 0)
                return ids[i];
            return -1;
        }
    }

    int GetInit(char *name)
    {
        return init[GetID(name)];
    }

    SharedImageHeader* GetHeader(char *name)
    {
        return headers[GetID(name)];
    }

    void ShowImage(char *name, int rows, int cols, unsigned char *buf)
    {
        Mat image(rows, cols, CV_8UC3, buf);
        imshow(name, image);
        cv::waitKey(1);
    }

    int InitShared(char *name, int width, int height, unsigned char *buf)
    {
        if(GetInit(name) == 0) // does not exist
        {
            // create shared image header
            unsigned long data_size = width * height * 3; // 3 channel
            std::string header_name = std::string(PREFIX) + name;
            int fd = shm_open(header_name.c_str(), O_RDWR | O_CREAT, S_IRWXU);
            if(fd <= 0)
                return -1;
            Ptr<SharedImageHeader> header = new SharedImageHeader();
            header->fd = fd;
            header->size = cv::Size(height, width);
            header->type = TYPE;
            header->data_size = data_size;

            // create semaphore
            sem_t *sem = sem_open(header_name.c_str(), O_CREAT, S_IRWXU, 1);
            if(sem <= 0)
                return -2;
            header->sem = sem;

            // allocate memory for image
            int total_size = sizeof(SharedImageHeader) + data_size;
            ftruncate(fd, total_size);

            // put first frame into memory
            void *mem = mmap(0, total_size, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0); // let os create buffer
            if(mem <= 0)
                return -3;
            header->data = (char*)mem + sizeof(SharedImageHeader); // set sim data segment to data segment of mem
            sem_wait(header->sem); // lock memory
            memcpy(mem, header, sizeof(SharedImageHeader)); // copy header
            memcpy(header->data, buf, data_size);    // copy data
            sem_post(header->sem); // unlock memory

            // add header
            headers[GetID(name)] = header;

            // initalized
            init[GetID(name)] = 1;
        }

        // write to buffer
        return UpdateShared(name, width, height, buf);
    }

    int UpdateShared(char *name, int width, int height, unsigned char *buf)
    {
        if(GetInit(name) == 0) // does not exist
            return InitShared(name, width, height, buf); // create it
        sem_wait(headers[GetID(name)]->sem); // lock memory
        memcpy(headers[GetID(name)]->data, buf, headers[GetID(name)]->data_size); // write
        sem_post(headers[GetID(name)]->sem); // unlock memory
        return 0;
    }

    int ShutdownShared(char *name)
    {
        if(GetInit(name) == 0) // does not exist
            return -1;

        // delete sem
        sem_close(headers[GetID(name)]->sem);
        sem_unlink(name);

        // delete shm
        munmap(headers[GetID(name)]->data, sizeof(SharedImageHeader) + headers[GetID(name)]->data_size);
        close(headers[GetID(name)]->fd);
        shm_unlink(name);

        // uninitialized
        init[GetID(name)] = 0;

        return 0;
    }
}
