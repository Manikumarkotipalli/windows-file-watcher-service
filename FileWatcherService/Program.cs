using FileWatcherService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<DatabaseWorker>(); 

var host = builder.Build();
host.Run();