#!/bin/bash

ip=`ifconfig wlan0 | grep "inet " | awk '{print $2}' | sed -e 's/addr://'`

if [ ! -z "$ip" ]; then
	#wget -qO- "parkunt.tech/dyndns.php?ip=$ip";
	wget -qO- "massivesoft.net/api/ip2host.php?sub_domain=hackuntpiscreen&super_domain=massivesoft.net&ip=$ip&ttl=3600";
fi

echo $ip;
