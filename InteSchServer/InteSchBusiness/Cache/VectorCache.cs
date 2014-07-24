using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Cache 
{
    class VectorCache : InteSchCache
    {
        public VectorCache(string _server)
            : base(_server)
        {
        }

        public VectorCache(string[] _servers)
            : base(_servers)
        {
        }

        new protected object GetCache(string keyid)
        {
            return base.GetCache(keyid + "-vector-");
        }

        new protected void SetCache(string keyid, object Value)
        {
            base.SetCache(keyid + "-vector-", Value);
        }

        public long GetInline(string key)
        {
            object inline = GetCache("inl" + key);
            if (inline == null)
                return 0;
            return (long)inline;
        }

        public void SetInline(string key, long inline)
        {
            SetCache("inl" + key, inline);
        }

        public object GetMapCheckPoints(string map_id)
        {
            return this.GetCache("cpids" + map_id);
        }

        public void SetMapCheckPoints(string map_id, string[] cp_ids)
        {
            SetCache("cpids" + map_id, cp_ids);
        }
    }
}
