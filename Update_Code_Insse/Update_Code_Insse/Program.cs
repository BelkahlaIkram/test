
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;


namespace Update_Code_Insse
{
    class Program
    {
        static void Main(string[] args)
        {
            String urlListOfferJobBoardIdst = @"..\ListCodeInsse.json";

            var serializert = new DataContractJsonSerializer(typeof(object));
            ElementResult elementResultt = new ElementResult();
            using (var jsonStream = File.OpenText(urlListOfferJobBoardIdst))
            {
                string jsont = jsonStream.ReadToEnd();
                elementResultt = JsonConvert.DeserializeObject<ElementResult>(jsont);

            }

            foreach (var item in elementResultt.data)
            {
                //var convert = EncryptionHelper.Encrypt64(item);
                string url = string.Format("https://geo.api.gouv.fr/communes?code={0}", item);
                HttpWebRequest webRequest = CreateWebRequest(url, "GET");

                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                asyncResult.AsyncWaitHandle.WaitOne();

                using (HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        dynamic results = JsonConvert.DeserializeObject(rd.ReadToEnd());
                        var testf = results.HasValues ? results[0]["nom"] : "";
                        Console.WriteLine(item + ":" + testf);

                    }



                }


            }
        }
        protected static HttpWebRequest CreateWebRequest(string url, string method)
        {

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(url);
            //webRequest.Headers.Clear();

            webRequest.ContentType = "application/json";
            webRequest.Method = method;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            //SetBasicAuthHeader(webRequest, "technicals@talentplug.com", "pBpaI0ODRLV74zgTiMlG");
            String username = "technicals@talentplug.com";
            String password = "pBpaI0ODRLV74zgTiMlG";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            webRequest.Headers.Add("Authorization", "Basic " + encoded);


            return webRequest;
        }
    }
    public class ElementResult
    {
        [JsonProperty("data")]
        public List<string> data { get; set; }

    }

}
