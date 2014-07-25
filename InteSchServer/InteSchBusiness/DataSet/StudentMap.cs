using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace InteSchBusiness.DataSet
{
    [DataContract]
    public class StudentMap
    {
        [DataMember]
        public int id = 0;

        [DataMember]
        public string student_id = null;

        [DataMember]
        public string map_id = null;

        [DataMember]
        public int max_right = 0;

        [DataMember]
        public int right = 0;

        [DataMember]
        public int total_right=0;

        [DataMember]
        public int checkpoints = 0;

        [DataMember]
        public bool reward = false;

        public StudentMap() { }

        public StudentMap(InteSchDataSet.student_mapsRow sm)
        {
            Read(sm);
        }

        public void Read(InteSchDataSet.student_mapsRow sm)
        {
            this.id = sm.id;
            this.student_id = sm.student_id;
            this.map_id = sm.map_id;
            this.total_right = sm.total_rights;
            this.max_right = sm.max_constant_rights;
            this.right = sm.constant_rights;
            this.checkpoints = sm.checkpoints;
            this.reward = sm.reward;
        }

        public void Write(InteSchDataSet.student_mapsRow sm)
        {
            sm.BeginEdit();
            sm.id = this.id;
            sm.student_id = this.student_id;
            sm.map_id = this.map_id;
            sm.max_constant_rights = this.max_right;
            sm.total_rights = this.total_right;
            sm.constant_rights = this.right;
            sm.checkpoints = this.right;
            sm.reward = this.reward;
            sm.EndEdit();
        }

        public string Serialize()
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(StudentMap));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                json.WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static StudentMap Unserialize(string str)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(StudentMap));

            using (System.IO.MemoryStream stream
                = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (StudentMap)json.ReadObject(stream);
            }
        }

        //public void _Unserialize(string str)
        //{
        //        InteSchDataSet.student_mapsDataTable table = new InteSchDataSet.student_mapsDataTable();
        //        InteSchDataSet.student_mapsRow row = table.Newstudent_mapsRow();
        //        StudentMap sm = StudentMap.Unserialize(str);
        //        sm.Write(row);
        //        table.Addstudent_mapsRow(row);
        //        this.Read(row);
        //}
    }
}
