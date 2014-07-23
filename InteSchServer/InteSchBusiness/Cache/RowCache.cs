using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InteSchBusiness.Error;
using InteSchBusiness.DataSet;

namespace InteSchBusiness.Cache
{
    class RowCache : InteSchCache
    {
        public RowCache(string student_id)
            : base(student_id)
        {

        }

        // 这里存的应该是数据行
        public object GetStudent(string keyid)
        {
            // todo: 学生信息缓存的存取规则
            return this.GetCache(this.Student_ID);
        }

        public void SetStudent(string keyid, InteSchDataSet.studentsRow value)
        {
            this.SetCache(keyid, value);
        }
    }
}
