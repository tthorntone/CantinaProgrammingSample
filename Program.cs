using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CantinaProgrammingSample
{
    class Program
    {
        static void Main(string[] args)
        {
            view.DisplayTools displayTools = new view.DisplayTools();
            model.SelectorModel selectorFunctionality = new model.SelectorModel();
            controller.Controller controller = new controller.Controller(displayTools, selectorFunctionality);

            controller.BeginProcess();   
        }
    }
}
