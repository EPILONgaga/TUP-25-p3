using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<ProductoDto>> ObtenerProductosAsync(string? busqueda = null)
    {
        var url = "/productos" + (string.IsNullOrWhiteSpace(busqueda) ? "" : $"?busqueda={Uri.EscapeDataString(busqueda)}");
        return await _httpClient.GetFromJsonAsync<List<ProductoDto>>(url) ?? new List<ProductoDto>();
    }

    public async Task<Guid> CrearCarritoAsync()
    {
        var resp = await _httpClient.PostAsJsonAsync("/carritos", new { });
        var data = await resp.Content.ReadFromJsonAsync<CrearCarritoRespuesta>();
        return data?.id ?? Guid.Empty;
    }

    public async Task<List<ItemCarritoDto>> ObtenerCarritoAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{id}") ?? new List<ItemCarritoDto>();
    }

    public async Task<bool> AgregarProductoAsync(Guid carritoId, int productoId)
    {
        var resp = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> QuitarProductoAsync(Guid carritoId, int productoId)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarCarritoAsync(Guid carritoId)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> ConfirmarCompraAsync(Guid carritoId, ConfirmacionDto datos)
    {
        var resp = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
        return resp.IsSuccessStatusCode;
    }
}

public class ProductoDto {
    public int id { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public double precio { get; set; }
    public int stock { get; set; }
    public string imagen { get; set; }
}
public class ItemCarritoDto {
    public int id { get; set; }
    public string nombre { get; set; }
    public double precio { get; set; }
    public string imagen { get; set; }
    public int cantidad { get; set; }
    public double subtotal { get; set; }
}
public class CrearCarritoRespuesta { public Guid id { get; set; } }
public class ConfirmacionDto {
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
}
public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}