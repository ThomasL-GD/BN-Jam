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
    
    [Space]

    [SerializeField] private int m_materialIDToChange = 1;
    [SerializeField] private Material m_openMaterial;
    [SerializeField] private MeshRenderer m_meshRenderer = null;

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
        if(isUnlocked) return;
        if (m_rightTilesContent == null) {
            if (p_numberofpressedbuttons >= m_leftTilesContent.buttons.Count) Unlock();
        }
        else {
            Debug.Log($"There are {p_numberofpressedbuttons} buttons pressed so far", this);
            if(p_numberofpressedbuttons >= (m_leftTilesContent.buttons.Count + m_rightTilesContent.buttons.Count)) Unlock();
        }
    }

    private void Unlock() {
        StartCoroutine(GoDownAndOpen());
        isUnlocked = true;
        Material[] mats = m_meshRenderer.materials;
        mats[m_materialIDToChange] = m_openMaterial;
        m_meshRenderer.materials = mats;
    }

    IEnumerator GoDownAndOpen() {
        float elapsedTime = 0f;
        Vector3 initialPos = transform.position;
        Vector3 initialRot = m_chestLid.localRotation.eulerAngles;
        Vector3 lidForward = Vector3.forward;
        
        while (elapsedTime < m_animationTime) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / m_animationTime;
            float coeff = ratio * ratio;

            transform.position = initialPos + Vector3.down * m_height * coeff;
            m_chestLid.localRotation = Quaternion.Euler(initialRot + lidForward * m_lidMaxAngle * coeff);
        }

        transform.position = initialPos + Vector3.down * m_height;
        m_chestLid.localRotation = Quaternion.Euler(initialRot + lidForward * m_lidMaxAngle);
    }

    private void OnDestroy() {
        ButtonOnGroundBehaviour.OnButtonPressed -= CheckForWin;
        isUnlocked = false;
    }
}
