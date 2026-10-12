using UnityEngine;
using UnityEngine.SceneManagement;

namespace DShotgun
{
    public static class TableauManager
    {
        public static void LoadTableau(int buildIndexTableau, int buildIndexState)
        {
            _ = SwitchTableau(buildIndexTableau, buildIndexState);
        }

        private static async Awaitable SwitchTableau(int buildIndexTableau, int buildIndexState)
        {
            await SceneManager.LoadSceneAsync(buildIndexTableau, LoadSceneMode.Single);
            await SceneManager.LoadSceneAsync(buildIndexState, LoadSceneMode.Additive);
        }
    }
}
