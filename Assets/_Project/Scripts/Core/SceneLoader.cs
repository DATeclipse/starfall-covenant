using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace StarfallCovenant.Core
{
    public static class SceneLoader
    {
        private static string currentLevelId;

        public static string CurrentLevelId => currentLevelId;

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void LoadBattleWithLevel(string levelId)
        {
            currentLevelId = levelId;
            SceneManager.LoadScene("Battle");
        }

        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
