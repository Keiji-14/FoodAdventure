using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding
{
    public class AStar : MonoBehaviour
    {
        public Tilemap stageTilemap; // ステージを描画するためのタイルマップ
        public Tilemap obstacleTilemap; // 壁・障害物を描画するタイルマップ

        public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, LayerMask wallLayer)
        {
            // オープンリストとクローズドリストを初期化
            var openList = new List<Vector3Int>();
            var closedList = new HashSet<Vector3Int>();

            // スタート地点をオープンリストに追加
            openList.Add(start);

            // スタート地点から各マスへの移動コストを保持する辞書を初期化
            var gScore = new Dictionary<Vector3Int, float>();
            gScore[start] = 0;

            // スタート地点からゴール地点までの推定コストを保持する辞書を初期化
            var fScore = new Dictionary<Vector3Int, float>();
            fScore[start] = Heuristic(start, goal);

            while (openList.Count > 0)
            {
                // オープンリストから最小コストのマスを取り出す
                Vector3Int current = GetLowestFScore(openList, fScore);

                // ゴールに到達した場合、最短経路を復元して返す
                if (current == goal)
                {
                    return ReconstructPath(goal, start, fScore);
                }

                // オープンリストから取り出したマスをクローズドリストに移動
                openList.Remove(current);
                closedList.Add(current);

                // マスの隣接マスを調べる
                foreach (var neighbor in GetNeighbors(current))
                {
                    // 隣接マスが障害物であるか、クローズドリストに含まれているかを確認
                    if (IsObstacle(neighbor, wallLayer) || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    // 移動コストを計算
                    float tentativeGScore = gScore[current] + 1; // 1は隣接マスへの移動コスト（ここでは単純に1としています）

                    // 隣接マスがオープンリストに含まれていないか、より低いコストで到達できる場合
                    if (!openList.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        // 最短経路が見つかった場合、移動コストと推定コストを更新
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goal);

                        Debug.Log("fScore Dictionary: ");
                        foreach (var pair in fScore)
                        {
                            Debug.Log(pair.Key + ": " + pair.Value);
                        }

                        // 隣接マスをオープンリストに追加
                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            // ゴールに到達できなかった場合は空のリストを返す
            return new List<Vector3Int>();
        }

        // ヒューリスティック関数（推定コスト）を定義します。ここではマンハッタン距離を使用していますが、他の方法も利用可能です。
        private float Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        // 指定したマスの隣接マスを取得します。
        private List<Vector3Int> GetNeighbors(Vector3Int cell)
        {
            var neighbors = new List<Vector3Int>
            {
                cell + Vector3Int.up,
                cell + Vector3Int.down,
                cell + Vector3Int.left,
                cell + Vector3Int.right,
                cell + new Vector3Int(1, 1, 0),     // 右上
                cell + new Vector3Int(1, -1, 0),    // 右下
                cell + new Vector3Int(-1, 1, 0),    // 左上
                cell + new Vector3Int(-1, -1, 0),   // 左下
            };

            return neighbors;
        }

        // 指定したマスが障害物かどうかを判定します。
        private bool IsObstacle(Vector3Int cell, LayerMask wallLayer)
        {
            Vector3 cellWorldPos = stageTilemap.CellToWorld(cell);
            RaycastHit2D hit = Physics2D.Raycast(cellWorldPos, Vector2.zero, Mathf.Infinity, wallLayer);
            return hit.collider != null && hit.collider.gameObject.layer == obstacleTilemap.gameObject.layer;
        }

        // オープンリスト内で最小のfスコアを持つマスを取得します。
        private Vector3Int GetLowestFScore(List<Vector3Int> openList, Dictionary<Vector3Int, float> fScore)
        {
            Vector3Int lowestCell = openList[0];
            float lowestFScore = fScore[lowestCell];

            foreach (var cell in openList)
            {
                //Debug.Log($"fScore[cell]:{fScore[cell]} < lowestFScore{lowestFScore}");
                // キーが存在する場合は、そのキーに対応する値を取得し、最小値を更新
                /*if (fScore[cell] < lowestFScore)
                {
                   lowestCell = cell;
                   lowestFScore = fScore[cell];
                }*/

                // キーが存在する場合は、そのキーに対応する値を取得し、最小値を更新
                if (fScore.ContainsKey(cell))
                {
                    if (fScore[cell] < lowestFScore)
                    {
                        lowestCell = cell;
                        lowestFScore = fScore[cell];
                    }
                }
            }   

            return lowestCell;
        }

        // 最短経路を復元します。
        private List<Vector3Int> ReconstructPath(Vector3Int current, Vector3Int start, Dictionary<Vector3Int, float> fScore)
        {
            var path = new List<Vector3Int>();
            path.Add(current);

            while (current != start)
            {
                current = current + GetLowestFScore(GetNeighbors(current), fScore);
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}