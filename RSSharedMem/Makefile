PLUGIN_PATH = ../Simulator-Unity/Assets/Plugins/


all: shared

shared: RSSharedMem.cpp
	g++ -g -Wall -Werror -fpic -o $(PLUGIN_PATH)RSSharedMem.so RSSharedMem.cpp -shared -I/usr/local/include/opencv2 -L/usr/local/lib -lopencv_core -lopencv_highgui -lopencv_imgproc -lopencv_imgcodecs -lrt -lpthread
