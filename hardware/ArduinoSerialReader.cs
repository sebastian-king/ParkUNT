using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

using WebSocketSharp;

namespace ParkUNT
{
    /// <summary>
    /// A Class to handle the reading of an Arduino over Serial connection and Update the Websockets that a spot is connected to 
    /// </summary>
    public class ArduinoSerialReader : IDisposable
    {
       
        public int Spot{ get; private set; }
        public bool FirstRun { get; private set; } = true;
        public double RunningAverage { get; private set; }
        public double RunningAverageCount { get; private set; } = 0;
        public bool Availability { get; private set; } = true;
        public bool AllowSerial { get; set; } = true;
        public int PercentToChange { get; set; } = 25;

        private SerialPort _serialPort;
        private WebSocket _ws;


        /// <summary>
        ///  Initializes the Serial and Websocket and assigns the event handler for the serial port
        /// </summary>
        /// <param name="portName">The COM port the connection will happen over</param>
        /// <param name="spot">The spot that the websocket will update</param>
        public ArduinoSerialReader(string portName, int spot)
        {

            this.Spot = spot;

            if (AllowSerial)
            {
                if (!SerialPort.GetPortNames().Contains(portName))
                    throw new ArgumentException("Port not found or not available");

                _serialPort = new SerialPort(portName)
                {
                    BaudRate = 19200
                };
                _serialPort.Open();
                _serialPort.DataReceived += SerialPort_DataReceived;
            }

            this._ws = new WebSocket("wss://websocket.parkunt.tech", "feed");
            this._ws.Connect();

        }
        public ArduinoSerialReader(string portName, int spot, bool allowSerial)
        {
            this.AllowSerial = allowSerial;

            this.Spot = spot;

            if (AllowSerial)
            {
                if (!SerialPort.GetPortNames().Contains(portName))
                    throw new ArgumentException("Port not found or not available");

                _serialPort = new SerialPort(portName)
                {
                    BaudRate = 19200
                };
                _serialPort.Open();
                _serialPort.DataReceived += SerialPort_DataReceived;
            }

            this._ws = new WebSocket("wss://websocket.parkunt.tech", "feed");
            this._ws.Connect();

        }


        /// <summary>
        /// Evaluates the availability of a spot based upon sensor values. 
        /// Returns True if the availability has changed and false if it has not
        /// </summary>
        /// <param name="newValue">A value to evaluate on</param>
        public bool EvaluateIfAvailabilityHasChanged(int newValue)
        {

            ///If <paramref name="newValue"/> has a percent change of more than 25% less then switch the availability to unavailable and update running average to <paramref name="newValue"/> and set <see cref="RunningAverageCount"/> to one. 
            ///If <paramref name="newValue"/> has a percent change of more than 25% more then switch the availability to available and update the running average to <paramref name="newValue"/>  and set <see cref="RunningAverageCount"/> to one. 
            ///If <paramref name="newValue"/> is within 25% on either side of running average, then add it to running average and keep curent availability and add one to  <see cref="RunningAverageCount"/>. 

            bool tempAvailibility;

            //If its the first run, set the running average
            if (FirstRun)
            {
                this.RunningAverage = newValue;
                this.FirstRun = false;
            }

            double tempPercentChange = CalculatePercentChange(newValue, this.RunningAverage);

            if (tempPercentChange < -PercentToChange)
            {
                tempAvailibility = false;
                this.RunningAverage = newValue;
                this.RunningAverageCount = 1;
            }
            else if(tempPercentChange > PercentToChange)
            {
                tempAvailibility = true;
                this.RunningAverage = newValue;
                this.RunningAverageCount = 1;
            }
            else
            {
                tempAvailibility = this.Availability;
                this.RunningAverage =((this.RunningAverage*this.RunningAverageCount)+newValue)/(this.RunningAverageCount+1);
                this.RunningAverageCount++;
            }

            //If the availibity is changed, Update the websocket 
            if (tempAvailibility != this.Availability)
            {
                this.Availability = tempAvailibility;
                return true;
                
            }

            return false;
        }


        /// <summary>
        /// Updates the <see cref="WebSocket"/> <see cref="_ws"/> with the availability it is passed. 
        /// </summary>
        /// <param name="availability">The availability of a <see cref="Spot"/> as true for available and false for not available</param>
        public void UpdateWebSocketWithNewAvailability(bool availability)
        {
            this.Availability = availability;

            StringBuilder stringBuilder = new StringBuilder("{ \"key\": \"update\", \"values\": { \"spot\": ").Append(this.Spot).Append(", \"available\": ").Append(this.Availability ? 1 : 0).Append(" } }");

            if (this._ws.IsAlive)
            {
                this._ws.Send(stringBuilder.ToString());
            }

            Console.WriteLine("Updated websocket for spot {0} to {1}", this.Spot, this.Availability);
        }

        /// <summary>
        /// Calculates the Percent change. 
        /// </summary>
        /// <param name="newValue">the new Value to compare to</param>
        /// <param name="oldValue">the old value that is being compared to </param>
        /// <returns>A positive number if greater than, negitive number if less than. If oldValue is zero, it returns positive or negative ∞</returns>
        public static double CalculatePercentChange(double newValue, double oldValue)
        {
            ///Subtracts the <paramref name="newValue"/> from the <paramref name="oldValue"/> and divides the result by <paramref name="oldValue"/>
            //((y2-y1)/y1)*100
           return (((double)newValue - (double)oldValue) / oldValue) * 100.0;
           
        }
        /// <summary>
        /// Calculates the Percent change. 
        /// </summary>
        /// <param name="newValue">the new Value to compare to</param>
        /// <param name="oldValue">the old value that is being compared to </param>
        /// <returns>A positive number if greater than, negitive number if less than. If oldValue is zero, it returns positive or negative ∞</returns>
        public static double CalculatePercentChange(int newValue, double oldValue) => CalculatePercentChange((double)newValue, oldValue);


        /// <summary>
        /// Evaluates the Input <paramref name="str"/> with regards to whether to update the Websocket 
        /// 
        /// If it is the first run, it sets the running average to the first input value.
        /// </summary>
        /// <param name="str"></param>
        public static bool EvaluateInputs(String str, out int output)
        {
            if (str.IsNullOrEmpty())
                throw new ArgumentNullException("No value passed to Evaluate Inputs");
     
            
            //Parse out the input value
           return Int32.TryParse(str, out output);

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
            if (AllowSerial)
            {
                String str = _serialPort.ReadLine();


                Console.WriteLine(str);

                EvaluateInputs(str, out int output);



                //Evaluate if the Availibility has changed and if so, update the websocket
                if (EvaluateIfAvailabilityHasChanged(output))
                    UpdateWebSocketWithNewAvailability(this.Availability);
            }
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
