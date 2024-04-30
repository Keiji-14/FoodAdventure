using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        public int mapWidth = 50;
        public int mapHeight = 50;

        public TileBase groundTile; // 地形用のタイル
        public TileBase wallTile;   // 壁や障害物用のタイル
        public Tilemap groundTilemap; // 地形用のTileMap
        public Tilemap wallTilemap;   // 壁や障害物用のTileMap

        public int minRoomSize; // 部屋の最小サイズ
        public int maxRoomSize; // 部屋の最大サイズ
        public int maxRooms; // 最大部屋数
        private List<RectInt> rooms = new List<RectInt>(); // 生成された部屋のリスト
        private List<Vector2Int> corridors = new List<Vector2Int>(); // 生成された通路のリスト

        /// <summary>
        /// マップを生成する
        /// </summary>
        public void GenerateDungeon()
        {
            InitializeMap(); // マップの初期化
            GenerateRooms(); // 部屋の生成
            ConnectRooms(); // 部屋を通路で接続
            PlaceWalls(); // 壁の配置
        }

        // マップの初期化
        private void InitializeMap()
        {
            groundTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            rooms.Clear();
            corridors.Clear();
        }

        // 部屋の生成
        private void GenerateRooms()
        {
            for (int i = 0; i < maxRooms; i++)
            {
                int roomWidth = Random.Range(minRoomSize, maxRoomSize);
                int roomHeight = Random.Range(minRoomSize, maxRoomSize);
                int x = Random.Range(1, mapWidth - roomWidth - 1);
                int y = Random.Range(1, mapHeight - roomHeight - 1);

                RectInt newRoom = new RectInt(x, y, roomWidth, roomHeight);

                bool roomIntersects = false;
                foreach (RectInt room in rooms)
                {
                    if (newRoom.Overlaps(room))
                    {
                        roomIntersects = true;
                        break;
                    }
                }

                if (!roomIntersects)
                {
                    rooms.Add(newRoom);
                }
            }
        }

        // 部屋を通路で接続する
        private void ConnectRooms()
        {
            // 各部屋に対して、他の部屋との距離を計算し、距離の近い順に通路を作成する
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    RectInt roomA = rooms[i];
                    RectInt roomB = rooms[j];
                    Vector2Int startPoint = GetRandomPointInRoom(roomA);
                    Vector2Int endPoint = GetRandomPointInRoom(roomB);

                    List<Vector2Int> corridorPath = DrawCorridor(startPoint, endPoint);
                    if (corridorPath != null)
                    {
                        corridors.AddRange(corridorPath);
                        break;
                    }
                }
            }
        }

        // 部屋内のランダムな座標を取得する
        private Vector2Int GetRandomPointInRoom(RectInt room)
        {
            int x = Random.Range(room.x, room.xMax);
            int y = Random.Range(room.y, room.yMax);
            return new Vector2Int(x, y);
        }

        // 通路を描画する
        private List<Vector2Int> DrawCorridor(Vector2Int startPoint, Vector2Int endPoint)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            Vector2Int direction = endPoint - startPoint;
            int stepCount = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            direction /= stepCount;

            Vector2Int currentPosition = startPoint;
            for (int i = 0; i <= stepCount; i++)
            {
                path.Add(currentPosition);
                if (corridors.Contains(currentPosition))
                {
                    return null; // 既に通路がある場合は接続しない
                }
                currentPosition += direction;
            }

            return path;
        }

        // 壁の配置
        private void PlaceWalls()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (!IsRoomOrCorridor(new Vector2Int(x, y)))
                    {
                        // 部屋や通路と重ならない場所に壁を配置する
                        wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                    }
                }
            }
        }

        // // 部屋や通路かどうかを判定
        private bool IsRoomOrCorridor(Vector2Int position)
        {
            foreach (RectInt room in rooms)
            {
                if (room.Contains(position))
                {
                    return true;
                }
            }

            foreach (Vector2Int corridor in corridors)
            {
                if (corridor.x == position.x && corridor.y == position.y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}