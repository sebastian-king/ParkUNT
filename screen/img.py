import PIL
from PIL import ImageFont
from PIL import Image
from PIL import ImageDraw

import sys

font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",36)
number_font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",50)
img=Image.new("RGBA", (200,200),(0,0,0))
draw = ImageDraw.Draw(img)


if (sys.argv[1] == "0"):
	r=255
	g=0
	b=0
else:
	r=0
	g=255
	b=0

draw.text((25, 50),sys.argv[1],(r,g,b),font=number_font)

spot_text = "spots"
if (sys.argv[1] == "1"):
	spot_text = "spot"


draw.text((75, 50),"spots",(255,255,255),font=font)
draw.text((5, 100),"available",(255,255,255),font=font)
#draw.chord((100, 75, 125, 100), 0, 360, fill='green')
#draw.chord((75, 100, 100, 125), 0, 360, fill='blue')
#draw.chord((125, 125, 150, 150), 0, 360, fill='yellow')
draw = ImageDraw.Draw(img)

img.save("rendered_image_" + sys.argv[1] + ".png")
