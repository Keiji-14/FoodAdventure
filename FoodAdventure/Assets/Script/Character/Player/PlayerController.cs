using System;
using System.Collections;
using UnityEngine;

namespace Character.Player
{
    /// <summary>プレイヤーの操作状態</summary>
    public enum OperationStatus
    {
        DefaultMove,
        DiagonalMove,
        ChangingDirection,
    }

    public class PlayerController : CharacterController
    {    
        #region PublicField
        /// <summary>プレイヤーの移動完了を知らせるイベント</summary>
        public event Action PlayerMoved;
        #endregion

        #region PrivateField
        /// <summary>移動にかかる時間の逆数</summary>
        private float inverseMoveTime;
        /// <summary>プレイヤーが移動中かどうか</summary>
        private bool isMoving = false;
        /// <summary>Shiftキーが押されているかどうか</summary>
        private bool isShiftPressed = false;
        private Rigidbody2D rb;
        /// <summary>操作状態</summary>
        private OperationStatus operationStatus;
        /// <summary>向いている方向</summary>
        private CharacterDirection characterDirection;
        #endregion

        #region SerializeField
        /// <summary>移動にかかる時間</summary>
        [SerializeField] float moveTime;
        /// <summary>障害物レイヤー</summary>
        [SerializeField] LayerMask blockingLayer;
        #endregion

        #region UnityEvent
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            inverseMoveTime = 1f / moveTime;

            // 初期配置の方向は下方向にする
            characterDirection = CharacterDirection.Down;
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
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                operationStatus = OperationStatus.DiagonalMove;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                operationStatus = OperationStatus.ChangingDirection;
            }
            else
            {
                operationStatus = OperationStatus.DefaultMove;
            }

            switch (operationStatus)
            {
                case OperationStatus.DefaultMove:
                    UpDownLeftRightMove();
                    break;
                case OperationStatus.DiagonalMove:
                    DiagonalMove();
                    break;
                case OperationStatus.ChangingDirection:
                    ChangingDirection();
                    break;
            }
        }

        /// <summary>
        /// 斜め移動を行う処理
        /// </summary>
        private void DiagonalMove()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.UpRight;
                AttemptMove(1, 1);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.UpLeft;
                AttemptMove(-1, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.DownRight;
                AttemptMove(1, -1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.DownLeft;
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
                characterDirection = CharacterDirection.Up;
                AttemptMove(0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                characterDirection = CharacterDirection.Down;
                AttemptMove(0, -1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.Right;
                AttemptMove(1, 0);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.Left;
                AttemptMove(-1, 0);
            }   
        }


        /// <summary>
        /// 方向転換を行う処理
        /// </summary>
        private void ChangingDirection()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                characterDirection = CharacterDirection.Up;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                characterDirection = CharacterDirection.Down;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.Right;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.Left;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.UpRight;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.UpLeft;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                characterDirection = CharacterDirection.DownRight;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                characterDirection = CharacterDirection.DownLeft;
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

        /// <summary>
        /// 移動完了時に呼び出される処理
        /// </summary>
        private void OnMoveComplete()
        {
            PlayerMoved?.Invoke();
        }

        /// <summary>
        /// プレイヤーを滑らかに移動させるコルーチン
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

            OnMoveComplete();
            // 移動が完了したらフラグをリセット
            isMoving = false;
        }
        #endregion
    }
}