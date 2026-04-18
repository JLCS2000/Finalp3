using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace P1SC08
{
    // ─────────────────────────────────────────────────────────────────
    // Clase de conexión
    // ─────────────────────────────────────────────────────────────────
    public class cnn
    {
        public static string db = @"Data Source=localhost;Initial Catalog=DBpractica04;Integrated Security=true;";
    }

    // ─────────────────────────────────────────────────────────────────
    // Clase auxiliar de búsquedas
    // ─────────────────────────────────────────────────────────────────
    public class Busco
    {
        /// <summary>
        /// Obtiene el siguiente número de secuencia para el ID dado.
        /// BUG CORREGIDO: cmd.Dispose() y cnx.Close() estaban después del return
        /// (código muerto). Ahora se usa 'using' para garantizar la liberación
        /// de recursos incluso si ocurre una excepción.
        /// </summary>
        public static string BuscaUltimoNumero(string nmId)
        {
            string sQuery = "SELECT SECUENCIA + 1 AS ULTIMO_NUMERO " +
                            "  FROM SECUENCIA " +
                            " WHERE ID = @ID";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(sQuery, cnx))
                {
                    cmd.Parameters.AddWithValue("@ID", nmId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["ULTIMO_NUMERO"].ToString();
                        }
                    }
                }
            }

            return "1"; // valor por defecto si no existe la secuencia
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Utilidad de seguridad: hash de contraseñas (SHA-256)
    // RNF-03: Las contraseñas deben almacenarse con hash SHA-256
    // ─────────────────────────────────────────────────────────────────
    public class Seguridad
    {
        /// <summary>
        /// Genera el hash SHA-256 de una contraseña en texto plano.
        /// </summary>
        public static string HashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>
        /// Compara una contraseña en texto plano contra su hash almacenado.
        /// </summary>
        public static bool VerificarPassword(string inputPlano, string hashAlmacenado)
        {
            return HashSHA256(inputPlano) == hashAlmacenado;
        }
    }
}
