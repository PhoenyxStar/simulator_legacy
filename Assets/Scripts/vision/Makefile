PLUGIN_PATH = ../../../Assets/Plugins/

all: shared

shared: SharedImage.cpp
	g++ -std=c++11 -g -Wall -Werror -fpic -shared -o SharedImage.so SharedImage.cpp -I../../../../include -I/usr/local/include/opencv2 -L/usr/local/lib -lopencv_core -lopencv_highgui -lopencv_imgproc -lopencv_imgcodecs -lrt -lpthread
	mv SharedImage.so $(PLUGIN_PATH)SharedImage.so
