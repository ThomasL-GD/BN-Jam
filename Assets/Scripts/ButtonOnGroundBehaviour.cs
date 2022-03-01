
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class ButtonOnGroundBehaviour : MonoBehaviour {

    [HideInInspector] public bool isPressed = false;
    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;

    [SerializeField] private int m_materialIDToChange = 1;
    [SerializeField] private Material m_pressedMaterial;
    private Material m_defaultMaterial;
    [SerializeField] private MeshRenderer m_meshRenderer = null;

    public delegate void StepButtonDelegator(Vector2Int p_coordinates, BoardBehavior p_board);

    public static StepButtonDelegator ISteppedOnAButton;
    public static StepButtonDelegator ISteppedOutOfAButton;

    private void Start() {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_defaultMaterial = m_meshRenderer.materials[m_materialIDToChange];
        ISteppedOnAButton += AmIPressedOn;
        ISteppedOutOfAButton += AmILetGo;
    }

    private void AmIPressedOn(Vector2Int p_coord, BoardBehavior p_board) {
        if(CheckForCoordinates(p_coord, p_board)) PressOnMe();
    }

    private void AmILetGo(Vector2Int p_coord, BoardBehavior p_board) {
        if(CheckForCoordinates(p_coord, p_board)) LetGoOfMe();
    }

    private void PressOnMe() {
        isPressed = true;
        Material[] mats = m_meshRenderer.materials;
        mats[m_materialIDToChange] = m_pressedMaterial;
        m_meshRenderer.materials = mats;
        m_board.AddPressedButton();
    }

    private void LetGoOfMe() {
        isPressed = false;
        Material[] mats = m_meshRenderer.materials;
        mats[m_materialIDToChange] = m_defaultMaterial;
        m_meshRenderer.materials = mats;
        m_board.RemovePressedButton();
    }

    private bool CheckForCoordinates(Vector2Int p_potentialCoordinates, BoardBehavior p_board) => p_board == m_board && p_potentialCoordinates == coordinates;

    private void OnDestroy() {
        ISteppedOnAButton -= AmIPressedOn;
        ISteppedOutOfAButton -= AmILetGo;
    }
}