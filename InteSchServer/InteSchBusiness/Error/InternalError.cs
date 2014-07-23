using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Error
{
    public class InternalError : Exception
    {
        public InternalError(string msg) : base(msg) { }
    }
}
