using System;
using System.IO;
using System.Net;
using System.Text;

namespace Update_jobapplication
{
    public static class Client
    {
        public static HttpWebRequest CreateWebRequest(string url, string method, string token)
        {
            
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(url);
            //webRequest.Headers.Clear();

            webRequest.ContentType = "application/json";
            webRequest.Method = method;
            string headerBr = "authorization:Bearer {0}";
            string header = string.Format(headerBr, token);
            webRequest.Headers.Add(header);

            return webRequest;
        }
        public static void InsertRequest(HttpWebRequest webRequest, string request)
        {
            var data = Encoding.UTF8.GetBytes(request);
            webRequest.ContentLength = data.Length;

            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
        public static void PutJobApplication(string url ,string token ,string request)
        {
            HttpWebRequest webRequestPut = CreateWebRequest(url, "put", token);
            InsertRequest(webRequestPut, request);
            IAsyncResult asyncResultPut = webRequestPut.BeginGetResponse(null, null);

            asyncResultPut.AsyncWaitHandle.WaitOne();


            using (HttpWebResponse responseput = (HttpWebResponse)webRequestPut.EndGetResponse(asyncResultPut))
            {
                using (StreamReader rd = new StreamReader(responseput.GetResponseStream()))
                {
                    //var results = JsonConvert.DeserializeObject(rd.ReadToEnd()).ToString();


                }
            }
        }
    }
}
