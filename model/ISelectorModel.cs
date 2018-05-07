using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.model
{
    interface ISelectorModel
    {
        string[] ParseSelectors(string selector);
        JObject ParseJSONSource(string source);
        Task<string> LoadSource();
        void ChainSelector(Queue<string> selectors, IEnumerable<JObject> startNodes, List<JToken> values);
    }
}
