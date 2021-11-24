using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using myStore.entities;
using FastMember;
using System;
using System.Linq;
using System.Threading.Tasks;

/*
 * 1. Соединение
 * 2. Реализация запросов
 */

namespace myStore
{
    public static class Database
    {
        private static string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        public static async IAsyncEnumerable<T> Enumerate<T>(string sql) where T : class, new()
        {
            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        yield return reader.ConvertToObject<T>();
                    }
                }

                await connection.CloseAsync();
            }
        }

        public static async IAsyncEnumerable<T> EnumerateList<T>(string sql) where T : class
        {
            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        yield return (T)reader.GetValue(0);
                    }
                }

                await connection.CloseAsync();
            }
        }

        public static async Task<T> GetObject<T>(string sql) where T : class, new()
        {
            T obj = null;

            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        obj = reader.ConvertToObject<T>();
                    }
                }

                await connection.CloseAsync();
            }

            return obj;
        }

        public static T ConvertToObject<T>(this NpgsqlDataReader rd) where T : class, new()
        {
            Type type = typeof(T);
            TypeAccessor accessor = TypeAccessor.Create(type);
            MemberSet members = accessor.GetMembers();
            T t = new T();

            for (int i = 0; i < rd.FieldCount; i++)
            {
                string fieldName = rd.GetName(i);

                if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (rd.IsDBNull(i)) accessor[t, fieldName] = null;
                    else accessor[t, fieldName] = rd.GetValue(i);
                }
            }

            return t;
        }
    }
}
