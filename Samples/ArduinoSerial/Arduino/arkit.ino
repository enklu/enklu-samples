// payload
// [ulong][char[4]]
uint8_t payload_[8];
uint8_t prevPayload_[8];
uint8_t index_ = 0;

void setup(){
    Serial.begin(115200);

    // status light
    pinMode(3, OUTPUT);

    // inputs
    pinMode(A2, INPUT);
    pinMode(A3, INPUT);
    pinMode(A4, INPUT);
    pinMode(A5, INPUT);
}

uint8_t read(uint8_t pin) {
    return analogRead(pin) / 8;
}

void loop(){
    // copy previous payload
    memcpy(prevPayload_, payload_, 8);
    
    // make payload
    unsigned long t = millis();
    memcpy(payload_, &t, 4);
    payload_[4] = read(A2);
    payload_[5] = read(A3);
    payload_[6] = read(A4);
    payload_[7] = read(A5);

    // compare payloads
    //if (memcmp(payload_ + 4, prevPayload_ + 4, 4) != 0) {
    if (payload_[7] == prevPayload_[7]) { // testing
        digitalWrite(3, LOW);
        return;
    }

    // send any new data
    Serial.write(payload_, 8);

    // indicator light
    digitalWrite(3, HIGH);
}