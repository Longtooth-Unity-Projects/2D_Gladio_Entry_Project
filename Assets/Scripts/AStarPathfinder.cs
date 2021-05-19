using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class AStarPathfinder : MonoBehaviour
{
    private readonly int _orthogonalMoveCost = 10;
    private readonly int _diagonalMoveCost = 14;

    private Vector3Int _startPos;
    private Vector3Int _goalPos;
    private AStarNode _currentNode;

    private HashSet<AStarNode> _openList;
    private HashSet<AStarNode> _closedList;
    private Stack<Vector3Int> _path ;

    private Dictionary<Vector3Int, AStarNode> _allNodes = new Dictionary<Vector3Int, AStarNode>();


    //cached references
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private BoardManager _boardManager;






    /*****************************custom methods********************************************/

    /// <summary>
    /// Takes two gridcell locations and plots the shortest path between them
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="goalPos"></param>
    /// <returns>a Vector3Int Stack of gridcell coordinates</returns>
    public Stack<Vector3Int> GetPathToCell(Vector3Int startPos, Vector3Int goalPos)
    {
        
        Reset();

        _startPos = startPos;
        _goalPos = goalPos;

        SearchAlgorithm();

        return _path;
    }



    private void SearchAlgorithm()
    {
        //only want to initialize the first time we run
        if (_currentNode == null)
            Initialize();

        while ((_openList.Count > 0) && (_currentNode.CellPosition != _goalPos))
        {
            List<AStarNode> neighbors = FindOrthogonalNeighbors(_currentNode.CellPosition);
            ExamineNeighbors(neighbors, _currentNode);

            UpdateCurrentTile(ref _currentNode);
        }//end of while

        if (_currentNode.CellPosition == _goalPos)
            GeneratePath();
    }//end of function Algorithm()


    private void Initialize()
    {
        _currentNode = GetNode(_startPos);
        _openList = new HashSet<AStarNode>();
        _closedList = new HashSet<AStarNode>();
        _path = new Stack<Vector3Int>();
        _openList.Add(_currentNode);
    }


    /// <summary>
    /// finds orthogonal neighbors only and prevents
    /// startNode, unwalkable, and null tiles from being added
    /// </summary>
    /// <param name="parentPosition"></param>
    /// <returns></returns>
    private List<AStarNode> FindOrthogonalNeighbors(Vector3Int parentPosition)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        for (int x = -1; x <= 1; ++x)
            for (int y = -1; y <= 1; ++y)
            {
                Vector3Int neighborPos = new Vector3Int(parentPosition.x - x, parentPosition.y - y, parentPosition.z);

                //we want to only include orthogonal neighbors
                int xDif = parentPosition.x - neighborPos.x;
                int yDif = parentPosition.y - neighborPos.y;

                if (Math.Abs(xDif - yDif) % 2 == 1)
                    if (neighborPos != _startPos && _boardManager.GetIsWalkable(neighborPos) && _tilemap.GetTile(neighborPos))
                    {
                        AStarNode neighbor = GetNode(neighborPos);
                        neighbors.Add(neighbor);
                    }
            }

        return neighbors;
    }// end of FindNeighbors()


    /// <summary>
    /// Examines neighbors, adds new ones to open list and calculates their values
    /// Neighbors not on list are checked to see if path through current to them 
    /// is shorter than current path from start to them
    /// </summary>
    /// <param name="neighbors"></param>
    /// <param name="current"></param>
    private void ExamineNeighbors(List<AStarNode> neighbors, AStarNode current)
    {
        for(int i = 0; i < neighbors.Count; ++i)
        {
            AStarNode neighbor = neighbors[i];

            int gScore = DetermineGScore(neighbors[i].CellPosition, current.CellPosition);

            if (_openList.Contains(neighbor))
            {
                if(current.G + gScore < neighbor.G)
                    CalcValues(neighbor, current, gScore);
            }
            else if (!_closedList.Contains(neighbor))
            {
                CalcValues(neighbor, current, gScore);
                _openList.Add(neighbor);
            }
        }
    }//end of ExamineNeighbors


    private void CalcValues(AStarNode neighbor, AStarNode current, int costToGetHereFromParent)
    {
        neighbor.Parent = current;
        neighbor.G = current.G + costToGetHereFromParent;

        //Heuristic Manhatan: calculate vertical and horizontal nodes between between neighbor and goal and multiply by orthogonal movement cost
        neighbor.H = (Math.Abs((neighbor.CellPosition.x - _goalPos.x)) + Math.Abs((neighbor.CellPosition.y - _goalPos.y))) * _orthogonalMoveCost;

        neighbor.F = neighbor.G + neighbor.H;
    }


    //TODO separate the removing of the current and the picking of the new
    //adds current to closed list and gets lowest F value node on openList
    private void UpdateCurrentTile(ref AStarNode current)
    {
        _openList.Remove(current);
        _closedList.Add(current);
        //TODO use different structore for this, min heap preferably and have AStarNode Implement IComparable
        if (_openList.Count > 0)
        {
            current = _openList.OrderBy(x => x.F).First();
        }
    }



    /// <summary>
    /// Determine if the neighbor is orthogonal or diagonal and
    /// assign appropriate gScore
    /// </summary>
    /// <param name="neighbor"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        int gScore;

        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;

        if(Math.Abs(x-y) % 2 == 1)
            gScore = _orthogonalMoveCost;
        else
            gScore = _diagonalMoveCost;

        return gScore;
    }



    /// <summary>
    /// Generates path of cellPositions
    /// Does not include the starting node in the path
    /// </summary>
    private void GeneratePath()
    {
        while (_currentNode.CellPosition != _startPos)
        {
            _path.Push(_currentNode.CellPosition);
            _currentNode = _currentNode.Parent;
        }
    }

    /*************************Utility Methods****************************************/
    private AStarNode GetNode(Vector3Int position)
    {
        if (_allNodes.ContainsKey(position))
            return _allNodes[position];
        else
        {
            //only use nodes involved in pathfinding
            AStarNode node = new AStarNode(position);
            _allNodes.Add(position, node);
            return node;
        }
    }


    private void Reset()
    {
        _startPos.Set(0,0,0);
        _goalPos.Set(0, 0, 0);
        _allNodes.Clear();
        _path = null;
        _currentNode = null;
    }
}//end of class AStar
