using UnityEngine;
using UnityEngine.AI;

public class TroopClass : MonoBehaviour
{
    public GameObject box;

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
        myAgent.SetDestination(box.transform.position);
    }
}
