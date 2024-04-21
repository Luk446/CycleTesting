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
