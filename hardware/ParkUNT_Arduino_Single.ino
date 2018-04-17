// These constants won't change:
const int photo=A0;    // pin that the sensor is attached to

const int ledPin = 9;        // pin that the LED is attached to

// variables:
int sensorValue=0;         // the sensor value

void setup() {
  
  Serial.begin(19200); //Start Serial
}

void loop() {
  
    // read the sensor:
    sensorValue = analogRead(photo);
   
    // fade the LED using the calibrated value:
    analogWrite(ledPin, sensorValue);
    
    Serial.print(sensorValue);

  Serial.println();
  delay(200);
  
  
}	
