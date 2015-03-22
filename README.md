Netduino Samples
===

A collection of Netduino sample applications.

###BasicXBeeExample

Simple program that monitors the serial port for an ON or OFF command and then turns the onboard LED on or off based on the command received from the XBee.  You need to have another program to send the command.  For testing purposes you can simply use a terminal like X-CTU and connect one of the XBees to your computers usb port.

###WirelessInfraredSensor

This example will show you how to build a wireless intrusion detection device.  The example consists of a remote sensor that detects when objects approach it via an infrared sensor.  The remote sensor will only report intrusions when it is "*armed*".

The remote sensor is armed by the controller which sends a signal to it via XBee telling it to arm (*This will flash the onboard led 3 times in quick succession and then the led will remain lit*).  Once the remote sensor is armed it will begin reporting back intrusions to the controller via XBee causing the onboard led on the controller to light up whenever an intrusion is reported.
