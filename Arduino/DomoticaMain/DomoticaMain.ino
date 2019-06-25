#include <SPI.h>
#include <Ethernet.h>             
#include <SD.h>
#include <Servo.h>
#include <Wire.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
int ethPort = 3300;                                    // Take a free port (check your router) 
String fileName = "test.txt";

#define RFPin        3  // output, pin to control the RF-sender (and Click-On Click-Off-device)
#define switchPin    7  // input, connected to some kind of inputswitch
#define ledPin       8  // output, led used for "connect state": blinking = searching; continuously = connected
#define servoPin     9  // output, more information
#define analogPin    0  // sensor value
#define TIME 10000      // Timer for updating food sensor

EthernetServer server(ethPort);              // EthernetServer instance (listening on port <ethPort>).
File myFile;                                 // SD card file instance
Servo servo;

bool pinState = false;                   // Variable to store actual pin state
bool pinChange = false;                  // Variable to store actual pin change
int  sensorValue = 0;                    // Variable to store actual sensor value
int  ultrasonicValue = 0;                // Variable to store sensor value
uint32_t timer;
String bytes;

void setup()
{
   Serial.begin(9600);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
   }
   //Init I/O-pins
   pinMode(switchPin, INPUT);            // hardware switch, for changing pin state
   pinMode(trigPin, OUTPUT);
   pinMode(echoPin, INPUT);
   pinMode(RFPin, OUTPUT);
   pinMode(ledPin, OUTPUT);
      
   //Default states
   digitalWrite(switchPin, HIGH);        // Activate pullup resistors (needed for input pin)
   digitalWrite(RFPin, LOW);
   digitalWrite(ledPin, LOW);


  Serial.println(F("Initializing SD card..."));
  
  if (!SD.begin(4)) {
    Serial.println(F("initialization failed!"));
    while (1);
  }
  Serial.println(F("initialization done."));
  
  //Try to get an IP address from the DHCP server.
  if (Ethernet.begin(mac) == 0)
  {
    Serial.println(F("Could not obtain IP-address from DHCP -> do nothing"));
    while (true){     // no point in carrying on, so do nothing forevermore; check your router
    }
  }
   
   Serial.print(F("LED (for connect-state and pin-state) on pin ")); Serial.println(ledPin);
   Serial.print(F("Input switch on pin ")); Serial.println(switchPin);
   Serial.print(F("Ultrasonic sensor on pins: triggerPin -> ")); Serial.print(trigPin); Serial.print(F(" echoPin -> ")); Serial.println(switchPin);
   Serial.println(F("Ethernetboard connected (pins 10, 11, 12, 13 and SPI)"));
   Serial.println(F("Connect to DHCP source in local network (blinking led -> waiting for connection)"));
   
   //Start the ethernet server.
   server.begin();

   // Print ethernet client related info for debugging
   Serial.print(F("SERVER STARTED AT: ")); Serial.print(F("[IP: ")); Serial.print(Ethernet.localIP()); Serial.print(F(" PORT: ")); Serial.print(ethPort); Serial.println(F("]"));
   
   Wire.begin(8);                // join i2c bus with address #8
   Wire.onReceive(receiveEvent); // register event
   
   delay(5);
   timer = millis();

   servo.attach(servoPin);
   servo.write(1);
}

void loop()
{
   // Start backgroundtimer for sensor updates
   backgroundTimer();
   
   // Listen for incomming connection (app)
   EthernetClient ethernetClient = server.available();
   if (!ethernetClient) {
      blink(ledPin);
      return; // wait for connection and blink LED
   }
   
   Serial.println("Application connected");
   digitalWrite(ledPin, LOW);

   // Do what needs to be done while the socket is connected.
   while (ethernetClient.connected()) 
   {
      // Activate pin based op pinState
      if (pinChange) {
         if (pinState) { digitalWrite(ledPin, HIGH); }
         else { digitalWrite(ledPin, LOW); }
         pinChange = false;
      }
   
      // Execute when byte/command is received, for example [a].
      while (ethernetClient.available())
      {

         bytes = ethernetClient.readString();
         unsigned int lastStringLength = bytes.length();    
         Serial.println(lastStringLength);
         
         if(lastStringLength == 1){
          char inByte = bytes[0];
          Serial.println(inByte);
          executeCommand(inByte);                // Wait for command to execute
          inByte = NULL;                         // Reset the read byte.
         } else {
          float rotationTimeInSeconds;
          rotationTimeInSeconds = atof(bytes.c_str());
          Serial.println(rotationTimeInSeconds);
          Serial.println(bytes);
          moveServo(rotationTimeInSeconds);
         }
      } 
   }
   Serial.println("Application disonnected");


}

// Implementation of (simple) protocol between app and Arduino
// Request (from app) is single char ('a', 's', 't', 'i' etc.)
// Response (to app) is 4 chars  (not all commands demand a response)
void executeCommand(char cmd)
{     
         char buf[4] = {'\0', '\0', '\0', '\0'};

         // Command protocol
         Serial.print("["); Serial.print(cmd); Serial.print("] -> ");
         switch (cmd) {
         case 'a': // Report sensor value off sensor 1 to app  
            intToCharBuf(sensorValue, buf, 4);                // convert to charbuffer
            server.write(buf, 4);                             // response is always 4 chars (\n included)
            Serial.print("Sensor: "); Serial.println(buf);
            break;
         case 'b': // Report sensor value off sensor 1 to app   
            intToCharBuf(sensorValue, buf, 4);                // convert to charbuffer
            server.write(buf, 4);                             // response is always 4 chars (\n included)
            Serial.print("Sensor: "); Serial.println(buf);
            break;
         case 'c': 
              storeRfidToDatabase();   
            break;
         case 'd': 
              getRfidFromDatabase();
            break;  
         case 't': // Toggle state; If state is already ON then turn it OFF
            pinChange = true; 
            break;
         default:
            Serial.println(F("No valid command found, disconnecting client"));
         }
}

void backgroundTimer() {
  if (timer != 0) {
      if ((millis() - timer) > TIME ) {
          storeRfidToDatabase();              // Update local storage        
          checkEvent(switchPin, pinState);    // update pin state
          sensorValue = readSensor(0, 100);   // update sensor value
          timer = millis();
      }
    }
}

// read value from pin pn, return value is mapped between 0 and mx-1
int readSensor(int pn, int mx)
{
  return map(analogRead(pn), 0, 1023, 0, mx-1);    
}

void storeRfidToDatabase() {
  // open the file. note that only one file can be open at a time,
  // so you have to close this one before opening another.
  myFile = SD.open(fileName, FILE_WRITE);

  // if the file opened okay, write to it:
  if (myFile) {
    Serial.print(F("Writing to test.txt..."));
    myFile.println(F("testing 1, 2, 3."));
    // close the file:
    myFile.close();
    Serial.println(F("done."));
  } else {
    // if the file didn't open, print an error:
    Serial.println(F("error opening test.txt"));
  }
}

void getRfidFromDatabase() {
  // re-open the file for reading:
  myFile = SD.open(fileName);
  if (myFile) {
    Serial.println(F("test.txt:"));

    // read from the file until there's nothing else in it:
    while (myFile.available()) {
      Serial.write(myFile.read());
    }
    // close the file:
    myFile.close();
  } else {
    // if the file didn't open, print an error:
    Serial.println("error opening test.txt");
  }
}

// Convert int <val> char buffer with length <len>
void intToCharBuf(int val, char buf[], int len)
{
   String s;
   s = String(val);                        // convert tot string
   if (s.length() == 1) s = "0" + s;       // prefix redundant "0" 
   if (s.length() == 2) s = "0" + s;  
   s = s + "\n";                           // add newline
   s.toCharArray(buf, len);                // convert string to char-buffer
}

// Check switch level and determine if an event has happend
// event: low -> high or high -> low
void checkEvent(int p, bool &state)
{
   static bool swLevel = false;       // Variable to store the switch level (Low or High)
   static bool prevswLevel = false;   // Variable to store the previous switch level

   swLevel = digitalRead(p);
   if (swLevel)
      if (prevswLevel) delay(1);
      else {               
         prevswLevel = true;   // Low -> High transition
         state = true;
         pinChange = true;
      } 
   else // swLevel == Low
      if (!prevswLevel) delay(1);
      else {
         prevswLevel = false;  // High -> Low transition
         state = false;
         pinChange = true;
      }
}

// blink led on pin <pn>
void blink(int pn)
{
  digitalWrite(pn, HIGH); 
  delay(1000); 
  digitalWrite(pn, LOW); 
  delay(1000);
}

// Visual feedback on pin, based on IP number, used for debug only
// Blink ledpin for a short burst, then blink N times, where N is (related to) IP-number
void signalNumber(int pin, int n)
{
   int i;
   for (i = 0; i < 30; i++)
       { digitalWrite(pin, HIGH); delay(20); digitalWrite(pin, LOW); delay(20); }
   delay(1000);
   for (i = 0; i < n; i++)
       { digitalWrite(pin, HIGH); delay(300); digitalWrite(pin, LOW); delay(300); }
    delay(1000);
}

void moveServo(int anmount) {

  for (int portions = 0; portions < anmount; portions++) {
      servo.write(180);
      delay(2000);
      servo.write(1);
      delay(1000);
        for(int i = 0; i < 5; i++){    
          servo.write(1);
          delay(250);
          servo.write(20);
          delay(250);
      }
  } 
}

// function that executes whenever data is received from master
// this function is registered as an event, see setup()
void receiveEvent(int howMany) {
  while (1 < Wire.available()) { // loop through all but the last
    char c = Wire.read(); // receive byte as a character
    Serial.print(c);         // print the character
  }
  int x = Wire.read();    // receive byte as an integer
  Serial.println(x);         // print the integer
}
