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
            _ = SwitchTableau(buildIndexTableau, buildIndexState);
        }

        public static void LoadTableau(int buildIndexTableau, int buildIndexState)
        {
            _ = SwitchTableau(buildIndexTableau, buildIndexState);
        }
        
        private static async Awaitable SwitchTableauWithTransition(int buildIndexTableau, int buildIndexState, TransitionSceneData data)
        {
            IsLoading = true;
            try
            {
                //await transi fade to black
                //await SceneManager.LoadSceneAsync(buildIndexTableau, LoadSceneMode.Single);
                //Mettre les bonnes Infos
                //await transi fade to white
                
                //await fini l'affichage
                //await transi fade to black
                await SceneManager.LoadSceneAsync(buildIndexTableau, LoadSceneMode.Single);
                await SceneManager.LoadSceneAsync(buildIndexState, LoadSceneMode.Additive);
                //await transi fade to white
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
