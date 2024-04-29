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

        const int wall = 9;
        const int road = 0;

        const int roomMinHeight = 5;
        const int roomMaxHeight = 10;

        const int roomMinWidth = 5;
        const int roomMaxWidth = 10;

        const int RoomCountMin = 10;
        const int RoomCountMax = 15;

        //public int minRoomSize = 3; // 部屋の最小サイズ
        //public int maxRoomSize = 10; // 部屋の最大サイズ
        //public int maxRooms = 10; // 最大部屋数
        //private List<RectInt> rooms = new List<RectInt>(); // 生成された部屋のリスト

        /// <summary>
        /// マップを生成する
        /// </summary>
        public void GenerateDungeon()
        {
            // マップの初期化
            InitializeMap();

            CreateSpaceData();
            CreateDungeon();
            // BSP手法によるダンジョン生成
            //SplitSpace(new RectInt(0, 0, mapWidth, mapHeight));
        }

        // マップの初期化
        private void InitializeMap()
        {
            groundTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            //rooms.Clear();
        }

        private void CreateSpaceData()
        {
            int[,] map = new int[mapHeight, mapWidth];
            int roomCount = Random.Range(RoomCountMin, RoomCountMax);

            for (int i = 0; i < roomCount; i++)
            {
                int roomHeight = Random.Range(roomMinHeight, roomMaxHeight);
                int roomWidth = Random.Range(roomMinWidth, roomMaxWidth);
                int roomPointX = Random.Range(2, mapWidth - roomMaxWidth - 2);
                int roomPointY = Random.Range(2, mapWidth - roomMaxWidth - 2);

                CreateRoomData(roomHeight, roomWidth, roomPointX, roomPointY, map);
            }
        }

        private void CreateRoomData(int roomHeight, int roomWidth, int roomPointX, int roomPointY, int[,] map)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                for (int j = 0; j < roomWidth; j++)
                {
                    map[roomPointY + i, roomPointX + j] = road;
                }
            }
        }

        private void CreateDungeon()
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    if (groundTilemap.GetTile(tilePosition) == null)
                    {
                        wallTilemap.SetTile(tilePosition, wallTile);
                    }
                    else
                    {
                        groundTilemap.SetTile(tilePosition, groundTile);
                    }
                }
            }
        }

        // BSP手法による空間分割
        /*private void SplitSpace(RectInt space)
        {
            // 再帰停止条件
            if (rooms.Count >= maxRooms || space.width <= maxRoomSize * 2 || space.height <= maxRoomSize * 2)
            {
                return;
            }

            // 空間を水平または垂直に分割
            bool splitHorizontally = Random.value < 0.5f;
            int splitSize = splitHorizontally ? space.height : space.width;

            // 分割線の位置を決定
            int minSplit = Mathf.RoundToInt(splitSize * 0.3f);
            int maxSplit = Mathf.RoundToInt(splitSize * 0.7f);
            int splitPosition = Random.Range(minSplit, maxSplit);

            // 空間を分割
            RectInt leftSpace, rightSpace;
            if (splitHorizontally)
            {
                leftSpace = new RectInt(space.x, space.y, space.width, splitPosition);
                rightSpace = new RectInt(space.x, space.y + splitPosition, space.width, space.height - splitPosition);
            }
            else
            {
                leftSpace = new RectInt(space.x, space.y, splitPosition, space.height);
                rightSpace = new RectInt(space.x + splitPosition, space.y, space.width - splitPosition, space.height);
            }

            // 分割した空間に部屋を配置
            CreateRoom(leftSpace);
            CreateRoom(rightSpace);

            // 部屋同士を通路で結ぶ
            Vector2Int leftRoomCenter = GetRoomCenter(leftSpace);
            Vector2Int rightRoomCenter = GetRoomCenter(rightSpace);
            CreateCorridor(leftRoomCenter, rightRoomCenter);

            // 再帰的に空間を分割
            SplitSpace(leftSpace);
            SplitSpace(rightSpace);
        }

        // 部屋の生成
        private void CreateRoom(RectInt roomSpace)
        {
            // 部屋のサイズをランダムに決定
            int roomWidth = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, roomSpace.width - 2));
            int roomHeight = Random.Range(minRoomSize, Mathf.Min(maxRoomSize, roomSpace.height - 2));

            // 部屋の位置をランダムに決定
            int roomX = Random.Range(roomSpace.x + 1, roomSpace.xMax - roomWidth - 1);
            int roomY = Random.Range(roomSpace.y + 1, roomSpace.yMax - roomHeight - 1);

            // 部屋の矩形を作成してリストに追加
            RectInt newRoomRect = new RectInt(roomX, roomY, roomWidth, roomHeight);

            // 既存の部屋との衝突判定
            bool roomOverlaps = false;
            foreach (RectInt existingRoom in rooms)
            {
                if (newRoomRect.Overlaps(existingRoom))
                {
                    roomOverlaps = true;
                    break;
                }
            }

            // 衝突がなければ部屋を生成
            if (!roomOverlaps)
            {
                rooms.Add(newRoomRect);

                // 部屋のタイルを敷き詰める
                for (int x = roomX; x < roomX + roomWidth; x++)
                {
                    for (int y = roomY; y < roomY + roomHeight; y++)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                    }
                }

                // 部屋の周りに壁を敷き詰める
                for (int x = roomX - 1; x <= roomX + roomWidth; x++)
                {
                    for (int y = roomY - 1; y <= roomY + roomHeight; y++)
                    {
                        if (x == roomX - 1 || x == roomX + roomWidth || y == roomY - 1 || y == roomY + roomHeight)
                        {
                            wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("部屋が重なっているため、再配置を行います。");
                // 重なっている場合は部屋の再配置を行うなどの処理を実装
            }
        }

        private void CreateCorridor(Vector2Int start, Vector2Int end)
        {
            int deltaX = Mathf.Abs(end.x - start.x);
            int deltaY = Mathf.Abs(end.y - start.y);
            int signX = start.x < end.x ? 1 : -1;
            int signY = start.y < end.y ? 1 : -1;

            int error = deltaX - deltaY;
            int currentX = start.x;
            int currentY = start.y;

            while (currentX != end.x || currentY != end.y)
            {
                // タイルを配置する処理
                groundTilemap.SetTile(new Vector3Int(currentX, currentY, 0), groundTile);

                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    currentX += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    currentY += signY;
                }
            }
        }

        // 部屋の中心座標を取得する
        private Vector2Int GetRoomCenter(RectInt room)
        {
            int centerX = room.x + room.width / 2;
            int centerY = room.y + room.height / 2;
            return new Vector2Int(centerX, centerY);
        }
    }*/


        /*
         public void GenerateMap()
            {
                // Clear existing tiles
                groundTilemap.ClearAllTiles();
                wallTilemap.ClearAllTiles();

                // Generate rooms and corridors
                for (int i = 0; i < maxAttempts; i++)
                {
                    int roomWidth = Random.Range(minRoomSize, maxRoomSize);
                    int roomHeight = Random.Range(minRoomSize, maxRoomSize);
                    int posX = Random.Range(1, mapWidth - roomWidth - 1);
                    int posY = Random.Range(1, mapHeight - roomHeight - 1);

                    RectInt newRoom = new RectInt(posX, posY, roomWidth, roomHeight);

                    bool roomOverlaps = false;
                    foreach (RectInt room in rooms)
                    {
                        if (newRoom.Overlaps(room))
                        {
                            roomOverlaps = true;
                            break;
                        }
                    }

                    if (!roomOverlaps)
                    {
                        CreateRoom(newRoom);
                        if (rooms.Count != 0)
                        {
                            Vector2Int prevRoomCenter = GetRoomCenter(rooms[rooms.Count - 1]);
                            Vector2Int newRoomCenter = GetRoomCenter(newRoom);

                            CreateCorridor(prevRoomCenter, newRoomCenter);
                        }
                        rooms.Add(newRoom);
                    }

                    if (rooms.Count >= maxRooms)
                    {
                        break;
                    }
                }
            }*/

        /// <summary>
        /// 部屋を生成する
        /// </summary>
        /// <param name="room">生成する部屋の矩形領域</param>
        /*private void CreateRoom(RectInt room)
        {
            for (int x = room.x; x < room.xMax; x++)
            {
                for (int y = room.y; y < room.yMax; y++)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }

            // 部屋の周囲に壁を配置する
            for (int x = room.x - 1; x <= room.xMax; x++)
            {
                for (int y = room.y - 1; y <= room.yMax; y++)
                {
                    if (x == room.x - 1 || x == room.xMax || y == room.y - 1 || y == room.yMax)
                    {
                        wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                    }
                }
            }
        }*/
    }
}
        /// <summary>
        /// 通路を生成する
        /// </summary>
        /// <param name="start">通路の始点</param>
        /// <param name="end">通路の終点</param>
        /*private void CreateCorridor(Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).normalized; // ベクトルを正規化する
            Vector2Int roundedDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

            Vector2 currentPosition = start;

            // 座標の差が非常に小さい（ほぼゼロ）場合にループを終了する
            while (currentPosition.x != end.x || currentPosition.y != end.y)
            {
                groundTilemap.SetTile(new Vector3Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y), 0), groundTile);

                currentPosition += direction;
            }
        }

        /// <summary>
        /// 部屋の中心座標を取得する
        /// </summary>
        /// <param name="room">対象の部屋の矩形領域</param>
        /// <returns>部屋の中心座標</returns>
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        }
    }*/