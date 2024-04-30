using UnityEngine;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
        /// <summary>キャラクターの向いている方向</summary>
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