#include <map>
#include <opencv2/opencv.hpp>
#include "vision/SharedImageHeader.hpp"

using namespace cv;

extern "C"
{
    #define NCAMERAS 2
    #define PREFIX "robosub_"
    #define TYPE CV_8UC3
    SharedImageHeader *headers[NCAMERAS];
    const char *names[NCAMERAS] = {"left", "right"};
    int init[NCAMERAS] = {0, 0};
    FILE *file = fopen("sharedimage.log", "w+");

    int GetID(char *name);
    int GetInit(char *name);
    SharedImageHeader* GetHeader(char *name);
    void UnityText2Mat(int width, int height, unsigned char *unity, unsigned char *mat);
    void ShowImage(char *name, int width, int height, unsigned char *buf);
    int InitShared(char *name, int width, int height, unsigned char *buf);
    int UpdateShared(char *name, int width, int height, unsigned char *buf);
    int ShutdownShared(char *name);
}
