using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // VARIABLES

    public int deathCount;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> coinsCollected;
    public string userName;


    // ALL GAMEDATA

    public GameData() 
    {

        // 
        //
        // "NEW GAME" DEFAULT VALUES

        this.deathCount = 0;
        playerPosition = Vector3.zero;
        coinsCollected = new SerializableDictionary<string, bool>();
        userName = "UserName";
    }
}
