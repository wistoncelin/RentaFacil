using Microsoft.EntityFrameworkCore;
using VehicleService.Application.Interfaces;
using VehicleService.Application.Services;
using VehicleService.Domain.Entities;
using VehicleService.Infrastructure.Persistence;
using VehicleService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VehicleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<VehicleApplicationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VehicleDbContext>();
    db.Database.EnsureCreated();
    SeedData(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void SeedData(VehicleDbContext context)
{
    if (context.Vehicles.Any())
    {
        return;
    }

    context.Vehicles.AddRange(
        new Vehicle { Plate = "ABC123", Brand = "Renault", Model = "Clio", Type = VehicleType.Sedan, DailyRate = 45m, IsActive = true },
        new Vehicle { Plate = "DEF456", Brand = "Chevrolet", Model = "Tracker", Type = VehicleType.SUV, DailyRate = 75m, IsActive = true },
        new Vehicle { Plate = "GHI789", Brand = "Mercedes", Model = "Sprinter", Type = VehicleType.Van, DailyRate = 110m, IsActive = true },
        new Vehicle { Plate = "JKL012", Brand = "Toyota", Model = "Hilux", Type = VehicleType.Pickup, DailyRate = 95m, IsActive = true }
    );

    context.SaveChanges();
}
