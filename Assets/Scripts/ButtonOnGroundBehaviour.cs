
using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

[Serializable]
public class ButtonOnGroundBehaviour : MonoBehaviour {

    [HideInInspector] public bool isPressed = false;
    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;

    [SerializeField] private int m_materialIDToChange = 1;
    [SerializeField] private Material m_pressedMaterial;
    private Material m_defaultMaterial;
    private MeshRenderer m_meshRenderer = null;

    private static int s_numberOfPressedButtons = 0;

    public delegate void StepButtonDelegator(Vector2Int p_coordinates, BoardBehavior p_board);

    public static StepButtonDelegator ISteppedOnAButton;
    public static StepButtonDelegator ISteppedOutOfAButton;

    public delegate void CheckIfAllDelegator(int p_numberOfPressedButtons);

    public static CheckIfAllDelegator OnButtonPressed;

    private void Start() {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_defaultMaterial = m_meshRenderer.materials[m_materialIDToChange];
        ISteppedOnAButton += AmIPressedOn;
        ISteppedOutOfAButton += AmILetGo;
        CharacterController.OnReset += p_isBigReset => Reset();
    }

    private void AmIPressedOn(Vector2Int p_coord, BoardBehavior p_board) {
        if(CheckForCoordinates(p_coord, p_board)) PressOnMe();
    }

    private void AmILetGo(Vector2Int p_coord, BoardBehavior p_board) {
        if(CheckForCoordinates(p_coord, p_board)) LetGoOfMe();
    }

    private void Reset() {
        if(isPressed) LetGoOfMe();
    }

    private void PressOnMe() {
        isPressed = true;
        Material[] mats = m_meshRenderer.materials;
        mats[m_materialIDToChange] = m_pressedMaterial;
        m_meshRenderer.materials = mats;
        s_numberOfPressedButtons++;
        Debug.Log($"Current number of pressed buttons : {s_numberOfPressedButtons}  :)", this);
        OnButtonPressed?.Invoke(s_numberOfPressedButtons);
    }

    private void LetGoOfMe() {
        isPressed = false;
        Material[] mats = m_meshRenderer.materials;
        mats[m_materialIDToChange] = m_defaultMaterial;
        m_meshRenderer.materials = mats;
        s_numberOfPressedButtons--;
        Debug.Log($"Current number of pressed buttons : {s_numberOfPressedButtons}  :(", this);
    }

    private bool CheckForCoordinates(Vector2Int p_potentialCoordinates, BoardBehavior p_board) => p_board == m_board && p_potentialCoordinates == coordinates;

    private void OnDestroy() {
        ISteppedOnAButton -= AmIPressedOn;
        ISteppedOutOfAButton -= AmILetGo;
        s_numberOfPressedButtons = 0;
    }
}