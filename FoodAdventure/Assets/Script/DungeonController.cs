using Character.Player;
using Character.Enemy;
using UnityEngine;

namespace Dungeon
{
    public class DungeonController : MonoBehaviour
    {
        #region PrivateField
        private EnemyController enemyController;
        #endregion

        #region SerializeField
        /// <summary>プレイヤーの処理</summary>
        [SerializeField] PlayerController playerController;
        #endregion

        #region PublicMethod
        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            enemyController = FindObjectOfType<EnemyController>();

            if (playerController != null)
            {
                playerController.PlayerMoved += OnPlayerMoved;
            }
        }
        #endregion

        /// <summary>
        /// プレイヤーの移動が完了した時に呼び出されるメソッド
        /// </summary>
        private void OnPlayerMoved()
        {
            // プレイヤーの移動が完了したので、ターンを切り替える処理を行う
            SwitchTurn();
        }

        /// <summary>
        /// ターンを切り替える処理
        /// </summary>
        private void SwitchTurn()
        {
            // 敵の行動を実行する
            enemyController.ExecuteAction();
            // Todo: ここにターンを切り替える処理を実装する
            Debug.Log("プレイヤーのターンが終了しました。敵のターンを開始します。");
        }
    }
}