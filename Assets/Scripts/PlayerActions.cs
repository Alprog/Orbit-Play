using System.Linq;
using Mirror;
using UnityEngine;

/// <summary>
/// Correspond for Player-specific actions: particles consuming, TODO: movement, TODO:shooting
/// </summary>
public class PlayerActions : NetworkBehaviour
{
    public float ParticlesConsumingTime = 1f;
    public float ParticlesConsumingProgress => ParticlesConsumingTimeProgress / ParticlesConsumingTime;

    [SyncVar(hook = nameof(SetConsumingTimeHook))]
    public float ParticlesConsumingTimeProgress;
    public bool IsConsumingInProgress => ParticlesConsumingProgress > ConsumingStartedThreshold;


    public void SetConsumingTimeHook(float oldTime, float newTime)
    {
        Debug.Log($"[set progress hook] server: {isServer} | me: {isLocalPlayer} | val = {ParticlesConsumingTimeProgress} => {newTime}");
        ParticlesConsumingTimeProgress = newTime;

        PlayAnimation();
    }

    private const float ConsumingStartedThreshold = 0.9f;
    
    private float _startOrbitShift;
    private float _startColliderRadius;

    private float _finalOrbitShift => - (((_attractor.OrbitCollider.radius + _attractor.InnerCollider.radius) * transform.localScale.x) / 2f) + 0.1f;
    private float _finalColliderRadius;
    
    private Attractor _attractor;
    private CircleCollider2D _innerCollider;
    private CircleCollider2D _outerCollider;
    private PlayerAnimationController _animator;

    void Start()
    {
        _attractor = GetComponent<Attractor>();
        _innerCollider = GetComponents<CircleCollider2D>().Single(x => !x.isTrigger);
        _animator = GetComponent<PlayerAnimationController>();
        
        _startOrbitShift = _attractor.OrbitDistanceShift;
        _startColliderRadius = _innerCollider.radius;
        
        _finalColliderRadius = 0f + 0.01f; // 0.01f - threshold 
        // _finalOrbitShift = _startOrbitShift - 4f;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        
        // TODO: add mobile control
        if (Input.GetKey(KeyCode.Space))
        {
            if (ParticlesConsumingTimeProgress < ParticlesConsumingTime)
            {
                ParticlesConsumingTimeProgress += Time.deltaTime;
                
                PlayAnimation();
            }
        }
        else if (ParticlesConsumingTimeProgress > 0.1f)
        {
            ParticlesConsumingTimeProgress -= Time.deltaTime * 5;

            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        // Debug.Log("Consuming progress" + ParticlesConsumingProgress);

        _animator.SetConsumingProgress(ParticlesConsumingProgress);
        
        _attractor.OrbitDistanceShift = Mathf.Lerp(_startOrbitShift, _finalOrbitShift, ParticlesConsumingProgress);

        _innerCollider.radius = Mathf.Lerp(_startColliderRadius, _finalColliderRadius, ParticlesConsumingProgress);
    }
}
