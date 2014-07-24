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
        public RowCache(string _id)
            : base(_id)
        {

        }

        new protected object GetCache(string keyid)
        {
            return base.GetCache(keyid + "-row-");
        }

        new protected void SetCache(string keyid, object Value)
        {
            // todo: set the attribute value to cache server.
            base.SetCache(keyid + "-row-", Value);
        }

        // 这里存的应该是数据行
        public object GetStudent(string keyid)
        {
            // todo: 学生信息缓存的存取规则
            return this.GetCache("stu" + keyid);
        }

        public void SetStudent(string keyid, InteSchDataSet.studentsRow value)
        {
            this.SetCache("stu" + keyid, value);
        }

        public object GetMap(string key)
        {
            return this.GetCache("map" + key);
        }

        public void SetMap(string keyid, InteSchDataSet.mapsRow value)
        {
            this.SetCache("map" + keyid, value);
        }

        public object GetCheckPoints(string id)
        {
            return this.GetCache("cp" + id);
        }

        public void SetCheckPoints(string id, object value)
        {
        }
    }
}
