using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ParkUNT
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
}
