using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.model
{
    class SelectorModel : ISelectorModel
    {
        private void WalkNode(JToken node, Func<JObject, bool> action)//, List<JObject> results)
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

        private void LoadCheck(List<string> checkList, ref string chainValue, char identifier)
        {
            int workingIndex;
            int nextIndex;

            while ((workingIndex = chainValue.IndexOf(identifier)) >= 0)
            {
                nextIndex = chainValue.IndexOf('#', workingIndex + 1);
                string val = chainValue.Substring(workingIndex + 1, (nextIndex >= 0) ? nextIndex - workingIndex - 1 : chainValue.Length - workingIndex - 1);
                checkList.Add(val);
                chainValue = chainValue.Substring(0, workingIndex) + chainValue.Substring(workingIndex + val.Length + 1);
            }
        }

        public void ChainSelector(Queue<string> selectors, IEnumerable<JObject> startNodes, List<JToken> values)//, bool start = false)
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


            LoadCheck(classNamesChecks, ref chainValue, '.');
            LoadCheck(identifierChecks, ref chainValue, '#');

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

            ChainSelector(selectors, foundMatches, values);
        }

        public async Task<string> LoadSource()
        {
            return await SourceRetrieval.GetSource();
        }

        public string[] ParseSelectors(string selector)
        {
            return selector.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        }

        public JObject ParseJSONSource(string source)
        {
            return JObject.Parse(source);
        }
    }
}
