using Application.Dtos.ViaCep;
using Application.Ports;
using Application.Services;
using Integrations.ViaCep;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework;
using Persistence.Repositories;
using Application.UseCases.Addresses.Interfaces;
using Application.UseCases.Addresses.Implement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core (SQLite para desenvolvimento rápido)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ports -> Adapters
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddHttpClient<IViaCepClient, ViaCepClient>(client =>
{
    client.BaseAddress = new Uri("https://viacep.com.br/");     
    client.Timeout = TimeSpan.FromSeconds(10);
});

// UseCases
builder.Services.AddScoped<ICreateAddress, CreateAddress>();
builder.Services.AddScoped<IGetAddress, GetAddress>();
builder.Services.AddScoped<IUpdateAddress, UpdateAddress>();
builder.Services.AddScoped<IDeleteAddress, DeleteAddress>();

// Facade usada pela Controller
builder.Services.AddScoped<IAddressService, AddressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
