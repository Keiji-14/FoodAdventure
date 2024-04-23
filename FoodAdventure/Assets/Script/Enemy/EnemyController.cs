using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        #region PrivateField
        /// <summary>�ړ��ɂ����鎞�Ԃ̋t��</summary>
        private float inverseMoveTime;
        private Transform playerTransform;
        private Rigidbody2D rb;
        #endregion

        #region SerializeField
        /// <summary>�ړ��ɂ����鎞��</summary>
        [SerializeField] float moveTime;
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
        private void MoveTowardsPlayer()
        {
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();

            AttemptMove(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }

        private void AttemptMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            RaycastHit2D hit = Physics2D.Linecast(start, end);

            if (hit.transform == null)
            {
                StartCoroutine(SmoothMovement(end));
            }
        }

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
            MoveTowardsPlayer();
            // �����ɓG�̍s���̃R�[�h��ǉ�����
            Debug.Log("�G���v���C���[�Ɍ������čU�����܂��I");
        }
    }
}