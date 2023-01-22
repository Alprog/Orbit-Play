using Mirror;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    public float SpeedScale = 10f;
    public float CameraLerpedSpeedScale = 3f;

    private Camera _camera;
    private Joystick _joystick;
    private Rigidbody2D _rigidbody2D;
    private Transform _cameraTransform;
    private PlayerActions _playerActions;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();

        _joystick = GameObject.FindGameObjectWithTag("Joystick")?.GetComponent<FixedJoystick>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        var cam = GameObject.FindGameObjectWithTag("MainCamera");

        _camera = cam.GetComponent<Camera>();
        _cameraTransform = _camera.GetComponent<Transform>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (_playerActions.IsConsumingInProgress)
            return;
        
        MoveTarget();
        MoveCamera();
    }

    private void MoveTarget()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 addForce = Vector2.zero;

        if (horizontal > 0.01f || horizontal < -0.01f)
        {
            addForce.x = horizontal;
        }

        if (vertical > 0.01f || vertical < -0.01f)
        {
            addForce.y = vertical;
        }

        if (_joystick)
        {
            addForce += _joystick.Direction;
        }

        addForce *= SpeedScale * Time.deltaTime;

        _rigidbody2D.AddForce(addForce, ForceMode2D.Force);
    }

    private void MoveCamera()
    {
        Vector3 desiredCameraPosition =
            new Vector3(transform.position.x, transform.position.y, _cameraTransform.position.z);
        
        _cameraTransform.position = Vector3.Lerp(
            _cameraTransform.position,
            desiredCameraPosition,
            Time.deltaTime * CameraLerpedSpeedScale);
    }
}
