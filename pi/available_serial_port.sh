#!/bin/bash

echo -n `ls /dev/* | grep "/dev/ttyACM" | head -n 1`;
