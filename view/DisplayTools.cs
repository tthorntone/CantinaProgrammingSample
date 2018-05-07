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
        //Input: Nada
        //Output: a selector. We may want to add verification of some sort. 
        //Function: Prompts and gets selector from the stdin/user.
        public string GetSelector()
        {
            Console.WriteLine("Please Enter A Selector (CTRL+C to quit):");
            return Console.ReadLine();
        }

        //Input: the result of the JSON selector match, and a count for which one we are on.
        //Output: Nope
        //Function: Properly display to the screen one of the matches we found!
        public void ShowResults(string result, int count)
        {
            Console.WriteLine(string.Format("\nResult #{0}:\n{1}\n\n", count, result));
        }

        /* Stripped, because it'd be better to not have that JSON library here if we don't need to...
        private void ShowResults(JToken result, int count)
        {
            ShowResults(result.ToString(), count);
        }*/

        //Input: Nothing
        //Output: Still Nothing...
        //Function: to let the user know that we are getting the source file. Might want to expand into a progress bar-like system, eventually.
        public void GettingSource()
        {
            Console.WriteLine("Getting Source-- Please Wait...");
        }
    }
}
