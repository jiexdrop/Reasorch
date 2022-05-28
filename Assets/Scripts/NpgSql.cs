using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Npgsql;

public class NpgSql : MonoBehaviour
{
    TextMeshProUGUI dbCon;
    // Start is called before the first frame update
    void Start()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder();
        connStringBuilder.Host = "free-tier13.aws-eu-central-1.cockroachlabs.cloud";
        connStringBuilder.Port = 26257;
        connStringBuilder.SslMode = SslMode.Require;
        connStringBuilder.Username = "jorge";
        connStringBuilder.Password = "ur6sWgO7G4hpcusvtqHmSg";
        connStringBuilder.Database = "thorn-python-1626.defaultdb";
        connStringBuilder.RootCertificate = "~/.postgres/root.crt";
        connStringBuilder.TrustServerCertificate = true;
        Simple(connStringBuilder.ConnectionString);
    }

    static void Simple(string connString)
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            // Create the "accounts" table.
            new NpgsqlCommand("CREATE TABLE IF NOT EXISTS accounts (id INT PRIMARY KEY, balance INT)", conn).ExecuteNonQuery();

            // Insert two rows into the "accounts" table.
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPSERT INTO accounts(id, balance) VALUES(@id1, @val1), (@id2, @val2)";
                cmd.Parameters.AddWithValue("id1", 1);
                cmd.Parameters.AddWithValue("val1", 1000);
                cmd.Parameters.AddWithValue("id2", 2);
                cmd.Parameters.AddWithValue("val2", 250);
                cmd.ExecuteNonQuery();
            }

            // Print out the balances.
            System.Console.WriteLine("Initial balances:");
            using (var cmd = new NpgsqlCommand("SELECT id, balance FROM accounts", conn))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    Debug.Log($"\taccount {reader.GetValue(0)}: {reader.GetValue(1)}\n");
        }
    }


}
