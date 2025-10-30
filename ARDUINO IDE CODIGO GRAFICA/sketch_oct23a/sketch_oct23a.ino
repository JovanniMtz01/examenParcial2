// 🟢 Ejemplo: Envío de datos analógicos al puerto serial
// Compatible con Arduino UNO, MEGA, Nano, ESP32, etc.

const int pinSensor = A0;   // Pin analógico de entrada (puedes cambiarlo)
int valor = 0;              // Variable para guardar el valor analógico

void setup() {
  Serial.begin(115200);     // ⚠️ Asegúrate que coincida con el baud rate de Visual Studio
  delay(1000);              // Pequeña pausa para estabilidad
  Serial.println("Inicio de transmisión..."); // Mensaje inicial (opcional)
}

void loop() {
  valor = analogRead(pinSensor);  // Lee el valor analógico (0 a 1023)
  
  // Envía el valor como texto con salto de línea
  Serial.println(valor);

  delay(50);  // Pequeño retardo para controlar la velocidad (20 Hz aprox.)
}
