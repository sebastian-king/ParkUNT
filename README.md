# ParkUNT
ParkUNT is my HackUNT 2018 project. A demonstration of sensor-based car park management.

Dependencies & install procedure:

```
mkdir -p /home/parkunt/
cd /home/parkunt/
git clone https://github.com/sebastian-king/ParkUNT.git .
```

```
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
