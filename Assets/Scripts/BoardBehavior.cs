using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardBehavior : MonoBehaviour {

    [SerializeField] [Range(2,20)] private int m_numberOfXTiles = 2;
    [SerializeField] [Range(2,20)] private int m_numberOfYTiles = 2;

    [SerializeField] private Vector2 m_dimensions;
    private Vector3 m_localOriginPosition;

    private float m_sizeOfATile;

    private void OnValidate() {
        CalculateLengths();
    }

    private void Start() {
        CalculateLengths(true);
    }

    private void CalculateLengths(bool p_mustDisplayErrors = false) {
        float sizeX = m_dimensions.x / (float)m_numberOfXTiles;
        float sizeY = m_dimensions.y / (float)m_numberOfYTiles;
#if UNITY_EDITOR
        if (p_mustDisplayErrors && Math.Abs(sizeY - sizeX) > 0.01f) {
            Debug.LogError($"Error with tile size calculation   x : {sizeX}    y : {sizeY}", this);
        }
#endif
        m_sizeOfATile = sizeX;

        m_localOriginPosition = transform.position - new Vector3(m_dimensions.x/2f, 0f, m_dimensions.y/2f);
    }


    private void OnDrawGizmosSelected() {
        
        DrawGizmoCross(m_localOriginPosition, Color.magenta, 0.5f);
        
        Transform transform1 = transform;
        Vector3 tileDir = (transform1.right * m_sizeOfATile * 0.5f + transform1.forward * m_sizeOfATile * 0.5f);
        DrawGizmoCross(PositionFromCoordinates(0, 0), Color.blue, 0.2f);
        DrawGizmoCross(PositionFromCoordinates(0, 0) + tileDir, Color.cyan, 0.2f);
        
        Gizmos.color = Color.grey;
        for (int i = 1; i < m_numberOfXTiles; i++) {
            Gizmos.DrawLine(PositionFromCoordinates(i, 0) - tileDir, PositionFromCoordinates(i, m_numberOfYTiles) - tileDir);
        }
        
        for (int i = 1; i < m_numberOfYTiles; i++) {
            Gizmos.DrawLine(PositionFromCoordinates(0, i) - tileDir, PositionFromCoordinates(m_numberOfXTiles, i) - tileDir);
        }
    }

    
    private void DrawGizmoCross(Vector3 p_pos, float p_radius = 1f) => DrawGizmoCross(p_pos, p_radius, Gizmos.color);
    private void DrawGizmoCross(Vector3 p_pos, Color p_color, float p_radius = 1f) => DrawGizmoCross(p_pos, p_radius, p_color);
    private void DrawGizmoCross(Vector3 p_pos, float p_radius, Color p_color) {
        Gizmos.color = p_color;
        Gizmos.DrawLine(p_pos + Vector3.down * p_radius, p_pos + Vector3.up * p_radius);
        Gizmos.DrawLine(p_pos + Vector3.forward * p_radius, p_pos + Vector3.back * p_radius);
        Gizmos.DrawLine(p_pos + Vector3.left * p_radius, p_pos + Vector3.right * p_radius);
        
    }

    public Vector3 PositionFromCoordinates(int p_x, int p_y) {
        Transform transform1 = transform;
        return m_localOriginPosition + (transform1.right * (m_sizeOfATile * (p_x + 0.5f))) + (transform1.forward * (m_sizeOfATile * (p_y + 0.5f)));
    }
}
