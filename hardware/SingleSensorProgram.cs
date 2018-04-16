using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using WebSocketSharp;


namespace ParkUNTSingleArduino
{
    class Program
    {
        static void Main(string[] args)
        {

            ListComPorts();
            Console.WriteLine("Please enter the COM port to use: ");
            var line = Console.ReadLine();
            Console.WriteLine("Please enter the spot to be represented: ");
            Int32.TryParse(Console.ReadLine(), out int spot);
            var comreader = new ArduinoSerialReader(line, spot);
            Console.ReadLine();
        }

        /// <summary>
        /// Lists all com ports that are available to the computer
        /// </summary>
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

    /// <summary>
    /// A Class to handle the reading of an Arduino over Serial connection and Update the Websockets that a spot is connected to 
    /// </summary>
    public class ArduinoSerialReader : IDisposable
    {
        private int _spot;
        private bool _firstRun = true;
        private double _runningAverage;
        private SerialPort _serialPort;
        private WebSocket _ws;

        private bool availability = true;

        /// <summary>
        ///  Initializes the Serial and Websocket and assigns the event handler for the serial port
        /// </summary>
        /// <param name="portName">The COM port the connection will happen over</param>
        /// <param name="spot">The spot that the websocket will update</param>
        public ArduinoSerialReader(string portName, int spot)
        {

            this._spot = spot;

            _serialPort = new SerialPort(portName)
            {
                BaudRate = 19200
            };
            _serialPort.Open();
            _serialPort.DataReceived += SerialPort_DataReceived;


            this._ws = new WebSocket("wss://websocket.parkunt.tech", "feed");
            this._ws.Connect();

        }


        /// <summary>
        /// Evaluates the availability of a spot based upon sensor values
        /// </summary>
        /// <param name="newValue">A value to evaluate on</param>
        public static bool EvaluateAvailability(int newValue)
        {

            return newValue > 300 ? true : false;
        }


        /// <summary>
        /// Evaluates the Input <paramref name="str"/> with regards to wether to update the Websocket 
        /// </summary>
        /// <param name="str"></param>
        public void EvaluateInputs(String str)
        {

            string stringArray = str;

            //Parse out the input value
            Int32.TryParse(stringArray, out int output);

            //If its the first run, set the running average
            if (_firstRun)
            {
                this._runningAverage = output;
                this._firstRun = false;
            }

            //If the availability changes, Update the WebSocket
            if (EvaluateAvailability(output) != this.availability)
            {
                this.availability = !this.availability;
                StringBuilder stringBuilder = new StringBuilder("{ \"key\": \"update\", \"values\": { \"spot\": ").Append(this._spot).Append(", \"available\": ").Append(this.availability ? 1 : 0).Append(" } }");

                if (this._ws.IsAlive)
                {
                    this._ws.Send(stringBuilder.ToString());
                }
            }

        }

        /// <summary>
        /// Event handler for when the Serial Port recieves a new line of Data
        /// 
        /// It writes the data to the console and calls <see cref="EvaluateInputs(string)"/>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        void SerialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
           

            String str = _serialPort.ReadLine();

            Console.WriteLine(str);

            EvaluateInputs(str);
        }

        /// <summary>
        /// Disposes of the serial and closes the websocket 
        /// </summary>
        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
            }
            if (_ws != null)
            {
                _ws.Close();
            }
        }
    }


}
