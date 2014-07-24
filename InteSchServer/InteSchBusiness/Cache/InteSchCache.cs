using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Cache
{
    public class InteSchCache
    {
        protected string conn = null;

        public InteSchCache(string _conn)
        {
            this.conn = _conn;
        }

        protected object GetCache(string keyid)
        {
            return null;
        }

        protected void SetCache(string keyid, object Value)
        {
            // todo: set the attribute value to cache server.
        }
    }
}
