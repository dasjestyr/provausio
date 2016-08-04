using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
                using (command.Connection)
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

    /// <summary>
    /// </summary>
    /// <typeparam name="T">The type of the object being used to build the parameter list</typeparam>
    /// <seealso cref="CommandBase" />
    public abstract class BulkNonQuerySource<T> : CommandBase
    {
        protected BulkNonQuerySource(string connectionString, string commandText, CommandType commandType = CommandType.StoredProcedure) 
            : base(connectionString, commandText, commandType)
        {
        }

        /// <summary>
        /// Executes the bulk non query.
        /// </summary>
        /// <param name="parameterSource">The parameter source.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Bulk insert 100% failure!</exception>
        public async Task<bool> ExecuteBulkNonQueryAsync(IEnumerable<T> parameterSource, SqlTransaction transaction = null)
        {
            using (var command = GetCommand(transaction))
            {
                using (command.Connection)
                {
                    await command.Connection.OpenAsync();

                    var failed = 0;
                    var sourceItems = parameterSource.ToList();
                    foreach (var item in sourceItems)
                    {
                        command.Parameters.Clear();
                        var parameters = GetParameters(item);
                        command.Parameters.AddRange(parameters);

                        var result = await command.ExecuteNonQueryAsync();

                        if (result < 0)
                            failed++;
                    }

                    if (failed > 0)
                        Trace.TraceError($"{failed} non-queries did not succeed ({command.ToInfoString()})");

                    if (failed == sourceItems.Count)
                        throw new Exception("Bulk insert 100% failure!");
                }
            }

            return true;
        }

        /// <summary>
        /// When implemented, uses input to create a collection of <see cref="SqlParameter"/>
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected abstract SqlParameter[] GetParameters(T input);
    }
}
