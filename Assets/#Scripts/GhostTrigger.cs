using UnityEngine;
using UnityEngine.Events;

public class GhostTrigger : MonoBehaviour
{
    public UnityEvent onTouched;  // Unity에서 Inspector에서 연결 가능

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("손이 유령에 닿았다!");
            onTouched.Invoke();
        }
    }
}
