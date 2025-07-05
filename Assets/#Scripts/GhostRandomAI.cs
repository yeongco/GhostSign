using UnityEngine;
using UnityEngine.AI;

public class GhostRandomAI : MonoBehaviour
{
    public float areaRadius = 5f;       // ������ ���� �� �ִ� �ݰ�
    public float floatHeight = 1.5f;    // Y�� ���� ����
    public float floatSpeed = 2.0f;     // ���Ʒ� ������ �ӵ�
    public float floatAmplitude = 0.3f; // ���� ����

    private NavMeshAgent agent;
    private float floatOffset;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        floatOffset = Random.Range(0f, Mathf.PI * 2f);
        MoveToRandomPoint();
    }

    void Update()
    {
        // ���Ʒ��� �����ϴ� ������
        Vector3 pos = transform.position;
        pos.y = floatHeight + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmplitude;
        transform.position = pos;

        // �����ϸ� ���ο� ��ġ�� �̵�
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveToRandomPoint();
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * areaRadius;
        randomDirection += transform.position;
        randomDirection.y = 0; // NavMesh�� ��� �����̹Ƿ� Y=0 ����

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, areaRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
