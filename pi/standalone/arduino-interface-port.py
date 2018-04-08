#!/usr/bin/python

import RPi.GPIO as GPIO
import time
import subprocess
import os
from websocket import create_connection

import sys

import Adafruit_GPIO.SPI as SPI
import Adafruit_MCP3008

#dir_path = os.path.dirname(os.path.realpath(__file__))
#serial_devices=subprocess.check_output([dir_path + '/../main/available_serial_port.sh', ''])

SPI_PORT   = 0
SPI_DEVICE = 0
mcp = Adafruit_MCP3008.MCP3008(spi=SPI.SpiDev(SPI_PORT, SPI_DEVICE))

ws = create_connection("wss://websocket.parkunt.tech", subprotocols=["feed"]);
spot=0;

index=10;

first_loop = True # the first loop is generally short
second_loop = True
while True:
	value = str(mcp.read_adc(0))
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
