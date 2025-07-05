using UnityEngine;
using UnityEngine.Events;

public class GhostTrigger : MonoBehaviour
{
    public UnityEvent onTouched;  // Unity���� Inspector���� ���� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            Debug.Log("���� ���ɿ� ��Ҵ�!");
            onTouched.Invoke();
            Invoke("DestroySelf", 3f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
