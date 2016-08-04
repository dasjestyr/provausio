using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Provausio.Data.Ado.Extensions;

namespace Provausio.Data.Ado.SqlClient
{
    public abstract class NonQuerySource : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonQuerySource"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        protected NonQuerySource(string connectionString, string commandText, CommandType commandType = CommandType.StoredProcedure)
            : base(connectionString, commandText, commandType)
        {
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        public async Task<SqlParameterCollection> ExecuteNonQueryAsync(SqlTransaction transaction = null)
        {
            using (var command = GetCommand(transaction))
            {
                await command.Connection.OpenAsync().ConfigureAwait(false);

                Trace.TraceInformation($"Executing {command.ToInfoString()}");

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                command.Connection.Close();
                return command.Parameters;
            }
        }
    }
}
