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

    public GameObject ParticlePrefab;

    [FormerlySerializedAs("CreatedMasses")] 
    public List<GameObject> CreatedParticles = new List<GameObject>();

    [FormerlySerializedAs("MaxMass")] 
    public int MaxParticles = 50;

    [FormerlySerializedAs("TIME_TO_CREATE_MASS")] 
    public float TIME_TO_CREATE_PARTICLE = 0.5f;

    public Vector2 pos;

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

        if (CreatedParticles.Count < MaxParticles)
        {
            Vector2 p = new Vector2(Random.Range(-pos.x, pos.x), Random.Range(-pos.y, pos.y));
            p /= 2;

            GameObject particleObject = Instantiate(ParticlePrefab, p, Quaternion.identity); // parent was removed to sync spawn over network correctly
            NetworkServer.Spawn(particleObject);

            AddMass(particleObject);
        }

        StartCoroutine(CreateMass());
    }
    
    public void AddMass(GameObject m)
    {
        if (CreatedParticles.Contains(m) == false)
        {
            CreatedParticles.Add(m);
        }
    }
    
    public void RemoveMass(GameObject m)
    {
        if (CreatedParticles.Contains(m) == true)
        {
            CreatedParticles.Remove(m);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, pos);
    }
}
