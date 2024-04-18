using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        public int dungeonWidth = 50;
        public int dungeonHeight = 50;
        public int numRooms = 10;
        public GameObject roomPrefab;
        public GameObject corridorPrefab;

        private List<Room> rooms = new List<Room>();
        private List<GameObject> corridors = new List<GameObject>();

        void Start()
        {
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            GenerateRooms();
            GenerateCorridors();
        }

        void GenerateRooms()
        {
            for (int i = 0; i < numRooms; i++)
            {
                Vector2 position = new Vector2(Random.Range(0, dungeonWidth), Random.Range(0, dungeonHeight));
                GameObject roomObject = Instantiate(roomPrefab, position, Quaternion.identity) as GameObject;
                Room room = roomObject.GetComponent<Room>();
                rooms.Add(room);
            }
        }

        void GenerateCorridors()
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Vector2 startPos = rooms[i].transform.position;
                Vector2 endPos = rooms[i + 1].transform.position;
                GameObject corridorObject = Instantiate(corridorPrefab, startPos, Quaternion.identity) as GameObject;
                // Connect rooms with corridors (implement your own logic here)
                corridors.Add(corridorObject);
            }
        }
    }
}