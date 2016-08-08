using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Provausio.Data.Ado.SqlClient
{
    public abstract class CommandBase
    {
        private readonly string _commandText;
        private readonly string _connectionString;
        private readonly IDbConnection _connection;
        private readonly CommandType _commandType;
        private readonly Stopwatch _sw = new Stopwatch();

        internal readonly string CommandText;

        protected CommandBase(string connectionString, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            CommandText = commandText;

            _connectionString = connectionString;
            _commandType = commandType;
            
            var conn = new SqlConnection(connectionString);
            conn.StateChange += (sender, args) => Trace.TraceInformation($"Connection to {_connectionString} is {args.CurrentState}.");
            _connection = conn;
        }

        protected CommandBase(IDbConnection connection, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            _connection = connection;
            _commandText = commandText;
            _commandType = commandType;

            var conn = _connection as SqlConnection;
            if(conn != null)
                conn.StateChange += (sender, args) => Trace.TraceInformation($"Connection to {_connectionString} is {args.CurrentState}.");
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<SqlParameter> GetParameters()
        {
            // default (no parameters)
            return new List<SqlParameter>();
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        protected IDbCommand GetCommand(SqlTransaction transaction = null)
        {
            var parameters = GetParameters().ToList();

            var command = _connection.CreateCommand();
            command.CommandText = _commandText;
            command.CommandType = _commandType;

            if (parameters.Any())
                parameters.ForEach(p => command.Parameters.Add(p));

            if (transaction != null)
                command.Transaction = transaction;

            return command;
        }

        [Conditional("DEBUG")]
        protected void StartTimer()
        {
            _sw.Reset();
            _sw.Start();
        }

        [Conditional("DEBUG")]
        protected void StopTimer(string operationName = null)
        {
            _sw.Stop();
            Trace.WriteLine($"{operationName ?? "Operation"} took {_sw.ElapsedMilliseconds}ms to complete.");
        }
    }
}