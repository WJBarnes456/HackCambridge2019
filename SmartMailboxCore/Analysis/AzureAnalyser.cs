using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SmartMailbox.Analysis
{
    public class AzureAnalyser : IImageAnalyser
    {
        private Classification parseObject(string filePath, JObject jObject)
        {
            double spamProb = 0;
            double bestTagScore = Double.NegativeInfinity;
            string bestTag = "Post";

            foreach (var tagObject in jObject["predictions"])
            {
                string tagName = tagObject["tagName"].ToString();
                double tagScore = double.Parse(tagObject["probability"].ToString());

                if (tagName == "SPAM")
                {
                    spamProb = tagScore;
                } else if (tagName != "OK" && tagScore > bestTagScore)
                {
                    bestTag = tagName;
                    bestTagScore = tagScore;
                }
            }
            
            return new Classification(spamProb > 0.5, filePath, bestTag);
        }

        public Classification ClassifyImage(string filePath) {
            Task<JObject[]> task = CustomVisionPredictor.MakePredictionRequest(filePath);
            task.Wait();
            
            return parseObject(filePath, task.Result[0]);
        }
    }

}
    