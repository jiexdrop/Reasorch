using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

public class SqlPositions : MonoBehaviour
{
    public string playerId;

    public NpgSql npgSql;

    public GameObject playerPrefab;

    public Dictionary<string, Player> players = new Dictionary<string, Player>();

    void Start()
    {
        playerId = PlayerPrefs.GetString("player_id");

        InstantiateAllPlayers();
    }

    public void InstantiateAllPlayers()
    {
        // Print out the players.
        System.Console.WriteLine("Players:");

        using (var cmd = new NpgsqlCommand("SELECT name, health, money FROM players", npgSql.conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debug.Log($"{reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}");
                }
            }
        }

        UpdatePlayerPositions();
    }

    void UpdatePlayerPositions()
    {
        System.Console.WriteLine("Positions:");

        using (var cmd = new NpgsqlCommand("SELECT id, x, y FROM positions", npgSql.conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string updatePlayerId = reader.GetValue(0).ToString();
                    Debug.Log($"{reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}");
                    if (players.ContainsKey(updatePlayerId))
                    {
                        players[updatePlayerId].SetPosition(reader.GetInt32(1), reader.GetInt32(2));
                    }
                    else
                    {
                        GameObject playerGO = Instantiate(playerPrefab, new Vector3(reader.GetInt32(1), reader.GetInt32(2)), Quaternion.identity);
                        Player player = playerGO.GetComponent<Player>();
                        players[updatePlayerId] = player;
                    }

                }
            }
        }
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
            UpdatePlayerPositions();
        }
    }

}
