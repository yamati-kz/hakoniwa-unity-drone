#!/bin/bash

CONDUCTOR_PID=
WEBSERVER_PID=
AR_BRIDGE_PID=
UNITY_PID=
function kill_processes() {
    if [ -n "$UNITY_PID" ]; then
        kill -9 $UNITY_PID
    fi
    if [ -n "$WEBSERVER_PID" ]; then
        kill -9 $WEBSERVER_PID
    fi
    if [ -n "$CONDUCTOR_PID" ]; then
        kill -9 $CONDUCTOR_PID
    fi
    if [ -n "$AR_BRIDGE_PID" ]; then
        kill -9 $AR_BRIDGE_PID
    fi

    exit 0
}
function signal_handler() {
    echo "Signal received"
    kill_processes
}
trap signal_handler SIGINT

BASE_DIR=`pwd`

function start_conductor() {
    echo "Starting conductor"
    cd hakoniwa-webserver 
    python server/conductor.py --delta_time_usec 20000 --max_delay_time_usec 100000 &
    CONDUCTOR_PID=$!
    cd $BASE_DIR
}

function start_webserver() {
    echo "Starting webserver"
    cd hakoniwa-webserver 
    python -m server.main --asset_name WebServer --config_path ../simulation/sharesim-drone.json --delta_time_usec 20000 &
    WEBSERVER_PID=$!
    cd $BASE_DIR
}

function start_unity() {
    echo "Starting Unity"
    cd MacShareSimApps
    ./ShareSimApps.app/Contents/MacOS/simulation &
    UNITY_PID=$!
    cd $BASE_DIR
}

function start_ar_bridge() {
    echo "Starting ar_bridge"
    cd hakoniwa-ar-bridge
    python -m asset_lib.main &
    AR_BRIDGE_PID=$!
    cd $BASE_DIR
}

start_conductor
sleep 1

start_unity
sleep 3

hako-cmd start

sleep 2

start_webserver
sleep 1


start_ar_bridge

echo "Start Unity, then press Enter key"
read

echo "Ctrl+C to stop"
while [ 1 ]; do
    sleep 1
done
