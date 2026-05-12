using Labb3Cosmos.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(service =>
    {
        service.AddSingleton<CosmosService>();
        service.AddApplicationInsightsTelemetryWorkerService();
        service.ConfigureFunctionsApplicationInsights();
    })
    .Build();

await host.RunAsync();

