using System.Configuration;
using RemindSME.Desktop.Properties;
using Flurl;
using Flurl.Http;

namespace RemindSME.Desktop.Helpers
{
    public interface ICompanyApiClient
    {
        void UpdateCompanyName(int companyId);
    }
    public class CompanyApiClient : ICompanyApiClient
    {
        public CompanyApiClient() { }

        public async void UpdateCompanyName(int companyId)
        {
            var serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
            var company = await serverUrl
                .AppendPathSegments("registration", companyId)
                .GetStringAsync();
            Settings.Default.CompanyName = company != "Company not found" ? company : null;
        }
    }
}
