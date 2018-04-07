 //Number of parking spots
 const int numSpots=7;

// These constants won't change:
const int photos[numSpots] = {A0,A1,A2,A3,A4,A5,A6};    // pin that the sensor is attached to

const int ledPin[numSpots] = {9,8,7,6,5,4,3};        // pin that the LED is attached to

// variables:
int sensorValue[numSpots] = {0,0,0,0,0,0,0};         // the sensor value

int sensorMin[numSpots] = {1023,1023,1023,1023,1023,1023,1023};        // minimum sensor value
int sensorMax[numSpots] = {0,0,0,0,0,0,0};           // maximum sensor value


void setup() {
  
  Serial.begin(9600); //Start Serial
  
  
  // turn on LED to signal the start of the calibration period:
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);


for(int i =0; i<numSpots;i++){
  // calibrate during the first five seconds
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
}	
