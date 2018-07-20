using System.Configuration;
using System.Threading.Tasks;
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
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private readonly ISettings settings;

        public CompanyApiClient(ISettings settings)
        {
            this.settings = settings;
        }

        public async Task UpdateCompanyName(string companyId)
        {
            if (!IsValidCompanyIdFormat(companyId))
            {
                settings.CompanyName = null;
                return;
            }

            try
            {
                var companyData = await ServerUrl.AppendPathSegments("company", companyId).WithHeader("Accept", "application/json").GetJsonAsync();
                settings.CompanyName = companyData.company;
            }
            catch
            {
                settings.CompanyName = null;
            }
        }

        private static bool IsValidCompanyIdFormat(string id)
        {
            return id.Length == 6;
        }
    }
}
