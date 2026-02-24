using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LiveUML.Models;

namespace LiveUML.Rendering
{
    public class LayoutEngine
    {
        private const int BoxWidth = 220;
        private const int HeaderHeight = 28;
        private const int AttributeLineHeight = 18;
        private const int BasePadding = 30;
        private const int BaseColumnGap = 100;
        private const int BaseRowGap = 60;
        private const int OverlapMargin = 20;

        public DiagramLayout ComputeLayout(
            List<EntityBox> boxes,
            List<RelationshipMetadataModel> selectedRelationships,
            List<EntityMetadataModel> selectedEntities,
            Dictionary<string, Point> manualPositions)
        {
            if (boxes.Count == 0)
                return new DiagramLayout { TotalSize = Size.Empty };

            int n = boxes.Count;

            // Dynamic columns: scale with entity count
            int columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(n)));

            // Scale gaps when many entities
            int columnGap = BaseColumnGap + Math.Max(0, (n - 6) * 5);
            int rowGap = BaseRowGap + Math.Max(0, (n - 6) * 3);

            var nameToIndex = new Dictionary<string, int>();
            for (int i = 0; i < n; i++)
                nameToIndex[boxes[i].EntityLogicalName] = i;

            // Build adjacency from selected relationships
            var adj = BuildAdjacency(n, selectedRelationships, nameToIndex);

            // BFS order starting from most-connected entity
            var bfsOrder = ComputeBfsOrder(n, adj);

            // Compute all box heights upfront
            var boxHeights = new int[n];
            for (int i = 0; i < n; i++)
                boxHeights[i] = ComputeBoxHeight(boxes[i]);

            // Phase 1: place manually positioned boxes
            var placed = new bool[n];
            if (manualPositions != null)
            {
                for (int i = 0; i < n; i++)
                {
                    Point manualPos;
                    if (manualPositions.TryGetValue(boxes[i].EntityLogicalName, out manualPos))
                    {
                        boxes[i].Bounds = new Rectangle(manualPos.X, manualPos.Y, BoxWidth, boxHeights[i]);
                        placed[i] = true;
                    }
                }
            }

            // Phase 2: auto-place remaining boxes in BFS order using columns
            var columnHeights = new int[columns];
            for (int c = 0; c < columns; c++)
                columnHeights[c] = BasePadding;

            var placedCol = new Dictionary<int, int>();

            foreach (int idx in bfsOrder)
            {
                if (placed[idx]) continue;

                int targetCol = FindTargetColumn(idx, adj, placedCol, columnHeights, columns);
                int x = BasePadding + targetCol * (BoxWidth + columnGap);
                int y = columnHeights[targetCol];

                boxes[idx].Bounds = new Rectangle(x, y, BoxWidth, boxHeights[idx]);
                columnHeights[targetCol] = y + boxHeights[idx] + rowGap;
                placedCol[idx] = targetCol;
                placed[idx] = true;
            }

            // Phase 3: resolve any overlaps (manual boxes may collide with auto-placed)
            ResolveOverlaps(boxes);

            // Compute total size
            int maxRight = 0, maxBottom = 0;
            foreach (var box in boxes)
            {
                if (box.Bounds.Right > maxRight) maxRight = box.Bounds.Right;
                if (box.Bounds.Bottom > maxBottom) maxBottom = box.Bounds.Bottom;
            }

            var entityBoxMap = boxes.ToDictionary(b => b.EntityLogicalName);
            var lines = BuildRelationshipLines(selectedRelationships, entityBoxMap);

            return new DiagramLayout
            {
                EntityBoxes = boxes,
                RelationshipLines = lines,
                TotalSize = new Size(maxRight + BasePadding, maxBottom + BasePadding)
            };
        }

        public static void RecomputeLines(DiagramLayout layout)
        {
            var boxMap = layout.EntityBoxes.ToDictionary(b => b.EntityLogicalName);
            foreach (var line in layout.RelationshipLines)
            {
                EntityBox source, target;
                if (boxMap.TryGetValue(line.SourceEntityName, out source) &&
                    boxMap.TryGetValue(line.TargetEntityName, out target))
                {
                    line.StartPoint = GetConnectionPoint(source.Bounds, target.Bounds);
                    line.EndPoint = GetConnectionPoint(target.Bounds, source.Bounds);
                }
            }
        }

        private static List<HashSet<int>> BuildAdjacency(int n, List<RelationshipMetadataModel> relationships, Dictionary<string, int> nameToIndex)
        {
            var adj = new List<HashSet<int>>();
            for (int i = 0; i < n; i++)
                adj.Add(new HashSet<int>());

            foreach (var rel in relationships)
            {
                int a = -1, b = -1;
                if (rel.ReferencedEntity != null) nameToIndex.TryGetValue(rel.ReferencedEntity, out a);
                if (rel.ReferencingEntity != null) nameToIndex.TryGetValue(rel.ReferencingEntity, out b);
                if (a >= 0 && b >= 0 && a != b)
                {
                    adj[a].Add(b);
                    adj[b].Add(a);
                }
            }

            return adj;
        }

        private List<int> ComputeBfsOrder(int n, List<HashSet<int>> adj)
        {
            var visited = new bool[n];
            var order = new List<int>(n);

            while (order.Count < n)
            {
                int start = -1;
                int maxConn = -1;
                for (int i = 0; i < n; i++)
                {
                    if (!visited[i] && adj[i].Count > maxConn)
                    {
                        maxConn = adj[i].Count;
                        start = i;
                    }
                }

                var queue = new Queue<int>();
                queue.Enqueue(start);
                visited[start] = true;

                while (queue.Count > 0)
                {
                    int curr = queue.Dequeue();
                    order.Add(curr);

                    foreach (int neighbor in adj[curr].OrderByDescending(x => adj[x].Count))
                    {
                        if (!visited[neighbor])
                        {
                            visited[neighbor] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return order;
        }

        private int FindTargetColumn(int idx, List<HashSet<int>> adj, Dictionary<int, int> placedCol, int[] columnHeights, int columns)
        {
            var neighborCols = adj[idx]
                .Where(ni => placedCol.ContainsKey(ni))
                .Select(ni => placedCol[ni])
                .ToList();

            if (neighborCols.Count > 0)
            {
                int avgCol = (int)Math.Round(neighborCols.Average());
                avgCol = Math.Max(0, Math.Min(avgCol, columns - 1));

                // Try the target column first, then adjacent ones
                int bestCol = avgCol;
                int bestHeight = columnHeights[avgCol];
                for (int offset = 1; offset <= 1; offset++)
                {
                    if (avgCol - offset >= 0 && columnHeights[avgCol - offset] < bestHeight)
                    {
                        bestCol = avgCol - offset;
                        bestHeight = columnHeights[bestCol];
                    }
                    if (avgCol + offset < columns && columnHeights[avgCol + offset] < bestHeight)
                    {
                        bestCol = avgCol + offset;
                        bestHeight = columnHeights[bestCol];
                    }
                }
                return bestCol;
            }

            // No placed neighbors: pick shortest column
            int best = 0;
            for (int c = 1; c < columns; c++)
                if (columnHeights[c] < columnHeights[best])
                    best = c;
            return best;
        }

        private void ResolveOverlaps(List<EntityBox> boxes)
        {
            // Multiple passes to push overlapping boxes apart
            for (int pass = 0; pass < 10; pass++)
            {
                bool anyOverlap = false;

                for (int i = 0; i < boxes.Count; i++)
                {
                    for (int j = i + 1; j < boxes.Count; j++)
                    {
                        var inflatedA = Rectangle.Inflate(boxes[i].Bounds, OverlapMargin / 2, OverlapMargin / 2);
                        var inflatedB = Rectangle.Inflate(boxes[j].Bounds, OverlapMargin / 2, OverlapMargin / 2);

                        if (!inflatedA.IntersectsWith(inflatedB))
                            continue;

                        anyOverlap = true;

                        // Push box j away from box i
                        int centerAx = boxes[i].Bounds.X + boxes[i].Bounds.Width / 2;
                        int centerAy = boxes[i].Bounds.Y + boxes[i].Bounds.Height / 2;
                        int centerBx = boxes[j].Bounds.X + boxes[j].Bounds.Width / 2;
                        int centerBy = boxes[j].Bounds.Y + boxes[j].Bounds.Height / 2;

                        int dx = centerBx - centerAx;
                        int dy = centerBy - centerAy;

                        // If boxes are on the same spot, push right and down
                        if (dx == 0 && dy == 0)
                        {
                            dx = BoxWidth + OverlapMargin;
                            dy = 0;
                        }

                        // Push in the direction of least overlap
                        int overlapX = (inflatedA.Right - inflatedB.Left);
                        int overlapY = (inflatedA.Bottom - inflatedB.Top);

                        if (dx < 0) overlapX = inflatedB.Right - inflatedA.Left;
                        if (dy < 0) overlapY = inflatedB.Bottom - inflatedA.Top;

                        int pushX = 0, pushY = 0;
                        if (Math.Abs(dx) >= Math.Abs(dy))
                        {
                            pushX = dx > 0 ? overlapX : -overlapX;
                        }
                        else
                        {
                            pushY = dy > 0 ? overlapY : -overlapY;
                        }

                        var bj = boxes[j].Bounds;
                        int newX = Math.Max(BasePadding, bj.X + pushX);
                        int newY = Math.Max(BasePadding, bj.Y + pushY);
                        boxes[j].Bounds = new Rectangle(newX, newY, bj.Width, bj.Height);
                    }
                }

                if (!anyOverlap) break;
            }
        }

        private int ComputeBoxHeight(EntityBox box)
        {
            return HeaderHeight + 10 + Math.Max(box.AttributeLines.Count, 1) * AttributeLineHeight + 10;
        }

        private List<RelationshipLine> BuildRelationshipLines(List<RelationshipMetadataModel> relationships, Dictionary<string, EntityBox> entityBoxMap)
        {
            var lines = new List<RelationshipLine>();
            var drawnPairs = new HashSet<string>();

            foreach (var rel in relationships)
            {
                string entityA, entityB;
                if (rel.Type == RelationshipType.ManyToOne)
                {
                    entityA = rel.ReferencingEntity;
                    entityB = rel.ReferencedEntity;
                }
                else if (rel.Type == RelationshipType.OneToMany)
                {
                    entityA = rel.ReferencedEntity;
                    entityB = rel.ReferencingEntity;
                }
                else
                {
                    entityA = rel.ReferencingEntity;
                    entityB = rel.ReferencedEntity;
                }

                if (entityA == null || entityB == null)
                    continue;
                if (!entityBoxMap.ContainsKey(entityA) || !entityBoxMap.ContainsKey(entityB))
                    continue;

                var pairKey = string.Compare(entityA, entityB, StringComparison.Ordinal) < 0
                    ? entityA + "|" + entityB + "|" + rel.SchemaName
                    : entityB + "|" + entityA + "|" + rel.SchemaName;

                if (drawnPairs.Contains(pairKey))
                    continue;
                drawnPairs.Add(pairKey);

                var boxA = entityBoxMap[entityA];
                var boxB = entityBoxMap[entityB];

                lines.Add(new RelationshipLine
                {
                    StartPoint = GetConnectionPoint(boxA.Bounds, boxB.Bounds),
                    EndPoint = GetConnectionPoint(boxB.Bounds, boxA.Bounds),
                    Label = rel.Type == RelationshipType.ManyToMany ? "N:N" : "1:N",
                    Type = rel.Type,
                    SourceEntityName = entityA,
                    TargetEntityName = entityB
                });
            }

            return lines;
        }

        public static Point GetConnectionPoint(Rectangle fromBox, Rectangle toBox)
        {
            int fromCenterX = fromBox.X + fromBox.Width / 2;
            int fromCenterY = fromBox.Y + fromBox.Height / 2;
            int toCenterX = toBox.X + toBox.Width / 2;
            int toCenterY = toBox.Y + toBox.Height / 2;

            int dx = toCenterX - fromCenterX;
            int dy = toCenterY - fromCenterY;

            if (Math.Abs(dx) * fromBox.Height > Math.Abs(dy) * fromBox.Width)
            {
                return dx > 0
                    ? new Point(fromBox.Right, fromCenterY)
                    : new Point(fromBox.Left, fromCenterY);
            }
            else
            {
                return dy > 0
                    ? new Point(fromCenterX, fromBox.Bottom)
                    : new Point(fromCenterX, fromBox.Top);
            }
        }
    }
}
