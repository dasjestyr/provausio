using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Provausio.Data.Ado.SqlClient
{
    public abstract class CommandBase
    {
        internal readonly string CommandText;

        private readonly string _connectionString;
        private readonly CommandType _commandType;

        protected CommandBase(string connectionString, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            _connectionString = connectionString;
            CommandText = commandText;
            _commandType = commandType;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<SqlParameter> GetParameters()
        {
            return new List<SqlParameter>();
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        protected SqlCommand GetCommand(SqlTransaction transaction = null)
        {
            var parameters = GetParameters().ToList();
            var connection = new SqlConnection(_connectionString);
            connection.StateChange += (sender, args) => Trace.TraceInformation($"Connection to {_connectionString} is {args.CurrentState}.");

            var command = new SqlCommand(CommandText, connection) {CommandType = _commandType};

            if (parameters.Any())
                command.Parameters.AddRange(parameters.ToArray());

            if (transaction != null)
                command.Transaction = transaction;

            return command;
        }
    }
}