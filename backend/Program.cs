using BilleteraCriptoProg3.Data;
using BilleteraCriptoProg3.Services;
using BilleteraCriptoProg3.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Billetera Cripto API",
        Version = "v1",
        Description = "API para gestionar clientes, criptomonedas y transacciones de la billetera cripto.",
        Contact = new OpenApiContact
        {
            Name = "Gaston Rosso",
            Email = "correo@example.com"
        }
    });
    // anotaciones
    options.EnableAnnotations();

    // comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName()}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CriptoYaService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Documetacion API - Billetera Cripto";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Billetera Cripto v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
