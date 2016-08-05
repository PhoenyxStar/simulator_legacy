# Robosub Simulator #

Robosub Club of The Palouse consists of students from Washington State University. This club is focused on building an
Autonomous Unmanned Vehicle for the annual AUVSI competition in San Diego, CA.
You can find out more about the AUVSI competition at robosub.org.

# Installation #

The simulator requires a Unity Linux Editor 5.4.1 install along with the main
robosub dependencies.  This repo should be installed as a subrepo as scripts
refer to the main settings files.  Note that messing with any files from the
parent repo will affect the simulator so if something is not working right
check there first.

# Setup #

Make sure that the robosub repo has all dependencies built.

cd ..
./scripts/make_all

Build the shared vision simulator interface.

cd Assets/Scripts/vision
make

Load the project into Unity by opening the simulator folder as a project file.
NOTE: opening ANY other folder besides the simulator folder as a project will
generate assets for the wrong directory, clutter the folder, and will not work
properly.

Once loaded, you can select any of the scenes by double clicking on it.  You can
then select entities to attach scripts to for a run.

# Running #

To run the simulator make sure you have a valid scene with no compilation errors
and then press the play button.  You should see a rendered submarine floating to
the surface.  Then activate any runtime modules you want either manually or using
the helm GUI.  If working properly, you should see sensor data being sent through
the broker.  (If not make sure that the broker IP is valid).  

# Troubleshooting #

If anything crashes check the main logs for any symptoms as well as the shared vision
log in the main simulator directory.  If they are not at fault then the editor crashed
which happens occasionally as it is still an experimental build.
