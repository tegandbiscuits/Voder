using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Voder.Models;
using Xunit;

namespace Voder.IntegrationTests
{
    public abstract class AbstractControllerTest : IClassFixture<TestWebApplicationFactory<Startup>>, IDisposable
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

        protected async Task<T> SendRequest<T>(string path, HttpMethod method = null, object body = null)
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
