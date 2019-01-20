using Newtonsoft.Json.Linq;
using SmartMailboxCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartMailbox.Analysis
{
    class CustomVisionPredictor
    {

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        public static async Task<APIResult> MakePredictionRequest(string imageFilePath)
        {
            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            Console.WriteLine("Image loaded, loading HttpClient");

            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", Keys.PredictionKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Keys.OCRKey);

            // Prediction URL - replace this example URL with your valid prediction URL.
            string predictionURL = Keys.PredictionURL;
            string OCRURL = Keys.OCRURL;


            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                Console.WriteLine("Sending object to Azure");

                Task<HttpResponseMessage> predictionTask = client.PostAsync(predictionURL, content);
                Task<HttpResponseMessage> OCRtask = client.PostAsync(OCRURL, content);
                Task<HttpResponseMessage>[] tasks = { predictionTask, OCRtask };

                await Task.WhenAll(tasks);
                
                Console.WriteLine("Object sent, awaiting response");
                String predictionJsonRet = await predictionTask.Result.Content.ReadAsStringAsync();
                JObject predictionJObject = JObject.Parse(predictionJsonRet);
                Console.WriteLine(predictionJsonRet);

                String OCRJsonRet = await OCRtask.Result.Content.ReadAsStringAsync();
                JObject OCRJObject = JObject.Parse(OCRJsonRet);
                Console.WriteLine(OCRJsonRet);

                return new APIResult(imageFilePath, predictionJObject, OCRJObject);
            }
        }
    }

}
