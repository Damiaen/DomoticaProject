#include <SPI.h>
#include <Ethernet.h>             
#include <NewRemoteTransmitter.h> 
#define unitCode 32122670


byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192, 168, 137, 40);                       // Backup IP when dhcp failes
int ethPort = 3300;                                    // Take a free port (check your router)

#define RFPin        3  // output, pin to control the RF-sender (and Click-On Click-Off-device)
#define echoPin      5  // output, always LOW
#define trigPin      6  // output, always HIGH
#define switchPin    7  // input, connected to some kind of inputswitch
#define ledPin       8  // output, led used for "connect state": blinking = searching; continuously = connected
#define infoPin      9  // output, more information
#define analogPin    0  // sensor value

EthernetServer server(ethPort);              // EthernetServer instance (listening on port <ethPort>).
NewRemoteTransmitter apa3Transmitter(unitCode, RFPin, 260, 3);

bool pinState = false;                   // Variable to store actual pin state
bool Kaku_0_State = false;               // Variable to state of KAKU 0
bool Kaku_1_State = false;               // Variable to state of KAKU 1
bool Kaku_2_State = false;               // Variable to state of KAKU 2
bool pinChange = false;                  // Variable to store actual pin change
int  sensorValue = 0;                    // Variable to store actual sensor value
int  ultrasonicValue = 0;                // Variable to store sensor value

void setup()
{
   Serial.begin(9600);
   Serial.println("Domotica project, Arduino Domotica Server\n");
   
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

   //Try to get an IP address from the DHCP server.
   if (Ethernet.begin(mac) == 0)
   {
      Serial.println("Could not obtain IP-address from DHCP -> do nothing");
      while (true){     // no point in carrying on, so do nothing forevermore; check your router
      }
   }
   
   Serial.print("LED (for connect-state and pin-state) on pin "); Serial.println(ledPin);
   Serial.print("Input switch on pin "); Serial.println(switchPin);
   Serial.print("Ultrasonic sensor on pins: triggerPin -> "); Serial.print(trigPin); Serial.print(" echoPin -> "); Serial.println(switchPin);
   Serial.println("Ethernetboard connected (pins 10, 11, 12, 13 and SPI)");
   Serial.println("Connect to DHCP source in local network (blinking led -> waiting for connection)");
   
   //Start the ethernet server.
   server.begin();

   // Print IP-address and led indication of server state (show everything related to connection)
   Serial.print("Server started at: ");
   Serial.print(Ethernet.localIP());
   int IPnr = getIPComputerNumber(Ethernet.localIP());
   Serial.print(" ["); Serial.print(IPnr); Serial.print("] ");
   Serial.print("  [ Assigned IP: "); Serial.print(Ethernet.localIP()); Serial.print(" Port: "); Serial.print(ethPort); Serial.println("]");
}

void loop()
{
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
      checkEvent(switchPin, pinState);          // update pin state
      sensorValue = readSensor(0, 100);         // update sensor value
        
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

// Choose and switch your Kaku device, state is true/false (HIGH/LOW)
void switchDefault(int kakuId, bool state)
{   
   apa3Transmitter.sendUnit(kakuId, state);               
   delay(100);
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
            if (Kaku_0_State) { Kaku_0_State = false; switchDefault(0, false); Serial.println("Set KAKU 0 state to \"OFF\""); }
            else { Kaku_0_State = true; switchDefault(0, true); Serial.println("Set KAKU 0 state to \"ON\""); }  
            break;
         case 'd': // Report status of KAKU 0 to app  
            if (Kaku_0_State) { server.write(" ON\n"); Serial.println("KAKU 0 is ON"); }
            else { server.write("OFF\n"); Serial.println("KAKU 0 is OFF"); }
            break;  
         case 'e': // Toggle KAKU 1 state; If state is already ON then turn it OFF 
            if (Kaku_1_State) { Kaku_1_State = false; switchDefault(1, false); Serial.println("Set KAKU 1 state to \"OFF\""); }
            else { Kaku_1_State = true; switchDefault(1, true); Serial.println("Set KAKU 1 state to \"ON\""); }  
            break;
         case 'f': // Report status of KAKU 1 to app  
            if (Kaku_1_State  ) { server.write(" ON\n"); Serial.println("KAKU 1 is ON"); }
            else { server.write("OFF\n"); Serial.println("KAKU 1 is OFF"); }
            break;
         case 'g': // Toggle KAKU 2 state; If state is already ON then turn it OFF 
            if (Kaku_2_State) { Kaku_2_State = false; switchDefault(2, false); Serial.println("Set KAKU 2 state to \"OFF\""); }
            else { Kaku_2_State = true; switchDefault(2, true); Serial.println("Set KAKU 2 state to \"ON\""); }  
            break;
         case 'h': // Report status of KAKU 2 to app  
            if (Kaku_2_State) { server.write(" ON\n"); Serial.println("KAKU 2 is ON"); }
            else { server.write("OFF\n"); Serial.println("KAKU 2 is OFF"); }
            break;   
         case 's': // Report switch state to the app
            if (pinState) { server.write(" ON\n"); Serial.println("Pin state is ON"); }
            else { server.write("OFF\n"); Serial.println("Pin state is OFF"); }
            break;
         case 't': // Toggle state; If state is already ON then turn it OFF
            if (pinState) { pinState = false; Serial.println("Set pin state to \"OFF\""); }
            else { pinState = true; Serial.println("Set pin state to \"ON\""); }  
            pinChange = true; 
            break;
         case 'i':
            ultrasonic_sensor();
            break;
         default:
            Serial.println("No valid command found, disconnecting client");
         }
}

// read value from pin pn, return value is mapped between 0 and mx-1
int readSensor(int pn, int mx)
{
  return map(analogRead(pn), 0, 1023, 0, mx-1);    
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

void ultrasonic_sensor() {
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
    // Print de gegevens naar de seriële monitor
    Serial.print("afstand in cm: ");
    Serial.print(distanceCm);
    Serial.println();
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

// Convert IPAddress tot String (e.g. "192.168.1.105")
String IPAddressToString(IPAddress address)
{
    return String(address[0]) + "." + 
           String(address[1]) + "." + 
           String(address[2]) + "." + 
           String(address[3]);
}

// Returns B-class network-id: 192.168.1.3 -> 1)
int getIPClassB(IPAddress address)
{
    return address[2];
}

// Returns computernumber in local network: 192.168.1.3 -> 3)
int getIPComputerNumber(IPAddress address)
{
    return address[3];
}

// Returns computernumber in local network: 192.168.1.105 -> 5)
int getIPComputerNumberOffset(IPAddress address, int offset)
{
    return getIPComputerNumber(address) - offset;
}
