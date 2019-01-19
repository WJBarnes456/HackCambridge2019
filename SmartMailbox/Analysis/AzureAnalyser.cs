using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace SmartMailbox.Analysis
{
    public class AzureAnalyser : IImageAnalyser
    {
        private const string SouthCentralUsEndpoint = "https://southcentralus.api.cognitive.microsoft.com";

        public Classification ClassifyImage(string filePath) {
            Classification cr = new Classification();
            Task<double> task = CustomVisionPredictor.MakePredictionRequest(filePath);
            task.Wait();
            double res = task.Result;
            if (res> 0.5) cr.isSpam = true;
            else cr.isSpam = false;
            return cr;
        }
    }

}
    