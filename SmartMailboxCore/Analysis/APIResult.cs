using Newtonsoft.Json.Linq;
using SmartMailbox;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

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

        private int getArea(string boundingBoxString)
        {
            if(boundingBoxString == null)
            {
                return 0;
            }

            string[] parts = boundingBoxString.Split(",");
            if(parts.Length != 4)
            {
                return 0;
            } else
            {
                return int.Parse(parts[2]) * int.Parse(parts[3]);
            }
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

            string mainText = "";
            double bestArea = double.NegativeInfinity;

            foreach (var regionToken in OCRResult["regions"])
            {
                foreach(var lineToken in regionToken["lines"])
                {
                    int area = getArea(lineToken.Value<string>("boundingbox"));
                    if(area > bestArea)
                    {
                        bestArea = area;
                        mainText = String.Join(" ", lineToken["words"].Select(x => x.Value<string>("text")));
                    }
                }
            }

            return new Classification(spamProb > 0.5, FilePath, mainText, bestTag);
        }
    }
}
