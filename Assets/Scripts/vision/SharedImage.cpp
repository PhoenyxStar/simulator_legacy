#include <map>
#include <iostream>
#include <opencv2/highgui.hpp>
#include "vision/SharedImageWriter.hpp"

std::map<string,Ptr<SharedImageWriter> > segments;

extern "C"
{
    void SharedImage(char *name, int width, int height, void *buf)
    {
        /*
        string sname(name);
        UMat frame;
        if(segments.count(sname) == 0) // does not exist
        {
            frame = Mat(width, height, CV_8UC3, buf).getUMat(ACCESS_RW);
            segments.insert(std::pair<string,Ptr<SharedImageWriter> >(sname, new SharedImageWriter(sname, frame))); // insert new segment
        }
        segments[name]->Write(frame); // write
        */
    }
}
