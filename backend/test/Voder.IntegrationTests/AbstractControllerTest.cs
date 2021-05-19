using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Voder.Models;
using Microsoft.Extensions.DependencyInjection;
using AutoFixture;
using System.Text;
using System;

namespace Voder.IntegrationTests
{
    abstract public class AbstractControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>, IDisposable
    {
        protected readonly VoderContext dbContext;
        protected readonly HttpClient client;
        protected readonly Fixture fixture;

        public AbstractControllerTest(TestWebApplicationFactory<Startup> factory)
        {
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            this.dbContext = scope.ServiceProvider.GetService<VoderContext>();

            this.client = factory.CreateClient();

            this.fixture = new Fixture();
        }

        public void Dispose()
        {
            this.dbContext.Database.EnsureDeleted();
        }

        protected async Task<T> SendRequest<T>(String path, HttpMethod method = null, Object body = null)
        {
            var request = new HttpRequestMessage
            {
                Method = method != null ? method : HttpMethod.Get,
                RequestUri = new Uri($"{this.client.BaseAddress.OriginalString}{path}"),
            };

            if (body != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            }

            var httpResponse = await this.client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();

            if (request.Method == HttpMethod.Delete)
            {
                return default(T);
            }

            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<T>(stringResponse);

            return parsedResponse;
        }
    }
}
