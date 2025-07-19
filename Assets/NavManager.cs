using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavManager : MonoBehaviour
{
    [SerializeField]
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    private List<Transform> targets = new List<Transform>();
    
    void Update()
    {
        
    }
}
