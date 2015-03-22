using System;
using Microsoft.SPOT;
using System.IO.Ports;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
using System.Text;

namespace XBeeTest
{
    public class Program
    {
        private static SerialPort serialPort;
        private static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private static int MAX_COMMAND_STRING_LENGTH = 3;
        private static byte[] commandString = new Byte[MAX_COMMAND_STRING_LENGTH];
        private static int commandStringIndex = 0;

        private static String COMMAND_ON = "ON";
        private static String COMMAND_OFF = "OFF";

        public static void Main()
        {
            serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
            serialPort.DataReceived += serialPort_DataReceived;

            Thread.Sleep(Timeout.Infinite);
        }

        static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = serialPort.BytesToRead;

            byte[] buffer = new byte[bytesToRead];

            serialPort.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < bytesToRead; i++)
            {
                byte aByte = buffer[i];

                switch (aByte)
                {
                    case 10:
                        break;
                    case 13:
                        string cmd = new string(Encoding.UTF8.GetChars(commandString));
                        
                        ExecuteCommand(cmd);

                        ClearCommandBuffer();

                        break;
                    default:
                        if (commandStringIndex < MAX_COMMAND_STRING_LENGTH)
                        {
                            commandString[commandStringIndex++] = aByte;
                        }
                        break;
                }
            }
        }

        static void ExecuteCommand(string command)
        {
            if (command == COMMAND_ON)
            {
                led.Write(true);
            }
            else
            {
                led.Write(false);
            }
        }

        static void ClearCommandBuffer()
        {
            for (commandStringIndex = 0; commandStringIndex < MAX_COMMAND_STRING_LENGTH; commandStringIndex++)
            {
                commandString[commandStringIndex] = 0;
            }

            commandStringIndex = 0;
        }
    }
}
