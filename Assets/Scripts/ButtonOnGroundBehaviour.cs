
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class ButtonOnGroundBehaviour : MonoBehaviour {

    [HideInInspector] public bool isPressed = false;
    [HideInInspector] public Vector2Int coordinates;
    [HideInInspector] public BoardBehavior m_board;

    [SerializeField] private int[] m_materialIDToChange = new int[1] { 0 };
    [SerializeField] private Material m_pressedMaterial;
    private Material m_defaultMaterial;
    [SerializeField] private MeshRenderer m_meshRenderer = null;

    public delegate void StepButtonDelegator(Vector2Int p_coordinates);

    public static StepButtonDelegator ISteppedOnAButton;
    public static StepButtonDelegator ISteppedOutOfAButton;

    private void Start() {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_defaultMaterial = m_meshRenderer.materials[m_materialIDToChange[0]];
        ISteppedOnAButton += AmIPressedOn;
        ISteppedOutOfAButton += AmILetGo;
    }

    private void AmIPressedOn(Vector2Int p_coord) {
        if(CheckForCoordinates(p_coord)) PressOnMe();
    }

    private void AmILetGo(Vector2Int p_coord) {
        if(CheckForCoordinates(p_coord)) LetGoOfMe();
    }

    private void PressOnMe() {
        isPressed = true;
        foreach (int i in m_materialIDToChange) {
            m_meshRenderer.material = m_pressedMaterial;
        }
        m_board.AddPressedButton();
    }

    private void LetGoOfMe() {
        isPressed = false;
        foreach (int i in m_materialIDToChange) {
            m_meshRenderer.material = m_defaultMaterial;
        }
        m_board.RemovePressedButton();
    }

    private bool CheckForCoordinates(Vector2Int p_potentialCoordinates) => p_potentialCoordinates == coordinates;
}