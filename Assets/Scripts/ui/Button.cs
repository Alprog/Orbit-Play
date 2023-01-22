using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsButtonPressed { get; private set; }
    
    public void OnPointerUp(PointerEventData data)
    {
        IsButtonPressed = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        IsButtonPressed = true;
    }
}
