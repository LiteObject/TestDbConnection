namespace TestDbConnection
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private const string ConnectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cp3-ordermanagement-local;Integrated Security=True;Connect Timeout=1;Max Pool Size=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Application Name=TestDbConnectionApp";

        /// <summary>
        /// The main.
        /// Article:
        /// https://github.com/aspnet/EntityFrameworkCore/issues/10169
        /// https://github.com/dotnet/corefx/issues/24966
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            /*
               select	count(*) as sessions,
                        s.host_name,
                        s.host_process_id,
                        s.program_name,
                        db_name(s.database_id) as database_name
                from	sys.dm_exec_sessions s
                where	is_user_process = 1
                group by host_name, host_process_id, program_name, database_id
                order by count(*) desc;             
             */
             
            Test(50000);

            /*try
            {
                for (var i = 0; i <= 500; i++)
                {
                    var sqlConnection = new SqlConnection(ConnectionString);
                    
                    using (var sqlCommand = new SqlCommand("SELECT * FROM dbo.Orders", sqlConnection))
                    {
                        // Do not close connection to simulate connection leak
                        sqlConnection.Open();
                        Console.WriteLine($"Connection Open: {i}");

                        var sqlDataReader = sqlCommand.ExecuteReader();
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }*/

            Console.WriteLine("\n\nPress any key to exit.");
            Console.Read();
        }

        /// <summary>
        /// The test.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        private static void Test(int count)
        {
            Parallel.For(
                1,
                count,
                i =>
                    {
                        var id = DoSomething(i).Result;
                        Console.WriteLine($"Id: {id}");
                    });
        }

        /// <summary>
        /// The do something.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<int> DoSomething(int id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                Console.WriteLine($"Connection {id}");
                await connection.OpenAsync();
                using (var command = new SqlCommand($"SELECT * FROM dbo.Orders;", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                reader.GetValue(i);
                            }
                        }

                        await Task.Delay(50000);
                        // Thread.Sleep(1000);
                    }
                }
            }

            return id;
        }
    }
}
