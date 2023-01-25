using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ParticlesSpawner : NetworkBehaviour
{

    #region  Instance
    public static ParticlesSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion  

    [Header("Particle Spawner Settings")]

    public GameObject ParticlePrefab;

    [FormerlySerializedAs("CreatedMasses")] 
    public List<GameObject> CreatedParticles = new List<GameObject>();

    [FormerlySerializedAs("MaxMass")] 
    public int MaxParticlesCount = 50;

    [FormerlySerializedAs("TIME_TO_CREATE_MASS")] 
    public float TIME_TO_CREATE_PARTICLE = 0.5f;

    public Vector2 CenterSpawnPosition;

    private GameObject ParentForParticles = null;


    public override void OnStartServer()
    {
        base.OnStartServer();

        StartCoroutine(CreateMass());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        foreach (var particle in CreatedParticles)
        {
            Destroy(particle);
        }

        CreatedParticles = new List<GameObject>();
    }

    void Start()
    {
        ParentForParticles = GameObject.FindGameObjectWithTag("ParticlesParent");
        if(ParentForParticles == null)
        {
            Debug.LogWarning("Particles parent is null. Can't find GameObject with tag - ParticlesParent");
        }
    }

    public IEnumerator CreateMass()
    {
        yield return new WaitForSecondsRealtime(TIME_TO_CREATE_PARTICLE);

        if (CreatedParticles.Count < MaxParticlesCount)
        {
            Vector2 p = new Vector2(Random.Range(-CenterSpawnPosition.x, CenterSpawnPosition.x), Random.Range(-CenterSpawnPosition.y, CenterSpawnPosition.y));
            p /= 2;

            GameObject particleObject = Instantiate(ParticlePrefab, p, Quaternion.identity); // parent was removed to sync spawn over network correctly
            NetworkServer.Spawn(particleObject);

            AddMass(particleObject);
        }

        StartCoroutine(CreateMass());
    }
    
    public void AddMass(GameObject mass)
    {
        if (CreatedParticles.Contains(mass) == false)
        {
            CreatedParticles.Add(mass);
        }
    }
    
    public void RemoveMass(GameObject mass)
    {
        if (CreatedParticles.Contains(mass) == true)
        {
            CreatedParticles.Remove(mass);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CenterSpawnPosition);
    }
}
