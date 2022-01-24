using System.IO;
using System.Net;

namespace jwt_user
{
    public static class ApiRequester
    {
        public static void SetToken(string token)
        {
            Token = token;
        }

        private static string Token = string.Empty;

        private static void AddAuthHeader(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                request.Headers.Add($"authorization: Bearer {Token}");
            }
        }

        public static string Get(string route)
        {
            string result = string.Empty;
            string url = Config.GetApiUrl() + route;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            AddAuthHeader(request);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }

        public static string Post(string route, string json)
        {
            string result = string.Empty;
            string url = Config.GetApiUrl() + route;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";

            AddAuthHeader(request);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                writer.Write(json);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }

        public static string Put(string route, string json)
        {
            string result = string.Empty;
            string url = Config.GetApiUrl() + route;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "PUT";

            AddAuthHeader(request);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                writer.Write(json);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }

        public static string Delete(string route)
        {
            string result = string.Empty;
            string url = Config.GetApiUrl() + route;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Method = "DELETE";

            AddAuthHeader(request);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }

    }
}
