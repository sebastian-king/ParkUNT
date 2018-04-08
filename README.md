# ParkUNT
ParkUNT is my HackUNT 2018 project. A demonstration of sensor-based car park management.

Dependencies:

```
sudo apt-get install python-serial
sudo pip install websocket-client
```

```
@reboot         /home/parkunt/pi/dyn-pi-main.sh
*/30 * * * *    /home/parkunt/pi/dyn-pi-main.sh
@reboot         runsvdir /etc/service/
```
