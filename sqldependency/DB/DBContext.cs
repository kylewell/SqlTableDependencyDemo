using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace sqldependency.DB
{
    class DBContext
    {
        public static SqlConnection SourceConnection
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings[@"DBSource"] != null)
                {
                    return new SqlConnection(ConfigurationManager.ConnectionStrings[@"DBSource"].ConnectionString);
                }
                else
                {
                    return null;
                }
            }
        }

        public static SqlConnection TargetConnection
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings[@"DBTarget"] != null)
                {
                    return new SqlConnection(ConfigurationManager.ConnectionStrings[@"DBTarget"].ConnectionString);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
