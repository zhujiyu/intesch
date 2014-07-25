using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using InteSchBusiness.Error;
using InteSchBusiness.Cache;
using InteSchBusiness.DataSet;
using InteSchBusiness.DataSet.InteSchDataSetTableAdapters;

namespace InteSchBusiness
{
    [DataContract]
    public class Map
    {
        [DataMember]
        private string id = null;

        public string Id
        {
            get { return id; }
        }

        [DataMember]
        private string subject = null;

        public string Subject
        {
            get { return subject; }
        }

        [DataMember]
        private int checkpoints = 0;

        public int Checkpoints
        {
            get { return checkpoints; }
        }

        [DataMember]
        private int edition = 0;

        [DataMember]
        private string name = null;

        [DataMember]
        private string remark = null;

        [DataMember]
        private int semester = 0;

        public void Read(InteSchDataSet.mapsRow map)
        {
            this.id = map.id;
            this.subject = map._subject;
            this.checkpoints = map.checkpoints;
            this.edition = map.edition;
            this.name = map.name;
            this.remark = map.remark;
            this.semester = map.semester;
        }

        public void Write(InteSchDataSet.mapsRow map)
        {
            map.BeginEdit();
            map.id = this.id;
            map._subject = this.subject;
            map.checkpoints = this.checkpoints;
            map.edition = this.edition;
            map.name = this.name;
            map.remark = this.remark;
            map.semester = this.semester;
            map.EndEdit();
        }

        public Map(string map_id)
        {
            this.id = map_id;
            GetFromCache();
        }

        public string Serialize()
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Map));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                json.WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static Map Unserialize(string str)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Map));
            using (System.IO.MemoryStream stream
                = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (Map)json.ReadObject(stream);
            }
        }

        public void GetFromDB()
        {
            mapsTableAdapter adapter = new mapsTableAdapter();
            InteSchDataSet.mapsDataTable table = adapter.GetDataByID(this.id);

            if (table.Count != 1)
                throw new ContentError("不存在此张地图");
            this.Read(table[0]);

            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            cache.SetMap(this.id, this.Serialize());

            //DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Map));
            //using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    json.WriteObject(stream, this);
            //    RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            //    cache.SetMap(this.id, Encoding.UTF8.GetString(stream.ToArray()));
            //}
        }

        public void GetFromCache()
        {
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object map = cache.GetMap(id);

            if (map == null)
            {
                GetFromDB();
            }
            else if (map is string)
            {
                InteSchDataSet.mapsDataTable table = new InteSchDataSet.mapsDataTable();
                InteSchDataSet.mapsRow row = table.NewmapsRow();
                Map.Unserialize(map as string).Write(row);
                table.AddmapsRow(row);
                this.Read(row);

                //DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
                //using (System.IO.MemoryStream stream
                //    = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(map.ToString())))
                //{
                //    InteSchDataSet.mapsDataTable table = new InteSchDataSet.mapsDataTable();
                //    InteSchDataSet.mapsRow row = table.NewmapsRow();
                //    ((Map)json.ReadObject(stream)).Write(row);
                //    table.AddmapsRow(row);
                //    this.Read(row);
                //}
            }
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
                    InteSchDataSet.checkpointsRow row = table.NewcheckpointsRow();
                    cp.Write(row);
                    table.AddcheckpointsRow(row);
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
