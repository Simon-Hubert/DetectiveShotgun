using UnityEngine;
using NaughtyAttributes;

namespace DShotgun
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDSAnimator : MonoBehaviour
    {
        [Header("Resource Management")]
        [Foldout("Resources"), SerializeField] private string _spriteFolderPath;
        [Foldout("Resources"), SerializeField] private string _fileName;
        [Foldout("Resources"), SerializeField] private string _fileSuffix;
        [Space(7)]
        [Foldout("Resources"), SerializeField, Min(0)] private int _nbOfElements;
        [Foldout("Resources"), SerializeField, Min(0)] private int _startAtIndex;
        
        [Header("Sprite Rendering")]
        [SerializeField, ReadOnly] private Sprite[] _spriteList;
        [Foldout("Resources"), SerializeField, ReadOnly] private int _index;
        [Space(7)]
        [Foldout("Resources"), SerializeField] private SpriteRenderer _spriteRenderer;
        
        [Header("Animation")]
        [SerializeField] private bool _loop = false;
        [SerializeField, Min(0.1f)] private float _fps = 8;
        private float mspf { get => 1.0f / _fps; }
        private float currenTime = 0.0f;
        public float GetAnimationDuration() => _fps / _spriteList.Length;

        [Space(7)]
        [SerializeField] public bool isPlaying;
        public void PlayAnim() => isPlaying = true;
        public void StopAnim() => isPlaying = false;
        [SerializeField] private bool _playOnAwake;

        private void Awake() {
            if (_playOnAwake) isPlaying = true;
            currenTime = mspf;
        }
        private void Start() {
            if (_spriteList != null || _spriteList.Length > 0) {
                _spriteRenderer.sprite = _spriteList[0];
            }

            if (_spriteRenderer == null) {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
        
        private void Update() {
            if (!isPlaying) return;

            while (((_index < _spriteList.Length && !_loop) || _loop)
                   && currenTime >= mspf)
            {
                if (_loop && _index >= _spriteList.Length) _index = 0;
                if (_index < _spriteList.Length) {
                    _spriteRenderer.sprite = _spriteList[_index];
                
                    currenTime -= mspf;
                    Debug.Log($"{_index}: {_spriteRenderer.sprite.name}");
                    _index++;
                }
            }
            
            currenTime += Time.deltaTime;
        }


        [Button]
        private void GetSprites() {
            _spriteList = new Sprite[_nbOfElements];
        
            _spriteFolderPath = _spriteFolderPath[^1] == '/' ? _spriteFolderPath : $"{_spriteFolderPath}/";
            string filePath = $"{_spriteFolderPath}{_fileName}";
        
            for (int i = 0; i < _spriteList.Length; i++) {
                if (_nbOfElements < 10)
                    _spriteList[i] = Resources.Load<Sprite>($"{filePath}{i+_startAtIndex:D1}{_fileSuffix}");
                else if (_nbOfElements < 100)
                    _spriteList[i] = Resources.Load<Sprite>($"{filePath}{i+_startAtIndex:D2}{_fileSuffix}");
                else if (_nbOfElements < 1000)
                    _spriteList[i] = Resources.Load<Sprite>($"{filePath}{i+_startAtIndex:D3}{_fileSuffix}");
            }

            if (!_spriteRenderer) TryGetComponent<SpriteRenderer>(out _spriteRenderer);
            if (_spriteRenderer && _nbOfElements > 0 && _spriteList[0] != null) _spriteRenderer.sprite = _spriteList[0];
        }
    }
}
