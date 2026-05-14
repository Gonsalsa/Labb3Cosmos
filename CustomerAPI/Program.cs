
using CustomerAPI.Controllers;
using CustomerAPI.Service;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;

namespace CustomerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<CosmosService>();

            var app = builder.Build();

            CustomerEndpoints.MapCustomerEndpoints(app);

            app.Run();
        }
    }
}
