using Microsoft.Azure.Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddSingleton<IFunctionMetadataProvider, GeneratedFunctionMetadataProvider>();
    })
    .Build();

host.Run();
