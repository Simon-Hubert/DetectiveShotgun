using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DShotgun
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Transform _from;
        [SerializeField] private Transform _to;
        [SerializeField, Min(1f)] private float _sensitivity;

        private void Update()
        {
            float p = Mathf.InverseLerp(0, 1920, Mouse.current.position.ReadValue().x);
            p = Gain(p, _sensitivity);
            transform.position = Vector3.Lerp(_from.position, _to.position, p);
        }
        
        float Gain( float x, float k ) 
        {
            float a = 0.5f*Mathf.Pow(2.0f*((x<0.5f)?x:1.0f-x), k);
            return (x<0.5f) ? a : 1.0f-a;
        }
    }
}
