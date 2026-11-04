using System;
using UnityEngine;

namespace DShotgun
{
    /// <summary>
    /// Définie les states du jeu
    /// </summary>
    public enum GameState
    {
        Playing,
        Paused,
        Dialogue,
        Title
    }
    
    /// <summary>
    /// Gère l'état du jeu et les transitions entre les différents <see cref="GameState"/>
    /// </summary>
    public static class GameManager
    {
        private static GameState _currentState = GameState.Title;
        
        /// <summary>
        /// Indique l'état courant du jeu, peut etre changer via l'appelle de la fonction <see cref="SwitchState"/> 
        /// </summary>
        public static GameState CurrentState => _currentState;
        
        /// <summary>
        /// Event déclenché lorsqu'un changement de <see cref="CurrentState"/> est effectué
        /// </summary>
        public static event Action<GameState> OnStateChange;
        
        /// <summary>
        /// Bascule le <see cref="CurrentState"/> vers un <see cref="GameState"/> défini
        /// </summary>
        /// <param name="newState"> le prochain <see cref="GameState"/> </param>
        /// <returns> true si la transition a pu être effectuée, false sinon </returns>
        public static bool SwitchState(GameState newState)
        {
            if (!ExitState(_currentState)) return false;
            
            EnterState(newState);
            _currentState = newState;
            OnStateChange?.Invoke(newState);

            return true;
        }

        private static void EnterState(GameState nextState)
        {
            switch (nextState)
            {
                case GameState.Playing:
                    
                    break;
                case GameState.Paused:
                    
                    break;
                case GameState.Dialogue:
                    
                    break;
                case GameState.Title:
                    
                    break;
                default:
                    Debug.LogWarning($"Unhandled game state: {nextState}");
                    break;
            }
        }

        private static bool ExitState(GameState previousState)
        {
            switch (previousState)
            {
                case GameState.Playing:
                    
                    break;
                case GameState.Paused:
                    
                    break;
                case GameState.Dialogue:
                    
                    break;
                case GameState.Title:
                    
                    break;
                default:
                    Debug.LogWarning($"Unhandled game state: {previousState}");
                    return false;
                    break;
            }

            return true;
        }
    }
}
