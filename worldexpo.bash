#!/bin/bash

export  DYLD_LIBRARY_PATH=/usr/local/lib/hakoniwa/py:/usr/local/lib/hakoniwa
MUJOCO_DIR=../../hakoniwalab/hakoniwa-mujoco-robots
ROVER_PID=
UNITY_PID=
function kill_processes() {
    if [ -n "$UNITY_PID" ]; then
        kill -9 $UNITY_PID
    fi
    if [ -n "$ROVER_PID" ]; then
        kill -9 $ROVER_PID
    fi

    exit 0
}
function signal_handler() {
    echo "Signal received"
    kill_processes
}
trap signal_handler SIGINT

BASE_DIR=`pwd`

function start_rover() {
    echo "Starting rover"
    cd ${MUJOCO_DIR} 
     ./src/cmake-build/main_for_sample/rover/rover_sim &
    ROVER_PID=$!
    cd $BASE_DIR
}


function start_unity() {
    echo "Starting Unity"
    cd MacWorldExpo
    ./WorldExpoApps.app/Contents/MacOS/simulation &
    UNITY_PID=$!
    cd $BASE_DIR
}

function start_python_rc() {
    echo "Starting python rc"
    cd ${MUJOCO_DIR}/python
    python rover_gamepad.py Rover ../../../oss/hakoniwa-unity-drone/simulation/rover.json ./rc_config/xbox-control.json
    cd $BASE_DIR
}

start_rover
sleep 1

start_unity
sleep 5

hako-cmd start

sleep 2

start_python_rc


echo "Ctrl+C to stop"
while [ 1 ]; do
    sleep 1
done
