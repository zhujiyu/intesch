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
    class CheckPoint
    {
        [DataMember]
        private string id = null;

        public string Id
        {
            get { return id; }
        }

        [DataMember]
        private string map_id = null;

        public string Map_id
        {
            get { return map_id; }
        }

        [DataMember]
        private int serial = 0;

        public int Serial
        {
            get { return serial; }
        }

        [DataMember]
        private string name = null;

        public string Name
        {
            get { return name; }
        }

        [DataMember]
        private string photo = null;

        public string Photo
        {
            get { return photo; }
        }

        [DataMember]
        private string remark = null;

        public string Remark
        {
            get { return remark; }
        }

        public void Read(InteSchDataSet.checkpointsRow cp)
        {
            this.id = cp.id;
            this.map_id = cp.map_id;
            this.serial = cp.serial;
            this.name = cp.name;
            this.photo = cp.photo;
            this.remark = cp.remark;
        }

        public void Write(InteSchDataSet.checkpointsRow cp)
        {
            cp.BeginEdit();
            cp.id = this.id;
            cp.map_id = this.map_id;
            cp.serial = this.serial;
            cp.name = this.name;
            cp.photo = this.photo;
            cp.remark = this.remark;
            cp.EndEdit();
        }

        public CheckPoint(string cpid)
        {
            this.id = cpid;
            GetFromCache();
        }

        public void LoadFromDB()
        {
            checkpointsTableAdapter adapter = new checkpointsTableAdapter();
            InteSchDataSet.checkpointsDataTable table = adapter.GetDataByID(id);

            if (table.Count != 1)
                throw new ContentError("不存在此张地图");
            Read(table[0]);

            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(CheckPoint));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                json.WriteObject(stream, this);
                RowCache cache = new RowCache(Properties.Settings.Default.memcache);
                cache.SetCheckPoints(this.id, Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public void GetFromCache()
        {
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object cp = cache.GetCheckPoints(id);

            if (cp == null)
            {
                LoadFromDB();
                return;
            }

            if (cp is string)
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
                using (System.IO.MemoryStream stream
                    = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(cp.ToString())))
                {
                    InteSchDataSet.checkpointsDataTable table = new InteSchDataSet.checkpointsDataTable();
                    InteSchDataSet.checkpointsRow row = table.NewcheckpointsRow();
                    ((CheckPoint)json.ReadObject(stream)).Write(row);
                    table.AddcheckpointsRow(row);
                    this.Read(row);
                }
            }
            else
                throw new Error.InternalError("行缓存里的数据格式不正确");
        }

    }
}
