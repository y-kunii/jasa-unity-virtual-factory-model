using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubuAI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    //追いかける対象
    [SerializeField]
    private Transform _player;

    void Update()
    {
        _navMeshAgent.SetDestination(_player.position);
    }
}
