using UnityEngine;

namespace Dungeon
{
    public class Room : MonoBehaviour
    {
        public int width = 10;
        public int height = 10;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
        }
    }
}