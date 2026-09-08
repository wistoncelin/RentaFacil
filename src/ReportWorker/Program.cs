using Microsoft.EntityFrameworkCore;
using ReportWorker;
using ReportWorker.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<ReportWorkerService>();

var host = builder.Build();
host.Run();
