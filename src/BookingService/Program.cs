using BookingService.Application.Interfaces;
using BookingService.Application.Services;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Clients;
using BookingService.Infrastructure.Persistence;
using BookingService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var useInMemory = builder.Configuration.GetValue<bool?>("UseInMemoryDatabase") ?? false;
if (useInMemory)
{
    builder.Services.AddDbContext<BookingDbContext>(options =>
        options.UseInMemoryDatabase("RentaFacilBookingDb"));
}
else
{
    builder.Services.AddDbContext<BookingDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<BookingApplicationService>();

builder.Services.AddHttpClient<IVehicleReservationClient, VehicleReservationClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VehicleService:BaseUrl"]!);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.EnsureCreated();
    SeedData(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();

static void SeedData(BookingDbContext context)
{
    if (context.Clients.Any())
    {
        return;
    }

    context.Clients.Add(new Client
    {
        DocumentNumber = "1012345678",
        Name = "Ana",
        LastName = "García",
        Email = "ana@rentafacil.com"
    });

    context.SaveChanges();
}
