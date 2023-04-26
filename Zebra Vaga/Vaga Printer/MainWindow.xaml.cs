using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vaga_Printer.Properties;

namespace Vaga_Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;         //<-- declares a SerialPort Variable to be used throughout the form
        internal static string FilePath = "";
        private string scaleWeight = "";
        public MainWindow()
        {
            InitializeComponent();

            string[] portNames = SerialPort.GetPortNames();     //<-- Reads all available comPorts
            foreach (var portName in portNames)
            {
                cbPort.Items.Add(portName);                  //<-- Adds Ports to combobox
            }
            cbPort.SelectedIndex = 0;

        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog browseLog = new OpenFileDialog();
            browseLog.Filter = "All Files (*.*)|*.*";
            browseLog.FilterIndex = 1;
            browseLog.Multiselect = true;

            browseLog.ShowDialog();

            if (!String.IsNullOrEmpty(browseLog.FileName))
                FilePath = browseLog.FileName;
            else
            {
                MessageBox.Show("Izaberi file ponovo!", "ERROR", MessageBoxButton.OK);
                FilePath = string.Empty;
            }
            labelFileName.Content = browseLog.SafeFileName.Replace(".prn","");
            
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            Zebra zebra = new Zebra();
            string content = "";

            if (String.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("Izaberi etiketu!!", "ERROR", MessageBoxButton.OK);
                return;
            }
            //TODO VAGA TEZINA
            content = zebra.GetLabel(FilePath);
            content = zebra.ReplaceTextWeight(content,scaleWeight);

            try
            {

                content = zebra.ReplaceTextDate1(content, txtDatum1.SelectedDate.Value.ToString("dd.MM.yyyy"));


                if (int.TryParse(txtBoxDana.Text, out int dana) && int.TryParse(txtBoxMeseci.Text, out int meseci))
                {

                    DateTime datum2 = txtDatum1.SelectedDate.Value.AddDays(dana);
                    datum2 = datum2.AddMonths(meseci);
                    content = zebra.ReplaceTextDate2(content, datum2.ToString("dd.MM.yyyy"));
                }
                else
                {
                    MessageBox.Show("Unesite validne datume!!", "ERROR", MessageBoxButton.OK);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unesite validne datume!!", "ERROR", MessageBoxButton.OK);
            }


            if (int.TryParse(txtBoxCount.Text,out int count) && count > 0)
            {
                
                   try
                   {
                        for (int i = 0; i < count; i++)
                        { 
                            zebra.SendToPrinter(content,Settings.Default.ip,Settings.Default.port);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Problem sa slanjem na sampac!\n" + ex.Message, "ERROR", MessageBoxButton.OK);
                    }
            }
            else
            {
                MessageBox.Show("Unesite validnu kolicinu!", "ERROR", MessageBoxButton.OK);
            }
        }

        private void txtBoxMeseci_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxMeseci.Text = "";
        }

        private void txtBoxDana_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxDana.Text = "";
        }

        private void txtBoxCount_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxCount.Text = "";
        }

        private void btnScale_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();
            if (_serialPort != null)
                _serialPort.Dispose();
            //<-- End of Block

            _serialPort = new SerialPort(cbPort.Text, Settings.Default.BaudRate, Parity.None, 8, StopBits.One);       //<-- Creates new SerialPort using the name selected in the combobox
            _serialPort.DataReceived += SerialPortOnDataReceived;                                       //<-- this event happens everytime when new data is received by the ComPort
            _serialPort.Open();                                                                          //<-- make the comport listen
            lblPort.Content = "Slusam na " + _serialPort.PortName + "...\r\n";
        }
        private void getScaleData(string data)
        {

            

            
        }

        private delegate void Closure();
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {

            //if(InvokeRequired)     //<-- Makes sure the function is invoked to work properly in the UI-Thread
          //  Dispatcher.BeginInvoke(new Closure(() => { SerialPortOnDataReceived(sender, serialDataReceivedEventArgs); }));     //<-- Function invokes itself
                                                                                                                               //else
                                                                                                                               // {
            int dataLength = _serialPort.BytesToRead;

            byte[] data = new byte[dataLength];

            int nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;
            string str = System.Text.Encoding.UTF8.GetString(data);

            scaleWeight = str.Substring(3, 18);

            this.Dispatcher.Invoke(() =>
            {
                txtVaga.Text = scaleWeight;
            });
        }
    }
}
