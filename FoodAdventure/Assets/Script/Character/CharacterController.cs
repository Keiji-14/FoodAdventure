using UnityEngine;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
        /// <summary>�L�����N�^�[�̌����Ă������</summary>
        public enum CharacterDirection
        {
            Up,
            Down,
            Right,
            Left,
            UpRight,
            UpLeft,
            DownRight,
            DownLeft,
        }
    }
}