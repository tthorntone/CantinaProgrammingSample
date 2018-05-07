﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CantinaProgrammingSample.model
{
    class SourceRetrieval
    {
        private const string JSON_FILE = "https://raw.githubusercontent.com/jdolan/quetoo/master/src/cgame/default/ui/settings/SystemViewController.json";

        public static async Task<string> GetSource()
        {
            string source = null;

            using (var client = new HttpClient())
            {
                try
                {
                    source = await client.GetStringAsync(JSON_FILE);
                }
                catch (Exception e)
                {
                    throw new Exception("Couldn't Get Source...");
                }
            }

            return source;
        }
    }
}
