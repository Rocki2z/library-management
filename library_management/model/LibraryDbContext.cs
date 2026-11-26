using System;
using System.IO;
using Npgsql;

namespace library_management.model
{
    /// <summary>Loads local database settings and opens PostgreSQL connections.</summary>
    public static class LibraryDbContext
    {
        private static readonly string ConnectionString;

        static LibraryDbContext()
        {
            LoadEnvironmentFile();
            ConnectionString = Environment.GetEnvironmentVariable("LIBRARY_DB_CONNECTION");
        }

        public static NpgsqlConnection OpenConnection()
        {
            if (String.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("Set LIBRARY_DB_CONNECTION in .env or the process environment before starting the app.");

            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        private static void LoadEnvironmentFile()
        {
            var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            if (!File.Exists(envPath))
            {
                var projectEnvPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".env"));
                if (File.Exists(projectEnvPath)) envPath = projectEnvPath;
                else return;
            }

            foreach (var rawLine in File.ReadAllLines(envPath))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                var separator = line.IndexOf('=');
                if (separator <= 0) continue;

                var key = line.Substring(0, separator).Trim();
                var value = line.Substring(separator + 1).Trim();
                if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') || (value[0] == '\'' && value[value.Length - 1] == '\'')))
                    value = value.Substring(1, value.Length - 2);

                if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                    Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
