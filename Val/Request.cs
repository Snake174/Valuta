using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Val
{
    class Request
    {
        private string URL;
        public string Status { get; set; }
        public string ContentType { get; set; }
        private List<string> urlParams = new List<string>();

        public Request(string url)
        {
            URL = url;
        }

        public void AddParam(string key, string value)
        {
            urlParams.Add($"{key}={value}");
        }

        public string Send()
        {
            if (urlParams.Count > 0)
            {
                URL += "?" + string.Join("&", urlParams);
            }

            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";

            //string postData = "";
            byte[] byteArray = Encoding.UTF8.GetBytes("");

            request.ContentType = ContentType;
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = null;

            try
            {
                response = request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine("Request.Send: " + e.Message);

                return e.Message;
            }

            Status = ((HttpWebResponse)response).StatusDescription;

            string responseFromServer = "";

            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream, Encoding.Default);
                responseFromServer = reader.ReadToEnd();
            }

            response.Close();

            return responseFromServer;
        }
    }
}
