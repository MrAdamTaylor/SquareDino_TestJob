using System.Collections.Generic;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.GameControl
{
    public class GraphCreator
    {
        private readonly GameTaskCreator _gameTaskCreator;
        private GraphWrapper _graph;

        private string _startName;
        private string _endName;

        public GraphCreator(GameTaskCreator gameTaskCreator, Transform startPoint, Transform endPoint)
        {
            _gameTaskCreator = gameTaskCreator;
            _startName = startPoint.name;
            _endName = endPoint.name;
        }

        [Inject]
        public void Construct(List<EdgeConfig> edges)
        {
            int graphSize = edges.CountGraphNodes(_startName, _endName, out List<string> edgeNames);
            //Debug.Log($"Graph size: {graphSize}");
            for (int i = 0; i < graphSize; i++)
            {
                Debug.Log($"Edge Name: {edgeNames[i]}");
            }

            List<List<int>> adjacency = EdgeGraphExtension.BuildDetailedAdjacencyFromGraphNodes(edges, edgeNames);
            
            _graph = new GraphWrapper(edges, edgeNames, adjacency);

            for (int i = 0; i < edgeNames.Count; i++)
            {
                _graph.GetNodeNameAndEdges(i);
            }
            //EdgeGraphExtension.FixMissingBranchLinks(adjacency, edgeNames, edges);
            /**/
            
            /*for (int count = 0; count < adjacency.Count; count++)
            {
                for (int i = 0; i < adjacency[count].Count; i++)
                {
                    Debug.Log($"<color=magenta>Edge Neighbour:  Edge Index {count} and neighbour index {adjacency[count][i]}</color>");
                }
            }*/
        }

        public class GraphWrapper
        {

            private GraphInt _graph;
            private Dictionary<int, string> _indexToName { get; set; }
            private Dictionary<string, int> _nameToIndex { get; set; }

            public GraphWrapper(List<EdgeConfig> edges, List<string> graphNodes, List<List<int>> adjacency)
            {
                _indexToName = new Dictionary<int, string>();
                _nameToIndex = new Dictionary<string, int>();

                for (int i = 0; i < graphNodes.Count; i++)
                {
                    _indexToName[i] = graphNodes[i];
                    _nameToIndex[graphNodes[i]] = i;
                }

                _graph = new GraphInt(graphNodes.Count);
                for (int i = 0; i < adjacency.Count; i++)
                {
                    _graph.AddEdge(i, adjacency[i]);
                }
            }
            // Восстановим связи на основе EdgeConfig.Way
            /*foreach (var edge in edges)
            {
                var way = edge.Way;
                for (int i = 0; i < way.Count - 1; i++)
                {
                    if (_nameToIndex.TryGetValue(way[i], out int from) &&
                        _nameToIndex.TryGetValue(way[i + 1], out int to))
                    {
                        _graph.addEdge(from, to);
                    }
                }
            }*/

            

            public string GetNodeName(int index)
            {
                return _indexToName.TryGetValue(index, out var name) ? name : null;
            }

            public void GetNodeNameAndEdges(int index)
            {
                Debug.Log($"Name Graph By Index" + _indexToName[index] + " and Edges");
                for (int i = 0; i < _graph.ReturnEdgesByIndex(index).Count; i++)
                {
                    var neighbour = _graph.ReturnEdgesByIndex(index)[i];
                    Debug.Log($"Neighbor {i}: {neighbour}");
                }
            }

            public int GetNodeIndex(string name)
            {
                return _nameToIndex.TryGetValue(name, out var index) ? index : -1;
            }
        }


        public class GraphInt
        {
            private int V;
            private List<int>[] l;

            public GraphInt(int v)
            {
                V = v;
                l = new List<int>[v];
            }

            public List<int> ReturnEdgesByIndex(int index)
            {
                if (l[index] == null)
                {
                    l[index] = new List<int>();
                }

                return l[index];
            }

            public void addEdge(int i, int j, bool undir = true)
            {
                if (l[i] != null)
                {
                    l[i].Add(j);
                }
                else
                {
                    l[i] = new List<int>();
                    l[i].Add(j);
                }

                if (undir)
                {
                    if (l[j] != null)
                        l[j].Add(i);
                    else
                    {
                        l[j] = new List<int>();
                        l[j].Add(i);
                    }
                }
            }

            public void AddEdge(int i, List<int> value, bool undir = true)
            {
                if (l[i] == null)
                {
                    l[i] = value;
                }
            }

            public void PrintAdjList()
            {
                for (int i = 0; i < V; i++)
                {
                    Debug.Log(i + "Gavnokod-->");
                    for (int j = 0; j < l[i].Count; j++)
                    {
                        Debug.Log("Gavnokod:" + l[i][j]);
                    }
                }
            }
        }
    }
}