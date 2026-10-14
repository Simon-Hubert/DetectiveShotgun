using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        transform.up = -((Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())).normalized;
    }
}
