using System;
using UnityEngine;

[Serializable]
public enum SceneState {
    Idle,
    Dialogue,
    Map
}
public class SwitchState : MonoBehaviour
{
    [SerializeField] private SceneState _state = SceneState.Idle;
    private SceneState _lastState;
    [Space(10)]
    [SerializeField] private GameObject _UIIdle;
    [SerializeField] private GameObject _UIMove;
    [SerializeField] private GameObject _UIDialogue;
    [SerializeField] private GameObject _OnScene;

    private void Start() {
        _lastState = _state;
        ChangeState(_state);
    }

    private void Update() {
        if (_state != _lastState) ChangeState(_state);
        
        _lastState = _state;
    }

    public void ChangeState(SceneState a_state) {
        switch (a_state) {
            case SceneState.Map :
                _UIIdle.SetActive(true);
                _UIMove.SetActive(true);
                _UIDialogue.SetActive(false);
                _OnScene.SetActive(true);
                break;
                
            case SceneState.Dialogue :
                _UIIdle.SetActive(false);
                _UIMove.SetActive(false);
                _UIDialogue.SetActive(true);
                _OnScene.SetActive(false);
                break;
                
            case SceneState.Idle :
            default :
                _UIIdle.SetActive(true);
                _UIMove.SetActive(false);
                _UIDialogue.SetActive(false);
                _OnScene.SetActive(true);
                break;
        }
    }
}
