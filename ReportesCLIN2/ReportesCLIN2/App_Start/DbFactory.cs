using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ReportesCLIN2.App_Start
{
    public class DbFactory
    {
        private static ILog log = LogManager.GetLogger(typeof(DbFactory));
        public static IDbConnection Conn()
        {
            
            var connectionString = @"server=clindev17;database={0};uid={1};password={2};";
            HttpContext context = HttpContext.Current;
            var usuario = "sa";
            var empresa = "CLIN";
            var password = "P@ssw0rd";
           
            var connection = new SqlConnection(
                string.Format(connectionString, empresa, usuario, password)
                );
            return connection;
        }
    }
}