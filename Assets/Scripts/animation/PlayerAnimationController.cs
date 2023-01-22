using Mirror;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    private Animator _animator;
    
    void Start() {
        _animator = GetComponent<Animator>();
        _animator.speed = 0;
    }

    public void SetConsumingProgress(float progress) {
        if (progress > 0.99f) return;

        _animator.Play("ConsumingAnimation", -1, progress);
    }
}
