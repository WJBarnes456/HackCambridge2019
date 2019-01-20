using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox
{
    public struct Classification
    {
        public Classification(bool isSpam, string imageFileName, string mainText, string description, double confidence)
        {
            this.isSpam = isSpam;
            this.imageFileName = imageFileName;
            this.mainText = mainText;
            this.description = description;
            this.confidence = confidence;
        }

        public bool isSpam { get; private set; }

        public string imageFileName { get; private set; }

        public string mainText { get; private set; }

        public string description { get; private set; }

        public double confidence { get; private set; }

        public override string ToString()
        {
            return "Classification " + (isSpam ? "" : "NOT ") + "SPAM, fileName=" + imageFileName + ", description=" + description + " with confidence " + confidence + ", mainText=" + mainText;
        }
    }
}
