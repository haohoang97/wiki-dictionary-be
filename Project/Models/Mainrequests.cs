using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Mainrequests
    {
        public List<requests> requests { get; set; }
    }

    public class requests
    {
        public image image { get; set; }
        public List<features> features { get; set; }
    }

    public class image
    {
        public string content { get; set; }
    }
    public class features
    {
        public string type { get; set; }
    }
}
