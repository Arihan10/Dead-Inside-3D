using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NavManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> targets = new List<Transform>();

    [SerializeField] private List<Transform> spawnpoints = new List<Transform>();
    
    private List<List<Transform>> alternates = new List<List<Transform>>();
    
    [SerializeField]
    private float rayLength;
    
    [SerializeField]
    private int rayCount;

    [SerializeField]
    private LayerMask layerMask;
    
    [SerializeField]
    private LayerMask playerMask;
    
    [SerializeField]
    private LayerMask antiZombie;
    
    public static NavManager inst { get; private set; }

    public void Awake()
    {
        if (inst || !PhotonNetwork.IsMasterClient) Destroy(this);
        else inst = this;
    }


    [Serializable]
    class ZombieAgent
    {
        public NavMeshAgent agent;
        public int targetA, targetB;

        public ZombieAgent(NavMeshAgent agent, int targetA, int targetB)
        {
            this.agent = agent;
            this.targetA = targetA;
            this.targetB = targetB;
        }
    }

    private int currentSpawn = 0;
    public Vector3 getNextSpawn()
    {
        currentSpawn = (++currentSpawn) % spawnpoints.Count;
        return spawnpoints[currentSpawn].position;
    }
    
    [SerializeField]
    private List<ZombieAgent> zombies = new List<ZombieAgent>();


    [SerializeField] private GameObject zombiePrefab;
    
    [SerializeField]
    Vector3 spawnAreaCenter = Vector3.zero;
    
    [SerializeField]
    Vector3 spawnAreaSize = new Vector3(10, 1, 10);

    [SerializeField] private float lockCoefficient = 0.3f;
    private void Start()
    {
        Regen();

        
        /*
        for (int i = 0; i < 13; i++)
        {
            Vector3 spawnPos = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                0,
                Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );

            GameObject obj = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                AddAgent(agent);
            }
        }
        */
    }

    public NavMeshAgent SpawnZombie(Vector3 spawnPos)
    {
        GameObject obj = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            AddAgent(agent);
        }

        return agent;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }

    public void AddAgent(NavMeshAgent agent)
    {
        if (alternates.Count != targets.Count)
            Regen();
        
        // randomly assign to one of players
        int targetA = Random.Range(0, targets.Count);
        
        // randomly assign to one alternate
        int targetB = Random.Range(0, rayCount);

        // add to dict/set target
        zombies.Add(new ZombieAgent(agent, targetA, targetB));
    }
    
    private void Regen()
    {
        alternates.Clear();
        for (int i = 0; i < targets.Count; ++i)
        {
            List<Transform> altList = new List<Transform>();
            for (int j = 0; j < rayCount; ++j)
            {
                //Transform t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                //Destroy(t.GetComponent<Collider>());
                Transform t = new GameObject().transform;
                t.SetParent(targets[i]);
                altList.Add(t);
            }
            alternates.Add(altList);
        }
    }
    
    [SerializeField] int framesBetweenUpdates = 10;
    private int frameCounter = 0;

    private int currentWave = 0;
    private string yeah = "Wide range of funny and unusual issues.";
    
    private void Update()
    {
        // cull invalid/dead
        for (int i = 0; i < zombies.Count; ++i)
        {
            if (zombies[i] == null || zombies[i].agent == null)
            {
                zombies.RemoveAt(i--);
            }
        }

        if (currentWave == 0 )
        {
            Debug.Log("NEW ROUND (only round)");
            GameManager.instance.StartNewWave(7, yeah);
            ++currentWave;
        }
        
        
        float step = 360.0f / rayCount;

        for (int agent = 0; agent < targets.Count; ++agent)
        {
            float ang = 0;
            
            for (int i = 0; i < rayCount; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(ang, Vector3.up) * Vector3.forward;
                Debug.DrawRay(targets[agent].position, dir * rayLength, Color.red);
                ang += step;
            }
        }
    }

    void FixedUpdate()
    {
        float step = 360.0f / rayCount;

        for (int agent = 0; agent < targets.Count; ++agent)
        {
            float ang = 0;
            
            for (int i = 0; i < rayCount; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(ang, Vector3.up) * Vector3.forward;
                RaycastHit hitInfo;
                if (Physics.Raycast(targets[agent].position, dir, out hitInfo, rayLength, layerMask))
                {
                    alternates[agent][i].position = hitInfo.point;
                }
                else
                {
                    alternates[agent][i].position = targets[agent].position + dir * rayLength;
                }
                ang += step;
            }
        }
        
        // set targets...
        for (int i = 0; i < zombies.Count; i++)
        {
            Vector3 pos = zombies[i].agent.transform.position;
            Vector3 dir = targets[zombies[i].targetA].position - pos;
            
            if ((frameCounter + i) % framesBetweenUpdates == 0)
            {
                RaycastHit hit;
                
                Vector3 dest;

                bool followingAlt = false;
                // set position
                if (Physics.Raycast(pos, dir, out hit, rayLength * lockCoefficient, antiZombie) && ((1 << hit.transform.gameObject.layer) & playerMask) != 0)
                {
                    dest = targets[zombies[i].targetA].position;
                }
                else
                {
                    dest = alternates[zombies[i].targetA][zombies[i].targetB].position;
                    followingAlt = true;
                }

                if (followingAlt && Vector3.SqrMagnitude(dest - zombies[i].agent.transform.position) < 4f)
                {
                    zombies[i].targetB = Random.Range(0, rayCount);
                }
                    
                if ((zombies[i].agent.destination - dest).sqrMagnitude > 0.1f) zombies[i].agent.SetDestination(dest);
            }
        }
        
        frameCounter++;
    }
}
