#!/usr/bin/python

from websocket import create_connection
from subprocess import call
import os

dir_path = os.path.dirname(os.path.realpath(__file__))

ws = create_connection("wss://websocket.parkunt.tech", subprotocols=["feed"])

#result = "test"
ws.recv()
ws.recv()

call([dir_path + "/update_screen.sh", ""])

while True:
	result = ""
	result = ws.recv()

	call([dir_path + "/update_screen.sh", ""])	

	#print "Received '%s'" % result

ws.close()
