using UnityEngine;
using UnityEngine.AI;

public class GhostRandomAI : MonoBehaviour
{
    public float areaRadius = 5f;       // 유령이 떠돌 수 있는 반경
    public float floatHeight = 1.5f;    // Y축 부유 높이
    public float floatSpeed = 2.0f;     // 위아래 움직임 속도
    public float floatAmplitude = 0.3f; // 부유 진폭

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
        // 위아래로 부유하는 움직임
        Vector3 pos = transform.position;
        pos.y = floatHeight + Mathf.Sin(Time.time * floatSpeed + floatOffset) * floatAmplitude;
        transform.position = pos;

        // 도착하면 새로운 위치로 이동
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            MoveToRandomPoint();
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * areaRadius;
        randomDirection += transform.position;
        randomDirection.y = 0; // NavMesh는 평면 기준이므로 Y=0 고정

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, areaRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
