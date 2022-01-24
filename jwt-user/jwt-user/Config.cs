using System;
using Newtonsoft.Json;
using System.IO;

namespace jwt_user
{
    public static class Config
    {

        class ConfigData
        {
            public string ApiUrl { get; set; }
        }

        private static ConfigData Data = null;

        public static void ReadConfig()
        {
            string filename = "runtime-config.json";
            filename = LocalFilePath(filename);

            if (!File.Exists(filename))
            {
                throw new InvalidOperationException($"config not found at {filename}");
            }

            string json = File.ReadAllText(filename);

            try
            {
                Data = JsonConvert.DeserializeObject<ConfigData>(json);
                Validate();
            }
            catch (Exception e)
            {
                throw new ArgumentException($"failed to load config at {filename}: {e.Message}");
            }
        }

        private static void Validate()
        {
            if (string.IsNullOrEmpty(Data.ApiUrl)) throw new ArgumentException("missing value for ApiUrl");
        }

        internal static string GetApiUrl()
        {
            if (Data == null) throw new InvalidOperationException();
            return Data.ApiUrl;
        }
        
        private static string LocalFilePath(string filename)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string source = Path.Combine(exeDir, filename);
            return source;
        }
    }
}
