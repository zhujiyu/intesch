using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Error
{
    class ParameterError:Exception
    {
        public ParameterError(string msg) : base(msg) { }
    }
}
