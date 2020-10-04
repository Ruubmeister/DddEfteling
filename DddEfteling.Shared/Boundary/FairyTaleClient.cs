using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DddEfteling.Shared.Boundary
{
    public class FairyTaleClient : RestClient, IFairyTaleClient
    {
        public FairyTaleClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["FairyTaleUrl"]);
        }

        public List<FairyTaleDto> GetFairyTalesAsync()
        {
            string url = "/api/v1/fairy-tales";
            return JsonConvert.DeserializeObject<List<FairyTaleDto>>(GetResource(url));
        }

        public FairyTaleDto GetRandomFairyTaleAsync()
        {
            string url = "/api/v1/fairy-tales/random";
            return JsonConvert.DeserializeObject<FairyTaleDto>(GetResource(url));
        }

        public FairyTaleDto GetNearestFairyTale(Guid guid, List<Guid> excludedGuid)
        {
            string url = $"/api/v1/fairy-tales/{guid}/nearest";

            var urlParams = new Dictionary<string, string>
            {
                {"exclude", String.Join(",", excludedGuid.ToArray())}
            };

            string dto = GetResource(url, urlParams);

            return JsonConvert.DeserializeObject<FairyTaleDto>(dto);

        }
    }

    public interface IFairyTaleClient
    {
        public List<FairyTaleDto> GetFairyTalesAsync();

        public FairyTaleDto GetRandomFairyTaleAsync();

        public FairyTaleDto GetNearestFairyTale(Guid guid, List<Guid> excludedGuid);

    }
}
