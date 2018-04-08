import serial
import RPi.GPIO as GPIO
import time

ser=serial.Serial("/dev/ttyACM1", 19200)  #change ACM number as found from ls /dev/tty/ACM*
ser.baudrate=19200

spots=[0,0,0,0,0,0];

while True:
	read_ser=ser.readline()
	#print(read_ser)

	values = read_ser.rstrip().split("-");
	if (len(values) > 0):
		for 
		for i in range(0, len(values):
			if (len(val) > 0):
				spots[i] = values[i]
				print "Val: '%s'" % (values[i])
		print "----------"

	#if(read_ser=="Hello From Arduino!"):
	#blink(11)

ser.close();
