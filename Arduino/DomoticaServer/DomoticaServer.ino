
#include <SPI.h>
#include <Ethernet.h>             
#include <SD.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
int ethPort = 3300;                                    // Take a free port (check your router) 
String fileName = "test.txt";

#define RFPin        3  // output, pin to control the RF-sender (and Click-On Click-Off-device)
#define echoPin      5  // output, always LOW
#define trigPin      6  // output, always HIGH
#define switchPin    7  // input, connected to some kind of inputswitch
#define ledPin       8  // output, led used for "connect state": blinking = searching; continuously = connected
#define infoPin      9  // output, more information
#define analogPin    0  // sensor value
#define TIME 10000      // Timer for updating food sensor

EthernetServer server(ethPort);              // EthernetServer instance (listening on port <ethPort>).
File myFile;                                 // SD card file instance

bool pinState = false;                   // Variable to store actual pin state
bool pinChange = false;                  // Variable to store actual pin change
int  sensorValue = 0;                    // Variable to store actual sensor value
int  ultrasonicValue = 0;                // Variable to store sensor value
uint32_t timer;

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
   pinMode(infoPin, OUTPUT);
   
   //Default states
   digitalWrite(switchPin, HIGH);        // Activate pullup resistors (needed for input pin)
   digitalWrite(RFPin, LOW);
   digitalWrite(ledPin, LOW);
   digitalWrite(infoPin, LOW);


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
   
   delay(5);
   timer = millis();
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
         char inByte = ethernetClient.read();   // Get byte from the client.
         executeCommand(inByte);                // Wait for command to execute
         inByte = NULL;                         // Reset the read byte.
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
         case 'c': // Toggle KAKU 0 state; If state is already ON then turn it OFF 
              storeRfidToDatabase();   
            break;
         case 'd': // Report status of KAKU 0 to app  
              getRfidFromDatabase();
            break;  
         case 'e': // Toggle KAKU 1 state; If state is already ON then turn it OFF 

            break;
         case 'f': // Report status of KAKU 1 to app  

            break;
         case 'g': // Toggle KAKU 2 state; If state is already ON then turn it OFF 

            break;
         case 'h': // Report status of KAKU 2 to app  

            break;   
         case 's': // Report switch state to the app

            break;
         case 't': // Toggle state; If state is already ON then turn it OFF

            pinChange = true; 
            break;
         case 'i':
            intToCharBuf(ultrasonic_sensor(), buf, 4);        // convert to charbuffer
            server.write(buf, 4);                             // response is always 4 chars (\n included)
            Serial.print("Sensor: "); Serial.println(buf);
            break;
         default:
            Serial.println("No valid command found, disconnecting client");
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
    Serial.print("Writing to test.txt...");
    myFile.println("testing 1, 2, 3.");
    // close the file:
    myFile.close();
    Serial.println("done.");
  } else {
    // if the file didn't open, print an error:
    Serial.println("error opening test.txt");
  }
}

void getRfidFromDatabase() {
  // re-open the file for reading:
  myFile = SD.open(fileName);
  if (myFile) {
    Serial.println("test.txt:");

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

long ultrasonic_sensor() {
      // Variabelen configureren
    long echo, distanceCm;
 
    // De sensor wordt getriggerd bij 10 us, geef eerst een lage puls om een schone hoge puls te krijgen
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);
 
    // Wacht op een hoge puls en meet de tijd
    echo = pulseIn(echoPin, HIGH);
    distanceCm = distance(echo);
    return distanceCm;
}

int distance(int echo) {
    int cm = echo / 29 / 2;

    if (cm < 200) {
      return cm;
    }
    return -1;
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
