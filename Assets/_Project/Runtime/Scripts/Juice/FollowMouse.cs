using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private string _spriteFolderPath;
    [SerializeField] private string _fileName;
    [SerializeField] private string _fileSuffix;
    
    [Space(7)]
    [SerializeField, ReadOnly] private Sprite[] _spriteList;
    [SerializeField, ReadOnly] private int _index;
    [SerializeField] private Image _spriteRenderer;
    
    [Header("Threshold")]
    [SerializeField, Min(1)] private int _widthThreshold = 11;
    [SerializeField] private bool _invertWidth = false;
    [Space(7)]
    [SerializeField, Min(1)] private int _heightThreshold = 11;
    [SerializeField] private bool _invertHeight = false;

    [Header("VFX Spawnpoint")]
    [SerializeField] private Transform _vfxMinSpawnPos;
    [SerializeField] private Transform _vfxMaxSpawnPos;
    private Vector2 GetSpawnPosThreshold
    {
        get
        {
            Vector2 result = _vfxMaxSpawnPos.position - _vfxMinSpawnPos.position;
            return new Vector2(result.x / (_widthThreshold -1), result.y / (_heightThreshold -1));
        }
    }
    [Space(10)]
    [SerializeField, ReadOnly] private Vector2 _vfxSpawnPos;
    public Vector2 GetVFXSpawnPos { get => _vfxSpawnPos; }
    
    [SerializeField, ReadOnly] private float _vfxAngle;
    public float GetVFXAngle { get => _vfxAngle; }
    [Space(7)]
    [SerializeField] private bool _showTest;
    [SerializeField] private Transform _test;

    private void Start()
    {
        _test.gameObject.SetActive(_showTest);
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        Vector2 dir = (mousePos - (Vector2)transform.position);
        _vfxAngle = Vector2.SignedAngle(Vector2.up, dir);
        
        
        int spriteIndex = 0;
        
        int i;
        float _heightLevel = Screen.height / _heightThreshold;
        for (i = 1; i < _heightThreshold; i++) {
            float height = _invertHeight ? mousePos.y : Screen.height - mousePos.y;
            if (height < i * _heightLevel) {
                break;
            }
        }
        spriteIndex = _heightThreshold * (i - 1);

        int j;
        float _widthLevel = Screen.width / _widthThreshold;
        for (j = 1; j < _widthThreshold; j++) {
            float width = _invertWidth ? Screen.width - mousePos.x : mousePos.x;
            if (width < j * _widthLevel) {
                break;
            }
        }
        spriteIndex += j - 1;
        _index = spriteIndex;

        _vfxSpawnPos = (Vector2)_vfxMinSpawnPos.position + 
                       new Vector2(GetSpawnPosThreshold.x * (_invertWidth ? (_widthThreshold - j) : j), 
                           GetSpawnPosThreshold.y * (_invertHeight ? i : (_widthThreshold - i)));

        if (_test) {
            _test.position = _vfxSpawnPos;
            _test.rotation = Quaternion.Euler(0, 0, _vfxAngle);
        }
        
        // Debug.Log($"angle: {angle} | middle angle: {_rotationThreshold[Mathf.FloorToInt(_rotationThreshold.Length / 2)]}");
        //Debug.Log($"Sprite List Position (x: {j} | y: {i} | Index: {spriteIndex:D3})");
        if (spriteIndex >= 0 && spriteIndex < _spriteList.Length) _spriteRenderer.sprite = _spriteList[spriteIndex];
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

    [Button]
    private void DisplayDefaultSprite() => _spriteRenderer.sprite = _spriteList[(_heightThreshold / 2 * _heightThreshold) + _widthThreshold / 2];
    
    [Button]
    private void DisplayFirstSprite() => _spriteRenderer.sprite = _spriteList[0];
    [Button]
    private void DisplayLastSprite() => _spriteRenderer.sprite = _spriteList[^1];
}
