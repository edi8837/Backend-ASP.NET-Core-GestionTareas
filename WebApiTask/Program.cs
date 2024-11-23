using Microsoft.EntityFrameworkCore;
using WebApiTask.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Crear variable para la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("Connection");

// Registrar servicio para la conexión
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString)
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()   // Permitir cualquier origen
            .AllowAnyMethod()   // Permitir cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()); // Permitir cualquier encabezado
});


var app = builder.Build();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);
// Usar CORS antes de otras configuraciones
app.UseCors("AllowAll");  // Activar la política de CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Eliminar o comentar la línea siguiente para deshabilitar la redirección a HTTPS
// app.UseHttpsRedirection(); // Comentado para evitar la redirección a HTTPS

app.UseAuthorization();

app.MapControllers();

app.Run();
