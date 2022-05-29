using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqlPositions : MonoBehaviour
{
    public string playerId;

    public NpgSql npgSql;

    void Start()
    {
        playerId = PlayerPrefs.GetString("player_id");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
