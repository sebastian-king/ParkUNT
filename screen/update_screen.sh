#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "${DIR}";

number=`wget https://www.parkunt.tech/api.php?get_available -qO-`
if [ -z "${number}" ]; then
	number=0;
fi
image_path="rendered_image_${number}.png";
if [ ! -e "${image_path}" ]; then
	python img.py "${number}"
fi
fbi -a --noverbose -T 1 "${image_path}";
