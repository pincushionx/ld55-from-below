using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2Int Position;
    public float PathScore = 1f;

    public bool IsSummonTile = false;
    public bool IsPlayerOwned = false;
    public bool IsOpponentOwned = false;

    public float Height = 0;
}
