using System;
using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    [SerializeField] private KeyCode m_upKey;
    [SerializeField] private KeyCode m_leftKey;
    [SerializeField] private KeyCode m_downKey;
    [SerializeField] private KeyCode m_rightKey;

    [SerializeField] [Range(0.01f, 1f)] private float m_movementDuration;
    
    [SerializeField] private BoardBehavior m_board;

    [SerializeField] private Vector2Int m_startPos;

    private bool m_isMoving = false;

    private int m_posX;
    private int m_posY;

    private void Start() {
        transform.position = m_board.PositionFromCoordinates(m_startPos.x, m_startPos.y);
    }

    // Update is called once per frame
    void Update() {
        
        
        if(m_isMoving) return;

        if (Input.GetKeyDown(m_upKey)) StartCoroutine(MoveTo(m_posX, m_posY - 1));
        if (Input.GetKeyDown(m_leftKey)) StartCoroutine(MoveTo(m_posX - 1, m_posY));
        if (Input.GetKeyDown(m_downKey)) StartCoroutine(MoveTo(m_posX, m_posY + 1));
        if (Input.GetKeyDown(m_rightKey)) StartCoroutine(MoveTo(m_posX + 1, m_posY));
    }

    IEnumerator MoveTo(int p_xTarget, int p_yTarget) {
        m_isMoving = true;

        Vector3 originalPos = m_board.PositionFromCoordinates(m_posX, m_posY);
        Vector3 targetPos = m_board.PositionFromCoordinates(p_xTarget, p_yTarget);
        Vector3 direction = targetPos - originalPos;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < m_movementDuration) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_movementDuration;

            Vector3 newPos = originalPos + direction * ratio;
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        }

        m_posX = p_xTarget;
        m_posY = p_yTarget;
        transform.position = targetPos;
        m_isMoving = false;
    }
}
