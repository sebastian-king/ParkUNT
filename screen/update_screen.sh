#!/bin/bash

while [ true ]; do
	get_number=`wget https://www.parkunt.tech/api.php?get_available -qO-`
	if [ "${number}" != "${get_number}" ]; then
		number="${get_number}";
		if [ -z "${number}" ]; then
			number=0;
		fi
		image_path="rendered_image_${number}.png";
		if [ ! -e "${image_path}" ]; then
			python img.py "${number}"
		fi
		fbi -a --noverbose -T 1 "${image_path}";
	fi;
	sleep 0.2;
done;
