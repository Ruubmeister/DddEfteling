using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class StandClient : RestClient, IStandClient
    {
        public StandClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["StandUrl"]);
        }

        public List<StandDto> GetStandsAsync()
        {
            string url = "/api/v1/stands";
            return JsonSerializer.Deserialize<List<StandDto>>(GetResource(url));
        }
    }

    public interface IStandClient
    {
        public List<StandDto> GetStandsAsync();
    }
}
