using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public class VisitorClient : RestClient, IVisitorClient
    {

        public VisitorClient(IConfiguration Configuration)
        {
            this.setBaseUri(Configuration["VisitorUrl"]);
        }

        public async Task<List<VisitorDto>> GetVisitors()
        {
            string url = "/api/v1/visitors";
            return await JsonSerializer.DeserializeAsync<List<VisitorDto>>(await GetResource(url));
        }

        public async Task<VisitorDto> GetVisitor(Guid guid)
        {
            string url = $"/api/v1/visitors/{guid}";
            return await JsonSerializer.DeserializeAsync<VisitorDto>(await GetResource(url));
        }
    }

    public interface IVisitorClient
    {
        public Task<List<VisitorDto>> GetVisitors();

        public Task<VisitorDto> GetVisitor(Guid guid);

    }
}
