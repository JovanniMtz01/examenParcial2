//agregue unn comentario


void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  delay(1000);
}

void loop() {
  // put your main code here, to run repeatedly:
for (int i=0; i <=1023;i++){
  Serial.println(i);
  Serial.print("\r\n");
  delay(1000);
}
}