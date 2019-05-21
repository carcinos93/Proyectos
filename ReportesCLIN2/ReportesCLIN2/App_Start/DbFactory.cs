using Dapper;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ReportesCLIN2
{

    public class JsonObjectTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value)
        {
            return JsonConvert.DeserializeObject(value.ToString(), destinationType);
        }
    }
    public class DbFactory
    {
        private static ILog log = LogManager.GetLogger(typeof(DbFactory));
        public static IDbConnection Conn()
        {
            
            var connectionString = @"server=LAPTOP-0HM5B1NI\MSSQLSERVER2016;database={0};uid={1};password={2};";
            HttpContext context = HttpContext.Current;
            var usuario = "sa";
            var empresa = "CLIN";
            var password = "SqlServer2016";
           
            var connection = new SqlConnection(
                string.Format(connectionString, empresa, usuario, password)
                );
            return connection;
        }
    }
}