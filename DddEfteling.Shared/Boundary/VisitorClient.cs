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

        public List<VisitorDto> GetVisitors()
        {
            string url = "/api/v1/visitors";
            return JsonSerializer.Deserialize<List<VisitorDto>>(GetResource(url));
        }

        public VisitorDto GetVisitor(Guid guid)
        {
            string url = $"/api/v1/visitors/{guid}";
            return JsonSerializer.Deserialize<VisitorDto>(GetResource(url));
        }
    }

    public interface IVisitorClient
    {
        public List<VisitorDto> GetVisitors();

        public VisitorDto GetVisitor(Guid guid);

    }
}
