using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;
using Servicio_DW_Pagos.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddHttpClient("TipoCambioService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7109/api/Moneda/"); 
});


builder.Services.AddScoped<ServicioTipoCambio>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
