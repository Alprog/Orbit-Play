using System.Collections.Generic;
using System.Linq;
using interfaces;
using models;
using UnityEngine;

public class Attractor : MonoBehaviour, IAttractor
{
    public LayerMask AttractionLayer;    
    public CircleCollider2D OrbitCollider;
    public CircleCollider2D InnerCollider;

    [Tooltip("Satellites orbit speed.\nHigher speed leads to a wider orbit.\nForms actual satellites orbit along with OrbitRadius (magenta).")] 
    public float SpeedScale = 1600;
    [Tooltip("Shift ot the OrbitRadius (magenta) from the middle between colliders")] 
    public float OrbitDistanceShift = -3f;
    [SerializeField]
    private float effectionRadius = 10;

    // middle radius between _innerCollider and _orbitCollider plus OrbitDistanceShift
    public float OrbitRadius => ((OrbitCollider.radius + InnerCollider.radius) * transform.localScale.x) / 2f + OrbitDistanceShift;

    public IList<ISatellite> Satellites { get; } = new List<ISatellite>();

    void Start()
    {
        SetAttractedObjects();
        
        Debug.Log("Orbit radius - " + OrbitRadius);
    }

    void Update()
    {
        // TODO: uncomment and rewrite, when it will be the time
        // SetAttractedObjects();
    }

    public Ellipse GetOrbit()
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetGameObject() => this.gameObject;
    
    void OnDrawGizmosSelected()
    {
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, effectionRadius);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, OrbitRadius);
    }
    
    void SetAttractedObjects()
    {
        var satellites = Physics2D.OverlapCircleAll(transform.position, effectionRadius, AttractionLayer);
        
        foreach (var satellite in satellites)
        {
            AttachSatellite(satellite.GetComponent<Satellite>());
        }
    }

    private void AttachSatellite(Satellite satellite)
    {
        Debug.Log($"Attaching satellite: {satellite}");
        Satellites.Add(satellite);
        satellite.Attach(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Satellite satellite = other.GetComponent<Satellite>();

        if (satellite && satellite.currentAttractor == null)
        {
            Debug.Log("Entered orbit");
            AttachSatellite(satellite);
            satellite.IsOnOrbit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Satellite satellite = other.GetComponent<Satellite>();

        if (satellite && satellite.currentAttractor == this)
        {
            Debug.Log("Exited orbit");
            satellite.IsOnOrbit = false;
            satellite.Detach();
            Satellites.Remove(satellite);
        }
    }
}
