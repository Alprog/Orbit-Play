using System.Linq;
using Mirror;
using UnityEngine;

public class ParticlesEater : NetworkBehaviour
{
    public float EatingSpaceScale = 0.32f;

    private float EatingRadius => transform.localScale.x * EatingSpaceScale;
    
    private PlayerActions _playerActions;
    private Attractor _attractor;

    void Start()
    {
        _playerActions = GetComponent<PlayerActions>();
        _attractor = GetComponent<Attractor>();
        
        if (isLocalPlayer)
            InvokeRepeating(nameof(CheckMass), 0, 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, EatingRadius);
    }
    
    public void CheckMass()
    {
        if (!_playerActions.IsConsumingInProgress) return;
        
        var particlesToEat = _attractor.Satellites
            .Select(x => x.GameObject)
            .Where(x => Vector2.Distance(transform.position, x.transform.position) <= EatingRadius)
            .ToArray();


        foreach (var particle in particlesToEat)
        {
            Eat(particle);
        }
    }

    private void Eat(GameObject particle)
    {
        // Debug.Log($"[Eat] server: {isServer} | me: {isLocalPlayer} | scale: {transform.localScale.x}");

        DestroyParticle(particle); // call to server
        
        transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
    }

    [Command]
    private void DestroyParticle(GameObject particle)
    {
        ParticlesSpawner.Instance.RemoveMass(particle);
        Destroy(particle);
    }
}
