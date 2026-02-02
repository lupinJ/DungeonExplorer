using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace WallOfDefence.Utility
{
    public enum MapState
    {
        None,
        Wall
    }

    public struct Point : System.IEquatable<Point>
    {
        public int x, y;

        public Point(int x, int y) { this.x = x; this.y = y; }

        public bool Equals(Point other) => x == other.x && y == other.y;
        public override int GetHashCode() => (x, y).GetHashCode();
    }

    public class APathfinder
    {
        // 맵 정보
        private MapState[,] map;
        private int rows;
        private int cols;

        // 상하좌우 대각선 이동 (8방향)
        private readonly int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
        private readonly int[] dy = { 1, -1, 0, 0, 1, -1, -1, 1 };
        private readonly int[] cost = { 2, 2, 2, 2, 3, 3, 3, 3 };

        // 휴라스틱 node
        private class ANode
        {
            public Point pos;
            public int g;
            public int h;
            public int f; // f = g + h
            public Point parent;

            public ANode(Point pos, int g, int h, Point parent)
            {
                this.pos = pos;
                this.g = g;
                this.h = h;
                this.f = g + h;
                this.parent = parent;
            }
        }

        // 맵 초기화
        public void InitializeMap(MapState[,] map)
        {
            this.map = map;
            rows = map.GetLength(0);
            cols = map.GetLength(1);
        }

        // ------------------- A* 탐색 메인 함수 -------------------
        public List<Point> FindPath(Point start, Point end)
        {
            if (map[start.y, start.x] == MapState.Wall || map[end.y, end.x] == MapState.Wall)
            {
                Debug.LogError("시작점 또는 목표점에 장애물이 있습니다.");
                return new List<Point>();
            }

            // Open List: Min Heap 기반의 우선순위 큐 사용
            PriorityQueue<ANode, int> openList = new PriorityQueue<ANode, int>();

            // allNodes (Closed List 역할 겸): 이미 방문했거나 탐색 중인 노드의 최신 정보를 저장
            Dictionary<Point, ANode> allNodes = new Dictionary<Point, ANode>();

            // 시작 노드 생성 및 큐에 추가
            ANode startNode = new ANode(start, 0, ManhattanDistence(start, end), new Point(-1, -1));
            openList.Enqueue(startNode, startNode.f);
            allNodes.Add(start, startNode);

            while (openList.Count > 0)
            {
                // F값이 가장 낮은 노드를 추출
                ANode current = openList.Dequeue();

                // 목표 도달 시 return
                if (current.pos.Equals(end))
                {
                    return ReconstructPath(allNodes, end);
                }

                // 이전에 Open List에 들어간 노드가 Dictionary에 저장된 G 값보다 더 크다면(더 안 좋은 경로라면) 무시합니다.
                if (current.g > allNodes[current.pos].g) continue;

                // 8방향 이웃 노드 탐색
                for (int i = 0; i < 8; i++)
                {
                    Point nextPos = new Point(current.pos.x + dx[i], current.pos.y + dy[i]);

                    // 맵 경계/장애물 체크
                    if (IsRange(nextPos)) continue;
                    if (map[nextPos.y, nextPos.x] == MapState.Wall) continue;
                    if (i > 3) // 대각선일 경우
                    {
                        Point left = new Point(current.pos.x + dx[i % 4], current.pos.y + dy[i % 4]);
                        Point right = new Point(current.pos.x + dx[(i + 1) % 4], current.pos.y + dy[(i + 1) % 4]);
                        if(IsRange(left) || IsRange(right)) continue;
                        if (map[left.y, left.x] == MapState.Wall || map[right.y, right.x] == MapState.Wall)
                            continue;
                    }
                    int newG = current.g + cost[i]; // 이동 비용 1

                    // 이웃 노드가 Dictionary에 저장되어 있는지 확인
                    if (allNodes.TryGetValue(nextPos, out ANode existingNode))
                    {
                        // 더 좋은 경로 발견
                        if (newG < existingNode.g)
                        {
                            // 기존 노드 정보 업데이트
                            existingNode.g = newG;
                            existingNode.f = newG + existingNode.h;
                            existingNode.parent = current.pos;

                            // 업데이트된 노드를 큐에 다시 추가. (이전에 큐에 있던 오래된 노드는 무시됨)
                            openList.Enqueue(existingNode, existingNode.f);
                        }
                    }
                    else
                    {
                        // 새로운 노드 발견
                        int h = ManhattanDistence(nextPos, end);
                        ANode newNode = new ANode(nextPos, newG, h, current.pos);

                        openList.Enqueue(newNode, newNode.f);
                        allNodes.Add(nextPos, newNode);
                    }
                }
            }

            // 경로를 찾지 못한 경우
            return new List<Point>();
        }

        public bool TryFindPath(Point start, Point end, out List<Point> path)
        {
            path = FindPath(start, end);
            if (path.Count == 0)
                return false;
            return true;
        }


        // ------------------- 경로 역추적 함수 -------------------
        private List<Point> ReconstructPath(Dictionary<Point, ANode> allNodes, Point end)
        {
            List<Point> path = new List<Point>();
            Point current = end;

            // 부모가 (-1, -1)인 시작점까지 역추적
            while (allNodes.ContainsKey(current) && (current.x != -1 || current.y != -1))
            {
                path.Add(current);
                current = allNodes[current].parent;
            }

            path.Reverse();
            return path;
        }

        // 휴리스틱 함수 (맨해튼 거리) 
        private int ManhattanDistence(Point current, Point goal)
        {
            return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
        }

        // map범위를 넘어가는지 체크
        private bool IsRange(Point pos)
        {
            return pos.x < 0 || pos.x >= cols || pos.y < 0 || pos.y >= rows;
        }
    }
}

