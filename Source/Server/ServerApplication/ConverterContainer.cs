using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class ConverterContainer
    {
        public ConverterContainer(string type, string jSON)
        {
            Type = type;
            JSON = jSON;
        }

        public string Type { get; set; }
        public string JSON { get; set; }
    }
}
