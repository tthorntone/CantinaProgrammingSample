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
        /// <summary>
        /// Function: Walk through every node, starting at @node, performing @action on each.
        /// </summary>
        /// <param name="node">Node we'd like to walk through. Shouldn't be null...</param>
        /// <param name="action">What we'd  like to do for every node.</param>
        //Output: Nothing
        private void WalkNode(JToken node, Func<JObject, bool> action)//, List<JObject> results)
        {
            if (node == null)
                return;
            else
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

        /// <summary>
        /// Allows us to easily find and strip out the individual selector checks from the hole-sum.
        /// </summary>
        /// <param name="checkList">A list of all the checks that we'll be performing.</param>
        /// <param name="chainValue">The selector itself, passed as reference since we strip each match.</param>
        /// <param name="identifier">every check is under a different category, which this defines</param>
        //Output: Nothing.
        private void LoadCheck(List<string> checkList, ref string chainValue, char identifier)
        {
            int workingIndex;
            int nextIndex;

            while ((workingIndex = chainValue.IndexOf(identifier)) >= 0)
            {
                nextIndex = chainValue.IndexOfAny(new char[] { '.', '#' }, workingIndex + 1);
                string val = chainValue.Substring(workingIndex + 1, (nextIndex >= 0) ? nextIndex - workingIndex - 1 : chainValue.Length - workingIndex - 1);
                checkList.Add(val);
                chainValue = chainValue.Substring(0, workingIndex) + chainValue.Substring(workingIndex + val.Length + 1);
            }
        }

        /// <summary>
        /// To recursively walk through the @selector's references within the @startNodes, and save all matches to @values. 
        /// Selectors may be more than one, depending on how far down the user wants to go. EX: "Input#test.cool .Wow.cool Test"
        /// </summary>
        /// <param name="selectors">Each selector, separated by a space, that lets us get move down the JSON tree.</param>
        /// <param name="startNodes">The nodes we'll be running the current selector on. May be more than one, depending on last iterative result.</param>
        /// <param name="values">This is where we store the final results, that the user wants!</param>
        public void ChainSelector(Queue<string> selectors, IEnumerable<JObject> startNodes, List<JToken> values)//, bool start = false)
        {
            //oh cool, we got through all the selectors, don't have to go any deeper!
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

                //checking if the 'class check (StackView)' elements of the selector match the current JSON node.
                foreach (var classCheck in classChecks)
                {
                    //there should only be one (for now)!
                    token = n["class"];
                    if (token == null || token.Type != JTokenType.String || !token.Value<string>().Equals(classCheck))
                    {
                        return false;
                    }
                }

                //checking if the 'class names check (.container)' elements of the selector match the current JSON node.
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

                //checking if the 'identifier check (#videoMode)' elements of the selector match the current JSON node.
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

        /// <summary>
        /// Used to load the source, gotten from the interface.
        /// </summary>
        /// <returns>an asynchronous task that will return the source is successful.</returns> 
        public async Task<string> LoadSource()
        {
            return await SourceRetrieval.GetSource();
        }

        /// <summary>
        /// Separates the entire selector input into it's separate selector parts.
        /// </summary>
        /// <param name="selector">the entire selector, entered from the user.</param>
        /// <returns> a string array containing each separate group of selectors (each starts back at beginning)</returns>
        public string[] ParseSelectors(string selector)
        {
            return selector.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Helper function for parsing the source. Separated as we may want to do this using another library.
        /// </summary>
        /// <param name="source">the source in its original string format.</param>
        /// <returns>a representation of the source that is easy to work with.</returns>
        public JObject ParseJSONSource(string source)
        {
            return JObject.Parse(source);
        }
    }
}
