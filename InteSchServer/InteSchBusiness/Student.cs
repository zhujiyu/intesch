﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InteSchBusiness.Error;
using InteSchBusiness.Cache;
using InteSchBusiness.DataSet;
using InteSchBusiness.DataSet.InteSchDataSetTableAdapters;

namespace InteSchBusiness
{
    public class Student
    {
        string id = "";

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        InteSchDataSet.studentsRow data = null;

        public InteSchDataSet.studentsRow Data
        {
            get { return data; }
            set { data = value; }
        }

        public int exp_value
        {
            get
            {
                if (this.data != null)
                    return data.exp_value;
                return 0;
            }
        }

        public Student(string student_id)
        {
            this.id = student_id;
            this.data = GetFromCache();
        }

        private InteSchDataSet.studentsRow GetFromDB()
        {
            studentsTableAdapter adapter = new studentsTableAdapter();
            InteSchDataSet.studentsDataTable table = adapter.GetDataByID(this.id);

            if (table.Count != 1)
                throw new Error.UserError(@"编号为" + this.id + "的学生不存在");

            RowCache row = new RowCache(Properties.Settings.Default.memcache);
            row.SetStudent(this.id, table[0]);

            return table[0];
        }

        private InteSchDataSet.studentsRow GetFromCache()
        {
            RowCache row = new RowCache(Properties.Settings.Default.memcache);
            object stu = row.GetStudent(this.id);

            if (stu == null)
                return GetFromDB();

            if (stu is InteSchDataSet.studentsRow)
                return stu as InteSchDataSet.studentsRow;
            else
                throw new Error.InternalError("行缓存里的数据格式不正确");
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
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
            if (this.data == null)
                return false;
            return ConvertDateTimeInt(DateTime.Today) < this.data.last_checkin;
        }

        public int HasCheckinDays()
        {
            if (HasChecked())
                return data.checkin_days;
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

            this.data = GetFromDB();
            RowCache row = new RowCache(Properties.Settings.Default.memcache);
            row.SetStudent(this.id, this.data);
        }


    }
}
