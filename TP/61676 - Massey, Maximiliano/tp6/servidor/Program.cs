using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Agregar controladores si es necesario
builder.Services.AddControllers();

builder.Services.AddDbContext<ContenidoTiendaDb>(options => {
    options.UseSqlite("Data Source=tienda.db");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContenidoTiendaDb>();
    dbContext.Database.EnsureCreated(); // Asegurarse de que la base de datos esté actualizada
}

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapGet("/productos", async (ContenidoTiendaDb db, string? busqueda) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(busqueda))
        query = query.Where(p => p.Nombre.Contains(busqueda));
    return await query.ToListAsync();
});

app.Run();