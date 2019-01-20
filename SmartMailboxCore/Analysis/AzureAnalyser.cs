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
        
        public Classification ClassifyImage(string filePath) {
            Task<Classification> task = CustomVisionPredictor.MakePredictionRequest(filePath);
            task.Wait();
            // TODO: Handle exceptions from waiting for the task
            
            return task.Result;
        }
    }

}
    