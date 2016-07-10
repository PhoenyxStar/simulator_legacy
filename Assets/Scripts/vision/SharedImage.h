#include <map>
#include <opencv2/opencv.hpp>
#include "vision/SharedImageHeader.hpp"

using namespace cv;

extern "C"
{
    #define NCAMERAS 3
	#define NDISP 0
	#define WSIZE 21
    #define PREFIX "robosub_"
    SharedImageHeader *headers[NCAMERAS];
    const char *names[NCAMERAS] = {"left", "right", "front"};
    int init[NCAMERAS] = {0, 0};
    FILE *file = fopen("sharedimage.log", "w+");
	Ptr<StereoBM> stereobm;

	// helpers
    int GetID(char *name);
    int GetInit(char *name);
    SharedImageHeader* GetHeader(char *name);
    void Texture2Mat(int width, int height, unsigned char *buf);
    void ShowImage(char *name, int width, int height, int bytes_per_pixel, unsigned char *buf);
	Mat QMatrix(int cx, int cy, float baseline, float focal_length);

	// calculates a 3D rangemap and stores in shared memory
	void RangeMap(char *name, int width, int height, float baseline, float focal_length, unsigned char *left, unsigned char *right);

	// shared memory save functions
    int InitShared(char *name, int width, int height, int bytes_per_pixel, unsigned char *buf);
    int UpdateShared(char *name, int width, int height, int bytes_per_pixel, unsigned char *buf);
    int ShutdownShared(char *name);
}
