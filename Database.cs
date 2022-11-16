using System;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;


namespace SqliTest
{
	public class Database
	{
		private SqliteConnection Connection;
		public Database()
		{
			Directory.CreateDirectory("databases");
			var connectionString = (new SqliteConnectionStringBuilder
			{
				Mode = SqliteOpenMode.ReadWriteCreate
				, Password = "atestpassword"
				, DataSource = string.Format("databases/data-{0}.db", "b0db33a2-0e22-4baf-8450-b07ad51fd74e")
			}).ToString();

			// Create new connection object
			Connection = new SqliteConnection(connectionString);
			// Register event
			Connection.StateChange += Connection_OnStateChange;

			try
			{
				Connection.Open();
			}
			catch (SqliteException sqlex)
			{
				Console.WriteLine("Failed to open db file!");
				Console.WriteLine("Error msg: {0}", sqlex.Message);
			}
		}

		private void Connection_OnStateChange(object sender, System.Data.StateChangeEventArgs e)
		{
			if(e.CurrentState == System.Data.ConnectionState.Open)
			{
				Console.WriteLine("Connected!");
				Connection_PostOpen();
			}
		}
		
		private void Connection_PostOpen()
        {
            try
            {
                ExecuteNonQuery("CREATE TABLE IF NOT EXISTS group_invites (AgentKey TEXT PRIMARY KEY, LastAttempt INTEGER NOT NULL) WITHOUT ROWID");
            }
            catch (SqliteException sqlex)
            {
                Console.WriteLine("SQLite Error: {0}", sqlex.Message);
            }

            try
            {
                ExecuteNonQuery("CREATE TABLE IF NOT EXISTS motd_notifications (AgentKey TEXT PRIMARY KEY, LastAttempt INTEGER NOT NULL) WITHOUT ROWID");
            }
            catch (SqliteException sqlex)
            {
                Console.WriteLine("SQLite Error: {0}", sqlex.Message);
            }

            try
			{
				ExecuteNonQuery("CREATE TABLE IF NOT EXISTS motd_updated (RegionName TEXT PRIMARY KEY, LastAttempt INTEGER NOT NULL)");
            }
            catch (SqliteException sqlex)
            {
                Console.WriteLine("SQLite Error: {0}", sqlex.Message);
            }

            Console.WriteLine("Wrote tables!");
		}

		public void ExecuteNonQuery(string commandText)
		{
			using (var command = Connection.CreateCommand())
			{
				command.CommandText = commandText;
				command.ExecuteNonQuery();
			}
        }

        public Task<int> ExecuteNonQueryAsync(string commandText)
        {
			using (var command = Connection.CreateCommand())
			{
				command.CommandText = commandText;
				return command.ExecuteNonQueryAsync();
			}
        }

        public SqliteDataReader ExecuteQuery(string commandText)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteReader();
            }
        }

		public Task<SqliteDataReader> ExecuteQueryAsync(string commandText)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteReaderAsync();
            }
        }
	}
}
