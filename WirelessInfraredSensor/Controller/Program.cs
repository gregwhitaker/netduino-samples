using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO.Ports;
using System.Threading;
using System.Text;

namespace Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private const int MAX_CMD_LENGTH = 1;
        private const String CMD_ON = "ON";
        private const String CMD_OFF = "OFF";
        private const String CMD_SENSOR_DETECT = "Y";
        private const String CMD_SENSOR_NODETECT = "N";

        private static InterruptPort button = new InterruptPort(Pins.ONBOARD_SW1, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
        private static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private static SerialPort serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

        private static bool activated = false;
        private static byte[] cmdBuffer = new byte[MAX_CMD_LENGTH];     //Buffers Incoming Commands
        private static int cmdBufferIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.Open();

            button.OnInterrupt += button_OnInterrupt;

            Thread.Sleep(Timeout.Infinite);
        }

        static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = serialPort.BytesToRead;

            byte[] readBuffer = new byte[bytesToRead];

            serialPort.Read(readBuffer, 0, readBuffer.Length);

            for (int i = 0; i < bytesToRead; i++)
            {
                byte aByte = readBuffer[i];

                switch (aByte)
                {
                    case 10:
                        //Ignore Line Feeds
                        break;
                    case 13:
                        //Carriage Returns Mark End of Command
                        RegisterCommandReceived();
                        break;
                    default:
                        //Append to Command
                        BufferCommandByte(aByte);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="time"></param>
        private static void button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (activated)
            {
                SendCommand(CMD_OFF);
                activated = false;
            }
            else
            {
                SendCommand(CMD_ON);
                activated = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void RegisterCommandReceived()
        {
            string cmd = new string(Encoding.UTF8.GetChars(cmdBuffer)).ToUpper();

            try
            {
                if (cmd == CMD_SENSOR_DETECT)
                {
                    led.Write(true);
                }
                else if (cmd == CMD_SENSOR_NODETECT)
                {
                    led.Write(false);
                }
            }
            finally
            {
                ClearCommandBuffer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ClearCommandBuffer()
        {
            for (cmdBufferIndex = 0; cmdBufferIndex < MAX_CMD_LENGTH; cmdBufferIndex++)
            {
                cmdBuffer[cmdBufferIndex] = 0;
            }

            cmdBufferIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aByte"></param>
        private static void BufferCommandByte(byte aByte)
        {
            if (cmdBufferIndex < MAX_CMD_LENGTH)
            {
                cmdBuffer[cmdBufferIndex++] = aByte;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        private static void SendCommand(string command)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(command + "\r");
            serialPort.Write(sendBuffer, 0, sendBuffer.Length);
        }
    }
}
