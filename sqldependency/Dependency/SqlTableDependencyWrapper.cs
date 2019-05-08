using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TableDependency.SqlClient;

namespace sqldependency.Dependency
{
    public class SqlTableDependencyWrapper<T> where T : class, new()
    {
        private string connection;
        private string tableName;
        private SqlTableDependency<T> dep;

        public event Action<object, KeyValuePair<T, int>> OnChange;

        public SqlTableDependencyWrapper(string _connection, string _tableName)
        {
            connection = _connection;
            tableName = _tableName;
        }

        public void Start()
        {
            try
            {
                Init();
                StartDependency();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            try
            {
                dep.Stop();
                dep.Dispose();
                dep = null;
            }
            catch (Exception ex)
            {
                throw new Exception("依赖停止异常：" + ex.Message);
            }
        }

        private void Init()
        {
            dep = new SqlTableDependency<T>(connection, tableName);
            dep.OnChanged += Dep_OnChanged;
            dep.OnError += Dep_OnError;
        }

        private void Dep_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            throw e.Error;
        }

        private void StartDependency()
        {
            dep.Start();
        }

        private void Dep_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<T> e)
        {
            int typeValue;
            switch (e.ChangeType)
            {
                case TableDependency.SqlClient.Base.Enums.ChangeType.Insert:
                    typeValue = 1;
                    break;
                case TableDependency.SqlClient.Base.Enums.ChangeType.Update:
                    typeValue = 2;
                    break;
                case TableDependency.SqlClient.Base.Enums.ChangeType.Delete:
                    typeValue = 3;
                    break;
                default:
                    typeValue = 0;
                    break;
            }
            KeyValuePair<T, int> item = new KeyValuePair<T, int>(e.Entity, typeValue);
            OnChange(sender, item);
        }
    }
}
