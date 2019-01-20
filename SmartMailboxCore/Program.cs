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
        static IImageProvider provider = new ShellImageProvider();
        static IImageAnalyser analyser = new AzureAnalyser();

        static SerialDevice mySer;

        static object runLock = new object();
        static bool taskRunning = false;

        private static bool IsSerialAvailable()
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
            while(!IsSerialAvailable())
            {
                Console.WriteLine("Serial port not available, waiting 5 seconds");
                Thread.Sleep(5000);
            }
            Console.WriteLine("Serial port available, opening");
            mySer = new SerialDevice("/dev/ttyUSB0", BaudRate.B9600);
            mySer.DataReceived += MySer_DataReceived;
            outputComponents = new IOutputComponent[] { new EmailOutput(), new SerialOutput(mySer) };
            mySer.Open();
            while (!Console.KeyAvailable) ;
            mySer.Close();
        }
        
        private static void MySer_DataReceived(object arg1, byte[] arg2)
        {
            Console.WriteLine($"Received: {System.Text.Encoding.UTF8.GetString(arg2)}");
            if(arg2[0] == 49)
            {
                new Task(() =>
                {
                    lock (runLock)
                    {
                        if (taskRunning)
                        {
                            return;
                        }
                        else
                        {
                            taskRunning = true;
                        }
                    }

                    string filename;
                    {
                        filename = provider.TakeImage();
                        Console.WriteLine("Image taken, filename " + filename);
                    }

                    Classification classification = analyser.ClassifyImage(filename);
                    Console.WriteLine(classification);

                    foreach (IOutputComponent outputComponent in outputComponents)
                    {
                        outputComponent.HandleClassification(classification);
                    }

                    lock (runLock)
                    {
                        taskRunning = false;
                    }
                }).Start();
            }
        }
    }
}
