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

    int playerX = 0;
    int playerY = 0;

    void Start()
    {
        playerId = PlayerPrefs.GetString("player_id");

        UpdatePlayers();
    }

    public void UpdatePlayers()
    {
        // Also instantiates
        UpdatePlayerPositions();

        // Print out the players.
        System.Console.WriteLine("Players:");

        using (var cmd = new NpgsqlCommand("SELECT id, name, health, money, color FROM players", npgSql.conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string updatePlayerId = reader.GetValue(0).ToString();
                    //Debug.Log($"{reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)} : {reader.GetValue(3)}");

                    if (players.ContainsKey(updatePlayerId))
                    {
                        players[updatePlayerId].SetColor(reader.GetValue(4).ToString());
                    }

                }
            }
        }


    }

    void UpdatePlayerPositions()
    {
        System.Console.WriteLine("Positions:");

        using (var cmd = new NpgsqlCommand("SELECT player_id, x, y FROM positions", npgSql.conn))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string updatePlayerId = reader.GetValue(0).ToString();
                    //Debug.Log($"{reader.GetValue(0)}: {reader.GetValue(1)} : {reader.GetValue(2)}");
                    if (players.ContainsKey(updatePlayerId))
                    {
                        players[updatePlayerId].SetPosition(reader.GetInt32(1), reader.GetInt32(2));
                    }
                    else
                    {
                        // Instantiate a player with updatePlayerId
                        GameObject playerGO = Instantiate(playerPrefab, new Vector3(reader.GetInt32(1), reader.GetInt32(2)), Quaternion.identity);
                        Player player = playerGO.GetComponent<Player>();
                        players[updatePlayerId] = player;

                        // Set initial values if we are playing with the selected playerId
                        if (playerId == updatePlayerId)
                        {
                            playerX = reader.GetInt32(1);
                            playerY = reader.GetInt32(2);

                            // Update Camera position
                            UpdateCameraPostion(playerX, playerY);
                        }
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
            UpdatePlayers();
        }
    }

    public void Move(int x, int y)
    {
        // Insert two rows into the "accounts" table.
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = npgSql.conn;
            cmd.CommandText = $"UPDATE positions SET x = @x, y = @y WHERE player_id = @player_id";
            cmd.Parameters.AddWithValue("player_id", long.Parse(playerId));
            cmd.Parameters.AddWithValue("x", x);
            cmd.Parameters.AddWithValue("y", y);
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateCameraPostion(int x, int y)
    {
        float cameraZ = Camera.main.transform.position.z;
        Camera.main.transform.position = new Vector3(x, y, cameraZ);
    }

    public void MoveUp()
    {
        playerY++;
        Move(playerX, playerY);
        UpdateCameraPostion(playerX, playerY);
    }

    public void MoveDown()
    {
        playerY--;
        Move(playerX, playerY);
        UpdateCameraPostion(playerX, playerY);
    }

    public void MoveLeft()
    {
        playerX--;
        Move(playerX, playerY);
        UpdateCameraPostion(playerX, playerY);
    }


    public void MoveRight()
    {
        playerX++;
        Move(playerX, playerY);
        UpdateCameraPostion(playerX, playerY);
    }
}
