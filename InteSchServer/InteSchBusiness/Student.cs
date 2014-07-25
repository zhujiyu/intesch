using System;
using System.Collections.Generic;
using System.Data;
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
    public class Student
    {
        [DataMember]
        private string id = "";

        public string ID
        {
            get { return id; }
        }

        [DataMember]
        private int exp_value = 0;

        public int Exp_value
        {
            get { return exp_value; }
            set { exp_value = value; }
        }

        [DataMember]
        private int max_right = 0;

        public int Max_right
        {
            get { return max_right; }
            set { max_right = value; }
        }

        [DataMember]
        private long last_checkin = 0;

        public long Last_checkin
        {
            get { return last_checkin; }
            set { last_checkin = value; }
        }

        [DataMember]
        private int lower_gem = 0;

        public int Lower_gem
        {
            get { return lower_gem; }
            set { lower_gem = value; }
        }

        [DataMember]
        private int middle_gem = 0;

        public int Middle_gem
        {
            get { return middle_gem; }
            set { middle_gem = value; }
        }

        [DataMember]
        private int high_gem = 0;

        public int High_gem
        {
            get { return high_gem; }
            set { high_gem = value; }
        }

        [DataMember]
        private int certi_num = 0;

        public int Certi_num
        {
            get { return certi_num; }
            set { certi_num = value; }
        }

        [DataMember]
        private int checkin_days = 0;

        public int Checkin_days
        {
            get { return checkin_days; }
            set { checkin_days = value; }
        }

        [DataMember]
        private int constant_right = 0;

        [DataMember]
        private int total_rights = 0;

        [DataMember]
        private int treasure_frag = 0;

        public Student(string student_id)
        {
            this.id = student_id;
            GetFromCache();
        }

        public void Read(InteSchDataSet.studentsRow row)
        {
            this.id = row.id;
            this.exp_value = row.exp_value;
            this.high_gem = row.high_gem;
            this.lower_gem = row.lower_gem;
            this.middle_gem = row.middle_gem;
            this.certi_num = row.certi_num;
            this.last_checkin = row.last_checkin;
            this.max_right = row.max_constant_rights;
            this.checkin_days = row.checkin_days;
            this.constant_right = row.constant_rights;
            this.total_rights = row.total_rights;
            this.treasure_frag = row.treasure_frag;
        }

        public void Write(InteSchDataSet.studentsRow row)
        {
            row.BeginEdit();
            row.id = this.id;
            row.exp_value = this.exp_value;
            row.high_gem = this.high_gem;
            row.middle_gem = this.middle_gem;
            row.lower_gem = this.lower_gem;
            row.certi_num = this.certi_num;
            row.last_checkin = this.last_checkin;
            row.max_constant_rights = this.max_right;
            row.checkin_days = this.checkin_days;
            row.constant_rights = this.constant_right;
            row.total_rights = this.total_rights;
            row.treasure_frag = this.treasure_frag;
            row.EndEdit();
        }

        public string Serialize()
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                json.WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static Student Unserialize(string str)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
            using (System.IO.MemoryStream stream
                = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (Student)json.ReadObject(stream);
            }
        }

        private void LoadFromDB()
        {
            studentsTableAdapter adapter = new studentsTableAdapter();
            InteSchDataSet.studentsDataTable table = adapter.GetDataByID(this.id);

            if (table.Count != 1)
                throw new Error.UserError(@"编号为" + this.id + "的学生不存在");
            this.Read(table[0]);

            RowCache row = new RowCache(Properties.Settings.Default.memcache);
            row.SetStudent(this.id, this.Serialize());

            //DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
            //using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    json.WriteObject(stream, this);
            //    RowCache row = new RowCache(Properties.Settings.Default.memcache);
            //    row.SetStudent(this.id, Encoding.UTF8.GetString(stream.ToArray()));
            //}
        }

        private void GetFromCache()
        {
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object stu = cache.GetStudent(this.id);

            if (stu == null)
            {
                LoadFromDB();
                return;
            }

            if (stu is string)
            {
                InteSchDataSet.studentsDataTable table = new InteSchDataSet.studentsDataTable();
                InteSchDataSet.studentsRow row = table.NewstudentsRow();
                Student.Unserialize(stu.ToString()).Write(row);
                table.AddstudentsRow(row);
                this.Read(row);

                //DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
                //using (System.IO.MemoryStream stream 
                //    = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(stu.ToString())))
                //{
                //    InteSchDataSet.studentsDataTable table = new InteSchDataSet.studentsDataTable();
                //    InteSchDataSet.studentsRow row = table.NewstudentsRow();
                //    ((Student)json.ReadObject(stream)).Write(row);
                //    table.AddstudentsRow(row);
                //    this.Read(row);
                //}
            }
            else
                throw new Error.InternalError("行缓存里的数据格式不正确");
        }

        /// <summary>
        /// 学生第一次进入趣味学堂，需要给他初始化一条信息记录
        /// </summary>
        /// <param name="student_id">学生编号</param>
        public static void Register(string student_id)
        {
            studentsTableAdapter adapter = new studentsTableAdapter();
            adapter.Insert(student_id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 判断当前用户是否在线
        /// </summary>
        /// <returns>true表示在线，false表示离线</returns>
        public bool InLine()
        {
            long curr = ConvertDateTimeInt(DateTime.Now);

            //利用缓存判断用户是否在线
            VectorCache cache = new VectorCache(Properties.Settings.Default.memcache);
            long inline = cache.GetInline(this.id);
            if (inline > 0 && inline + 300 > curr)
                return true;

            student_loginsTableAdapter login = new student_loginsTableAdapter();
            InteSchDataSet.student_loginsDataTable table = login.GetDataByIDTop(this.id);
            if (table.Count == 0)
                return false;

            if (table[0].logout_time + 900 < curr) // 15分钟
                return false;

            return true;
        }

        public void SetInline()
        {
            long curr = ConvertDateTimeInt(DateTime.Now);
            VectorCache cache = new VectorCache(Properties.Settings.Default.memcache);
            cache.SetInline(this.id, curr);
        }

        public void Login(string ipaddress, string client)
        {
            if (this.InLine())
                return;

            student_loginsTableAdapter login = new student_loginsTableAdapter();
            long curr = ConvertDateTimeInt(DateTime.Now);
            login.Insert(this.id, curr, curr, ipaddress, client);

            InteSchDataSet.student_loginsDataTable table = login.GetDataByIDTop(this.id);
            // 存到缓存里
        }

        /// <summary>
        /// 学生进入趣味学堂后，将其ID放进一个队列里
        /// 队列每隔几分钟取出一批学生ID，检查他们是否还在线，
        /// 并记录他们的在线时长，
        /// 如果已经下线，超过10分钟没有活动可以认为下线了，
        /// 将其ID从队列中摘掉
        /// </summary>
        public void Logout()
        {
            student_loginsTableAdapter login = new student_loginsTableAdapter();
            long curr = ConvertDateTimeInt(DateTime.Now);

            InteSchDataSet.student_loginsDataTable table = login.GetDataByIDTop(this.id);
            if (table.Count == 0)
                throw new ContentError("不存在用户登录数据");

            table[0].BeginEdit();
            table[0].logout_time = curr;
            table[0].EndEdit();
            login.Update(table[0]);
            //清理缓存
        }

        
        /// <summary>
        /// 今天是否已经签到  
        /// </summary>
        /// <returns>false表示未签到，true表示已经签到</returns>
        public bool HasChecked()
        {
            return ConvertDateTimeInt(DateTime.Today) < this.last_checkin;
        }

        public int HasCheckinDays()
        {
            if (HasChecked())
                return this.checkin_days;
            return 0;
        }
        
        /// <summary>
        /// 签到
        /// </summary>
        public void Checkin()
        {
            if (HasChecked()) 
                return;

            QueriesTableAdapter signAdapter = new QueriesTableAdapter();
            long curr = ConvertDateTimeInt(DateTime.Now);
            long yes = ConvertDateTimeInt(DateTime.Today.AddDays(-1));

            signAdapter.student_checkin(this.id, curr, yes);
            LoadFromDB();

            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Student));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                json.WriteObject(stream, this);
                RowCache row = new RowCache(Properties.Settings.Default.memcache);
                row.SetStudent(this.id, Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public StudentMap[] LoadMaps()
        {
            StudentMap[] sms = null;
            InteSchDataSet.student_mapsDataTable table = null;
            RowCache cache = new RowCache(Properties.Settings.Default.memcache);
            object obj = cache.GetStudentMaps(id);

            if (obj == null)
            {
                student_mapsTableAdapter adapter = new student_mapsTableAdapter();
                table = adapter.GetDataByStudent(id);
                sms = new StudentMap[table.Count];
                string[] vs = new string[table.Count];

                for (int i = 0; i < table.Count; i++)
                {
                    sms[i] = new StudentMap(table[i]);
                    vs[i] = sms[i].Serialize();
                }

                cache.SetStudentMaps(id, vs);
            }
            else if (obj is string[])
            {
                string[] vs = obj as string[];
                sms = new StudentMap[vs.Length];

                for (int i = 0; i < vs.Length; i++)
                {
                    sms[i] = StudentMap.Unserialize(vs[i]);
                }
            }
            else
                throw new ContentError("缓存数据的格式不正确");

            return sms;
        }
    }
}
