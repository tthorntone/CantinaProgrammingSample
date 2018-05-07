using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CantinaProgrammingSample
{
    class Program
    {
        //Gets called on start.
        static void Main(string[] args)
        {
            //the view
            view.DisplayTools displayTools = new view.DisplayTools();

            //the model
            model.SelectorModel selectorFunctionality = new model.SelectorModel();

            //and the almighty controller
            controller.Controller controller = new controller.Controller(displayTools, selectorFunctionality);

            //as this is on the command line, and no events are needed, we can start right away.
            controller.BeginProcess();   
        }
    }
}
