using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text.RegularExpressions;
using WebSocketSharp;

namespace ParkUNTConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var comreader = new ArduinoSerialReader("COM5");

            Console.ReadLine();
        }

        static void ListComPorts()
        {
            // Get a list of serial port names. 
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console. 
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
        }

    }

    public class ArduinoSerialReader : IDisposable
    {
        private SerialPort _serialPort;
        private WebSocket ws;

        private bool[] availability = { true, true, true, true, true, true, true };

        /// <summary>
        /// Evaluates the availability of a spot based upon sensor values
        /// </summary>
        /// <param name="value">A value to evaluate on</param>
        public static bool EvaluateAvailability(int value) => value > 300 ? true : false;

        public void EvaluateInputs(String str)
        {
            string[] stringArray = str.Split('-');
            for (int i = 0; i < stringArray.Length; i++)
            {
                Int32.TryParse(stringArray[i], out int output);

                if (EvaluateAvailability(output) != this.availability[i])
                {
                    this.availability[i] = !this.availability[i];
                    StringBuilder stringBuilder = new StringBuilder("{ \"key\": \"update\", \"values\": { \"spot\": ").Append(i + 1).Append(", \"available\": ").Append(this.availability[i]?1:0).Append(" } }");

                    if (this.ws.IsAlive) {
                        this.ws.Send(stringBuilder.ToString());
                    }
                }

            }

            
        }

        public ArduinoSerialReader(string portName)
        {
            _serialPort = new SerialPort(portName)
            {
                BaudRate = 19200
            };
            _serialPort.Open();
            _serialPort.DataReceived += SerialPort_DataReceived;

            this.ws = new WebSocket("wss://websocket.parkunt.tech", "feed");
            this.ws.Connect();
        }

        void SerialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            
            String str = _serialPort.ReadLine();

            Console.WriteLine(str);

            EvaluateInputs(str);
        }

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
            }
        }
    }


}

