using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OtherTest
{
    class Program
    {
        public static void InitData()
        {
            //InteSchBusiness.Student.InitData("../../../InteSchUnitTest/students.xml");
            DataSet ds = new DataSet();
            ds.ReadXml("../../test.xml");

            //string conn = "Data Source=ZHUJIYU;Initial Catalog=app_intesch;Integrated Security=True";
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Data Source=ZHUJIYU;Initial Catalog=app_intesch;Integrated Security=True");
            System.Data.SqlClient.SqlDataAdapter adapter
                = new System.Data.SqlClient.SqlDataAdapter("", conn);
            conn.Open();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable table = ds.Tables[i];

                string command = "select count(*) as hastable from sysobjects where name = '" + table.TableName + "'";
                adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(command, conn);

                object hastable = adapter.SelectCommand.ExecuteScalar();
                if (!(hastable is int) || (int)hastable != 1)
                    throw new Exception("");

                adapter.DeleteCommand = new System.Data.SqlClient.SqlCommand("delete from " + table.TableName, conn);
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

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.Ticks);
            Console.WriteLine(ConvertDateTimeInt(DateTime.Now));
        }
    }
}
