using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public abstract class RestClient
    {
        public static readonly HttpClient client = new HttpClient();

        public async Task<Stream> GetResource(string url)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("Application/Json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            var streamTask = client.GetStreamAsync(url);

            return await streamTask;
        }

        public async Task<Stream> GetResource(string url, Dictionary<string, string> urlParams)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("Application/Json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            var streamTask = client.GetStreamAsync(QueryHelpers.AddQueryString(url, urlParams));

            return await streamTask;
        }

    }
}
