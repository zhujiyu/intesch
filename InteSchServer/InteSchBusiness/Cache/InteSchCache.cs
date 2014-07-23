using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchBusiness.Cache
{
    public class InteSchCache
    {
        private string student_id = null;

        public string Student_ID
        {
            get { return this.student_id; }
        }

        public InteSchCache() { }

        public InteSchCache(string student_id)
        {
            this.student_id = student_id;
        }

        protected object GetCache(string keyid)
        {
            return null;
        }

        protected int SetCache(string keyid, object Value)
        {
            // todo: set the attribute value to cache server.
            return 0;
        }
    }
}
