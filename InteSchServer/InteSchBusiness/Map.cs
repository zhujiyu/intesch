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
    class Map
    {
        private string id = null;

        public string Id
        {
            get { return id; }
        }

        InteSchDataSet.mapsRow data = null;

        public InteSchDataSet.mapsRow Data
        {
            get { return data; }
        }

        public Map(string map_id)
        {
            this.id = map_id;
            this.data = GetFromCache();
        }

        public InteSchDataSet.mapsRow GetFromDB()
        {
            mapsTableAdapter adapter = new mapsTableAdapter();
            InteSchDataSet.mapsDataTable table = adapter.GetDataByID(this.id);

            if (table.Count != 1)
                throw new ContentError("不存在此张地图");

            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            cache.SetMap(id, table[0]);

            return table[0];
        }

        public InteSchDataSet.mapsRow GetFromCache()
        {
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object map = cache.GetMap(id);

            if (map == null)
                return GetFromDB();

            if (map is InteSchDataSet.mapsRow)
                return map as InteSchDataSet.mapsRow;
            else
                throw new Error.InternalError("行缓存里的数据格式不正确");
        }

        public InteSchDataSet.checkpointsDataTable GetCheckPoints()
        {
            VectorCache vcache = new VectorCache(Properties.Settings.Default.memcache);
            object obj = vcache.GetMapCheckPoints(id);
            
            InteSchDataSet.checkpointsDataTable table = null;
            string[] cp_ids = null;

            if (obj != null)
            {
                if (!(obj is string[]))
                    throw new ContentError("缓存数据的格式不正确");
                cp_ids = obj as string[];
                table = new InteSchDataSet.checkpointsDataTable();

                for (int i = 0; i < cp_ids.Length; i++)
                {
                    CheckPoint cp = new CheckPoint(cp_ids[i]);
                    table.AddcheckpointsRow(cp.Data);
                }
            }
            else
            {
                checkpointsTableAdapter cpAda = new checkpointsTableAdapter();
                table = cpAda.GetDataByMapID(id);

                if (table.Count > 0)
                {
                    cp_ids = new string[table.Count];
                    for (int i = 0; i < table.Count; i++)
                        cp_ids[i] = table[i].id;
                    vcache.SetMapCheckPoints(id, cp_ids);
                }
            }

            return table;
        }
    }
}
