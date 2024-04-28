using UnityEngine;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
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