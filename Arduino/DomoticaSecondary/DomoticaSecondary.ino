#include <Wire.h>
int trigPin[] = {4};    // Trigger
int echoPin[] = {5};    // Echo
int avgArray[6];
int avgDistance;
int arraysize = 0;
long duration, cm;
int i = 0;
int z = 0;
int timeout = 100;
int minValue;
int maxValue;
int pins;

 
void setup() {
  //Serial Port begin
  Serial.begin (9600);
  //Define inputs and outputs
  pinMode(4, OUTPUT);
  pinMode(5, INPUT);
  DDRB = 0x3f;
  Wire.begin(); // join i2c bus (address optional for master)
}
 
void loop() {

distance(trigPin[i], echoPin[i]);
if(i<arraysize)
i++;
else
i=0;

}

int distance(int trig, int echo)
{
  digitalWrite(8, LOW);
  digitalWrite(9, LOW);
  digitalWrite(10, LOW);
  digitalWrite(11, LOW);
  digitalWrite(12, LOW);
  // The sensor is triggered by a HIGH pulse of 10 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
  digitalWrite(trig, LOW);
  delayMicroseconds(5);
  digitalWrite(trig, HIGH);
  delayMicroseconds(10);
  digitalWrite(trig, LOW);
    // Read the signal from the sensor: a HIGH pulse whose
  // duration is the time (in microseconds) from the sending
  // of the ping to the reception of its echo off of an object.
  pinMode(echo, INPUT);
  duration = pulseIn(echo, HIGH);
 // Convert the time into a distance
  cm = (duration/2) / 29.1;     // Divide by 29.1 or multiply by 0.0343

  if(cm > 35){
  Serial.print(-1);
  Serial.print("cm");
  Serial.println();
  for(int t = 0; t<10; t++){
  digitalWrite(8, HIGH);
  digitalWrite(9, HIGH);
  digitalWrite(10, HIGH);
  digitalWrite(11, HIGH);
  digitalWrite(12, HIGH);
  delay(timeout);
  digitalWrite(8, LOW);
  digitalWrite(9, LOW);
  digitalWrite(10, LOW);
  digitalWrite(11, LOW);
  digitalWrite(12, LOW);
  delay(timeout);
  }
  }
  else{
    if(minValue == NULL){minValue = cm;}
    if(maxValue == NULL){maxValue = cm;}
    if(cm < minValue){minValue = cm;}
    if(cm > maxValue){maxValue = cm;}
  Serial.print(cm);
  Serial.print("cm");
  Serial.println();

pins = round(cm/5)+8;
for(int p = 8; p < pins; p++){
  digitalWrite(p, HIGH);
  }


if(z<6){
  avgArray[z] = cm;
  z++;
}else{
  distanceAv();
  }
  delay(250);
  }

}

int distanceAv()
{
  int temp = avgArray[0]+avgArray[1]+avgArray[2]+avgArray[3]+avgArray[4]+avgArray[5];
  avgDistance = (temp-minValue-maxValue)/4;
  Serial.print("Avg distance = ");
  Serial.print(avgDistance);
  Serial.print("cm");
  Serial.println();
  minValue = 0;
  maxValue = 0;
  z = 0;

   Wire.beginTransmission(8); // transmit to device #8
  Wire.write("jghghgjffcchcgchcg");        // sends five bytes
  Wire.write(avgDistance);
  Wire.endTransmission();    // stop transmitting

  delay(4000);
}
