using SmartMailbox.Analysis;
using SmartMailbox.Inputs;
using SmartMailbox.Outputs;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartMailbox
{
    class Program
    {
        static IOutputComponent[] outputComponents;

        private static bool isSerialAvailable()
        {
            var ports = SerialDevice.GetPortNames();
            bool isTTY = false;
            foreach (var prt in ports)
            {
                Console.WriteLine($"Serial name: {prt}");
                if (prt.Contains("ttyUSB0"))
                {
                    isTTY = true;
                }
            }
            return isTTY;
        }

        static void Main(string[] args)
        {
            while (!isSerialAvailable())
            {
                Console.WriteLine("No ttyUSB0 serial port, retrying in 5 seconds");
                Thread.Sleep(5000);
            }

            Console.WriteLine("Opening serial port");
            SerialDevice mySer = new SerialDevice("/dev/ttyUSB0", BaudRate.B9600);

            mySer.DataReceived += MySer_DataReceived;
            mySer.Open();

            outputComponents = new IOutputComponent[] { new EmailOutput(), new SerialOutput(mySer) };

            while (!Console.KeyAvailable) ;

            mySer.Close();
        }


        private static void MySer_DataReceived(object arg1, byte[] arg2)
        {
            if(arg2[0] == 1)
            {
                string filename;
                {
                    IImageProvider provider = new ShellImageProvider();

                    filename = provider.TakeImage();
                    Console.WriteLine("Image taken, filename " + filename);
                }

                IImageAnalyser analyser = new AzureAnalyser();
                Classification classification = analyser.ClassifyImage(filename);
                Console.WriteLine(classification);

                foreach (IOutputComponent outputComponent in outputComponents)
                {
                    outputComponent.HandleClassification(classification);
                }
            }
        }
    }
}
