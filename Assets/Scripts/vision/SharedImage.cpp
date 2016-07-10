#include "SharedImage.h"

extern "C"
{
    int GetID(char *name)
    {
        for(int i = 0; i < NCAMERAS; ++i)
        {
            if(strcmp(name, names[i]) == 0)
                return i;
        }
        return -1;
    }

    int GetInit(char *name)
    {
        return init[GetID(name)];
    }

    SharedImageHeader* GetHeader(char *name)
    {
        return headers[GetID(name)];
    }

    void Texture2Mat(int width, int height, unsigned char *buf)
    {
        unsigned long data_size = width * height * 3;
        unsigned char *tmp = new unsigned char[data_size];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                tmp[((height - y - 1) * width + x) * 3] = buf[(y * width + x) * 3 + 2];
                tmp[((height - y - 1) * width + x) * 3 + 1] = buf[(y * width + x) * 3 + 1];
                tmp[((height - y - 1) * width + x) * 3 + 2] = buf[(y * width + x) * 3];
            }
        }
        memcpy(buf, tmp, data_size);
        delete tmp;
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
            // convert to opencv mat
            unsigned long data_size = width * height * 3; // 3 channel
            Texture2Mat(width, height, buf);

            // create shared image header
            std::string header_name = std::string(PREFIX) + name;
            int fd = shm_open(header_name.c_str(), O_RDWR | O_CREAT | O_TRUNC, S_IRWXU);
            if(fd <= 0)
            {
                fprintf(file, "Failed to open shm: %s - %s", header_name.c_str(), strerror(errno));
                fflush(file);
                return -1;
            }
            SharedImageHeader *header = new SharedImageHeader();
            header->fd = fd;
            header->size = cv::Size(height, width);
            header->type = TYPE;
            header->data_size = data_size;

            // create semaphore
            sem_t *sem = sem_open(header_name.c_str(), O_CREAT, S_IRWXU, 1);
            if(sem <= 0)
            {
                fprintf(file, "Failed to open sem: %s - %s", header_name.c_str(), strerror(errno));
                fflush(file);
                return -2;
            }
            header->sem = sem;

            // allocate memory for image
            int total_size = sizeof(SharedImageHeader) + data_size;
            ftruncate(fd, total_size);

            // put first frame into memory
            void *mem = mmap(0, total_size, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0); // let os create buffer
            if(mem <= 0)
            {
                fprintf(file, "Failed to open shm: %s - %s", header_name.c_str(), strerror(errno));
                fflush(file);
                return -3;
            }
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
        int id = GetID(name);
        if(GetInit(name) == 0) // does not exist
            return InitShared(name, width, height, buf); // create it

        // convert to opencv mat
        unsigned long data_size = width * height * 3; // 3 channel
        Texture2Mat(width, height, buf);

        // write
        sem_wait(headers[id]->sem); // lock memory
        memcpy(headers[id]->data, buf, headers[id]->data_size); // write
        sem_post(headers[id]->sem); // unlock memory

		delete buf;

        return 0;
    }

    int ShutdownShared(char *name)
    {
        int id = GetID(name);
        if(GetInit(name) == 0) // does not exist
            return -1;

        // delete sem
        sem_close(headers[id]->sem);
        sem_unlink(name);

        // delete shm
        munmap(headers[id]->data, sizeof(SharedImageHeader) + headers[id]->data_size);
        close(headers[id]->fd);
        shm_unlink(name);

        // uninitialize
        init[id] = 0;
        return 0;
    }
}
