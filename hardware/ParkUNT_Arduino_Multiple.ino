 //Number of parking spots
 const int numSpots=6;

// These constants won't change:
const int photos[numSpots] = {A0,A1,A2,A3,A4,A5};    // pin that the sensor is attached to

const int ledPin[numSpots] = {9,8,7,6,5,4};        // pin that the LED is attached to

// variables:
int sensorValue[numSpots] = {0,0,0,0,0,0};         // the sensor value

void setup() {
  
  Serial.begin(19200); //Start Serial
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
