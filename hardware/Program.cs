using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text.RegularExpressions;

namespace ParkUNTConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var comreader = new ArduinoSerialReader("COM5");

            //http://parkunt.tech/api.php?spot-id=1&available=1

            for (int i = 1; i < 7; i++)
            {
                ArduinoSerialReader.UpdateSite(i, true);

            }

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
        //static int[] convertedItems= { 0,0,0,0,0,0};

        /// <summary>
        /// Updates the ParkUNT Site with Data from the Arduino
        /// </summary>
        /// <param name="spot">The Spot to update, indexed at 1</param>
        /// <param name="available">True if the spot is available </param>
        public static void UpdateSite(int spot, bool available)
        {
            StringBuilder stringBuilder =new StringBuilder( "http://parkunt.tech/api.php?spot-id=");
            stringBuilder.Append(spot);
            stringBuilder.Append("&available=");
            stringBuilder.Append(available ? 1 : 0);
            var request = (HttpWebRequest)WebRequest.Create(stringBuilder.ToString());

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        /// <summary>
        /// Evaluates the availability of a spot based upon sensor values
        /// </summary>
        /// <param name="value">A value to evaluate on</param>
        public static bool EvaluateAvailability(int value)
        {
            return value > 200 ? true : false;
        }
        
       public static void EvaluateInputs(String str)
        {
            string[] stringArray = str.Split('\t');
            for (int i = 0; i < stringArray.Length; i++)
            {
                Int32.TryParse(stringArray[i], out int output);

                UpdateSite(i + 1, EvaluateAvailability(output));
            }

            //int[] convertedItems = Array.ConvertAll<string, int>(str.Split('\t'), int.Parse);
            //Int32.TryParse(stringArray[0], out convertedItems[0]);
            //Int32.TryParse(stringArray[1], out convertedItems[1]);
            //Int32.TryParse(stringArray[2], out convertedItems[2]);
            //Int32.TryParse(stringArray[3], out convertedItems[3]);
            //Int32.TryParse(stringArray[4], out convertedItems[4]);
            //Int32.TryParse(stringArray[5], out convertedItems[5]);

            //UpdateSite(1, EvaluateAvailability(convertedItems[0]));
            //UpdateSite(2, EvaluateAvailability(convertedItems[1]));
            //UpdateSite(3, EvaluateAvailability(convertedItems[2]));
            //UpdateSite(4, EvaluateAvailability(convertedItems[3]));
            //UpdateSite(5, EvaluateAvailability(convertedItems[4]));
            //UpdateSite(6, EvaluateAvailability(convertedItems[5]));

        }

        public ArduinoSerialReader(string portName)
        {
            _serialPort = new SerialPort(portName);
            _serialPort.BaudRate = 19200;
            _serialPort.Open();
            _serialPort.DataReceived += serialPort_DataReceived;
        }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
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

