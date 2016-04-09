using System;
using System.Data;
using System.Data.SqlClient;

namespace LooksFamiliar.Tools.SQLUpdate
{
    class Program
    {
        public static string SqlServerName { get; private set; }
        public static string SqlDatabaseName { get; private set; }
        public static string SqlUserName { get; private set; }
        public static string SqlPassword { get; private set; }

        static void Main(string[] args)
        {
            SqlServerName = string.Empty;
            SqlDatabaseName = string.Empty;
            SqlUserName = string.Empty;
            SqlPassword = string.Empty;

            if (args.Length < 4)
            {
                Usage();
                return;
            }

            // parse command line arguments
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-server":
                        i++;
                        SqlServerName = args[i];
                        break;
                    case "-database":
                        i++;
                        SqlDatabaseName = args[i];
                        break;
                    case "-username":
                        i++;
                        SqlUserName = args[i];
                        break;
                    case "-password":
                        i++;
                        SqlPassword = args[i];
                        break;
                    case "?": // help
                        Usage();
                        break;
                    default: // default
                        Usage();
                        break;
                }
            }

            if ((SqlServerName == string.Empty) ||
                (SqlDatabaseName == string.Empty) ||
                (SqlUserName == string.Empty) ||
                (SqlPassword == string.Empty))
            {
                Console.WriteLine("ERROR: missing command line arguments.");
                Usage();
                return;
            }

            // create biometrics table
            var commandText = @"CREATE TABLE[dbo].[biometrics] (
                                [deviceid] [char](256) NOT NULL,
                                [participantid] [char](256) NOT NULL,
                                [longitude] float NOT NULL,
	                            [latitude] float NOT NULL,
	                            [reading] datetime NOT NULL,
	                            [type] bigint NOT NULL,
	                            [value] float NOT NULL)

                            CREATE CLUSTERED INDEX[biometrics] ON[dbo].[biometrics]
                            (
	                            [deviceid] ASC
                            )";

            CreateTable(commandText);

            // create alarms table
            commandText = @"CREATE TABLE[dbo].[alarms] (
                                [deviceid] [char](256) NOT NULL,
                                [participantid] [char](256) NOT NULL,
                                [longitude] float NOT NULL,
	                            [latitude] float NOT NULL,
	                            [reading] datetime NOT NULL,
	                            [type] bigint NOT NULL,
	                            [value] float NOT NULL)

                            CREATE CLUSTERED INDEX[alarms] ON[dbo].[alarms]
                            (
	                            [deviceid] ASC
                            )";

            CreateTable(commandText);

        }

        private static void CreateTable(string commandText)
        {
            try
            {
                var scsBuilder = new SqlConnectionStringBuilder
                {
                    ["Server"] = "tcp:" + SqlServerName + ".database.windows.net,1433",
                    ["User ID"] = SqlUserName + "@" + SqlServerName,
                    ["Password"] = SqlPassword,
                    ["Database"] = SqlDatabaseName,
                    ["Trusted_Connection"] = false,
                    ["Integrated Security"] = false,
                    ["Encrypt"] = true,
                    ["Connection Timeout"] = 30
                };

                SqlConnection sqlConnection;

                using (sqlConnection = new SqlConnection(scsBuilder.ToString()))
                {
                    sqlConnection.Open();
                    IDbCommand command = new SqlCommand();
                    command.Connection = sqlConnection;
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err.Message);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: SQLUpdate -server [server name] -database [database name] -username [user name] -password [password]");
            Console.WriteLine("");
            Console.WriteLine("   -server     name of the sql database server");
            Console.WriteLine("   -database   name of the database");
            Console.WriteLine("   -username   sql server username");
            Console.WriteLine("   -password   sql server password");
            Console.WriteLine("");
        }
    }
}
