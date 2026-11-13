using UnityEngine;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    public static class TableauManager
    {
        public static bool IsLoading { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            IsLoading = false;
        }

        public static void LoadTableauWithTransition(int buildIndexTableau, int buildIndexState, TransitionSceneData data)
        {
            _ = SwitchTableauWithTransition(buildIndexTableau, buildIndexState, data);
        }

        public static void LoadTableau(int buildIndexTableau, int buildIndexState)
        {
            _ = SwitchTableau(buildIndexTableau, buildIndexState);
        }
        
        private static async Awaitable SwitchTableauWithTransition(int buildIndexTableau, int buildIndexState, TransitionSceneData data)
        {
            IsLoading = true;
            try {
                GameManager.SwitchState(GameState.Transition);
                await OverlaySingleton.instance.FadeToBlackTransitionAsync(2);
                await SceneManager.LoadSceneAsync(buildIndexTableau, LoadSceneMode.Single);
                await SceneManager.LoadSceneAsync(buildIndexState, LoadSceneMode.Additive);
                TransitionSingleton.Clear();
                TransitionSingleton.SetImage(data.Background);
                TransitionSingleton.Show();
                
                await OverlaySingleton.instance.FadeInTransitionAsync(2);

                await TransitionSingleton.ShowTextAsync($"{data.Date}\n{data.Heure}\n{data.Lieu}\n{data.Ville}");
                await Awaitable.WaitForSecondsAsync(2);
                
                await OverlaySingleton.instance.FadeToBlackTransitionAsync(2);
                TransitionSingleton.Hide();
                
                await OverlaySingleton.instance.FadeInTransitionAsync(2);
                GameManager.SwitchState(GameState.Playing);
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private static async Awaitable SwitchTableau(int buildIndexTableau, int buildIndexState)
        {
            IsLoading = true;
            try
            {
                await SceneManager.LoadSceneAsync(buildIndexTableau, LoadSceneMode.Single);
                await SceneManager.LoadSceneAsync(buildIndexState, LoadSceneMode.Additive);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
