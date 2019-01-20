using Newtonsoft.Json.Linq;
using SmartMailbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartMailbox.Analysis
{
    class APIResult
    {
        public string FilePath { get; private set; }
        public JObject PredictionResult { get; private set; }
        public JObject OCRResult { get; private set; }

        public APIResult(string filePath, JObject predictionResult, JObject ocrResult)
        {
            this.FilePath = filePath;
            this.PredictionResult = predictionResult;
            this.OCRResult = ocrResult;
        }

        public Classification ToClassification()
        {
            double spamProb = 0;
            double bestTagScore = Double.NegativeInfinity;
            string bestTag = "Post";

            foreach (var tagObject in PredictionResult["predictions"])
            {
                string tagName = tagObject["tagName"].ToString();
                double tagScore = double.Parse(tagObject["probability"].ToString());

                if (tagName == "SPAM")
                {
                    spamProb = tagScore;
                }
                else if (tagName != "OK" && tagScore > bestTagScore)
                {
                    bestTag = tagName;
                    bestTagScore = tagScore;
                }
            }

            return new Classification(spamProb > 0.5, FilePath, bestTag);
        }
    }
}
