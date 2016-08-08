using System;
using System.Data;
using System.Threading.Tasks;
using Provausio.Data.Ado.Extensions;
using Provausio.Data.Ado.SqlClient;
using Xunit;

namespace Provausio.Data.Ado.Tests
{
    public class TinkerTown
    {
        [Fact]
        public async Task Test1()
        {
            var source = new TestQuery();
            var r = await source.ExecuteQueryAsync();
        }

        public class TestQuery : QuerySource<object>
        {
            public TestQuery() 
                : base("Server=airstrike.database.windows.net;Database=base;User Id=das@airstrike; Password=U4PkXAiDd7GPiTUYHAUt;", "SELECT TOP 5 * FROM dbo.Accounts", CommandType.Text)
            {
            }

            public override object GetFromReader(IDataRecord record)
            {
                return new {Token = record.DbCast<Guid>("Token")};
            }
        }
    }
}
