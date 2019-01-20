using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox
{
    public struct Classification
    {
        public Classification(bool isSpam, string imageFileName, string summary)
        {
            this.isSpam = isSpam;
            this.imageFileName = imageFileName;
            this.summary = summary;
        }

        public bool isSpam { get; private set; }

        public string imageFileName { get; private set; }

        public string summary { get; private set; }

        public override string ToString()
        {
            return "Classification " + (isSpam ? "" : "NOT ") + "SPAM, fileName=" + imageFileName + ", summary =" + summary;
        }
    }
}
