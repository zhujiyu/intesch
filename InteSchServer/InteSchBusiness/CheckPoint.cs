using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InteSchBusiness.Error;
using InteSchBusiness.Cache;
using InteSchBusiness.DataSet;
using InteSchBusiness.DataSet.InteSchDataSetTableAdapters;

namespace InteSchBusiness
{
    class CheckPoint
    {
        private string id = null;

        private InteSchDataSet.checkpointsRow data = null;

        public InteSchDataSet.checkpointsRow Data
        {
            get { return data; }
        }

        public CheckPoint(string cpid)
        {
            this.id = cpid;

        }

        public InteSchDataSet.checkpointsRow GetFromDB()
        {
            checkpointsTableAdapter adapter = new checkpointsTableAdapter();
            InteSchDataSet.checkpointsDataTable table = adapter.GetDataByID(id);

            if (table.Count != 1)
                throw new ContentError("不存在此张地图");

            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            cache.SetCheckPoints(id, table[0]);

            return table[0];
        }

        public InteSchDataSet.checkpointsRow GetFromCache()
        {
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object cp = cache.GetCheckPoints(id);

            if (cp == null)
                return GetFromDB();

            if (cp is InteSchDataSet.checkpointsRow)
                return cp as InteSchDataSet.checkpointsRow;
            else
                throw new Error.InternalError("行缓存里的数据格式不正确");
        }

    }
}
