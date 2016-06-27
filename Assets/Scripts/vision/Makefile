PLUGIN_PATH = ../../../Assets/Plugins/

all: shared

shared: SharedImage.cpp
	gcc -std=c++11 -fpic -shared -o libSharedImage.so SharedImage.cpp -I../../../../include -I/usr/local/include/opencv2 -L/usr/local/lib -lopencv_core -lopencv_highgui -lopencv_imgproc -lopencv_imgcodecs -lrt -lpthread
	mv libSharedImage.so $(PLUGIN_PATH)libSharedImage.so
