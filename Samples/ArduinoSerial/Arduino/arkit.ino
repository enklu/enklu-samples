class HitSensor
{
private:
    uint8_t pin_;

public:

    void setup(uint8_t pin) {
        pin_ = pin;

        pinMode(pin_, INPUT);
    }
    
    bool isHit() {
        // read
        uint8_t val = analogRead(pin_) / 8;

        return val > 0;
    }
};

class TempoDetector
{
private: 
}

HitSensor sensors_[1];

void setup(){
    Serial.begin(9600);

    pinMode(3, OUTPUT);

    sensors_[0].setup(A5);
}
 
void loop(){
    if (sensors_[0].isHit()) {
        digitalWrite(3, HIGH);
    } else {
        digitalWrite(3, LOW);
    }
}