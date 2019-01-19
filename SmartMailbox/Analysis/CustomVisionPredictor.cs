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

            public static async Task<double> MakePredictionRequest(string imageFilePath)
            {
                var client = new HttpClient();

                // Request headers - replace this example key with your valid subscription key.
                client.DefaultRequestHeaders.Add("Prediction-Key", "sdfghjk");

                // Prediction URL - replace this example URL with your valid prediction URL.
                string url = "fghjk";

                HttpResponseMessage response;

                // Request body. Try this sample with a locally stored image.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);
                    String jsonRet = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(jsonRet);
                    Console.WriteLine(jsonRet);
                    foreach (var a in jObject["predictions"])
                    {
                        if (a["tagName"].ToString() == "SPAM")
                            return double.Parse(a["probability"].ToString());
                    }
                    //Console.WriteLine(jObject["predictions"][0]["probability"]);
                }
                return 0;
            }
        }

}
