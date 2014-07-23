using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Error
{
    public class ContentError : Exception
    {
        public ContentError(string msg) : base(msg) { }
    }
}
