using UnityEngine;
using UnityEngine.Events;

public class GhostTrigger : MonoBehaviour
{
    public UnityEvent onTouched;  // Unity���� Inspector���� ���� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("���� ���ɿ� ��Ҵ�!");
            onTouched.Invoke();
        }
    }
}
