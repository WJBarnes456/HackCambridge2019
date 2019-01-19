using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox.Inputs
{
    // Runs shell command to take an image. Assumes we're running on Linux and have streamer installed
    // NB: .NET webcam support isn't straightforward on a Linux platform; could substitute when running on Windows

    class ShellImageProvider : IImageProvider
    {
        private const string outputFormat = "jpeg";
        private const string outputResolution = "1920x1080";

        private string GetOutputFilename()
        {
            return "out." + outputFormat;
        }

        public string TakeImage()
        {
            string outputFile = GetOutputFilename();
            string command = "streamer -s " + outputResolution + " -f " + outputFormat + " -o " + outputFile;
            ShellHelper.Bash(command);
            return outputFile;
        }
    }
}
