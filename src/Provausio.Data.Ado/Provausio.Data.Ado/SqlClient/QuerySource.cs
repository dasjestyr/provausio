using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Provausio.Data.Ado.Extensions;

namespace Provausio.Data.Ado.SqlClient
{
    /// <summary> 
    /// </summary>
    /// <typeparam name="T">The object type of the result collection</typeparam>
    /// <seealso cref="CommandBase" />
    public abstract class QuerySource<T> : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySource{T}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        protected QuerySource(string connectionString, string commandText, CommandType commandType = CommandType.StoredProcedure)
            : base(connectionString, commandText, commandType)
        {
        }

        /// <summary>
        /// When implemented, provides logic for reading a single record into an object
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public abstract T GetFromReader(IDataRecord record);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteQueryAsync()
        {
            using (var command = GetCommand())
            {
                if(command.Connection.State != ConnectionState.Open)
                    await Task.Run(() => command.Connection.Open()).ConfigureAwait(false);

                Trace.TraceInformation($"Executing {command.ToInfoString()}...");

                StartTimer();
                using (var reader = await Task.Run(() => command.ExecuteReader(CommandBehavior.CloseConnection)).ConfigureAwait(false))
                {
                    var result = new List<T>();
                    while (reader.Read())
                    {
                        var item = GetFromReader(reader);
                        result.Add(item);
                    }

                    StopTimer();

                    Trace.TraceInformation($"Query completed with {result.Count} results!");

                    return result;
                }
            }
        }
    }
}