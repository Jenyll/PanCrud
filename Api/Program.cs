using Application.Services;
using Application.UseCases.Addresses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Registrar serviço de aplicação que a Controller consome
builder.Services.AddScoped<IAddressService, AddressService>();

// Registre os use cases quando implementar as classes concretas:
// builder.Services.AddScoped<ICreateAddressUseCase, CreateAddressUseCase>();
// builder.Services.AddScoped<IGetAddressUseCase, GetAddressUseCase>();
// builder.Services.AddScoped<IUpdateAddressUseCase, UpdateAddressUseCase>();
// builder.Services.AddScoped<IDeleteAddressUseCase, DeleteAddressUseCase>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
