using SmartMailbox.Analysis;
using SmartMailbox.Inputs;
using SmartMailbox.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox
{
    class Program
    {
        static IOutputComponent[] GetOutputComponents() {
            return new IOutputComponent[] { new EmailOutput() };
        }

        static void Main(string[] args)
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

            foreach(IOutputComponent outputComponent in GetOutputComponents())
            {
                outputComponent.HandleClassification(classification);
            }

            Console.ReadLine();
        }
    }
}
