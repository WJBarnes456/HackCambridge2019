using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace SmartMailbox.Outputs
{
    class SerialOutput : IOutputComponent
    {
        private SerialDevice device;

        public SerialOutput(SerialDevice device)
        {
            this.device = device;
        }

        public void HandleClassification(Classification classification)
        {
            if(classification.isSpam)
            {
                device.Write(new byte[] { ((byte) 1) });
            } else
            {
                device.Write(new byte[] { ((byte) 2) });
            }
        }
    }
}
