using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Vaga_Printer
{
    /// <summary>
    /// Interaction logic for Podesavanja.xaml
    /// </summary>
    public partial class Podesavanja : Window
    {
        private SerialPort _serialPort;         //<-- declares a SerialPort Variable to be used throughout the form
        private const int BaudRate = 9600;      //<-- BaudRate Constant. 9600 seems to be the scale-units default value
        public Podesavanja()
        {
            InitializeComponent();
            string[] portNames = SerialPort.GetPortNames();     //<-- Reads all available comPorts
            foreach (var portName in portNames)
            {
                combobox1.Items.Add(portName);                  //<-- Adds Ports to combobox
            }
            combobox1.SelectedIndex = 0;
        }
        string ip;

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            //<-- This block ensures that no exceptions happen
            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();
            if (_serialPort != null)
                _serialPort.Dispose();
            //<-- End of Block

            _serialPort = new SerialPort(combobox1.Text, BaudRate, Parity.None, 8, StopBits.One);       //<-- Creates new SerialPort using the name selected in the combobox
            _serialPort.DataReceived += SerialPortOnDataReceived;                                       //<-- this event happens everytime when new data is received by the ComPort
            _serialPort.Open();                                                                          //<-- make the comport listen
            textBox1.Text = "Listening on " + _serialPort.PortName + "...\r\n";
        }

        private delegate void Closure();
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            
           //if(InvokeRequired)     //<-- Makes sure the function is invoked to work properly in the UI-Thread
                Dispatcher.BeginInvoke(new Closure(() => { SerialPortOnDataReceived(sender, serialDataReceivedEventArgs); }));     //<-- Function invokes itself
            //else
           // {
                int dataLength = _serialPort.BytesToRead;

                byte[] data = new byte[dataLength];

                int nbrDataRead = _serialPort.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;
                string str = System.Text.Encoding.UTF8.GetString(data);
                textBox1.Text = str.ToString();
          //  }
        }
    }
}
