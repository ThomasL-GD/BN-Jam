using System;
using System.Collections;
using Tiles;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    [SerializeField] private KeyCode m_upKey;
    [SerializeField] private KeyCode m_leftKey;
    [SerializeField] private KeyCode m_downKey;
    [SerializeField] private KeyCode m_rightKey;

    [SerializeField] [Range(0.01f, 1f)] private float m_movementDuration = 0.2f;
    [SerializeField] [Range(0.01f, 0.5f)] private float m_bonkDuration = 0.1f;
    
    [SerializeField] private BoardBehavior m_board;

    [SerializeField] private Vector2Int m_startPos;

    private bool m_isMoving = false;

    private int m_posX;
    private int m_posY;
    private bool m_isSteppingOnButton;

    private void Start() {
        Vector3 newPos = m_board.PositionFromCoordinates(m_startPos.x, m_startPos.y);
        Transform transform1 = transform;
        transform1.position = new Vector3(newPos.x, transform1.position.y, newPos.z);
    }

    // Update is called once per frame
    void Update() {
        
        
        if(m_isMoving) return; //Return

        bool keyPressed = false;
        Vector2Int targetPos = new Vector2Int(m_posX, m_posY);

        if (Input.GetKey(m_upKey)) {
            targetPos = new Vector2Int(m_posX, m_posY - 1);
            keyPressed = true;
        }
        else if (Input.GetKey(m_leftKey)) {
            targetPos = new Vector2Int(m_posX + 1, m_posY);
            keyPressed = true;
        }
        else if (Input.GetKey(m_downKey)) {
            targetPos = new Vector2Int(m_posX, m_posY + 1);
            keyPressed = true;
        }
        else if (Input.GetKey(m_rightKey)) {
            targetPos = new Vector2Int(m_posX - 1, m_posY);
            keyPressed = true;
        }

        if (!keyPressed) return; //Return
        
        if(targetPos.x < 0 || targetPos.y < 0 || targetPos.x > m_board.numberOfXTiles -1 || targetPos.y > m_board.numberOfYTiles -1) StartCoroutine(Bonk(targetPos.x, targetPos.y));
        else {
            Tile targetTile = m_board.WhatIsOnThisTile(targetPos.x, targetPos.y);
            if (targetTile == Tile.Bloc) StartCoroutine(Bonk(targetPos.x, targetPos.y));
            else { //Movement is validated
                
                StartCoroutine(MoveTo(targetPos.x, targetPos.y));
                
                if (targetTile == Tile.Button) {
                    m_board.StepOnButton(new Vector2Int(targetPos.x, targetPos.y));
                    m_isSteppingOnButton = true;
                }
            }
        }
    }

    IEnumerator MoveTo(int p_xTarget, int p_yTarget) {
        m_isMoving = true;
        
        if(m_isSteppingOnButton) m_board.StepOutOfButton(new Vector2Int(m_posX, m_posY));

        Vector3 position = transform.position;
        Vector3 originalPosGrid = m_board.PositionFromCoordinates(m_posX, m_posY);
        Vector3 originalPos = new Vector3(originalPosGrid.x, position.y, originalPosGrid.z);
        Vector3 targetPosGrid = m_board.PositionFromCoordinates(p_xTarget, p_yTarget);
        Vector3 targetPos = new Vector3(targetPosGrid.x, position.y, targetPosGrid.z);
        Vector3 direction = targetPos - originalPos;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < m_movementDuration) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_movementDuration;

            Vector3 newPos = originalPos + direction * ratio;
            Transform transform1 = transform;
            transform1.position = new Vector3(newPos.x, transform1.position.y, newPos.z);
        }
        
        m_isSteppingOnButton = false;

        m_posX = p_xTarget;
        m_posY = p_yTarget;
        transform.position = targetPos;
        m_isMoving = false;
    }

    IEnumerator Bonk(int p_xTarget, int p_yTarget) {
        
        m_isMoving = true;

        Vector3 originalGridPos = m_board.PositionFromCoordinates(m_posX, m_posY);
        Vector3 originalPos = new Vector3(originalGridPos.x, transform.position.y, originalGridPos.z);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < m_bonkDuration) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_bonkDuration;
            float coeff = -Mathf.Abs(ratio - 0.5f) + 0.5f; // do a /\ kind of curve

            var transform1 = transform;
            transform1.position = originalPos + Vector3.up * coeff;
        }

        transform.position = originalPos;
        m_isMoving = false;
    }
}
