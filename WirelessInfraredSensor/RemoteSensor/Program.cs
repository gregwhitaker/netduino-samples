using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;
using System.IO.Ports;
using System.Text;

namespace RemoteSensor
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private const int MAX_CMD_LENGTH = 3;
        private const String CMD_ON = "ON";
        private const String CMD_OFF = "OFF";
        private static readonly byte[] CMD_SENSOR_DETECT = Encoding.UTF8.GetBytes("Y\r");
        private static readonly byte[] CMD_SENSOR_NODETECT = Encoding.UTF8.GetBytes("N\r");

        private static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private static InterruptPort infraredSensor = new InterruptPort(Pins.GPIO_PIN_D7, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
        private static SerialPort serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

        private static Object syncRoot = new Object();
        private static bool activated = false;
        private static byte[] cmdBuffer = new byte[MAX_CMD_LENGTH];     //Buffers Incoming Commands
        private static int cmdBufferIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {   
            serialPort.DataReceived += serialPort_DataReceived;
            infraredSensor.OnInterrupt += infraredSensor_OnInterrupt;

            serialPort.Open();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
                        ExecuteCommand();
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
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <param name="time"></param>
        private static void infraredSensor_OnInterrupt(uint port, uint data, DateTime time)
        {
            if (activated)
            {
                if (data == 0)
                {
                    //Found Something
                    serialPort.Write(CMD_SENSOR_DETECT, 0, CMD_SENSOR_DETECT.Length);
                }
                else
                {
                    //It's Gone
                    serialPort.Write(CMD_SENSOR_NODETECT, 0, CMD_SENSOR_NODETECT.Length);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ExecuteCommand()
        {
            string cmd = new string(Encoding.UTF8.GetChars(cmdBuffer)).ToUpper();

            lock (syncRoot)
            {
                try
                {
                    if (cmd == CMD_ON)
                    {
                        activated = true;

                        //Blink Onboard LED and Leave On
                        led.Write(true);
                        Thread.Sleep(200);
                        led.Write(false);
                        Thread.Sleep(200);
                        led.Write(true);
                        Thread.Sleep(200);
                        led.Write(false);
                        Thread.Sleep(200);
                        led.Write(true);
                    }
                    else if (cmd == CMD_OFF)
                    {
                        activated = false;
                        led.Write(false);
                    }
                }
                finally
                {
                    ClearCommandBuffer();
                }
            }
        }

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
    }
}
