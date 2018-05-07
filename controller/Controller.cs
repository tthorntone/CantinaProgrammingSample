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
        //local reference of view
        private view.IDisplayTools displayTools;

        //local reference of model
        private model.ISelectorModel selectorModel;

        public Controller(view.IDisplayTools displayTools, model.ISelectorModel selectorModel)
        {
            this.displayTools = displayTools;
            this.selectorModel = selectorModel;
        }
        
        //this gets called on start, and ties together the view and the model functionality together.
        //we'll get the source, parse it when done, get a selector from input and run it through said source.
        public void BeginProcess()
        {
            Task<string> sourceTask = selectorModel.LoadSource();

            displayTools.GettingSource();

            sourceTask.Wait();

            string source = sourceTask.Result;
            JObject startNode = selectorModel.ParseJSONSource(source);

            string inputSelector;

            while ((inputSelector = displayTools.GetSelector()) != null) //keep getting selectors, foreverrr. 
            {
                int i = 1;
                string[] selectors = selectorModel.ParseSelectors(inputSelector); //we need to parse 'em, because we allows commas as to get more than one group!

                foreach (var selector in selectors)
                {
                    var startNodes = new List<JObject>();
                    startNodes.Add(startNode);

                    List<JToken> selectedResults = new List<JToken>();

                    selectorModel.ChainSelector(new Queue<string>(selector.Split(' ')), startNodes, selectedResults); //this is where all the good stuff happens.

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
