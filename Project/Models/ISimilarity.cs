using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    interface ISimilarity
    {
        float GetSimilarity(string string1, string string2);
    }
}