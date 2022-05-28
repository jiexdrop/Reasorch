using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Npgsql;

public class NpgSql : MonoBehaviour
{
    public TextMeshProUGUI dbCon;

    private NpgsqlConnectionStringBuilder connStringBuilder;

    private NpgsqlConnection conn;

    void Start()
    {
        connStringBuilder = new NpgsqlConnectionStringBuilder();
        connStringBuilder.Host = "free-tier13.aws-eu-central-1.cockroachlabs.cloud";
        connStringBuilder.Port = 26257;
        connStringBuilder.SslMode = SslMode.Require;
        connStringBuilder.Username = "anon";
        connStringBuilder.Password = "t9tmPEKdqTjLpsBCSL8qeg";
        connStringBuilder.Database = "thorn-python-1626.defaultdb";
        connStringBuilder.RootCertificate = "~/.postgres/root.crt";
        connStringBuilder.TrustServerCertificate = true;
        //Simple(connStringBuilder.ConnectionString);
        conn = new NpgsqlConnection(connStringBuilder.ConnectionString);
        conn.Open();

    }


    private float nextActionTime = 0.0f;
    public float period = 1.0f;

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            // execute block of code here
            // Update every second 
            ReloadData(dbCon);
        }
    }

    public void Simple(string connString)
    {

        // Create the "accounts" table.
        new NpgsqlCommand("CREATE TABLE IF NOT EXISTS accounts (id INT PRIMARY KEY, name STRING, score INT)", conn).ExecuteNonQuery();

        // Insert two rows into the "accounts" table.
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "UPSERT INTO accounts(id, name, score) VALUES(@id1, @name1, @val1), (@id2, @name2, @val2)";
            cmd.Parameters.AddWithValue("id1", 1);
            cmd.Parameters.AddWithValue("name1", "CAT");
            cmd.Parameters.AddWithValue("val1", 0);
            cmd.Parameters.AddWithValue("id2", 2);
            cmd.Parameters.AddWithValue("name2", "DOGE");
            cmd.Parameters.AddWithValue("val2", 0);
            cmd.ExecuteNonQuery();
        }

    }

    public void ReloadData(TextMeshProUGUI text)
    {
        // Print out the balances.
        System.Console.WriteLine("Initial balances:");
        string allAccounts = "";
        using (var cmd = new NpgsqlCommand("SELECT name, score FROM accounts", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    allAccounts += $"{reader.GetValue(0)}: {reader.GetValue(1)}\n";
                }
            }
        }
        text.text = allAccounts;
    }


    public void Increment(int id)
    {


        // Insert two rows into the "accounts" table.
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE accounts SET score = @score WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("score", GetScore(id) + 1);
            cmd.ExecuteNonQuery();
        }

        ReloadData(dbCon);

    }

    public int GetScore(int id)
    {
        using (var cmd = new NpgsqlCommand($"SELECT * FROM accounts WHERE id = {id} LIMIT 1", conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Debug.Log($"\taccount {reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}\n");
                return reader.GetInt32(2);
            }
        }

        return 0;
    }

}
