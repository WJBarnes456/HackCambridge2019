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
            return new IOutputComponent[] { };
        }

        static void Main(string[] args)
        {
            IImageProvider provider = new ShellImageProvider();
            IImageAnalyser analyser = new AzureAnalyser();
            IOutputComponent[] outputComponents = GetOutputComponents();

            string filename = provider.TakeImage();
            Classification classification = analyser.ClassifyImage(filename);
            Console.WriteLine(classification.isSpam);

            Console.ReadLine();
        }
    }
}
