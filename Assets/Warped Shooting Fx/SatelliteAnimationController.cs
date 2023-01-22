using UnityEngine;

public class SatelliteAnimationController : MonoBehaviour
{
    private Animator _animator;
    private CircleCollider2D _collider;

    private float _idleColliderOffset = 0f;
    private float _satelliteColliderOffset = 0.12f;
    private float _chargedColliderOffset = 0.18f;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");

            SetIdle();
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("Keypad0");

            SetSatellite();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("Keypad1");

            SetCharged();
        }
    }

    public void SetCharged()
    {
        _collider.offset = new Vector2(_chargedColliderOffset, _collider.offset.y);
        _animator.Play("Charged Animation");
    }

    public void SetSatellite()
    {
        _collider.offset = new Vector2(_satelliteColliderOffset, _collider.offset.y);
        _animator.Play("Bolt Animation");
    }

    public void SetIdle()
    {
        _collider.offset = new Vector2(_idleColliderOffset, _collider.offset.y);
        _animator.Play("satellite_idle");
    }
}
