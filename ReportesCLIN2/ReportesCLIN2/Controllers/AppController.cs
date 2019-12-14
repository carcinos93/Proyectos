
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ReportesCLIN2.Controllers
{
    using ReportingServices;

    public class AppController : ApiController
    {
        [HttpGet(), ActionName("getreports")]
        public IEnumerable<object> GetReports()
        {

            NetworkCredential clientCredentials = new NetworkCredential("nrodas", "P@ssw0rd", "CLIN");
            ReportingServices.ReportingService2010SoapClient client = new ReportingServices.ReportingService2010SoapClient();
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.Windows.ClientCredential = clientCredentials;
            client.Open();
            TrustedUserHeader t = new TrustedUserHeader();
            CatalogItem[] items;
            // I need to list of children of a specified folder.
            ServerInfoHeader oServerInfoHeader = client.ListChildren(t, "/", true, out items);

            return items;
        }
    }

}
