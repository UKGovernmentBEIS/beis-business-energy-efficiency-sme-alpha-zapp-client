using System.Configuration;
using System.Threading.Tasks;
using RemindSME.Desktop.Properties;
using Flurl;
using Flurl.Http;
using RemindSME.Desktop.Configuration;

namespace RemindSME.Desktop.Helpers
{
    public interface ICompanyApiClient
    {
        Task UpdateCompanyName(string companyId);
    }
    public class CompanyApiClient : ICompanyApiClient
    {
        private readonly ISettings settings;

        public CompanyApiClient(ISettings settings)
        {
            this.settings = settings;
        }

        public async Task UpdateCompanyName(string companyId)
        {
            var serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
            try
            {
                var company =
                    await serverUrl
                        .AppendPathSegments("registration", companyId)
                        .GetStringAsync();
                settings.CompanyName = company;
            }
            catch (FlurlHttpException)
            {
                settings.CompanyName = null;
            }
        }
    }
}
