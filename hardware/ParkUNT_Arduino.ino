#include <SPI.h>
#include <Ethernet2.h>

//Ethernet Setup
byte mac[]={0x90,0xA2,0xDA,0x10,0x39,0x68};
char server[]="ParkUNT.tech";

IPAddress ip(192,168,1,37);
EthernetClient client;



 //Number of parking spots
 const int numSpots=6;

// These constants won't change:
const int photos[numSpots] = {A0,A1,A2,A3,A4,A5};    // pin that the sensor is attached to

const int ledPin[numSpots] = {9,8,7,6,5,4};        // pin that the LED is attached to

// variables:
int sensorValue[numSpots] = {0,0,0,0,0,0};         // the sensor value

int sensorMin[numSpots] = {1023,1023,1023,1023,1023,1023};        // minimum sensor value
int sensorMax[numSpots] = {0,0,0,0,0,0};           // maximum sensor value


bool spotAvailable[numSpots]={false,false,false,false,false,false};

void setup() {
  
  Serial.begin(19200); //Start Serial
  
  
  // turn on LED to signal the start of the calibration period:
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);



  // calibrate during the first five seconds
  for(int i =0; i<numSpots;i++){
    while (millis() < 5000) {
      sensorValue[i] = analogRead(photos[i]);
  
      // record the maximum sensor value
      if (sensorValue[i] > sensorMax[i]) {
        sensorMax[i] = sensorValue[i];
      }
  
      // record the minimum sensor value
      if (sensorValue[i] < sensorMin[i]) {
        sensorMin[i] = sensorValue[i];
      }
    }
  }
  // signal the end of the calibration period
  digitalWrite(13, LOW);
  
  /*if(Ethernet.begin(mac)==0)
  {
    Serial.println("Falled to configure Ethenet using DHCP");
    Ethernet.begin(mac,ip);
  }
  delay(1000);
  Serial.println("Connecting . . . ");
  
  //http://parkunt.tech/api.php?spot-id=1&available=1
  
  //if you get a connection report back via serial
  if(client.connect(server,80))
  {
    Serial.println("connected!");
    //Make a HTTP request
    client.println("GET /api.php?spot-id=1&available=1");
    client.println("Host: www.parkunt.tech");
    client.println("Connection: close");
    client.println();
  }
  else {
    Serial.println("Connection Failed:(");
  }*/
}

void loop() {
  for(int i=0; i<numSpots; i++){
    // read the sensor:
    sensorValue[i] = analogRead(photos[i]);
    
  
    // apply the calibration to the sensor reading
    //sensorValue[i] = map(sensorValue[i], sensorMin[i], sensorMax[i], 0, 255);
    
    // in case the sensor value is outside the range seen during calibration
    //sensorValue[i] = constrain(sensorValue[i], 0, 255);
    
    // fade the LED using the calibrated value:
    analogWrite(ledPin[i], sensorValue[i]);
    
    Serial.print(sensorValue[i]);
    Serial.print("\t");
  }
  Serial.println();
  delay(1000);
  
  /*for(int i=0;i<numSpots;i++)
  {
    if(sensorValue[i]>200)
    {
      spotAvailable[i]=true;
    }
    else
    {
      spotAvailable[i]=false;
    }
    if(client.connect(server,80))
    {
      Serial.println("connected!");
      //Make a HTTP request
      String message="GET /api.php?spot-id=";
      message.concat(String(i));
      message.concat("&available=");
      if(spotAvailable[i])
      {
        message.concat(String(1));
      }
      else
      {
        message.concat(String(0));
      }
      client.println("Host: www.parkunt.tech");
      client.println("Connection: close");
      client.println();
    }
    else 
    {
      Serial.println("Connection Failed:(");
    }
  }
  if (!client.connected()) 
  {
    Serial.println();
    Serial.println("disconnecting.");
    client.stop();

    // do nothing forevermore:
    while (true);
  }*/
}	
