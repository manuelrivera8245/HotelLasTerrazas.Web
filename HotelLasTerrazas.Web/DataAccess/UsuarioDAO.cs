using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using HotelLasTerrazas.Web.Models;


namespace HotelLasTerrazas.Web.DataAccess
{
    public class UsuarioDAO
    {

        /// Valida credenciales. Devuelve el Usuario si son correctas, o null si no.
        public Usuario ValidarCredenciales(string nombreUsuario, string password)
        {
            string claveHash = CalcularHash(password);

            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand(
                "SELECT u.IdUsuario, u.NombreUsuario, u.IdRol, r.NombreRol, u.Estado " +
                "FROM Usuario u INNER JOIN Rol r ON r.IdRol = u.IdRol " +
                "WHERE u.NombreUsuario = @NombreUsuario AND u.ClaveHash = @ClaveHash AND u.Estado = 1", conexion))
            {
                comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                comando.Parameters.AddWithValue("@ClaveHash", claveHash);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Usuario
                        {
                            IdUsuario = Convert.ToInt32(lector["IdUsuario"]),
                            NombreUsuario = lector["NombreUsuario"].ToString(),
                            IdRol = Convert.ToInt32(lector["IdRol"]),
                            NombreRol = lector["NombreRol"].ToString(),
                            Estado = Convert.ToBoolean(lector["Estado"])
                        };
                    }
                }
            }
            return null; // credenciales inválidas
        }

        /// Registra un nuevo Usuario + Cliente en una sola transacción ADO.NET.
        public int RegistrarCliente(Cliente cliente)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar Usuario (rol Cliente = 3, ver tabla Rol)
                        int idUsuario;
                        using (var comandoUsuario = new SqlCommand(
                            "INSERT INTO Usuario (NombreUsuario, ClaveHash, IdRol) " +
                            "VALUES (@NombreUsuario, @ClaveHash, 3); SELECT SCOPE_IDENTITY();",
                            conexion, transaccion))
                        {
                            comandoUsuario.Parameters.AddWithValue("@NombreUsuario", cliente.Correo);
                            comandoUsuario.Parameters.AddWithValue("@ClaveHash", CalcularHash(cliente.Password));
                            idUsuario = Convert.ToInt32(comandoUsuario.ExecuteScalar());
                        }

                        // 2. Insertar Cliente ligado al Usuario recién creado
                        using (var comandoCliente = new SqlCommand(
                            "INSERT INTO Cliente (IdUsuario, TipoDocumento, NumeroDocumento, Nombres, Apellidos, Telefono, Correo, Direccion) " +
                            "VALUES (@IdUsuario, @TipoDocumento, @NumeroDocumento, @Nombres, @Apellidos, @Telefono, @Correo, @Direccion)",
                            conexion, transaccion))
                        {
                            comandoCliente.Parameters.AddWithValue("@IdUsuario", idUsuario);
                            comandoCliente.Parameters.AddWithValue("@TipoDocumento", cliente.TipoDocumento);
                            comandoCliente.Parameters.AddWithValue("@NumeroDocumento", cliente.NumeroDocumento);
                            comandoCliente.Parameters.AddWithValue("@Nombres", cliente.Nombres);
                            comandoCliente.Parameters.AddWithValue("@Apellidos", cliente.Apellidos);
                            comandoCliente.Parameters.AddWithValue("@Telefono", (object)cliente.Telefono ?? DBNull.Value);
                            comandoCliente.Parameters.AddWithValue("@Correo", cliente.Correo);
                            comandoCliente.Parameters.AddWithValue("@Direccion", (object)cliente.Direccion ?? DBNull.Value);
                            comandoCliente.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return idUsuario;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        /// Obtiene el IdCliente a partir del IdUsuario (útil tras el login).
        public int ObtenerIdClientePorUsuario(int idUsuario)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            using (var comando = new SqlCommand(
                "SELECT IdCliente FROM Cliente WHERE IdUsuario = @IdUsuario", conexion))
            {
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                conexion.Open();
                object resultado = comando.ExecuteScalar();
                return resultado != null ? Convert.ToInt32(resultado) : 0;
            }
        }

        /// Hash simple SHA256 para fines académicos.
        /// En un entorno productivo se recomienda usar BCrypt o PBKDF2 con salt.
        public static string CalcularHash(string texto)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
