using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardBehavior : MonoBehaviour {

    [SerializeField,HideInInspector] private int m_numberOfXTiles = 2;
    public int numberOfXTiles => m_numberOfXTiles;
    [SerializeField,HideInInspector] private int m_numberOfYTiles = 2;
    public int numberOfYTiles => m_numberOfYTiles;

    [SerializeField,HideInInspector] private Vector2 m_dimensions;
    private Vector3 m_localOriginPosition;

    private float m_sizeOfATile;

    [SerializeField, HideInInspector] private SOTilesContent m_tilesContent;
    private Tile[,] m_tiles;
    
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private Vector2Int m_selectedTile;
    
    [SerializeField, HideInInspector] private GameObject m_prefabBloc;
    [SerializeField, HideInInspector] private GameObject m_prefabButton;
    [SerializeField, HideInInspector] private GameObject m_prefabChest;
#endif
    private void OnValidate() {
        UpdateTiles();
        
        CalculateLengths(false);
    }

    private void Start() {
        UpdateTiles();
        
        CalculateLengths(true);
        
#if UNITY_EDITOR
        if (m_tiles.GetLength(0) != m_numberOfXTiles) Debug.LogError($"Error with number of X tiles in the SO", this);
        if (m_tiles.GetLength(1) != m_numberOfYTiles) Debug.LogError($"Error with number of X tiles in the SO", this);
#endif
    }

    private void CalculateLengths(bool p_mustDisplayErrors = true) {
        float sizeX = m_dimensions.x / (float)m_numberOfXTiles;
        float sizeY = m_dimensions.y / (float)m_numberOfYTiles;
#if UNITY_EDITOR
        if (p_mustDisplayErrors && Math.Abs(sizeY - sizeX) > 0.01f) Debug.LogError($"Error with tile size calculation   x : {sizeX}    y : {sizeY}", this);
#endif
        m_sizeOfATile = sizeX;

        m_localOriginPosition = transform.position - new Vector3(m_dimensions.x/2f, 0f, m_dimensions.y/2f);
    }

    private void UpdateTiles() {
        m_tiles = new Tile[m_numberOfXTiles, m_numberOfYTiles];

        for (int i = 0; i < m_tiles.GetLength(0); i++) { for (int j = 0; j < m_tiles.GetLength(1); j++){m_tiles[i,j] = Tile.Empty;} } //Reset

        foreach (Vector2Int coordinates in m_tilesContent.walls) {
            m_tiles[coordinates.x, coordinates.y] = Tile.Bloc;
        }
        foreach (Vector2Int coordinates in m_tilesContent.buttons) {
            m_tiles[coordinates.x, coordinates.y] = Tile.Button;
        }
        foreach (Vector2Int coordinates in m_tilesContent.chests) {
            m_tiles[coordinates.x, coordinates.y] = Tile.Chest;
        }
    }

    public Vector3 PositionFromCoordinates(int p_x, int p_y) {
        Transform transform1 = transform;
        return m_localOriginPosition + (transform1.right * (m_sizeOfATile * (p_x + 0.5f))) + (transform1.forward * (m_sizeOfATile * (p_y + 0.5f)));
    }

    public Tile WhatIsOnThisTile(int p_x, int p_y) {
        return m_tiles[p_x, p_y];
    }

    public void SetTileWall(Vector2Int p_coord) {
        m_tilesContent.walls.Add(p_coord);
        UpdateTiles();
    }

    public void SetTileButton(Vector2Int p_coord, ButtonOnGroundBehaviour p_script) {
        p_script.coordinates = p_coord;
        p_script.m_board = this;
        m_tilesContent.buttons.Add(p_coord);
        UpdateTiles();
    }

    public void SetTileChest(Vector2Int p_coord, ChestBehavior p_script) {
        p_script.coordinates = p_coord;
        p_script.m_board = this;
        m_tilesContent.chests.Add(p_coord);
        UpdateTiles();
    }
    
    public void SetTileClear(Vector2Int p_coord){
        for (int i = 0; i < m_tilesContent.walls.Count; i++) {
            if(m_tilesContent.walls[i] == p_coord) m_tilesContent.walls.RemoveAt(i);
        }
        for (int i = 0; i < m_tilesContent.buttons.Count; i++) {
            if(m_tilesContent.buttons[i] == p_coord) m_tilesContent.buttons.RemoveAt(i);
        }
        UpdateTiles();
    }
    
    public void Win() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }


    private void OnDrawGizmos() {
        
        DrawGizmoCross(m_localOriginPosition, Color.magenta, 0.5f);
        
        Transform transform1 = transform;
        Vector3 tileDir = (transform1.right * m_sizeOfATile * 0.5f + transform1.forward * m_sizeOfATile * 0.5f);
        
        Gizmos.color = Color.grey;
        for (int i = 0; i < m_numberOfXTiles+1; i++) {
            Gizmos.DrawLine(PositionFromCoordinates(i, 0) - tileDir, PositionFromCoordinates(i, m_numberOfYTiles) - tileDir);
        }
        
        for (int i = 0; i < m_numberOfYTiles+1; i++) {
            Gizmos.DrawLine(PositionFromCoordinates(0, i) - tileDir, PositionFromCoordinates(m_numberOfXTiles, i) - tileDir);
        }
        
        if(m_tiles != null) {
            for (int i = 0; i < m_tiles.GetLength(0); i++) {
                for (int j = 0; j < m_tiles.GetLength(1); j++) {
                    if (m_tiles[i, j] == Tile.Bloc) DrawGizmoTile(new Vector2Int(x: i, y: j), 1f, new Color(0.9f, 0.4f, 0.2f));
                    if (m_tiles[i, j] == Tile.Button) DrawGizmoTile(new Vector2Int(x: i, y: j), 0.2f, Color.green);
                    if (m_tiles[i, j] == Tile.Chest) DrawGizmoTile(new Vector2Int(x: i, y: j), 1.2f, Color.yellow);
                }
            }
        }
        
        DrawGizmoTile(m_selectedTile, Color.red);
    }


    private void DrawGizmoCross(Vector3 p_pos, float p_radius = 1f) => DrawGizmoCross(p_pos, p_radius, Gizmos.color);
    private void DrawGizmoCross(Vector3 p_pos, Color p_color, float p_radius = 1f) => DrawGizmoCross(p_pos, p_radius, p_color);
    private void DrawGizmoCross(Vector3 p_pos, float p_radius, Color p_color) {
        Gizmos.color = p_color;
        Gizmos.DrawLine(p_pos + Vector3.down * p_radius, p_pos + Vector3.up * p_radius);
        Gizmos.DrawLine(p_pos + Vector3.forward * p_radius, p_pos + Vector3.back * p_radius);
        Gizmos.DrawLine(p_pos + Vector3.left * p_radius, p_pos + Vector3.right * p_radius);
        
    }

    
    private void DrawGizmoTile(Vector2Int p_coordinates, float p_yOffset = 0.1f) => DrawGizmoTile(p_coordinates, p_yOffset, Gizmos.color);
    private void DrawGizmoTile(Vector2Int p_coordinates, Color p_color, float p_yOffset = 0.1f) => DrawGizmoTile(p_coordinates, p_yOffset, p_color);
    private void DrawGizmoTile(Vector2Int p_coordinates, float p_yOffset, Color p_color) {
        Gizmos.color = p_color;
        
        Transform transform1 = transform;
        Vector3 right = transform1.right;
        Vector3 forward = transform1.forward;
        
        Vector3 centerOfTile = PositionFromCoordinates(p_coordinates.x, p_coordinates.y);
        Vector3 yOffset = Vector3.up * p_yOffset;

        Vector3 topRight = centerOfTile + right * m_sizeOfATile * 0.5f + forward * m_sizeOfATile * 0.5f + yOffset;
        Vector3 topLeft = centerOfTile - right * m_sizeOfATile * 0.5f + forward * m_sizeOfATile * 0.5f + yOffset;
        Vector3 bottomRight = centerOfTile + right * m_sizeOfATile * 0.5f - forward * m_sizeOfATile * 0.5f + yOffset;
        Vector3 bottomLeft = centerOfTile - right * m_sizeOfATile * 0.5f - forward * m_sizeOfATile * 0.5f + yOffset;
        
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
    }
}
