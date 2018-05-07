using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.view
{
    class DisplayTools : IDisplayTools
    {
        public string GetSelector()
        {
            Console.WriteLine("Please Enter A Selector (CTRL+C to quit):");
            return Console.ReadLine();
        }

        public void ShowResults(string result, int count)
        {
            Console.WriteLine(string.Format("\nResult #{0}:\n{1}\n\n", count, result));
        }

        private void ShowResults(JToken result, int count)
        {
            ShowResults(result.ToString(), count);
        }

        public void GettingSource()
        {
            Console.WriteLine("Getting Source-- Please Wait...");
        }
    }
}
