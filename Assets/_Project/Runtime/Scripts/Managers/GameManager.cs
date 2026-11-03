using UnityEngine;

namespace DShotgun
{
    public enum GameState
    {
        Playing,
        Paused,
        Dialogue
    }
    public static class GameManager
    {
        private static GameState _currentState = GameState.Playing;
        public static GameState CurrentState => _currentState;
        
        public static void SwitchState(GameState newState)
        {
            _currentState = newState;
        }
    }
}
