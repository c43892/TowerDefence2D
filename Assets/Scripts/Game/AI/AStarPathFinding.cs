using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    // A* pathfinding
    public static class AStarPathFinding
    {
        public class Node
        {
            public int x;
            public int y;
        }

        public static List<Node> FindPath(int xSrc, int ySrc, int xDst, int yDst, Func<int, int, bool> walkable)
        {
            var src = new Node {x = xSrc, y = ySrc};
            var dst = new Node {x = xDst, y = yDst};
            var closed = new HashSet<Node>();
            var open = new HashSet<Node> {src};
            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, float>
            {
                [src] = 0
            };
            var fScore = new Dictionary<Node, float>
            {
                [src] = HeuristicCostEstimate(src, dst)
            };

            while (open.Any())
            {
                var current = open.OrderBy(node => fScore[node]).First();
                if (current.x == xDst && current.y == yDst)
                    return ReconstructPath(cameFrom, dst);

                open.Remove(current);
                closed.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!walkable(neighbor.x, neighbor.y) || closed.Contains(neighbor))
                        continue;

                    var tentativeGScore = gScore[current] + 1;
                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                    else if (tentativeGScore >= gScore[neighbor])
                        continue;

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, dst);
                }
            }

            return null;
        }

        static List<Node> GetNeighbors(Node node)
        {
            return new List<Node>
            {
                new Node {x = node.x - 1, y = node.y},
                new Node {x = node.x + 1, y = node.y},
                new Node {x = node.x, y = node.y - 1},
                new Node {x = node.x, y = node.y + 1},
            };
        }

        static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            if (!cameFrom.ContainsKey(current))
                return new List<Node> {current};

            var path = ReconstructPath(cameFrom, cameFrom[current]);
            path.Add(current);

            return path;
        }

        static float HeuristicCostEstimate(Node src, Node dst)
        {
            return Math.Abs(src.x - dst.x) + Math.Abs(src.y - dst.y);
        }
    }
}
