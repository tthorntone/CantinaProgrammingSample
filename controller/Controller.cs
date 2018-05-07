using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.controller
{
    class Controller
    {
        private view.IDisplayTools displayTools;
        private model.ISelectorModel selectorFunctionality;

        public Controller(view.IDisplayTools displayTools, model.ISelectorModel selectorFunctionality)
        {
            this.displayTools = displayTools;
            this.selectorFunctionality = selectorFunctionality;
        }

        public void BeginProcess()
        {
            Task<string> sourceTask = selectorFunctionality.LoadSource();

            displayTools.GettingSource();

            sourceTask.Wait();

            string source = sourceTask.Result;
            JObject startNode = selectorFunctionality.ParseJSONSource(source);

            string inputSelector;

            while ((inputSelector = displayTools.GetSelector()) != null)
            {
                int i = 1;
                string[] selectors = selectorFunctionality.ParseSelectors(inputSelector);

                foreach (var selector in selectors)
                {
                    var startNodes = new List<JObject>();
                    startNodes.Add(startNode);

                    List<JToken> selectedResults = new List<JToken>();

                    selectorFunctionality.ChainSelector(new Queue<string>(selector.Split(' ')), startNodes, selectedResults);

                    foreach (var result in selectedResults)
                    {
                        displayTools.ShowResults(result.ToString(), i);
                        i++;
                    }
                }
            }
        }
    }
}
