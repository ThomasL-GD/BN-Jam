using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Tiles {
    public enum Tile {
        Empty = 0,
        Bloc = 1,
    }
    
    [CreateAssetMenu(fileName = "TilesContent", menuName = "TilesContent")]
    public class SOTilesContent : ScriptableObject {

         public List<Vector2Int> walls;
    }
}
