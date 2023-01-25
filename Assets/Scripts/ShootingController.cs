using System.Linq;
using Mirror;
using UnityEngine;

public class ShootingController : NetworkBehaviour
{
    [Header("Shooting Settings")]
    public float ShootingSpeed = 2f;

    private GameObject Shell;

    private Attractor _attractor;

    private GameObject _joystick;

    private int _uniqueUserTouchId = Constants.NullUserTouchId;

    private void Start()
    {
        _attractor = GetComponent<Attractor>();
        _joystick = GameObject.FindGameObjectWithTag("Joystick");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
    
        ExecuteUserTouchInput();
    }

    private void ExecuteUserTouchInput()
    {
        if (Input.touchCount > 0)
        {

            foreach (var touch in Input.touches)
            {
                // Did the touch action just begin?
                if (touch.phase == TouchPhase.Began)
                {
                    if(IsTouchInJoystickRect(touch))
                    {
                        if (!(_joystick.activeSelf && Input.touches.Length > 1))
                            continue;
                    }
                    //Select nearest asteroid 
                    _uniqueUserTouchId = touch.fingerId;
                    SelectAsteroid();

                }

                //Shooting
                if (touch.phase == TouchPhase.Ended)
                {
                    if (Shell != null && _uniqueUserTouchId == touch.fingerId)
                    {
                        LookAtMousePosition(Shell);

                        ShootMeteorite(Shell);

                        Shell = null;
                        _uniqueUserTouchId = Constants.NullUserTouchId;
                    }
                }

            }
        }
    }

    private void SelectAsteroid()
    {
        if (Shell == null)
        {
            GameObject findedObject = MousePositionToNearestSatellite();

            if (findedObject != null)
            {
                Satellite script = findedObject.GetComponent<Satellite>();

                if (script != null && script.IsOnOrbit)
                {
                    Shell = findedObject;

                    Debug.Log("dew - 2");

                    Shell.GetComponent<SatelliteAnimationController>().SetCharged();
                }
            }
        }
    }

    private void ShootMeteorite(GameObject meteorite)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(meteorite.transform.position);
        Vector2 direction = (Vector2)(Input.mousePosition - screenPoint);

        float shootingForce = Vector2.Distance(screenPoint, Input.mousePosition) * 0.05f;

        direction.Normalize();

        ShootMeteorite_Cmd(meteorite, direction, shootingForce);
    }


    /// <summary>
    /// Object look at the mouse position on screen.
    /// </summary>
    private void LookAtMousePosition(GameObject lookingObject)
    {
        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(lookingObject.transform.position);

        LookInDirection_Cmd(lookingObject, direction);
    }

    [Command]
    private void ShootMeteorite_Cmd(GameObject meteorite, Vector2 direction, float shootingForce)
    {
        meteorite.GetComponent<Rigidbody2D>().AddForce(direction * (shootingForce * ShootingSpeed), ForceMode2D.Impulse);
        
        meteorite.GetComponent<SatelliteAnimationController>().SetSatellite();
    }

    [Command]
    private void LookInDirection_Cmd(GameObject lookingObject, Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lookingObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    /// <summary>
    /// Get GameObject under mouse position on screen.
    /// </summary>
    /// <returns></returns>
    private GameObject MousePositionRayToObject()
    {
        int layerObject = 0;
        Vector2 ray = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, layerObject);

        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }

        return null;
    }
    /// <summary>
    /// Get GameObject near mouse position on screen.
    /// </summary>
    /// <returns></returns>
    private GameObject MousePositionToNearestSatellite()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var nearestSatellite = _attractor.Satellites
            .Select(x => new
            {
                satellite = x,
                dist = Vector3.Distance(x.GameObject.transform.position, mouseWorldPoint)
            })
            .OrderBy(x => x.dist)
            .FirstOrDefault();


        return nearestSatellite?.satellite?.GameObject;
    }
    /// <summary>
    /// Check object is in JoystickRect on the screen.
    /// </summary>
    private bool IsTouchInJoystickRect(Touch touch)
    {
        var imgRectTransform = _joystick.GetComponent<RectTransform>();

        Vector2 localMousePosition = touch.position;
        if (imgRectTransform.rect.Contains(localMousePosition))
        {
            return true;
        }
        return false;
    }
}
