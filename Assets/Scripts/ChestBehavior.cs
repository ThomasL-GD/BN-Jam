using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour {

    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;
    
    [SerializeField] private Transform m_chestLid = null;
    [SerializeField] private float m_height = 2.1f;
    [SerializeField] private float m_animationTime = 2.1f;
    [SerializeField] private float m_lidMaxAngle = 60f;

    public delegate void ChestUnlockDelegator(BoardBehavior p_board);

    public static ChestUnlockDelegator OnChestUnlock;

    private void Start() {
        OnChestUnlock += Unlock;
    }

    private void Unlock(BoardBehavior p_board) {
#if UNITY_EDITOR
        if(p_board != m_board) Debug.LogError("Chest Unlock Board Error", this);
#endif
        StartCoroutine(GoDownAndOpen());
    }

    IEnumerator GoDownAndOpen() {
        float elapsedTime = 0f;
        Vector3 initialPos = transform.position;
        Vector3 initialRot = m_chestLid.rotation.eulerAngles;
        
        while (elapsedTime < m_animationTime) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / m_animationTime;
            float coeff = ratio * ratio;

            transform.position = initialPos + Vector3.down * m_height * coeff;
            m_chestLid.rotation = Quaternion.Euler(initialRot + m_chestLid.forward * m_lidMaxAngle * coeff);
        }

        transform.position = initialPos + Vector3.down * m_height;
        m_chestLid.rotation = Quaternion.Euler(initialRot + m_chestLid.forward * m_lidMaxAngle);
    }

    private void OnDestroy() {
        OnChestUnlock -= Unlock;
    }
}
