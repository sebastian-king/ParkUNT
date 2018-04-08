#!/usr/bin/python

import serial
import RPi.GPIO as GPIO
import time
import subprocess
import os
from websocket import create_connection

import sys

#dir_path = os.path.dirname(os.path.realpath(__file__))
#serial_devices=subprocess.check_output([dir_path + '/../main/available_serial_port.sh', ''])

serial_device=sys.argv[1];

#for device in serial_devices.split('|'):
#	print(device)

ser=serial.Serial(serial_device, 19200)  #change ACM number as found from ls /dev/tty/ACM*
ser.baudrate=19200

ws = create_connection("wss://websocket.parkunt.tech", subprotocols=["feed"]);
spot=0;

index=sys.argv[2];

first_loop = True # the first loop is generally short
second_loop = True
while True:
	read_ser=ser.readline()

	value = read_ser.rstrip();
	#print value;
	if (len(value) > 0):
		if (int(value) > 225 and (spot == 1 or second_loop)):
			spot = 0
			print "SET AVAIL FOR spot %s TO 1" % str(index)
			ws.send('{"key": "update", "values": {"spot": ' + str(index) + ',"available": 1}}');
		elif (int(value) <= 225 and (spot == 0 or second_loop)):
			spot = 1
			print "SET AVAIL FOR spot %s TO 0" % str(index)
			ws.send('{"key": "update", "values": {"spot": ' + str(index) + ',"available": 0}}');
	if (first_loop == False):
		second_loop = False
	else:
		first_loop = False

ser.close();
