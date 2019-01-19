using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox.Inputs
{
    // Mock image provider -- always returns the given filename, useful for testing
    class MockImageProvider : IImageProvider
    {
        private string filename;

        public MockImageProvider(string filename)
        {
            this.filename = filename;
        }

        public string TakeImage()
        {
            // Assume the filename we were originally given already exists
            return filename;
        }
    }
}
