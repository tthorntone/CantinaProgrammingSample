using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CantinaProgrammingSample
{
    class Program
    {

        private const string JSON_FILE = "https://raw.githubusercontent.com/jdolan/quetoo/master/src/cgame/default/ui/settings/SystemViewController.json";

        private static void WalkNode(JToken node, Func<JObject, bool> action)//, List<JObject> results)
        {
            if (node.Type == JTokenType.Object)
            {

                if (!action((JObject)node))
                    foreach (JProperty child in node.Children<JProperty>())
                    {
                        WalkNode(child.Value, action);
                    }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    WalkNode(child, action);
                }
            }
        }

        private static void chainSelector(Queue<string> selectors, IEnumerable<JObject> startNodes, List<JToken> values)//, bool start = false)
        {
            if (selectors.Count <= 0)
            {
                if (startNodes != null)
                    values.AddRange(startNodes);

                return;
            }

            string chainValue = selectors.Dequeue();

            List<string> classChecks = new List<string>();
            List<string> classNamesChecks = new List<string>();
            List<string> identifierChecks = new List<string>();

            int workingIndex;
            int nextIndex;

            while ((workingIndex = chainValue.IndexOf('.')) >= 0)
            {
                nextIndex = chainValue.IndexOfAny(new char[] { '.', '#' }, workingIndex + 1);
                string val = chainValue.Substring(workingIndex + 1, (nextIndex >= 0) ? nextIndex - workingIndex - 1 : chainValue.Length - workingIndex - 1);
                classNamesChecks.Add(val);
                chainValue = chainValue.Substring(0, workingIndex) + chainValue.Substring(workingIndex + val.Length + 1);
            }

            while ((workingIndex = chainValue.IndexOf('#')) >= 0)
            {
                nextIndex = chainValue.IndexOf('#', workingIndex + 1);
                string val = chainValue.Substring(workingIndex + 1, (nextIndex >= 0) ? nextIndex - workingIndex - 1 : chainValue.Length - workingIndex - 1);
                identifierChecks.Add(val);
                chainValue = chainValue.Substring(0, workingIndex) + chainValue.Substring(workingIndex + val.Length + 1);
            }

            if (!string.IsNullOrWhiteSpace(chainValue))
                classChecks.Add(chainValue);

            List<JObject> foundMatches = new List<JObject>();

            Func<JObject, bool> walkAction = (n) =>
            {
                JToken token = null;

                foreach (var classCheck in classChecks)
                {
                    //there should only be one (for now)!
                    token = n["class"];
                    if (token == null || token.Type != JTokenType.String || !token.Value<string>().Equals(classCheck))
                    {
                        return false;
                    }
                }

                foreach (var classNamesCheck in classNamesChecks)
                {
                    token = n["classNames"];

                    if (token == null || token.Type != JTokenType.Array || ((JArray)token).FirstOrDefault((x) =>
                    {
                        return x.Type == JTokenType.String && x.Value<string>().Equals(classNamesCheck);
                    }) == null)
                    {
                        return false;
                    }
                }

                foreach (var identifierCheck in identifierChecks)
                {
                    //there should only be one (for now)!

                    if (!n.TryGetValue("control", out token))
                        token = n;

                    if (!((JObject)token).TryGetValue("identifier", out token) || token.Type != JTokenType.String || !token.Value<string>().Equals(identifierCheck))
                    {
                        return false;
                    }
                }

                //chainSelector(selectors, n, values);
                foundMatches.Add(n);

                return true;
            };

            foreach (var startNode in startNodes)
            {
                //List<JObject> foundNodes = new List<JObject>();
                WalkNode((JObject)startNode, walkAction);
            }

            chainSelector(selectors, foundMatches, values);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Please Enter A Selector (CTRL+C to quit):");

            using (var client = new HttpClient())
            {
                try
                {
                    var jsonResult = client.GetStringAsync(JSON_FILE).Result;

                    JObject startNode = JObject.Parse(jsonResult);

                    //JObject startNode = JObject.Parse(Resources.jsonFile);

                    string s = Console.ReadLine();

                    while (s != null)
                    {
                        int i = 1;
                        var selectors = s.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var selector in selectors)
                        {
                            var startNodes = new List<JObject>();
                            startNodes.Add(startNode);

                            List<JToken> selectedResults = new List<JToken>();

                            chainSelector(new Queue<string>(selector.Split(' ')), startNodes, selectedResults);

                            foreach (var result in selectedResults)
                            {
                                Console.WriteLine(string.Format("\nResult #{0}:\n{1}\n\n", i, result.ToString()));
                                i++;
                            }
                        }

                        Console.WriteLine("Please Enter A Selector (CTRL+C to quit):");
                        s = Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
