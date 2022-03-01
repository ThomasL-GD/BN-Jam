using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Tiles {
    public enum Tile {
        Empty = 0,
        Bloc = 1,
        Button = 2,
    }
    
    [CreateAssetMenu(fileName = "TilesContent", menuName = "TilesContent")]
    public class SOTilesContent : ScriptableObject {

         public List<Vector2Int> walls;

         [Serializable]
         public class Button {
             public Vector2Int cordinates;
             public ButtonBehaviour script;
         }
         public List<Button> buttons;
    }
}
