using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class ResponVision
    {
        public List<Response> responses { get; set; }
    }

    public class Response
    {
        public FullTextAnnotation fullTextAnnotation { get; set; }
    }

    public class FullTextAnnotation
    {
        public string text { get; set; }
    }
}
