using UnityEngine;
using UnityEngine.AI;

public class ChiefClass : MonoBehaviour
{
    private NavMeshAgent myAgent;

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        move();
    }

    private void move()
    {
        myAgent.SetDestination(transform.parent.GetChild(0).position);
    }
}
