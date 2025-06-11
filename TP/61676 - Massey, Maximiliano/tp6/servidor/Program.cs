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


var carritos = new Dictionary<Guid, Dictionary<int, int>>();


app.MapPost("/carritos", () =>
{
    var id = Guid.NewGuid();
    carritos[id] = new Dictionary<int, int>();
    return Results.Ok(new { id });
});

app.MapGet("/carritos/{id}", (Guid id) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();
    return Results.Ok(carritos[id]);
});

app.MapPut("/carritos/{id}/{producto}", async (Guid id, int producto, ContenidoTiendaDb db) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    var prod = await db.Productos.FindAsync(producto);
    if (prod == null)
        return Results.NotFound("Producto no encontrado");

    if (prod.Stock < 1)
        return Results.BadRequest("Sin stock");

    if (!carritos[id].ContainsKey(producto))
        carritos[id][producto] = 0;
    carritos[id][producto]++;

    return Results.Ok(carritos[id]);
});

app.MapDelete("/carritos/{id}/{producto}", (Guid id, int producto) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();

    if (!carritos[id].ContainsKey(producto))
        return Results.NotFound("Producto no está en el carrito");

    carritos[id][producto]--;
    if (carritos[id][producto] <= 0)
        carritos[id].Remove(producto);

    return Results.Ok(carritos[id]);
});

app.MapDelete("/carritos/{id}", (Guid id) =>
{
    if (!carritos.ContainsKey(id))
        return Results.NotFound();
    carritos.Remove(id);
    return Results.Ok("Carrito eliminado");
});

app.Run();