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
        static private IImageProvider provider;
        static private IImageAnalyser analyser;
        static private IOutputComponent[] outputComponents;

        static void Main(string[] args)
        {
            analyser = new AzureAnalyser();

        }
    }
}
