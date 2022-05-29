using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Npgsql;
using UnityEngine.SceneManagement;

public class NpgSql : MonoBehaviour
{
    public TextMeshProUGUI playersTextList;

    private NpgsqlConnectionStringBuilder connStringBuilder;

    public NpgsqlConnection conn;

    void Awake()
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

    public void OnDestroy()
    {
        conn.Close();
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
            ReloadData(playersTextList);
        }
    }

    public void CreateAccount(TextMeshProUGUI name)
    {

        // Create the "players" table.
        //new NpgsqlCommand("CREATE TABLE IF NOT EXISTS players (id INT PRIMARY KEY, name STRING, score INT)", conn).ExecuteNonQuery();

        // Insert two rows into the "accounts" table.
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "UPSERT INTO players(name, health, money) VALUES(@name, @health, @money)";
            cmd.Parameters.AddWithValue("name", name.text);
            cmd.Parameters.AddWithValue("health", 100);
            cmd.Parameters.AddWithValue("money", 0);
            cmd.ExecuteNonQuery();
        }

    }

    public void Play(TextMeshProUGUI name)
    {
        long player_id = GetPlayerId(name.text);

        if(player_id == -1){
            return;
        }

        Debug.Log($"Got player id {player_id} for player {name.text}");

        PlayerPrefs.SetString("player_id", player_id.ToString());

        SceneManager.LoadScene("Game");
    }

    public long GetPlayerId(string name)
    {
        using (var cmd = new NpgsqlCommand($"SELECT * FROM players WHERE name = @name LIMIT 1", conn))
        {
            cmd.Parameters.AddWithValue("name", name);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debug.Log($"\taccount {reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}\n");
                    return reader.GetInt64(0);
                }
            }
        }

        return -1;
    }

    public void ReloadData(TextMeshProUGUI text)
    {
        // Print out the players.
        System.Console.WriteLine("Players:");
        string allAccounts = "";
        using (var cmd = new NpgsqlCommand("SELECT name, health, money FROM players", conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    allAccounts += $"{reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}\n";
                }
            }
        }
        text.text = allAccounts;
    }


    public void Move(int id, int x, int y)
    {


        // Insert two rows into the "accounts" table.
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = $"UPDATE positions SET x = @score WHERE player_id = {id}";
            cmd.Parameters.AddWithValue("x", x);
            cmd.Parameters.AddWithValue("y", y);
            cmd.ExecuteNonQuery();
        }

        ReloadData(playersTextList);

    }

    public Vector2 GetPosition(int id)
    {
        using (var cmd = new NpgsqlCommand($"SELECT * FROM positions WHERE id = {id} LIMIT 1", conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Debug.Log($"\taccount {reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}\n");
                return new Vector2(reader.GetInt32(2), reader.GetInt32(3));
            }
        }

        return Vector2.zero;
    }

}
