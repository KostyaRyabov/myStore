using Npgsql;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace myStore
{
    public static class Database
    {
        private static string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public static async IAsyncEnumerable<T> Enumerate<T>(string sql) where T : Accessored<T>, new()
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

        public static async IAsyncEnumerable<T> EnumerateArray<T>(string sql) where T : class
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

        public static async Task<T> GetObject<T>(string sql) where T : Accessored<T>, new()
        {
            T result = null;

            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result = reader.ConvertToObject<T>();
                    }
                }

                await connection.CloseAsync();
            }

            return result;
        }

        public static async void ExecuteOne(string sql, Dictionary<string, object> parameters = null, Tuple<string, object> binding = null)
        {
            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();

                if (parameters != null)
                {
                    sql = string.Format(sql, string.Join(", ", parameters.Select(item => $"{item.Key} = :{item.Key}")));
                }

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            command.Parameters.AddWithValue(p.Key, p.Value);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }

        public static async Task<int[]> Insert<T>(string tableName, IList<T> values, string ID_name) where T : Accessored<T>
        {
            int[] new_ids = new int[values.Count];
            
            using (var connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();


                var fieldNames = Accessored<T>.fieldNames().Where(fn => ID_name != fn);
                var header_template = string.Join(", ", fieldNames);
                var values_template = string.Join(", ", fieldNames.Select(h => $":{h}{{0}}")); // {{0}} - row indicator
                var enumerated_values_template = Enumerable.Range(0, values.Count()).Select(i => $"({string.Format(values_template, i)})");
                var values_pattern = string.Join(", ", enumerated_values_template);

                var sql = $@"   WITH idxs AS (INSERT INTO {tableName}({header_template}) VALUES {values_pattern} RETURNING { ID_name })
                                SELECT array_agg({ ID_name }) FROM idxs";

                //var sql = $"INSERT INTO {tableName}({header_template}) VALUES {values_pattern} RETURNING { ID_name }";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    for (int i = 0; i < values.Count(); i++)
                    {
                        foreach (var fn in fieldNames)
                        {
                            command.Parameters.AddWithValue($"{fn}{i}", values[i][fn] ?? DBNull.Value);
                        }
                    }

                    new_ids = await command.ExecuteScalarAsync() as int[];
                }

                await connection.CloseAsync();
            }

            return new_ids;
        }

        public static T ConvertToObject<T>(this NpgsqlDataReader rd) where T : Accessored<T>, new()
        {
            var fieldNames = Accessored<T>.fieldNames();
            T t = new T();

            for (int i = 0; i < rd.FieldCount; i++)
            {
                string fieldName = rd.GetName(i);

                if (fieldNames.Any(fn => string.Equals(fn, fieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (rd.IsDBNull(i)) t[fieldName] = null;
                    else t[fieldName] = rd.GetValue(i);
                }
            }

            return t;
        }
    }
}
