using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.view
{
    interface IDisplayTools
    {
        string GetSelector();
        void ShowResults(string result, int count);
        void GettingSource();
    }
}
