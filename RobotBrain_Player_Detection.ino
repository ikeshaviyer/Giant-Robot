/*
  NOTES FOR WHOEVER COMES AFTER (Lookin' at you Evan and Keshav)
  -I might compress the three varaiables associated with each limb into an array to clean up the code
  -The order of limbs in the array being printed to serial is:
  ---[leftArm, rightArm, leftLeg, rightLeg, button]---
*/

// ADJUST THIS VALUE TO CHANGE LIGHT SENSITIVITY FROM MOST(0) TO LEAST(1024).
int threshold = 512;

// ADJUST THIS VALUE TO SET SERIAL PRINT DELAY IN MILLISECONDS.
int serialDelayMilli = 500;


int leftArmPin = A0;
int rightArmPin = A3;
int leftLegPin = A1;
int rightLegPin = A2;
int buttonPin = 2;

bool reportLeftArm;
bool reportRightArm;
bool reportLeftLeg;
bool reportRightLeg;
bool reportButton;

String finalReport = "";

int allLimbs[4] = {leftArmPin, rightArmPin, leftLegPin, rightLegPin};
bool allReports[5] = {reportLeftArm, reportRightArm, reportLeftLeg, reportRightLeg, reportButton};

void setup() {
  pinMode(leftArmPin, INPUT);
  pinMode(rightArmPin, INPUT);
  pinMode(leftLegPin, INPUT);
  pinMode(rightLegPin, INPUT);

  Serial.begin(9600);
}

void loop() {

  for(int i = 0; i < 4; i++){
    if(analogRead(allLimbs[i]) <= threshold){
      allReports[i] = true;
    } else{
      allReports[i] = false;
    }
  }

  if(analogRead(buttonPin) == HIGH){
    reportButton = true;
  } else{
    reportButton = false;
  }

  finalReport = "";

  for(int i = 0; i < 5; i++){
    if(allReports[i] == true){
      finalReport += "1";
    } else{
      finalReport += "0";
    }
  }

  Serial.print(finalReport);
  Serial.print("\n");

  delay(serialDelayMilli);
}
