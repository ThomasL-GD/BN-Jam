using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour {

    [HideInInspector] public bool isPressed = false;

    public void PressOnMe() {
        isPressed = true;
    }

    public void LetGoOfMe() {
        isPressed = true;
    }
}