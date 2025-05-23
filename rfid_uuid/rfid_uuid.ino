#include <SPI.h>
#include <MFRC522.h>

#define RST_PIN 9   // Пин RST
#define SS_PIN 10    // Пин SDA (SS)
#define BUZZER_PIN 5

MFRC522 mfrc522(SS_PIN, RST_PIN);

void setup() {
  Serial.begin(9600);   // Инициализация Serial-порта
  SPI.begin();          // Инициализация SPI
  mfrc522.PCD_Init();   // Инициализация RC522
  noTone(BUZZER_PIN);
}

void loop() {
  // Проверяем: открыт ли порт на ПК (в браузере Web Serial API)
  if (!Serial.dtr()) {
    delay(100);
    return;
  }
  // Если новая метка не обнаружена
  if (!mfrc522.PICC_IsNewCardPresent() || !mfrc522.PICC_ReadCardSerial()) {
    delay(100);
    return;
  }

  // Выводим UID метки в HEX-формате
  Serial.print("UID метки: ");
  for (byte i = 0; i < mfrc522.uid.size; i++) {
    Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
    Serial.print(mfrc522.uid.uidByte[i], HEX);
  }
  Serial.println();

  // Останавливаем работу с меткой
  mfrc522.PICC_HaltA();
  tone(BUZZER_PIN, 1500);
  delay(100);
  noTone(BUZZER_PIN);
}