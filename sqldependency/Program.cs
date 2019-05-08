using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using TableDependency.SqlClient;

namespace sqldependency
{
    class Program
    {
        private static Dependency.SqlTableDependencyWrapper<Entity.UUMS_Owners> dep;
        private static Queue<KeyValuePair<string, int>> queueOwner;
        private static DataTable dtOwner;

        static void Main(string[] args)
        {
            Init();

            bool depSuccess = Dep_Start();
            if (depSuccess)
            {
                Thread theadUpdateSync = new Thread(new ThreadStart(UpdateOwners));
                theadUpdateSync.Start();
            }

            Console.ReadKey();

            Dep_Stop();

            Console.ReadKey();
        }

        private static void Init()
        {
            queueOwner = new Queue<KeyValuePair<string, int>>();

            dtOwner = new DataTable("UUMS_Owners");
            DataColumn dc1 = new DataColumn("OwnerGUID", Type.GetType("System.Guid"));
            DataColumn dc2 = new DataColumn("ChangeType", Type.GetType("System.Int32"));
            dtOwner.Columns.Add(dc1);
            dtOwner.Columns.Add(dc2);
        }

        private static bool Dep_Start()
        {
            try
            {
                dep = new Dependency.SqlTableDependencyWrapper<Entity.UUMS_Owners>(DB.DBContext.SourceConnection.ConnectionString, "UUMS_Owners");
                dep.OnChange += Dep_OnChange;
                dep.Start();

                Console.WriteLine("数据更新监测启动成功");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据更新监测启动失败：" + ex.Message);
                return false;
            }
        }

        private static void Dep_Stop()
        {
            try
            {
                dep.Stop();

                Console.WriteLine("已停止依赖");
            }
            catch
            {
                Console.WriteLine("依赖停止失败");
            }
        }

        private static void Dep_OnChange(object arg1, KeyValuePair<Entity.UUMS_Owners, int> arg2)
        {
            KeyValuePair<string, int> item = new KeyValuePair<string, int>(arg2.Key.OwnerGUID.ToString(), arg2.Value);
            queueOwner.Enqueue(item);
        }

        private static void UpdateOwners()
        {
            Hashtable hsResult = new Hashtable();
            while (true)
            {
                while (queueOwner.Count > 0)
                {
                    var item = queueOwner.Dequeue();
                    hsResult[item.Key] = item.Value;
                }

                if (hsResult.Count > 0)
                {
                    foreach (string id in hsResult.Keys)
                    {
                        DataRow dr = dtOwner.NewRow();
                        dr["OwnerGUID"] = Guid.Parse(id);
                        dr["ChangeType"] = hsResult[id];
                        dtOwner.Rows.Add(dr);
                    }

                    using (SqlConnection con = DB.DBContext.TargetConnection)
                    {
                        con.Open();

                        SqlCommand insertCommand = new SqlCommand("dep_Sync_UUMS_Owners", con);
                        insertCommand.CommandType = CommandType.StoredProcedure;
                        SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@new_UUMS_Owners", dtOwner);
                        tvpParam.SqlDbType = SqlDbType.Structured;

                        insertCommand.ExecuteNonQuery();
                    }

                    dtOwner.Clear();
                    hsResult.Clear();
                }
            }
        }
    }
}
