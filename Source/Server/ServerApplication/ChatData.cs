using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class chatData
    {
        public chatData(string type, Dictionary<int, string> data)
        {
            this.Type = type;
            this.Data = data;
        }

        public string Type { get; set; }
        public Dictionary<int, string> Data { get; set; }
    }
}
