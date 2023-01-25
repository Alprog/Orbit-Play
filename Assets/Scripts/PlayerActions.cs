using System.Linq;
using Mirror;
using UnityEngine;

/// <summary>
/// Correspond for Player-specific actions: particles consuming, TODO: movement, TODO:shooting
/// </summary>
public class PlayerActions : NetworkBehaviour
{
    [Header("Player Action Settings")]
    public float ParticlesConsumingTime = 1f;
    public float ParticlesConsumingProgress => ParticlesConsumingTimeProgress / ParticlesConsumingTime;

    [SyncVar(hook = nameof(SetConsumingTimeHook))]
    public float ParticlesConsumingTimeProgress;
    public bool IsConsumingInProgress => ParticlesConsumingProgress > ConsumingStartedThreshold;


    public void SetConsumingTimeHook(float oldTime, float newTime)
    {
        // Debug.Log($"[set progress hook] server: {isServer} | me: {isLocalPlayer} | val = {ParticlesConsumingTimeProgress} => {newTime}");
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
    private Button _eatButton;
    
    void Start()
    {
        _attractor = GetComponent<Attractor>();
        _innerCollider = GetComponents<CircleCollider2D>().Single(x => !x.isTrigger);
        _animator = GetComponent<PlayerAnimationController>();
        
        _startOrbitShift = _attractor.OrbitDistanceShift;
        _startColliderRadius = _innerCollider.radius;

        float _finalColliderRadiusThreshold = 0.01f;
        
        _finalColliderRadius = 0f + _finalColliderRadiusThreshold;
        // _finalOrbitShift = _startOrbitShift - 4f;
        
        
        _eatButton = GameObject.FindGameObjectsWithTag("Button")?
            .SingleOrDefault(x => x.name == "A")
            ?.GetComponent<Button>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        
        if (Input.GetKey(KeyCode.Space) || _eatButton.IsButtonPressed)
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
