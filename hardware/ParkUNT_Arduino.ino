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
  
  
}

void loop() {
  for(int i=0; i<numSpots; i++){
    // read the sensor:
    sensorValue[i] = analogRead(photos[i]);
   
    // fade the LED using the calibrated value:
    analogWrite(ledPin[i], sensorValue[i]);
    
    Serial.print(sensorValue[i]);
    Serial.print("-");
  }
  Serial.println();
  delay(200);
  
  
}	
