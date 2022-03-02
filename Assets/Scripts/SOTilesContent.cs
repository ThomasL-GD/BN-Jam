using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming


public enum Tile {
    Empty = 0,
    Bloc = 1,
    Button = 2,
    Chest = 3,
}
    
[CreateAssetMenu(fileName = "TilesContent", menuName = "TilesContent")]
public class SOTilesContent : ScriptableObject {

    public List<Vector2Int> walls = new List<Vector2Int>();
    
    public List<Vector2Int> buttons = new List<Vector2Int>();
    
    public List<Vector2Int> chests = new List<Vector2Int>();
}