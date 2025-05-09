

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EdgeGraphExtension
{
    public static int CountGraphNodes(
        this List<EdgeConfig> edges,
        string startSpawn,
        string endSpawn,
        out List<string> graphNodes)
    {
        HashSet<EdgeConfig> visited = new();
        Queue<string> queue = new();
        HashSet<string> addedNodes = new();
        graphNodes = new List<string>();

        int totalNodes = 0;

        queue.Enqueue(startSpawn);
        graphNodes.Add(startSpawn);
        addedNodes.Add(startSpawn);
        totalNodes++;

        while (queue.Count > 0)
        {
            string currentStart = queue.Dequeue();

            // Находим все ребра, которые начинаются с currentStart
            var matchingEdges = edges
                .Where(e => e.Way.Count > 0 && e.Way[0] == currentStart && !visited.Contains(e))
                .ToList();

            if (matchingEdges.Count == 0)
                continue;

            // Если одно направление — просто идём дальше
            if (matchingEdges.Count == 1)
            {
                var edge = matchingEdges[0];
                visited.Add(edge);

                if (edge.Way.Count >= 2)
                {
                    string next = edge.Way[1];
                    if (addedNodes.Add(next))
                    {
                        graphNodes.Add(next);
                        totalNodes++;
                    }

                    if (edge.Way[^1] != endSpawn)
                        queue.Enqueue(edge.Way[^1]);
                }
            }
            else // есть разветвление
            {
                // Один раз добавляем узел ветвления
                if (addedNodes.Add(currentStart))
                {
                    graphNodes.Add(currentStart);
                    totalNodes++;
                }

                foreach (var edge in matchingEdges)
                {
                    visited.Add(edge);

                    if (edge.Way.Count >= 2)
                    {
                        string next = edge.Way[1];
                        if (addedNodes.Add(next))
                        {
                            graphNodes.Add(next);
                            totalNodes++;
                        }

                        if (edge.Way[^1] != endSpawn)
                            queue.Enqueue(edge.Way[^1]);
                    }
                }
            }
        }

        // В конце обязательно добавляем EndSpawn
        if (addedNodes.Add(endSpawn))
        {
            graphNodes.Add(endSpawn);
            totalNodes++;
        }

        return totalNodes;
    }
    
        public static List<List<int>> BuildDetailedAdjacencyFromGraphNodes(
        List<EdgeConfig> edges,
        List<string> graphNodes)
    {
        Dictionary<string, int> nameToIndex = new();
        for (int i = 0; i < graphNodes.Count; i++)
            nameToIndex[graphNodes[i]] = i;

        List<List<int>> adjacency = new();
        for (int i = 0; i < graphNodes.Count; i++)
            adjacency.Add(new List<int>());

        for (int i = 0; i < graphNodes.Count; i++)
        {
            string current = graphNodes[i];

            foreach (var edge in edges)
            {
                if (edge.Way.Count < 2)
                    continue;

                // Обработка разветвлений: если текущий нод совпадает с началом Edge и есть дубликаты
                if (HandleBranching(edge, current, nameToIndex, i, adjacency))
                    continue;

                // --- Обычная логика: текущий узел внутри пути ---

                if (edge.Way.Contains(current))
                {
                    // Последний элемент Edge.Way (если в графе)
                    string last = edge.Way[^1];
                    if (last != current && nameToIndex.TryGetValue(last, out int lastIndex))
                    {
                        AddUndirected(adjacency, i, lastIndex);
                    }

                    // Добавление связей с соседями по Way
                    for (int j = 0; j < edge.Way.Count - 1; j++)
                    {
                        if (edge.Way[j] == current && nameToIndex.TryGetValue(edge.Way[j + 1], out int nextIndex))
                        {
                            AddUndirected(adjacency, i, nextIndex);
                        }

                        if (edge.Way[j + 1] == current && nameToIndex.TryGetValue(edge.Way[j], out int prevIndex))
                        {
                            AddUndirected(adjacency, i, prevIndex);
                        }
                    }
                }
            }
        }

        return adjacency;
    }
        
    public static void FixMissingBranchLinks(
        List<List<int>> adjacency,
        List<string> graphNodes,
        List<EdgeConfig> edges)
    {
        List<int> branchedIndexes = new();
        List<int> branchedCountByIndex = new();
        for (int i = 0; i < graphNodes.Count-1; i++)
        {
            string current = graphNodes[i];
            foreach (var edge in edges)
            {
                if (edge.Way.Contains(current))
                {
                    if (IsBranched(edge, edges))
                    {
                        branchedIndexes.Add(i);
                    }
                }
            }
        }

        for (int i = 0; i < branchedIndexes.Count; i++)
        {
            int branchCount = EdgeGraphExtension.BranchCalculate(graphNodes[branchedIndexes[i]],edges);
            branchedCountByIndex.Add(branchCount);
        }

        List<int> branchTargets = new();
        for (int i = 0; i < branchedIndexes.Count; i++)
        {
            
            int startIndex = branchedIndexes[i];
            int branchCount = branchedCountByIndex[i];

            for (int j = 1; j <= branchCount; j++)
            {
                int targetIndex = startIndex + j;

                // Защита от выхода за границы массива
                if (targetIndex < graphNodes.Count)
                    branchTargets.Add(targetIndex);
            }
        }

        branchTargets = branchTargets.Distinct().ToList();
        for (int i = 0; i < branchTargets .Count; i++)
        {
            var neighbours = adjacency[branchTargets[i]];
            
            Debug.Log($"Elements after branch: "+graphNodes[branchTargets[i]]);
            Debug.Log($"Index: {branchTargets[i]}");
        }

    }
        
    private static bool HandleBranching(
        EdgeConfig edge,
        string current,
        Dictionary<string, int> nameToIndex,
        int currentIndex,
        List<List<int>> adjacency)
    {
        if (edge.Way[0] != current)
            return false;

        // ищем другие Edge, начинающиеся с той же точки (разветвление)
        bool branched = edge.NeighbourEdges
            .Any(n => n != null && n.Way.Count > 1 && n.Way[0] == current);

        if (!branched)
            return false;

        string next = edge.Way[1];
        if (!nameToIndex.TryGetValue(next, out int nextIndex))
            return true;

        AddUndirected(adjacency, currentIndex, nextIndex);
        return true;
    }

    private static void AddUndirected(List<List<int>> adjacency, int a, int b)
    {
        if (!adjacency[a].Contains(b))
            adjacency[a].Add(b);

        if (!adjacency[b].Contains(a))
            adjacency[b].Add(a);
    }
    
    private static bool IsBranched(EdgeConfig target, List<EdgeConfig> allEdges)
    {
        string start = target.Way[0];

        // Если существует другой EdgeConfig с таким же стартом, но не тот же самый
        return allEdges.Any(e =>
            e != target &&
            e.Way.Count > 0 &&
            e.Way[0] == start);
    }
    
    public static int BranchCalculate(string nodeName, List<EdgeConfig> edges)
    {
        int count = 0;

        foreach (var edge in edges)
        {
            if (edge.Way.Count > 0 && edge.Way[0] == nodeName)
            {
                count++;
            }
        }

        return count;
    }

}
