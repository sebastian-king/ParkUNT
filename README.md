*Our official Hackathon submission, write-up & media can be found on [DevPost](https://devpost.com/software/parkunt-yhm0ac).*

# ParkUNT
ParkUNT is my HackUNT 2018 project. A demonstration of sensor-based car park management.

## What is our project?
ParkUNT is a system for managing and streamlining car parking lots with high utilisation, such as on large campuses.

## What problem does it solve?
The problem with large campuses is that they often have a lot of parking but spread out across a large area, and often not enough parking to facilitate all of those who need to attend. It is not uncommon for large companies and organisations to encourage alternative and less convenient methods of transportation due to the difficulty of finding a parking spot. The size of many campuses also discourages people from driving to other lots or overflow parking simply due to the significant extra time that it would take for them to reach their destination.

## How do we solve the problem?
We solve this problem by electronically mapping every single parking space and its usage, using ParkUNT. ParkUNT is a device that is embedded in a small puck-shaped casing in the centre of a parking space. It then uses sensors to determine if there is a car parked above it, and reports back via the internet to a central database. We also provide a multi-device front-end that shows available spots in real-time and gives the user directions to their desired parking space.

A physical display outside of the lot is also provided for people who do not have the information readily available via their GPS or in-car system. It is just a screen that displays the number of available spots, and can also provide information on availability in alternative parking lots.

This allows the lots to get maximum utilisation, which pays for itself by saving time and reducing the number of spaces required. In the future this system can be used to provide reserved/scheduled parking spots and times, as well as analytics and artificial intelligence which can be used to further increase the utilisation of parking lots by predicting when spaces will be available and allowing event scheduling during times of lower utilisation. Furthermore, ParkUNT could be used to issue citations and help amend situations of fraudulent parking. The networking of the individual devices in the spaces can be used for extremely accurate navigation around parking lots to direct the user to the correct spot with incredible precision and paves the way for autonomous systems.

## Technical challenges/constraints
Our main technical challenge was balancing technical needs with available resources. To do this we had to decide on what information we wanted from our sensors and see what we could get in reality, while assessing the accuracy of each option. We decided eventually on a photoresistive sensor to detect light, but ultimately a second type of sensor such as EMF or distance sensor would be used in conjunction with the photoresistive sensor to increase accuracy. However neither of these were available at the time.

Equivalently, when choosing a microcontroller system to use as the computer behind the in-space devices we had a choice of either an Arduino or Raspberry Pi. The sensors we have are only analogue, making them only compatible with the Arduino microcontroller. But, the Pi’s networking abilities were a necessity. This required a compromise between the two by interfacing them via a serial connection.

The next challenge was putting together the software, which was comparatively straightforward. We just had to read input values and push updates via the WebSocket protocol to the central system. The main challenge with the microcontrollers is the tendencies of Raspberry Pis to corrupt their storage when unexpected filesystem events occur. The Pi also cannot handle current fluctuations, which are hard to account for when working from inherently unstable sources of power such as battery power.

The backend of the project is a MySQL database that stores the current state and locations of each parking space. The database is updated and is pushed to users in real-time by using a WebSocket and Node.JS--which also allows a very large amount of concurrent connections without a large performance hit. Powering the web front-end is also some PHP that does pre-processed database queries  for static information such as which spots exists, and their locations.

Another endeavour was to build the screen that displays the number of available parking spaces. This was a challenge that required rendering graphics and outputting them to a TFT screen using a serial connection. Another big challenge is managing the load of the rendering and times when the parking state is frequently updated.

### Technical Security Considerations
Our security considerations were top of mind throughout the building process. Since the WebSocket and website both contain and transmit sensitive data it is paramount that the connections remain encrypted and un-interceptable. Therefore we ensure that all connections are encrypted using TLS and have CSRF protection, via HTTPS and WSS. This was a challenge to integrate with some systems such as Node.JS WebSockets, and furthermore on the client-side on simple microcontroller distributions that don’t necessarily contain systems to validate certificates nor accept encrypted sockets.

Another security challenge that had to be overcome was the fact that the enterprise network at the hackathon blocks all ports aside from 80 and 443, which conflicted with the hosted website and required using a second IP address that needed to be unassigned from the webserver and specifically to the websocket daemon to avoid port conflicts.

Most importantly with IoT-style systems it making sure all doors, not just backdoors are closed. This includes not having any passive inbound methods, and instead having only communications actively beginning from within the client device. This would require not having SSH access, or any direct access to the device. This means that update notifications must be pulled via the websocket and then enacted locally, so as to avoid having a means of entry but still being able to roll out changes.

During development special provision had to be made to mitigate risk by locking down necessary protocols such as SSH using systems like Fail2Ban and iptables restrictions.

Our internal database system also is bound to localhost, and therefore remains mostly inaccessible to outside requests, but internal requests can be hijacked therefore we use prepared SQL statements in all communications with the database to avoid any injection attacks, or even attempts at other code execution.

## Who are we?
We are a two person team from UNT, consisting of a CSE Sophomore and a CS Junior.



### Some install notes (must be adapted to the Pi client type):

```
mkdir -p /home/parkunt/
cd /home/parkunt/
git clone https://github.com/sebastian-king/ParkUNT.git .
```

```
apt-get install runit
sudo apt-get install python-serial
sudo pip install websocket-client
```

```
echo "@reboot         /home/parkunt/pi/main/dyn-pi-main.sh" > /tmp/cron
echo "*/30 * * * *    /home/parkunt/pi/main/dyn-pi-main.sh" >> /tmp/cron
echo "@reboot         runsvdir /etc/service/" >> /tmp/cron
crontab /tmp/cron
```

```
ln -s /home/parkunt/pi/main/serial-sv/ /etc/sv/serial-sv
ln -s /home/parkunt/pi/main/serial-sv/ /etc/service/serial-sv
```
