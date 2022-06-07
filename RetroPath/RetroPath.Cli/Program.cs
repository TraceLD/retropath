using CliFx;
using Serilog;

// setup the logger;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

// setup CLI;
return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();