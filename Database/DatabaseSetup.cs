using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

public static class DatabaseSetup
{
    private static readonly IConfigurationRoot configuration;

    // Static constructor to initialize configuration
    static DatabaseSetup()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static void ReinitializeDatabase()
    {
        string connectionString = configuration.GetConnectionString("AppetiteDatabase"); // Dynamically retrieve connection string

        string sqlScript = File.ReadAllText("Database/schema.sql"); // Load the schema SQL file
        
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = new SqlCommand(sqlScript, connection);
        command.ExecuteNonQuery();
        
        Console.WriteLine("Database reinitialized successfully.");
    }
}