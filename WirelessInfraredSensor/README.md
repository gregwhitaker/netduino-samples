# Wireless Infrared Intrusion Detector
This example will show you how to build a wireless intrusion detection device.  The example consists of a remote sensor that detects when objects approach it via an infrared sensor.  The remote sensor will only report intrusions when it is "*armed*".

The remote sensor is armed by the controller which sends a signal to it via XBee telling it to arm (*This will flash the onboard led 3 times in quick succession and then the led will remain lit*).  Once the remote sensor is armed it will begin reporting back intrusions to the controller via XBee causing the onboard led on the controller to light up whenever an intrusion is reported.

You can see a quick video of this project in action [HERE](https://www.youtube.com/watch?v=HASbKwD8XBk&feature=youtu.be)

## Wiring Diagram

The example consists of two circuits:

1. An XBee module that is transmitting and receiving serial data.
2. An XBee module that is transmitting and receiving serial data based upon the state of an infrared sensor.

![wiring_diagram](Docs/Diagram_Small.png)

The Fritzing design file can be found [HERE](Docs/NetduinoWirelessIntrusionDetection.fzz)
