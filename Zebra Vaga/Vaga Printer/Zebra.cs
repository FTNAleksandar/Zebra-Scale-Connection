using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vaga_Printer
{
    public class Zebra
    {
        public string FilePath { get; set; }
        public static string Content { get; set; }


        public Zebra()
        {
        }

        //Procita ceo ZPL kod i smesti ga u kontent
        public string GetLabel(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Izaberi etiketu!", "ERROR", MessageBoxButton.OK);
                return null;
            }

            return File.ReadAllText(path);
        }

        public string ReplaceTextWeight(string content , string scaleWeight)
        {
            return content.Replace("TEZINA", scaleWeight);
        }
        public string ReplaceTextDate1(string content, string date)
        {
            return content.Replace("DATUM1", date);
        }
        public string ReplaceTextDate2(string content, string date)
        {
            return content.Replace("DATUM2", date);
        }


        public void SendToPrinter(string content,string ip ,int port)
        {
            using(TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(ip, port);

                using (StreamWriter writer = new StreamWriter(tcpClient.GetStream()))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

    }
}
