using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;

    private int currentPatrolPosition = 0;
    private Vector3[] patrolPositions =
    {
        new Vector3 {x=-9.0f, y=0.0f, z=-4.0f},
        new Vector3 {x=0.0f, y=0.0f, z=-4.0f},
        new Vector3 {x=0.0f, y=0.0f, z=-9.0f},
        new Vector3 {x=-9.0f, y=0.0f, z=-9.0f}
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            PlayerTappedWall();
        }

        UpdatePatrolPosition();
    }

    private float GetDistanceToPosition(Vector3 playerPos, Vector3 enemyPos)
    {
        float distanceToPosition = Mathf.Abs(playerPos.x - enemyPos.x) + Mathf.Abs(playerPos.z - enemyPos.z);
        return distanceToPosition;
    }

    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private void UpdatePatrolPosition()
    {
        Vector3 agentPos = this.transform.position;
        if (GetDistanceToPosition(agentPos, patrolPositions[currentPatrolPosition]) < 1)
        {
            if (currentPatrolPosition < 3)
            {
                currentPatrolPosition++;
            }
            else
            {
                currentPatrolPosition = 0;
            }
        }

        SetDestination(patrolPositions[currentPatrolPosition]);
    }

    private IEnumerator SearchForPlayer()
    {
        float searchTime = 0.0f;
        while (searchTime < 5.0f)
        {
            Vector3 playerPos = player.transform.position;
            SetDestination(playerPos);
            searchTime += Time.deltaTime;
            yield return null;
        }

        SetDestination(patrolPositions[currentPatrolPosition]);
    }

    private void PlayerTappedWall()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 agentPos = this.transform.position;

        if (GetDistanceToPosition(playerPos, agentPos) < 10)
        {
            StartCoroutine(SearchForPlayer());
        }
    }

}
