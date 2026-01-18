using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Inputs : MonoBehaviour
{
    public UnityEvent jumpEvent = new();
    public Vector2 move;
    public bool jump;

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
        jumpEvent?.Invoke();
    }
}
