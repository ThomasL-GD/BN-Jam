using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour {

    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;

    public delegate void ChestUnlockDelegator(BoardBehavior p_board);

    public static ChestUnlockDelegator OnChestUnlock;

    private void Start() {
        OnChestUnlock += Unlock;
    }

    private void Unlock(BoardBehavior p_board) {
#if UNITY_EDITOR
        if(p_board != m_board) Debug.LogError("Chest Unlock Board Error", this);
#endif
        
    }

    private void OnDestroy() {
        OnChestUnlock -= Unlock;
    }
}
