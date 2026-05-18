
using CustomerAPI.Controllers;
using CustomerAPI.Service;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Scalar.AspNetCore;

namespace CustomerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<CosmosService>();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.MapOpenApi();
            app.MapScalarApiReference();

            CustomerEndpoints.MapCustomerEndpoints(app);

            app.Run();
        }
    }
}
