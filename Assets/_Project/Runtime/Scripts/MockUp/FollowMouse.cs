using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private Transform _gunPivot;
    
    // Update is called once per frame
    void Update()
    {
        float angle = Vector2.Angle(Vector2.right, Mouse.current.position.ReadValue());
        _gunPivot.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        
        Debug.Log($"angle : {angle} | rotation : {_gunPivot.transform.rotation.eulerAngles.z} | MousePos : {Mouse.current.position.ReadValue()}");
    }
}
