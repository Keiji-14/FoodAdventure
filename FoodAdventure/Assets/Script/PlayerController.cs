using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        #region PrivateField
        /// <summary>移動にかかる時間の逆数</summary>
        private float inverseMoveTime;
        private Rigidbody2D rb;
        /// <summary>移動にかかる時間の逆数</summary>
        private bool isMoving = false; // プレイヤーが移動中かどうかを示すフラグ
        #endregion

        #region SerializeField
        /// <summary>プレイヤーの移動にかかる時間</summary>
        [SerializeField] float moveTime;
        /// <summary>障害物レイヤー</summary>
        [SerializeField] LayerMask blockingLayer;
        #endregion

        #region PublicMethod
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;
        }

        void Update()
        {
            // プレイヤーが移動中でない場合のみ入力を受け付ける
            if (!isMoving)
            {
                PlayerMove();   
            }
        }
        #endregion

        #region PrivateMethod
        /// <summary>
        /// プレイヤーの移動処理
        /// </summary>
        private void PlayerMove()
        {
            // Shiftキーが押されているかどうかを確認
            bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (isShiftPressed)
            {
                DiagonalMove();
            }
            else
            {
                UpDownLeftRightMove();
            }
        }

        /// <summary>
        /// 斜め移動を行う処理
        /// </summary>
        private void DiagonalMove()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                AttemptMove(1, 1);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                AttemptMove(-1, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                AttemptMove(1, -1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                AttemptMove(-1, -1);
            }
        }

        /// <summary>
        /// 上下左右移動を行う処理
        /// </summary>
        private void UpDownLeftRightMove()
        {
            // 上下左右の移動を試みる
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AttemptMove(0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AttemptMove(0, -1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AttemptMove(-1, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AttemptMove(1, 0);
            }
        }

        /// <summary>
        /// 移動を行う処理
        /// </summary>
        private void AttemptMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            // 移動先の座標を整数値に丸める
            end = new Vector2(Mathf.Round(end.x), Mathf.Round(end.y));

            // Raycastで移動先に障害物があるか確認
            RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

            if (hit.transform == null) // 障害物がない場合
            {
                // 移動中フラグを設定
                isMoving = true;

                // 移動中に他の移動を受け付けないようにする
                StartCoroutine(SmoothMovement(end));
            }
        }

        // プレイヤーを滑らかに移動させるコルーチン
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

            // 移動が完了したらフラグをリセット
            isMoving = false;
        }
        #endregion
    }
}