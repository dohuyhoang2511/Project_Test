using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    namespace PathFinding
    {
        public enum PathFinderStatus
        {
            NOT_INIIALIZED,
            SUCCESS,
            FAILURE,
            RUNNING,
        }

        abstract public class Node<T>
        {
            public T Value { get; private set; }

            public Node(T value)
            {
                Value = value;
            }

            abstract public List<Node<T>> GetNeighbours();
        }

        abstract public class PathFinder<T>
        {
            #region class PathFinderNode
            public class PathFinderNode
            {
                public PathFinderNode Parent { get; set; }

                public Node<T> Location
                {
                    get;
                    set;
                }

                public float Fcost { get; private set; }
                public float Hcost { get; private set; }
                public float Gcost { get; private set; }

                public PathFinderNode(Node<T> node, PathFinderNode parent, float G, float H)
                {
                    Location = node;
                    Parent = parent;
                    Hcost = H;

                    SetGCost(G);
                }

                public void SetGCost(float g)
                {
                    Gcost = g;
                    Fcost = Hcost + Gcost;
                }
            }
            #endregion

            #region Cost functions related

            public delegate float CostFunction(T a, T b);

            public CostFunction HeuristicCost;
            public CostFunction NodeTraversalCost;
            #endregion

            #region Open and closed list related functionality

            protected List<PathFinderNode> mOpenList = new List<PathFinderNode>();
            protected List<PathFinderNode> mClosedList = new List<PathFinderNode>();

            protected PathFinderNode GetLeastCostNode(List<PathFinderNode> list)
            {
                int best_index = 0;
                float best_cost = list[0].Fcost;

                for (int i = 1; i < list.Count; ++i)
                {
                    if (best_cost > list[i].Fcost)
                    {
                        best_cost = list[i].Fcost;
                        best_index = i;
                    }
                }
                return list[best_index];
            }

            protected int IsInList(List<PathFinderNode> list, T item)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (EqualityComparer<T>.Default.Equals(list[i].Location.Value, item))
                    {
                        return i;
                    }
                }

                return -1;
            }
            #endregion

            #region Delegates to handle state changes to PathFinderNode

            public delegate void DelegatePathFinderNode(PathFinderNode node);
            public DelegatePathFinderNode onChangeCurrentNode;
            public DelegatePathFinderNode onAddToOpenList;
            public DelegatePathFinderNode onAddToClosedList;
            public DelegatePathFinderNode onDestinationFound;
            #endregion

            public Node<T> Start { get; private set; }
            public Node<T> Goal { get; private set; }

            protected PathFinderNode mCurrentNode = null;

            public PathFinderNode CurrentNode
            {
                get
                {
                    return mCurrentNode;
                }
            }

            public void Reset()
            {
                if (Status == PathFinderStatus.RUNNING)
                {
                    return;
                }
                mOpenList.Clear();
                mClosedList.Clear();
                Status = PathFinderStatus.NOT_INIIALIZED;
            }

            public bool Initialize(Node<T> start, Node<T> goal)
            {
                if (Status == PathFinderStatus.RUNNING)
                {
                    return false;
                }

                Reset();

                Start = start;
                Goal = goal;

                float H = HeuristicCost(Start.Value, Goal.Value);

                PathFinderNode root = new PathFinderNode(Start, null, 0.0f, H);

                mOpenList.Add(root);

                mCurrentNode = root;

                onAddToOpenList?.Invoke(mCurrentNode);
                onChangeCurrentNode?.Invoke(mCurrentNode);

                Status = PathFinderStatus.RUNNING;
                return true;
            }

            public PathFinderStatus Step()
            {
                mClosedList.Add(mCurrentNode);

                onAddToClosedList?.Invoke(mCurrentNode);

                if (mOpenList.Count == 0)
                {
                    Status = PathFinderStatus.FAILURE;

                    return Status;
                }

                mCurrentNode = GetLeastCostNode(mOpenList);

                onChangeCurrentNode?.Invoke(mCurrentNode);

                mOpenList.Remove(mCurrentNode);

                if (EqualityComparer<T>.Default.Equals(mCurrentNode.Location.Value, Goal.Value))
                {
                    Status = PathFinderStatus.SUCCESS;
                    onDestinationFound?.Invoke(mCurrentNode);
                    return Status;
                }

                List<Node<T>> neighbours = mCurrentNode.Location.GetNeighbours();

                foreach (Node<T> cell in neighbours)
                {
                    AlgorithmSpecificImplementation(cell);
                }

                Status = PathFinderStatus.RUNNING;
                return Status;
            }

            abstract protected void AlgorithmSpecificImplementation(Node<T> cell);

            public PathFinderStatus Status
            {
                get;
                private set;
            }
        }

        public class AStarPathFinder<T> : PathFinder<T>
        {
            protected override void AlgorithmSpecificImplementation(Node<T> cell)
            {
                if (IsInList(mClosedList, cell.Value) == -1)
                {
                    float G = mCurrentNode.Gcost + NodeTraversalCost(mCurrentNode.Location.Value, cell.Value);

                    float H = HeuristicCost(cell.Value, Goal.Value);

                    int idOlist = IsInList(mOpenList, cell.Value);
                    if (idOlist == -1)
                    {
                        PathFinderNode n = new PathFinderNode(cell, mCurrentNode, G, H);
                        mOpenList.Add(n);

                        onAddToOpenList?.Invoke(n);
                    }
                    else
                    {
                        float oldG = mOpenList[idOlist].Gcost;
                        if (G < oldG)
                        {
                            mOpenList[idOlist].SetGCost(G);

                            mOpenList[idOlist].Parent = mCurrentNode;

                            onAddToOpenList?.Invoke(mOpenList[idOlist]);
                        }
                    }
                }
            }
        }
    }
}
