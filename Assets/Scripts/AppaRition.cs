using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppaRition : MonoBehaviour {

    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private Vector2Int[] m_triggerTiles;
    [SerializeField] [Range(0.1f, 5f)] private float m_appearTime;
    [SerializeField] private Vector3 m_startPos;
    [SerializeField] private AnimationCurve m_speed;
    private Vector3 m_targetPos;
    private bool m_hasBeenTriggered = false;

    private void Start() {
        var transform1 = transform;
        m_targetPos = transform1.position;
        transform1.position = m_startPos;

        CharacterController.OnReset += Disappear;
    }

    private void Disappear(bool p_isBigReset) {
        if(p_isBigReset)Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() {
        if (m_hasBeenTriggered) return;
        foreach (Vector2Int coo in m_triggerTiles) {
            if (m_characterController.pos == coo) {
                m_hasBeenTriggered = true;
                StartCoroutine(Appear());
                return;
            }
        }
    }

    IEnumerator Appear() {
        float elapsedTime = 0f;

        while (elapsedTime < m_appearTime) {
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
            float ratio = elapsedTime / m_appearTime;

            transform.position = m_startPos + ((m_targetPos - m_startPos) * (m_speed.Evaluate(ratio)));
        }

        transform.position = m_targetPos;
    }

    private void OnDestroy() {
        CharacterController.OnReset -= Disappear;
    }
}
