using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Provausio.Data.Ado.Extensions
{
    internal static class DbCommandExt
    {
        /// <summary>
        /// Reads the <see cref="SqlCommand"/> object and returns the command text along with all parameters as a string
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns></returns>
        public static string ToInfoString(this IDbCommand cmd)
        {
            if (cmd == null) return string.Empty;

            var queryString = (
                from SqlParameter p
                in cmd.Parameters
                where p?.Value != null
                select $"{p.ParameterName} = {p.Value}, ")
                    .Aggregate(cmd.CommandText + " ", (current, parameter) => current + parameter)
                    .TrimEnd(", ".ToCharArray());

            return queryString;
        }
    }
}
