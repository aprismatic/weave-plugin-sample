using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Weave.Commons;
using Weave.Commons.Models;

namespace WeaveCustomPlugin
{
    public class DatasetStore : IDatasetStore
    {
        private string connString;

        public DatasetStore()
        {
            Console.WriteLine("Custom plugin created.");
        }

        public bool LoadConfig(IConfiguration configuration)
        {
            connString = configuration["CUSTOM_CONNSTRING"];
            Console.WriteLine($"Custom plugin loaded with connection string '{connString}'.");
            return true;
        }

        public bool DatasetReady(Dataset dataset)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                try
                {
                    // Create Table
                    // Creates VARBINARY column for tokenized values, and VARCHAR for plaintext
                    var sb = new StringBuilder($"CREATE TABLE {dataset.DatasetName} (");
                    sb.Append(String.Join(", ", dataset.Columns.Select(x
                        => x.ColumnName + (x.Tokenize ? " VARBINARY(100)" : " VARCHAR(100)")).ToArray()));
                    sb.Append(")");

                    using (var command = new SqlCommand(sb.ToString(), conn))
                        command.ExecuteNonQuery();

                    // Insert Data
                    // Inserts tokenized values as byte array, and plaintext values as string
                    sb = new StringBuilder($"INSERT INTO {dataset.DatasetName} VALUES ");

                    for (var i = 0; i < dataset.Rows.Count; i++)
                    {
                        sb.Append("(");

                        for (var j = 0; j < dataset.Columns.Count; j++)
                        {
                            if (dataset.Columns[j].Tokenize)
                                sb.Append(ByteArrayToString(Convert.FromBase64String(dataset.Rows[i][j].ToString())));
                            else
                                sb.Append($"'" + dataset.Rows[i][j].ToString() + "'");

                            if (j != dataset.Columns.Count - 1)
                                sb.Append(", ");
                        }

                        sb.Append(")");
                        if (i != dataset.Rows.Count - 1)
                            sb.Append(", ");
                    }

                    using (var command = new SqlCommand(sb.ToString(), conn))
                        command.ExecuteNonQuery();

                    Console.WriteLine($"Dataset '{dataset.DatasetName}' stored.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return true;
        }

        public bool UpdateDatasets(byte[] updateKey, byte[] publicKey)
        {
            // Select all datasets
            // Apply the following formula to tokens to update: newToken = (oldToken^updateKey) mod publicKey
            // For example:
            // var newToken = BigInteger.ModPow(new BigInteger(oldValue), new BigInteger(updateKey), new BigInteger(publicKey));
            // Or, if using Prisma/DB databases, you can use included UDF:
            // UPDATE table SET column = BigIntModPow(column, ByteArrayToString(updateKey), ByteArrayToString(publicKey));
            Console.WriteLine($"Update with '{Convert.ToBase64String(updateKey)}' update key and '{Convert.ToBase64String(publicKey)}' public key.");
            return true;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            hex.Append("0x");
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
