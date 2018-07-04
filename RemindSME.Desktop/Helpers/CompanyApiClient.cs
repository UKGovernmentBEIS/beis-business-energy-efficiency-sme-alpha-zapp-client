using System.Configuration;
using System.Threading.Tasks;
using RemindSME.Desktop.Properties;
using Flurl;
using Flurl.Http;

namespace RemindSME.Desktop.Helpers
{
    public interface ICompanyApiClient
    {
        Task UpdateCompanyName(string companyId);
    }
    public class CompanyApiClient : ICompanyApiClient
    {
        public async Task UpdateCompanyName(string companyId)
        {
            var serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
            var company = await serverUrl
                .AppendPathSegments("registration", companyId)
                .GetStringAsync();
            Settings.Default.CompanyName = company != "Company not found" ? company : null;
        }
    }
}
