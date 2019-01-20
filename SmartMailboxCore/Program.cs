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
            IImageProvider provider = new MockImageProvider(@"D:\dd.jpg");
            IImageAnalyser analyser = new AzureAnalyser();
            IOutputComponent[] outputComponents = GetOutputComponents();

            string filename = provider.TakeImage();
            Classification classification = analyser.ClassifyImage(filename);
            Console.WriteLine(classification);

            foreach(IOutputComponent outputComponent in outputComponents)
            {
                outputComponent.HandleClassification(classification);
            }

            Console.ReadLine();
        }
    }
}
