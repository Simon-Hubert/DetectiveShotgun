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

        public static void LoadTableau(int buildIndexTableau, int buildIndexState)
        {
            _ = SwitchTableau(buildIndexTableau, buildIndexState);
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
