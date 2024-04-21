/* Simplified just including print section but

tact2 should be attached to the fsr value
tempmath2 is the result of the thermistor calibration calculations
tempmath22 is the another thermistor
angle is the result of the flexsensor angle calculation

*/

void setup() {

  Serial.begin(9600);


}

void loop() {

  // all maths and calcs for values

  
  // print for debug + app linkage
  Serial.print("PV: "); // signal
  Serial.println(tact2); // fsr val

  Serial.print("TV: "); // signal
  int tempclean = (int)tempmath2; // clean signal for C# proc
  Serial.println(tempclean); // temp val

  Serial.print("TV2: ");
  int tempclean2 = (int)tempmath22;
  Serial.println(tempclean2);

  Serial.print("AV: ");
  int angleclean = (int)angle;
  Serial.println(angleclean);

  delay(100); // flooding delay

}
