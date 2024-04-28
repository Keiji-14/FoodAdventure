using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyController : CharacterController
    {
        #region PrivateField
        private int currentPathIndex;
        /// <summary>移動にかかる時間の逆数</summary>
        private float inverseMoveTime;
        private List<Vector3Int> currentPath;
        private Transform playerTransform;
        private Rigidbody2D rb;
        #endregion

        #region SerializeField
        /// <summary>移動にかかる時間</summary>
        [SerializeField] float moveTime;
        /// <summary>障害物レイヤー</summary>
        [SerializeField] LayerMask blockingLayer;

        [SerializeField] AStar aStar;
        #endregion

        #region UnityEvent
        void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            rb = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// 敵の行動処理
        /// </summary>
        private void EnemyMove()
        {
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();

            /*Vector3Int startInt = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
            Vector3Int goalInt = new Vector3Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y), Mathf.RoundToInt(playerTransform.position.z));

            //AttemptMove(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

            // 経路探索を行う
            currentPath = aStar.FindPath(startInt, goalInt, blockingLayer);
            currentPathIndex = 0;

            if (currentPath != null && currentPath.Count > 0 && currentPathIndex < currentPath.Count)
            {
                // 現在のセルから次のセルへの方向を取得
                Vector3Int nextCell = currentPath[currentPathIndex];
                Vector2Int direction = new Vector2Int(nextCell.x - Mathf.RoundToInt(transform.position.x), nextCell.y - Mathf.RoundToInt(transform.position.y));

                // 移動を試みる
                AttemptMove(direction.x, direction.y);
            }*/

            AttemptMove(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }

        /// <summary>
        /// 攻撃・移動を行う処理
        /// </summary>
        private void AttemptMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

            if (hit.transform == null)
            {
                Vector2 playerPos = playerTransform.position;
                Vector2 myPos = transform.position;
                float distanceX = Mathf.Abs(playerPos.x - myPos.x);
                float distanceY = Mathf.Abs(playerPos.y - myPos.y);
                float distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));

                // 斜め方向の場合は2未満、直線方向（上下左右）の場合は1未満をチェックする
                if (distance < 2.0f)
                {
                    // Todo: 攻撃処理を行う（未実装のため、ここに攻撃処理のコードを追加）
                    Debug.Log("プレイヤーが隣接しています。攻撃します。");
                }
                else
                {
                    // 隣接していない場合、移動を試みる
                    StartCoroutine(SmoothMovement(end));
                }
            }
            else if (hit.transform.CompareTag("Wall"))
            {
                // 壁にぶつかった場合、別のルートを探す
                Debug.Log("壁にぶつかりました。別のルートを探します。");
                // TODO: 別のルートを探す処理を追加
            }
        }

        /// <summary>
        /// 敵を滑らかに移動させるコルーチン
        /// </summary>
        private IEnumerator SmoothMovement(Vector3 end)
        {
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb.position, end, inverseMoveTime * Time.deltaTime);
                rb.MovePosition(newPosition);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
        }
        #endregion

        public void ExecuteAction()
        {
            EnemyMove();
        }
    }
}