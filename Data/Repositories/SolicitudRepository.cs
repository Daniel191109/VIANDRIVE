using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VIANDRIVEFULL.Models;
using VIANDRIVEFULL.Models.DTOs;

namespace VIANDRIVEFULL.Data.Repositories
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly string _connectionString;

        public SolicitudRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private bool TieneColumna(SqlDataReader reader, string nombreColumna)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(nombreColumna, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private SolicitudServicio MapearSolicitud(SqlDataReader reader)
        {
            var s = new SolicitudServicio();

            if (TieneColumna(reader, "Id")) s.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            if (TieneColumna(reader, "ClienteId") && !reader.IsDBNull(reader.GetOrdinal("ClienteId"))) s.ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId"));
            if (TieneColumna(reader, "UsuarioId") && !reader.IsDBNull(reader.GetOrdinal("UsuarioId"))) s.ClienteId = reader.GetInt32(reader.GetOrdinal("UsuarioId"));
            if (TieneColumna(reader, "ConductorId") && !reader.IsDBNull(reader.GetOrdinal("ConductorId"))) s.ConductorId = reader.GetInt32(reader.GetOrdinal("ConductorId"));
            if (TieneColumna(reader, "VehiculoId") && !reader.IsDBNull(reader.GetOrdinal("VehiculoId"))) s.VehiculoId = reader.GetInt32(reader.GetOrdinal("VehiculoId"));

            if (TieneColumna(reader, "Origen") && !reader.IsDBNull(reader.GetOrdinal("Origen"))) s.Origen = reader.GetString(reader.GetOrdinal("Origen"));
            if (TieneColumna(reader, "Destino") && !reader.IsDBNull(reader.GetOrdinal("Destino"))) s.Destino = reader.GetString(reader.GetOrdinal("Destino"));

            // 📍 MAPEO SEGURO DE COORDENADAS
            if (TieneColumna(reader, "LatOrigen") && !reader.IsDBNull(reader.GetOrdinal("LatOrigen"))) s.LatOrigen = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LatOrigen")));
            if (TieneColumna(reader, "LngOrigen") && !reader.IsDBNull(reader.GetOrdinal("LngOrigen"))) s.LngOrigen = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LngOrigen")));
            if (TieneColumna(reader, "LatDestino") && !reader.IsDBNull(reader.GetOrdinal("LatDestino"))) s.LatDestino = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LatDestino")));
            if (TieneColumna(reader, "LngDestino") && !reader.IsDBNull(reader.GetOrdinal("LngDestino"))) s.LngDestino = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LngDestino")));
            if (TieneColumna(reader, "LatConductor") && !reader.IsDBNull(reader.GetOrdinal("LatConductor"))) s.LatConductor = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LatConductor")));
            if (TieneColumna(reader, "LngConductor") && !reader.IsDBNull(reader.GetOrdinal("LngConductor"))) s.LngConductor = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("LngConductor")));

            if (TieneColumna(reader, "FechaSolicitud") && !reader.IsDBNull(reader.GetOrdinal("FechaSolicitud"))) s.FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("FechaSolicitud"));
            if (TieneColumna(reader, "FechaHora") && !reader.IsDBNull(reader.GetOrdinal("FechaHora"))) s.FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("FechaHora"));

            if (TieneColumna(reader, "Estado") && !reader.IsDBNull(reader.GetOrdinal("Estado"))) s.Estado = reader.GetString(reader.GetOrdinal("Estado"));

            if (TieneColumna(reader, "PrecioSugerido") && !reader.IsDBNull(reader.GetOrdinal("PrecioSugerido"))) s.PrecioSugerido = reader.GetDecimal(reader.GetOrdinal("PrecioSugerido"));

            if (TieneColumna(reader, "OfertaPasajero") && !reader.IsDBNull(reader.GetOrdinal("OfertaPasajero"))) s.OfertaPasajero = reader.GetDecimal(reader.GetOrdinal("OfertaPasajero"));
            if (TieneColumna(reader, "Tarifa") && !reader.IsDBNull(reader.GetOrdinal("Tarifa"))) s.OfertaPasajero = reader.GetDecimal(reader.GetOrdinal("Tarifa"));

            if (TieneColumna(reader, "ContraofertaConductor") && !reader.IsDBNull(reader.GetOrdinal("ContraofertaConductor"))) s.ContraofertaConductor = reader.GetDecimal(reader.GetOrdinal("ContraofertaConductor"));
            if (TieneColumna(reader, "OfertaAceptada") && !reader.IsDBNull(reader.GetOrdinal("OfertaAceptada"))) s.OfertaAceptada = reader.GetBoolean(reader.GetOrdinal("OfertaAceptada"));
            if (TieneColumna(reader, "OfertaRechazada") && !reader.IsDBNull(reader.GetOrdinal("OfertaRechazada"))) s.OfertaRechazada = reader.GetBoolean(reader.GetOrdinal("OfertaRechazada"));
            if (TieneColumna(reader, "MedioPago") && !reader.IsDBNull(reader.GetOrdinal("MedioPago"))) s.MedioPago = reader.GetString(reader.GetOrdinal("MedioPago"));
            if (TieneColumna(reader, "PagoConfirmado") && !reader.IsDBNull(reader.GetOrdinal("PagoConfirmado"))) s.PagoConfirmado = reader.GetBoolean(reader.GetOrdinal("PagoConfirmado"));

            // 🚗 Mapeo de Relaciones
            if (TieneColumna(reader, "ClienteNombre") && !reader.IsDBNull(reader.GetOrdinal("ClienteNombre")))
            {
                s.Cliente = new Usuario { Nombre = reader.GetString(reader.GetOrdinal("ClienteNombre")) };
            }

            if (TieneColumna(reader, "ConductorNombre") && !reader.IsDBNull(reader.GetOrdinal("ConductorNombre")))
            {
                // Cambiado de new Usuario a new Conductor para que coincida con el modelo
                s.Conductor = new Conductor { Nombre = reader.GetString(reader.GetOrdinal("ConductorNombre")) };
            }

            if (TieneColumna(reader, "VehiculoMarca") && !reader.IsDBNull(reader.GetOrdinal("VehiculoMarca")))
            {
                s.Vehiculo = new Vehiculo
                {
                    Marca = reader.GetString(reader.GetOrdinal("VehiculoMarca")),
                    Modelo = TieneColumna(reader, "VehiculoModelo") && !reader.IsDBNull(reader.GetOrdinal("VehiculoModelo")) ? reader.GetString(reader.GetOrdinal("VehiculoModelo")) : "",
                    Placa = TieneColumna(reader, "VehiculoPlaca") && !reader.IsDBNull(reader.GetOrdinal("VehiculoPlaca")) ? reader.GetString(reader.GetOrdinal("VehiculoPlaca")) : ""
                };
            }

            return s;
        }

        public async Task<bool> Registrar(SolicitudViajeDto dto)
        {
            try
            {
                var query = @"INSERT INTO SolicitudesServicio 
                            (ClienteId, ConductorId, VehiculoId, Origen, Destino, LatOrigen, LngOrigen, LatDestino, LngDestino, FechaSolicitud, Estado, PrecioSugerido, OfertaPasajero, MedioPago, PagoConfirmado, OfertaAceptada, OfertaRechazada)
                            VALUES 
                            (@ClienteId, 0, 0, @Origen, @Destino, @LatOrigen, @LngOrigen, @LatDestino, @LngDestino, GETDATE(), 'Pendiente', @OfertaPasajero, @OfertaPasajero, 'Efectivo', 0, 0, 0)";

                using var conexion = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(query, conexion);

                cmd.Parameters.AddWithValue("@ClienteId", dto.UsuarioId > 0 ? dto.UsuarioId : 1);
                cmd.Parameters.AddWithValue("@Origen", string.IsNullOrWhiteSpace(dto.Origen) ? "Ubicación cliente" : dto.Origen);
                cmd.Parameters.AddWithValue("@Destino", string.IsNullOrWhiteSpace(dto.Destino) ? "Destino cliente" : dto.Destino);
                cmd.Parameters.AddWithValue("@LatOrigen", dto.LatOrigen);
                cmd.Parameters.AddWithValue("@LngOrigen", dto.LngOrigen);
                cmd.Parameters.AddWithValue("@LatDestino", dto.LatDestino);
                cmd.Parameters.AddWithValue("@LngDestino", dto.LngDestino);
                cmd.Parameters.AddWithValue("@OfertaPasajero", dto.Tarifa > 0 ? dto.Tarifa : 8000);

                await conexion.OpenAsync();
                var filas = await cmd.ExecuteNonQueryAsync();

                return filas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error SQL al Registrar Solicitud]: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SolicitudServicio>> ObtenerSolicitudesPorEstadoAsync(string estado)
        {
            var solicitudes = new List<SolicitudServicio>();
            var query = @"SELECT * FROM SolicitudesServicio WHERE Estado = @Estado";

            using var conexion = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@Estado", estado);

            await conexion.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                solicitudes.Add(MapearSolicitud(reader));
            }

            return solicitudes;
        }

        public async Task<SolicitudServicio> ObtenerPorIdAsync(int id)
        {
            // 🔍 Consulta con LEFT JOIN para traer de forma segura el Cliente, Conductor y el Vehículo
            var query = @"SELECT s.*, 
                                c.Nombre AS ClienteNombre, 
                                cond.Nombre AS ConductorNombre, 
                                v.Marca AS VehiculoMarca, 
                                v.Modelo AS VehiculoModelo, 
                                v.Placa AS VehiculoPlaca
                          FROM SolicitudesServicio s
                          LEFT JOIN Usuarios c ON s.ClienteId = c.Id
                          LEFT JOIN Usuarios cond ON s.ConductorId = cond.Id
                          LEFT JOIN Vehiculos v ON s.VehiculoId = v.Id
                          WHERE s.Id = @Id";

            using var conexion = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@Id", id);

            await conexion.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapearSolicitud(reader);
            }

            return null;
        }

        public async Task<SolicitudServicio> ObtenerPorId(int id)
        {
            return await ObtenerPorIdAsync(id);
        }

        public async Task ActualizarAsync(SolicitudServicio s)
        {
            var query = @"UPDATE SolicitudesServicio SET 
                            ConductorId = @ConductorId,
                            VehiculoId = @VehiculoId,
                            Estado = @Estado,
                            ContraofertaConductor = @ContraofertaConductor,
                            OfertaAceptada = @OfertaAceptada,
                            LatConductor = @LatConductor,
                            LngConductor = @LngConductor
                          WHERE Id = @Id";

            using var conexion = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conexion);

            cmd.Parameters.AddWithValue("@Id", s.Id);
            cmd.Parameters.AddWithValue("@ConductorId", s.ConductorId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VehiculoId", s.VehiculoId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", s.Estado ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ContraofertaConductor", (object)s.ContraofertaConductor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OfertaAceptada", s.OfertaAceptada);
            cmd.Parameters.AddWithValue("@LatConductor", (object)s.LatConductor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LngConductor", (object)s.LngConductor ?? DBNull.Value);

            await conexion.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}