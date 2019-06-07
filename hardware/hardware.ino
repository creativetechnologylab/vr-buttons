#define numButtons 2

const int buttons[] = {12, 13};
const int leds[] = {10, 11};

boolean previousButtonStates[numButtons];

void setup() {
  Serial.begin(9600);

  for (int i = 0; i < numButtons; i++) {
    pinMode(buttons[i], INPUT_PULLUP);
    pinMode(leds[i], OUTPUT);
  }
}

void loop() {
  checkButtons();
  checkSerial();
}

void checkButtons() {
  boolean buttonStates[numButtons];
  boolean debounce = false;

  for (int i = 0; i < numButtons; i++) {
    buttonStates[i] = !digitalRead(buttons[i]);

    if (buttonStates[i] && !previousButtonStates[i]) {
      Serial.println(i + 1);
      debounce = true;
    }
  }

  if (debounce) delay(100);

  memcpy(previousButtonStates, buttonStates, numButtons);
}

void checkSerial() {
  if (Serial.available()) {
    String data = Serial.readStringUntil('\n');
    for (int i = 0; i < numButtons; i++) {
      switch(data.charAt(i)) {
        case '1':
          digitalWrite(leds[i], HIGH);
          break;
        case '0':
          digitalWrite(leds[i], LOW);
          break;
      }
    }
  }
}

