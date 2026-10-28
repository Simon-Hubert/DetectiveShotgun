using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private string _spriteFolderPath;
    [SerializeField] private string _fileName;
    [SerializeField] private string _fileSuffix;
    
    [Space(7)]
    [SerializeField, ReadOnly] private Sprite[] _spriteList;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [Space(10)]
    // [SerializeField] private float[] _rotationThreshold = { -30, -24, -18, -12, -6, 0, 6, 12, 18, 24, 30 };
    [SerializeField] private int _widthThreshold = 11;
    [SerializeField, Min(1)] private int _heightThreshold = 11;
    
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 dir = -((Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(mousePos)).normalized;
        float angle = -Vector2.SignedAngle(transform.up, dir);

        
        int spriteIndex = 0;
        
        int i;
        float _heightLevel = Screen.height / _heightThreshold;
        for (i = 1; i < _heightThreshold; i++) {
            if (Screen.height - mousePos.y < i * _heightLevel) {
                break;
            }
        }
        spriteIndex = _heightThreshold * (i - 1);

        int j;
        float _widthLevel = Screen.width / _widthThreshold;
        for (j = 1; j < _widthThreshold; j++) {
            if (mousePos.x < j * _widthLevel) {
                break;
            }
        }
        spriteIndex += j - 1;
        
        // Debug.Log($"angle: {angle} | middle angle: {_rotationThreshold[Mathf.FloorToInt(_rotationThreshold.Length / 2)]}");
        Debug.Log($"Sprite List Position (x: {j} | y: {i} | Index: {spriteIndex:D3})");
        if (spriteIndex > 0 && spriteIndex < _spriteList.Length) _spriteRenderer.sprite = _spriteList[spriteIndex];
    }

    [Button]
    private void GetSprites() {
        _spriteList = new Sprite[_heightThreshold * _widthThreshold];
        
        _spriteFolderPath = _spriteFolderPath[^1] == '/' ? _spriteFolderPath : $"{_spriteFolderPath}/";
        string filePath = $"{_spriteFolderPath}{_fileName}";
        
        for (int i = 0; i < _spriteList.Length; i++) {
            _spriteList[i] = Resources.Load<Sprite>($"{filePath}{i:D3}{_fileSuffix}");
        }
    }
}
