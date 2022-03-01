using System;
using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    private enum Side {
        Left,
        Right
    }

    private enum Direction {
        Up,
        Left,
        Down,
        Right
    }

    private class Vector2IntC {
        public Vector2Int coo;
    }

    private static CharacterController rightChara; 
    private static CharacterController leftChara;
    private List<Direction> m_myPath = new List<Direction>();
    private Direction[] m_clonePath = null;
    private int m_clonePathCurrentIndex = 0;
    private bool m_haveToReset = false;

    [SerializeField] private Side m_side;
    [SerializeField] private Transform m_clone;
    private Vector2IntC m_clonePos;

    [Space, Header("Keys")]
    [SerializeField] private KeyCode m_upKey;
    [SerializeField] private KeyCode m_leftKey;
    [SerializeField] private KeyCode m_downKey;
    [SerializeField] private KeyCode m_rightKey;
    [Space]
    [SerializeField] private KeyCode m_resetKey;

    [Space, Header("Timings")]
    [SerializeField] [Range(0.01f, 1f)] private float m_movementDuration = 0.2f;
    [SerializeField] [Range(0.01f, 0.5f)] private float m_bonkDuration = 0.1f;
    
    [Space, Header("Other")]
    [SerializeField] private BoardBehavior m_board;

    [SerializeField] private Vector2Int m_startPos;

    // ReSharper disable once RedundantDefaultMemberInitializer
    private bool m_isMeMoving = false;
    private bool m_isCloneMoving = false;
    private bool m_isAvailableForMovement => !(m_isMeMoving || m_isCloneMoving);

    private Vector2IntC m_myPos;
    private bool m_isSteppingOnButton;
    private bool m_isCloneSteppingOnButton;

    private float m_yOffset;

    private void Awake() {
        switch (m_side) {
            case Side.Left:
                leftChara = this;
                break;
            case Side.Right:
                rightChara = this;
                break;
        }
    }

    private void Start() {
        m_myPos = new Vector2IntC() { coo = m_startPos };
        Vector3 newPos = m_board.PositionFromCoordinates(m_startPos.x, m_startPos.y);
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        position = new Vector3(newPos.x, position.y, newPos.z);
        transform1.position = position;
        m_yOffset = position.y;
    }

    // Update is called once per frame
    void Update() {
        
        if(Input.GetKeyDown(m_resetKey)) Restart();
        
        if(!m_isAvailableForMovement) return; //Return

        bool keyPressed = false;
        Vector2Int targetPos = m_myPos.coo;
        Direction dir = Direction.Up;

        if (Input.GetKey(m_upKey)) {
            targetPos = new Vector2Int(m_myPos.coo.x, m_myPos.coo.y - 1);
            dir = Direction.Up;
            keyPressed = true;
        }
        else if (Input.GetKey(m_leftKey)) {
            targetPos = new Vector2Int(m_myPos.coo.x + 1, m_myPos.coo.y);
            dir = Direction.Left;
            keyPressed = true;
        }
        else if (Input.GetKey(m_downKey)) {
            targetPos = new Vector2Int(m_myPos.coo.x, m_myPos.coo.y + 1);
            dir = Direction.Down;
            keyPressed = true;
        }
        else if (Input.GetKey(m_rightKey)) {
            targetPos = new Vector2Int(m_myPos.coo.x - 1, m_myPos.coo.y);
            dir = Direction.Right;
            keyPressed = true;
        }

        if (!keyPressed) return; //Return
        
        if(targetPos.x < 0 || targetPos.y < 0 || targetPos.x > m_board.numberOfXTiles -1 || targetPos.y > m_board.numberOfYTiles -1) BonkMe(targetPos.x, targetPos.y);
        else {
            Tile targetTile = m_board.WhatIsOnThisTile(targetPos.x, targetPos.y);
            if (targetTile == Tile.Bloc) BonkMe(targetPos.x, targetPos.y);
            else { //Movement is validated
                

                if (m_haveToReset) {
                    m_myPath.Clear();
                    m_haveToReset = false;
                }
                m_myPath.Add(dir); //Record movements
                
                //Move clone
                if(m_clonePath != null && m_clonePathCurrentIndex < m_clonePath.Length) {

                    Vector2Int cloneTargetPos;
                    switch (m_clonePath[m_clonePathCurrentIndex]) {
                        case Direction.Up:
                            cloneTargetPos = new Vector2Int(m_clonePos.coo.x, m_clonePos.coo.y - 1);
                            break;
                        case Direction.Left:
                            cloneTargetPos = new Vector2Int(m_clonePos.coo.x + 1, m_clonePos.coo.y);
                            break;
                        case Direction.Down:
                            cloneTargetPos = new Vector2Int(m_clonePos.coo.x, m_clonePos.coo.y + 1);
                            break;
                        case Direction.Right:
                            cloneTargetPos = new Vector2Int(m_clonePos.coo.x - 1, m_clonePos.coo.y);
                            break;
                        default:
                            cloneTargetPos = new Vector2Int(0,0);
                            Debug.LogError("No direction assigned to movement", this);
                            break;
                    }
                    if(cloneTargetPos.x < 0 || cloneTargetPos.y < 0 || cloneTargetPos.x > m_board.numberOfXTiles -1 || cloneTargetPos.y > m_board.numberOfYTiles -1) BonkClone(cloneTargetPos.x, cloneTargetPos.y);
                    else if (m_board.WhatIsOnThisTile(cloneTargetPos.x, cloneTargetPos.y) == Tile.Bloc) BonkClone(cloneTargetPos.x, cloneTargetPos.y);
                    else {
                        Tile targetTypeTile = m_board.WhatIsOnThisTile(cloneTargetPos.x, cloneTargetPos.y);
                        if (targetTypeTile == Tile.Button) {
                            m_board.StepOnButton(new Vector2Int(cloneTargetPos.x, cloneTargetPos.y));
                            m_isCloneSteppingOnButton = true;
                        }
                        
                        MoveCloneTo(m_clonePath[m_clonePathCurrentIndex]);
                    }
                    
                    m_clonePathCurrentIndex++;
                }
                
                MoveMeTo(dir); // <<<<<<<<<<<<<<<<<<<<<<<MOVE MYSELF<<<<<<<<<<<<<<<<<<<<<<<<
                
                if (targetTile == Tile.Button) {
                    m_board.StepOnButton(new Vector2Int(targetPos.x, targetPos.y));
                    m_isSteppingOnButton = true;
                }
            }
        }
    }

    private void MoveCloneTo(Direction p_direction) => StartCoroutine(MoveTo(p_direction, m_clone, m_clonePos));
    private void MoveMeTo(Direction p_direction) => StartCoroutine(MoveTo(p_direction, transform, m_myPos));
    
    private void BonkClone(int p_xTarget, int p_yTarget) { StartCoroutine(Bonk(p_xTarget, p_yTarget, m_clone, m_clonePos)); }
    private void BonkMe(int p_xTarget, int p_yTarget) => StartCoroutine(Bonk(p_xTarget, p_yTarget, transform, m_myPos));

    IEnumerator MoveTo(Direction p_direction, Transform p_transform, Vector2IntC p_currentPos) {
        if (p_transform == transform) m_isMeMoving = true;
        else if (p_transform == m_clone) m_isCloneMoving = true;
        
        Vector2Int targetPosDir;
        switch (p_direction) {
            case Direction.Up:
                targetPosDir = new Vector2Int(p_currentPos.coo.x, p_currentPos.coo.y - 1);
                break;
            case Direction.Left:
                targetPosDir = new Vector2Int(p_currentPos.coo.x + 1, p_currentPos.coo.y);
                break;
            case Direction.Down:
                targetPosDir = new Vector2Int(p_currentPos.coo.x, p_currentPos.coo.y + 1);
                break;
            case Direction.Right:
                targetPosDir = new Vector2Int(p_currentPos.coo.x - 1, p_currentPos.coo.y);
                break;
            default:
                targetPosDir = new Vector2Int(0,0);
                Debug.LogError("No direction assigned to movement", this);
                break;
        }
        
        if(m_isSteppingOnButton || p_transform == transform) m_board.StepOutOfButton(new Vector2Int(p_currentPos.coo.x, p_currentPos.coo.y));
        if(m_isCloneSteppingOnButton || p_transform == m_clone) m_board.StepOutOfButton(new Vector2Int(p_currentPos.coo.x, p_currentPos.coo.y));
        
        
        Vector3 originalPosGrid = m_board.PositionFromCoordinates(p_currentPos.coo.x, p_currentPos.coo.y);
        Vector3 originalPos = new Vector3(originalPosGrid.x, m_yOffset, originalPosGrid.z);
        Vector3 targetPosGrid = m_board.PositionFromCoordinates(targetPosDir.x, targetPosDir.y);
        Vector3 targetPos = new Vector3(targetPosGrid.x, m_yOffset, targetPosGrid.z);
        Vector3 direction = targetPos - originalPos;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < m_movementDuration) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_movementDuration;

            Vector3 newPos = originalPos + direction * ratio;
            p_transform.position = new Vector3(newPos.x, m_yOffset, newPos.z);
        }
        
        m_isSteppingOnButton = false;

        p_currentPos.coo = targetPosDir;
        p_transform.position = targetPos;
        if (p_transform == transform) m_isMeMoving = false;
        else if (p_transform == m_clone) m_isCloneMoving = false;
    }

    IEnumerator Bonk(int p_xTarget, int p_yTarget, Transform p_transform, Vector2IntC p_currentPos) {
        
        if (p_transform == transform) m_isMeMoving = true;
        else if (p_transform == m_clone) m_isCloneMoving = true;

        Vector3 originalGridPos = m_board.PositionFromCoordinates(p_currentPos.coo.x, p_currentPos.coo.y);
        Vector3 originalPos = new Vector3(originalGridPos.x, m_yOffset, originalGridPos.z);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < m_bonkDuration) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_bonkDuration;
            float coeff = -Mathf.Abs(ratio - 0.5f) + 0.5f; // do a /\ kind of curve

            var transform1 = p_transform;
            transform1.position = originalPos + Vector3.up * coeff;
        }

        p_transform.position = originalPos;
        if (p_transform == transform) m_isMeMoving = false;
        else if (p_transform == m_clone) m_isCloneMoving = false;
    }

    private void Restart() {
        CharacterController target;
        switch (m_side) {
            case Side.Left:
                target = rightChara;
                break;
            case Side.Right:
                target = leftChara;
                break;
            default:
                target = this;
                break;
        }
        m_clonePath = target.m_myPath.ToArray();
        m_clonePos = new Vector2IntC() { coo = target.m_startPos };
        m_clone.position = m_board.PositionFromCoordinates(m_clonePos.coo.x, m_clonePos.coo.y) + Vector3.up * m_yOffset;

        m_clonePathCurrentIndex = 0;

        m_myPos.coo = m_startPos;
        transform.position = m_board.PositionFromCoordinates(m_myPos.coo.x, m_myPos.coo.y) + Vector3.up * m_yOffset;

        m_haveToReset = true;
    }

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.magenta;
        Vector2Int pos = m_startPos;
        foreach (Direction pathDir in m_myPath) {
            switch (pathDir) {
                case Direction.Up:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x, pos.y-1) + Vector3.up*0.3f);
                    pos.y--;
                    break;
                case Direction.Left:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x+1, pos.y) + Vector3.up*0.3f);
                    pos.x++;
                    break;
                case Direction.Down:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x, pos.y+1) + Vector3.up*0.3f);
                    pos.y++;
                    break;
                case Direction.Right:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x-1, pos.y) + Vector3.up*0.3f);
                    pos.x--;
                    break;
            }
        }

        if (m_clonePath == null) return;
        Gizmos.color = Color.green;
        pos = m_startPos;
        foreach (Direction pathDir in m_clonePath) {
            switch (pathDir) {
                case Direction.Up:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x, pos.y-1) + Vector3.up*0.3f);
                    pos.y--;
                    break;
                case Direction.Left:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x+1, pos.y) + Vector3.up*0.3f);
                    pos.x++;
                    break;
                case Direction.Down:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x, pos.y+1) + Vector3.up*0.3f);
                    pos.y++;
                    break;
                case Direction.Right:
                    Gizmos.DrawLine(m_board.PositionFromCoordinates(pos.x, pos.y) + Vector3.up*0.3f, m_board.PositionFromCoordinates(pos.x-1, pos.y) + Vector3.up*0.3f);
                    pos.x--;
                    break;
            }
        }
    }
}
