using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour {

    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;

    [SerializeField] private SOTilesContent m_leftTilesContent = null;
    [SerializeField] private SOTilesContent m_rightTilesContent = null;
    
    [SerializeField] private Transform m_chestLid = null;
    [SerializeField] private float m_height = 2.1f;
    [SerializeField] private float m_animationTime = 2.1f;
    [SerializeField] private float m_lidMaxAngle = 60f;

    public static bool isUnlocked = false;

    private void Start() {
#if UNITY_EDITOR
        if(m_leftTilesContent == null)Debug.LogError("HEYYYY ! The chest can't work if there is no SoTilesContent serialized, at least for the left one", this);
        if(m_rightTilesContent == null)Debug.LogWarning("HEYYY ! The chest only have one SoTilesContent serialized, it might cause issues except if there's only one character in the scene", this);
#endif
        
        ButtonOnGroundBehaviour.OnButtonPressed += CheckForWin;
        isUnlocked = false;
    }

    private void CheckForWin(int p_numberofpressedbuttons) {
        if (m_rightTilesContent == null) {
            if (p_numberofpressedbuttons >= m_leftTilesContent.buttons.Count) Unlock();
        }
        else {
            if(p_numberofpressedbuttons >= (m_leftTilesContent.buttons.Count + m_rightTilesContent.buttons.Count)) Unlock();
        }
    }

    private void Unlock() {
        StartCoroutine(GoDownAndOpen());
        isUnlocked = true;
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
        ButtonOnGroundBehaviour.OnButtonPressed -= CheckForWin;
        isUnlocked = false;
    }
}
