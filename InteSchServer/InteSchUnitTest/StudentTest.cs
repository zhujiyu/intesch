using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InteSchBusiness;
using InteSchBusiness.DataSet;
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;

namespace InteSchUnitTest
{
    [TestClass]
    public class StudentTest
    {
        public static void InitData(string file)
        {
            if (!System.IO.File.Exists(file))
                throw new Exception("文件" + file + "不存在");
            DataSet ds = new DataSet(file);
            ds.ReadXml(file);

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection
                ("Data Source=ZHUJIYU;Initial Catalog=app_intesch;Integrated Security=True");
            System.Data.SqlClient.SqlDataAdapter adapter
                = new System.Data.SqlClient.SqlDataAdapter("", conn);
            conn.Open();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable table = ds.Tables[i];

                string command = "select count(*) as hastable from sysobjects where name = '"
                    + table.TableName + "'";
                adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(command, conn);

                object hastable = adapter.SelectCommand.ExecuteScalar();
                if (!(hastable is int) || (int)hastable != 1)
                    throw new Exception("");

                adapter.DeleteCommand = new System.Data.SqlClient.SqlCommand("delete from "
                    + table.TableName, conn);
                adapter.DeleteCommand.ExecuteNonQuery();

                for (int k = 0; k < table.Rows.Count; k++)
                {
                    DataRow row = table.Rows[k];
                    string cmd = "insert into " + table.TableName;
                    string value = "values";

                    DataColumn col = table.Columns[0];
                    if (!row.IsNull(col))
                    {
                        cmd += "(" + col.ColumnName;
                        value += "('" + row[col] + "'";
                    }

                    for (int c = 1; c < table.Columns.Count; c++)
                    {
                        col = table.Columns[c];
                        if (row.IsNull(col))
                            continue;

                        cmd += ", " + col.ColumnName;
                        value += ", '" + row[col] + "'";
                    }
                    cmd += ") ";
                    value += ")";
                    cmd += value;

                    adapter.InsertCommand = new System.Data.SqlClient.SqlCommand(cmd, conn);
                    int rows = adapter.InsertCommand.ExecuteNonQuery();
                    if (rows != 1)
                        throw new Exception("插入数据出现异常情况！");
                }
            }

            conn.Close();
        }

        [ClassInitialize()]
        public static void Prepare(TestContext test)
        {
            InteSchBusiness.Cache.InteSchCache cache = new InteSchBusiness.Cache.InteSchCache("127.0.0.1:11211");
            cache.Flush();
            InitData("../../students.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(InteSchBusiness.Error.UserError))]
        public void InitStudentTest()
        {
            Student stu = new Student("100121");

            Assert.AreEqual("100121", stu.ID);
            Assert.AreEqual(1258, stu.Exp_value);

            stu = new Student("100212");
            Assert.AreEqual(stu.ID, "100212");
        }

        [TestMethod]
        public void InlineTest()
        {
            Student stu = new Student("100121");
            Assert.AreEqual(stu.InLine(), false);

            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () => new DateTime(2014, 7, 20, 5, 35, 22);//相差了12分钟
                Assert.AreEqual(stu.InLine(), true);
            }
        }

        [TestMethod]
        public void LoginTest()
        {
            Student stu = new Student("100121");
            stu.Login("192.168.12.38", "chrome v8.0.124");

            InteSchBusiness.DataSet.InteSchDataSetTableAdapters.student_loginsTableAdapter adapter
                = new InteSchBusiness.DataSet.InteSchDataSetTableAdapters.student_loginsTableAdapter();
            InteSchBusiness.DataSet.InteSchDataSet.student_loginsDataTable table
                = adapter.GetDataByIDTop("100121");
            long curr =Student.ConvertDateTimeInt(DateTime.Now);

            Assert.AreEqual(1, table.Count);
            Assert.AreEqual(curr, table[0].login_time);
            System.Diagnostics.Debug.WriteLine("登入测试完成");
        }

        [TestMethod]
        public void LogoutTest()
        {
            Student stu = new Student("100121");
            long ticks = DateTime.Now.Ticks;
            long curr = Student.ConvertDateTimeInt(DateTime.Now);

            InteSchBusiness.DataSet.InteSchDataSetTableAdapters.student_loginsTableAdapter adapter
                = new InteSchBusiness.DataSet.InteSchDataSetTableAdapters.student_loginsTableAdapter();
            InteSchBusiness.DataSet.InteSchDataSet.student_loginsDataTable table;

            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () =>
                {
                    DateTime a = new DateTime(ticks);
                    return a.AddMinutes(25);
                };
                stu.Logout();

                table = adapter.GetDataByIDTop("100121");
                Assert.AreEqual(1, table.Count);
                Assert.AreEqual(curr, table[0].login_time);
                Assert.AreEqual(curr + 25 * 60, table[0].logout_time);
            }
            System.Diagnostics.Debug.WriteLine("登出测试完成");
        }

        [TestMethod]
        public void HasCheckedTest()
        {
            Student stu = new Student("100121");
            Assert.AreEqual(stu.HasChecked(), false);

            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () => new DateTime(2014, 7, 20);
                Assert.AreEqual(stu.HasChecked(), true);
            }

            stu = new Student("100122");
            Assert.AreEqual(stu.HasChecked(), false);

            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () => new DateTime(2014, 7, 18, 15, 45, 0);
                ShimDateTime.TodayGet = () => new DateTime(2014, 7, 18);
                Assert.AreEqual(stu.HasChecked(), true);
            }
            System.Diagnostics.Debug.WriteLine("查询签到状态测试完成");
        }

        [TestMethod]
        public void SignDaysTest()
        {
            Student stu = new Student("100121");
            Assert.AreEqual(stu.HasCheckinDays(), 0);

            using (ShimsContext.Create())
            {
                ShimDateTime.TodayGet = () => new DateTime(2014, 7, 20);
                Assert.AreEqual(stu.HasCheckinDays(), 2);
            }
        }

        [TestMethod]
        public void SignTest()
        {
            Student stu = new Student("100122");
            stu.Checkin();
            Assert.AreEqual(stu.HasChecked(), true);
            Assert.AreEqual(stu.HasCheckinDays(), 1);

            using (ShimsContext.Create())
            {
                ShimDateTime.NowGet = () => new DateTime(2014, 7, 21, 15, 45, 0);
                ShimDateTime.TodayGet = () => new DateTime(2014, 7, 21);

                stu = new Student("100121");
                Assert.AreEqual(stu.HasChecked(), false);
                stu.Checkin();

                Assert.AreEqual(stu.HasChecked(), true);
                Assert.AreEqual(stu.HasCheckinDays(), 3);
            }
        }

        [TestMethod]
        public void LoadStudentMapsTest()
        {
            Student stu = new Student("100121");
            StudentMap[] maps = stu.LoadMaps();
            Assert.AreEqual(maps.Length, 2);

            maps = stu.LoadMaps();
            Assert.AreEqual(maps[0].student_id, "100121");
            Assert.AreEqual(maps[1].map_id, "125002");

            stu = new Student("100122");
            maps = stu.LoadMaps();
            Assert.AreEqual(maps.Length, 0);
        }

    }
}
