using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.view
{
    interface IDisplayTools //our interface for the view. Not a lot, but a good start.
    {
        string GetSelector();
        void ShowResults(string result, int count);
        void GettingSource();
    }
}
