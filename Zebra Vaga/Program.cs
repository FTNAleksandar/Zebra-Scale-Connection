using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zebra_Vaga.Properties;

namespace Zebra_Vaga
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ip = Zebra_Vaga.Properties.Settings.Default.ip;
            Console.WriteLine(ip);
           
            

            Console.ReadKey();
        }

        private SerialPort _serialPort;
        private List<byte> _buffer;
        private AutoResetEvent _autoResetEvent;
        private const int WriteTimeOut = 5;

        private event EventHandler ReceivedDataChanged;

        public Serialport()
        {
            _serialPort = new SerialPort();

            // set PortName, BaudRate etc

            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.DataReceived += ReceiveData;
        }

        private void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            var bytes = _serialPort.BytesToRead;

            byte[] buffer = new byte[bytes];
            if (_serialPort.IsOpen)
            {
                _serialPort.BaseStream.Read(buffer, 0, bytes);
                _buffer.AddRange(buffer);
            }

            ReceivedDataChanged?.Invoke(this, new ReceivedBytesEventArgs(_buffer.ToArray()));
            _buffer.Clear();
        }

        private void SendData(byte[] message, int answerLength)
        {
            _serialPort.ReceivedBytesThreshold = answerLength;
            _serialPort.WriteTimeout = WriteTimeOut;
            _serialPort.Write(message, 0, message.Length);
        }

        public string SendDataCommand()
        {
            if (_serialPort.IsOpen)
            {
                ReceivedDataChanged += InterpretAnswer;
                SendData(message, length);
                if (_autoResetEvent.WaitOne(100))
                {
                    ReceivedDataChanged -= InterpretAnswer;
                    //Data Received and interpreted and send to the caller
                    return _requestAnswer;
                }

                ReceivedDataChanged -= InterpretAnswer;
            }

            return "Connection not open";
        }

        private void InterpretAnswer(object sender, EventArgs e)
        {
            // handle all interpretation
            // Set the event
            _autoResetEvent.Set();
        }
    }
}
