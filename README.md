# ParkUNT
ParkUNT is my HackUNT 2018 project. A demonstration of sensor-based car park management.

Dependencies & install procedure:

```
sudo apt-get install python-serial
sudo pip install websocket-client
```

```
@reboot         /home/parkunt/pi/dyn-pi-main.sh
*/30 * * * *    /home/parkunt/pi/dyn-pi-main.sh
@reboot         runsvdir /etc/service/
```

```
ln -s /home/parkunt/pi/main/serial-sv/ /etc/sv/serial-sv
ln -s /home/parkunt/pi/main/serial-sv/ /etc/service/serial-sv
```
