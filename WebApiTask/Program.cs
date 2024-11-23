using Microsoft.EntityFrameworkCore;
using WebApiTask.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Crear variable para la cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("Connection");

// Registrar servicio para la conexi�n
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString)
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()   // Permitir cualquier origen
            .AllowAnyMethod()   // Permitir cualquier m�todo HTTP (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()); // Permitir cualquier encabezado
});


var app = builder.Build();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);
// Usar CORS antes de otras configuraciones
app.UseCors("AllowAll");  // Activar la pol�tica de CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Eliminar o comentar la l�nea siguiente para deshabilitar la redirecci�n a HTTPS
// app.UseHttpsRedirection(); // Comentado para evitar la redirecci�n a HTTPS

app.UseAuthorization();

app.MapControllers();

app.Run();
