using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Door : MonoBehaviour
{
    [SerializeField] private float m_maximumAngle;
    [SerializeField] private bool m_locked;
    [SerializeField] private AnimationCurve lockedAnimation;

    private Vector3 m_latchDirection;
    private Vector3 m_latchEulerAngles;

    private float m_elapsedTime;

    private bool m_scriptinAction;


    void Start() {
        // -- Not useful untill we make doors affected by physics.
        m_latchDirection = gameObject.transform.right;
        m_latchEulerAngles = gameObject.transform.eulerAngles;
        m_scriptinAction = false;
    }

    public void action() {
        // -- Tell the player what door to use
        GameObject.Find("Player").GetComponent<PlayerMovement>().updateDoorMode(gameObject);
    }


    public void rotateDoor(Vector3 playerForward, Vector3 mouseDir) {
        // -- Cannot rotate the door during a scripted action.
        if (m_scriptinAction) { return; }
        
        if (m_locked) {
            // -- Play door sound
            scriptedRotation(lockedAnimation, 0.5f);
        } 
        else{
            // -- Door directions.
            Vector3 doorRight = gameObject.transform.right;
            Vector3 perpendicular = gameObject.transform.forward * -1;

            // -- 1. Transform mouseDir into Player camera space
            float theta = Vector3.SignedAngle(Vector3.forward, playerForward, Vector3.up) * Mathf.Deg2Rad;
            Vector3 mouseDirTrans = new Vector3( Mathf.Cos(theta) * mouseDir.x + Mathf.Sin(theta) * mouseDir.z,
                                                 0.0f,
                                                -Mathf.Sin(theta) * mouseDir.x + Mathf.Cos(theta) * mouseDir.z);

            // -- 2. Rotate the door.
            float rho = Mathf.Acos(Vector3.Dot(perpendicular.normalized, mouseDirTrans.normalized));
            float value = 1.0f - (rho / (90.0f * Mathf.Deg2Rad));
            
            float curAngle = Vector3.Angle(m_latchDirection, doorRight);
            float newAngle = curAngle + m_latchEulerAngles.y + value;

            if (newAngle < m_latchEulerAngles.y + m_maximumAngle && newAngle > m_latchEulerAngles.y) {
                gameObject.transform.Rotate(0.0f, value, 0.0f);
            }
        }
    }

    public void setDoorLock(bool isLocked) {
        m_locked = isLocked;
    }

    // -- Change to take in a curve.
    public async void scriptedRotation(AnimationCurve curve, float duration) {
        m_scriptinAction = true;

        while (m_elapsedTime < duration){
            m_elapsedTime += Time.deltaTime;
            float t = m_elapsedTime / duration;

            gameObject.transform.Rotate(0.0f, 0.2f * curve.Evaluate(t), 0.0f);
            await UniTask.Yield();
        }
        gameObject.transform.rotation = gameObject.transform.parent.gameObject.transform.rotation;

        m_elapsedTime = 0.0f;
        m_scriptinAction = false;
    }

}
