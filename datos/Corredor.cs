using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TUP;

public enum ResultadoEjecucion {
    Ok,
    FallaCapeta,
    FallaServidor,
    FallaCliente,
    FallaNavegador,
    FallaBaseDatos,
    FallaGeneral
}

public class Corredor {
    public static async Task<ResultadoEjecucion> CorrerSistema(string origen) {
        var carpetaServidor = Path.Combine(origen, "servidor");
        var carpetaCliente = Path.Combine(origen, "cliente");

        try {
            // Matar procesos dotnet escuchando en puertos TCP (equivalente a: lsof -tiTCP -sTCP:LISTEN -c dotnet | xargs -r kill -9)
            await MatarProcesoPuerto("5184");
            await MatarProcesoPuerto("5177");

            var resultadoServidor = await EjecutarProyecto(carpetaServidor);
            if (resultadoServidor != "ok") {
                return resultadoServidor.Contains("base de datos")
                    ? ResultadoEjecucion.FallaBaseDatos
                    : ResultadoEjecucion.FallaServidor;
            }
            Consola.Escribir("[Auditoría] Servidor iniciado correctamente", ConsoleColor.Yellow);
            // Esperar un poco para que el servidor inicie
            await Task.Delay(1000);

            // Ejecutar cliente
            var resultadoCliente = await EjecutarProyecto(carpetaCliente);
            if (resultadoCliente != "ok")
            {

                return resultadoCliente.Contains("base de datos")
                    ? ResultadoEjecucion.FallaBaseDatos
                    : ResultadoEjecucion.FallaCliente;
            }

            // Esperar un poco para que el cliente inicie
            Consola.Escribir("[Auditoría] Cliente iniciado correctamente", ConsoleColor.Yellow);
            await Task.Delay(1000);

            // Abrir el navegador en localhost:5177
            try {
                var psi = new ProcessStartInfo {
                    FileName = "open",
                    Arguments = "http://localhost:5184",
                    UseShellExecute = true
                };
                Process.Start(psi);
                Consola.Escribir("[Auditoría] Navegador abierto en http://localhost:5184", ConsoleColor.Yellow);
                return ResultadoEjecucion.Ok;
            } catch {
                return ResultadoEjecucion.FallaNavegador;
            }
        } catch {
            return ResultadoEjecucion.FallaGeneral;
        }
    }

    private static async Task<string> EjecutarProyecto(string carpetaProyecto) {
        for (int intento = 1; intento <= 2; intento++) {
            Trace.WriteLine($"[Auditoría] EjecutarProyecto: Iniciando proyecto '{carpetaProyecto}', intento {intento}");
            try {
                var psi = new ProcessStartInfo {
                    FileName = "dotnet",
                    Arguments = "run",
                    WorkingDirectory = carpetaProyecto,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proceso = new Process { StartInfo = psi };
                proceso.Start();

                // Leer resultado tras Delay
                await Task.Delay(1000);
                if (!proceso.HasExited) {
                    return "ok";
                }
                var output = await proceso.StandardOutput.ReadToEndAsync();
                var errorCompleto = output + await proceso.StandardError.ReadToEndAsync();
                if (proceso.ExitCode != 0 &&
                    (errorCompleto.Contains("address already in use") || errorCompleto.Contains("puerto") || errorCompleto.Contains("port"))) {
                    // Extraer puerto del mensaje de error
                    var match = Regex.Match(errorCompleto, @":(\d+)");
                    var puertoBloqueado = match.Success ? match.Groups[1].Value : "desconocido";
                    Trace.WriteLine($"[Auditoría] Puerto ocupado detectado: {puertoBloqueado}");
                    if (intento == 1) {
                        await MatarProcesoPuerto(puertoBloqueado);
                        continue; // Reintentar
                    } else {
                        return $"error: Puerto {puertoBloqueado} ocupado después de limpiar procesos";
                    }
                } else if (errorCompleto.Contains("no such table") || errorCompleto.Contains("Migrations") || errorCompleto.Contains("SqliteException") || errorCompleto.Contains("Unhandled exception")) {
                    return $"error: Error de base de datos - falta crear migraciones o aplicar schema";
                } else {
                    return $"error: {errorCompleto}";
                }

                return $"ok en: {puertoBloqueado}";
            } catch (Exception ex) {
                if (intento == 2) {
                    return $"error: {ex.Message}";
                }
                await MatarProcesosPuertos();
            }
        }

        return $"error: Falló después de 2 intentos";
    }

    // Nuevo método para matar procesos en puerto específico
    private static async Task MatarProcesoPuerto(string puerto) {
        try {
            Trace.WriteLine($"[Auditoría] MatarProcesoPuerto: Puerto {puerto}");
            var psi = new ProcessStartInfo { FileName = "lsof", Arguments = $"-ti:{puerto}", UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true };
            using var proceso = Process.Start(psi);
            if (proceso != null) {
                var salida = await proceso.StandardOutput.ReadToEndAsync();
                await proceso.WaitForExitAsync();
                if (!string.IsNullOrWhiteSpace(salida)) {
                    foreach (var pid in salida.Trim().Split('\n')) {
                        if (int.TryParse(pid.Trim(), out int pidNumero)) {
                            try {
                                var kill = Process.Start("kill", $"-9 {pidNumero}");
                                await kill?.WaitForExitAsync();
                            } catch { }
                        }
                    }
                }
            }
        } catch { }
    }

    private static async Task MatarProcesosPuertos() {
        try {
            var psi = new ProcessStartInfo {
                FileName = "lsof",
                Arguments = "-tiTCP -sTCP:LISTEN -c dotnet",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using (var proceso = Process.Start(psi)) {
                if (proceso != null) {
                    var salida = await proceso.StandardOutput.ReadToEndAsync();
                    await proceso.WaitForExitAsync();
                    if (!string.IsNullOrWhiteSpace(salida)) {
                        foreach (var pid in salida.Trim().Split('\n')) {
                            if (int.TryParse(pid.Trim(), out int pidNumero)) {
                                try {
                                    var kill = Process.Start("kill", $"-9 {pidNumero}");
                                    await kill?.WaitForExitAsync();
                                } catch { }
                            }
                        }
                    }
                }
            }
        } catch {
            // Manejo de errores opcional o logging
        }
    }

}