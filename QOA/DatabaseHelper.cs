﻿using Npgsql;
using System.Configuration;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
    }

    public void ExecuteCommand(string commandText, Dictionary<string, object> parameters)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
                cmd.ExecuteNonQuery();
            }
        }
    }

    public T ExecuteScalar<T>(string commandText, Dictionary<string, object> parameters)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
                return (T)cmd.ExecuteScalar();
            }
        }
    }

    public NpgsqlDataReader ExecuteReader(string commandText, Dictionary<string, object> parameters)
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            return cmd.ExecuteReader();
        }
    }

    public NpgsqlDataReader ExecuteReader(string commandText)
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            return cmd.ExecuteReader(); // Executa a consulta sem parâmetros
        }
    }

    public async Task ExecuteCommandAsync(string commandText, Dictionary<string, object> parameters)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<NpgsqlDataReader> ExecuteReaderAsync(string commandText, Dictionary<string, object> parameters)
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        using (var cmd = new NpgsqlCommand(commandText, conn))
        {
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            return await cmd.ExecuteReaderAsync();
        }
    }


}
