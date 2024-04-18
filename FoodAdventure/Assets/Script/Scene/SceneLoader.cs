using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    /// <summary>
    /// シーン読み込みの処理
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        #region PrivateField
        private static SceneLoader instance = null;

        private Dictionary<SceneName, string> SceneNames = new Dictionary<SceneName, string>()
        {
            {SceneName.Title,         "Title"},
            {SceneName.Reversi,       "Reversi"},
        };
        #endregion

        #region PublicMethod

        public enum SceneName
        {
            Title,
            Reversi,
            GameOver
        }

        /// <summary>
        /// インスタンス化
        /// </summary>
        /// <returns></returns>
        public static SceneLoader Instance()
        {
            // オブジェクトを生成し、自身をAddCompleteして、DontDestroyに置く
            if (instance == null)
            {
                var obj = new GameObject("SceneLoader");
                DontDestroyOnLoad(obj);
                instance = obj.AddComponent<SceneLoader>();
            }

            return instance;
        }

        /// <summary>
        /// シーンロード
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="isAdditive">シーン追加するかどうか</param>
        public void Load(SceneName sceneName, bool isAdditive = false)
        {
            StartCoroutine(LoadAsync(SceneNames[sceneName], isAdditive));
        }
        #endregion


        #region PrivateMethod
        /// <summary>
        /// 非同期シーンロード
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="isAdditive">シーン追加するかどうか</param>
        private IEnumerator LoadAsync(string sceneName, bool isAdditive = false)
        {
            // シーンを非同期でロードする
            var loadMode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var async = SceneManager.LoadSceneAsync(sceneName, loadMode);

            // ロードが完了するまで待機する
            while (!async.isDone)
            {
                yield return null;
            }

            // シーンが追加された後に、アクティブ状態を変更する
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
        #endregion
    }
}