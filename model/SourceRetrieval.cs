using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CantinaProgrammingSample.model
{
    class SourceRetrieval
    {
        //Modified JSON Source URL to allow direct download. However, if we incorporate the GitHub API like we wanted to, this will have to get changed back.
        private const string OWNER = "quetoo";
        private const string REPO = "jdolan";
        private const string PATH = "src/cgame/default/ui/settings/SystemViewController.json";
        private const string BRANCH = "master";

        public SourceRetrieval()
        {

        }

        //Input: None
        //Output: an asynchronous Task that will return the source if successful.
        public static async Task<string> GetSource()
        {
            string source = null;
            string localPath = string.Format("/repos/{0}/{1}/contents/{2}", REPO, OWNER, PATH);

            using (var client = new HttpClient() { BaseAddress = new Uri("https://api.github.com") })
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CantinaProgrammingSample");

                try
                {
                    string githubSource = await client.GetStringAsync(string.Format(@"{0}?ref={2}", localPath, PATH, BRANCH));

                    JToken jObject = JObject.Parse(githubSource);

                    switch(jObject.Value<string>("encoding"))
                    {
                        case "base64":
                            source = Encoding.UTF8.GetString(Convert.FromBase64String(jObject.Value<string>("content")));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't Get Source...");
                }
            }

            return source;
        }
    }
}
