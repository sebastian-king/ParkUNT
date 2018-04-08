import serial
import RPi.GPIO as GPIO
import time
import subprocess
import os 
from websocket import create_connection

dir_path = os.path.dirname(os.path.realpath(__file__))
serial_device=subprocess.check_output([dir_path + '/available_serial_port.sh', ''])

ser=serial.Serial(serial_device, 19200)  #change ACM number as found from ls /dev/tty/ACM*
ser.baudrate=19200

ws = create_connection("wss://websocket.parkunt.tech", subprotocols=["feed"]);
spots=[1,1,1,1,1,1];

first_loop = True # the first loop is generally short
second_loop = True
while True:
	read_ser=ser.readline()
	#print(read_ser)

	values = read_ser.rstrip().split("-");
	if (len(values) > 0):
		#print len(values)
		#print(values)
		for i in range(0, len(values)):
			if (len(values[i]) > 0):
				if (int(values[i]) > 300 and (spots[i] == 1 or second_loop)):
					spots[i] = 0
					print "SET AVAIL FOR spot %d TO 1" % (i+1)
					ws.send('{"key": "update", "values": {"spot": ' + str(i+1) + ',"available": 1}}');
				elif (int(values[i]) <= 300 and (spots[i] == 0 or second_loop)):
					spots[i] = 1
					print "SET AVAIL FOR spot %d TO 0" % (i+1)
					ws.send('{"key": "update", "values": {"spot": ' + str(i+1) + ',"available": 0}}');
				#print "Val #%d: '%s'" % (i, values[i])
		#print "----------"
	if (first_loop == False):
		second_loop = False
	else:
		first_loop = False

	#if(read_ser=="Hello From Arduino!"):
	#blink(11)

ser.close();
