namespace TestDbConnection
{
    using System;
    using System.Data.SqlClient;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
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

            var connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TestWebApi;Integrated Security=True;Connect Timeout=10;Max Pool Size=100;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Application Name=TestDbConnectionApp";

            try
            {
                for (var i = 0; i < 500; i++)
                {
                    var sqlConnection = new SqlConnection(connectionString);
                    
                    using (var sqlCommand = new SqlCommand("SELECT * FROM Products", sqlConnection))
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
            }

            Console.WriteLine("\n\nPress any key to exit.");
            Console.Read();
        }
    }
}
