using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteSchData
{
    public class InteSchCache
    {
        private int student_id = 0;

        public int Student_ID
        {
            get { return this.student_id; }
        }

        public InteSchCache() { }

        public InteSchCache(int student_id)
        {
            this.student_id = student_id;
        }

        protected object GetCache(int keyid)
        {
            return null;
        }

        protected int SetCache(int keyid, object Value)
        {
            // todo: set the attribute value to cache server.
            return 0;
        }
    }
}
