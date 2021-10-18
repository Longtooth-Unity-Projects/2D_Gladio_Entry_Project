using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Transport : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _turnRate = 1f;
    [SerializeField] private int _resourceCapacity = 1;
    private int _resourceAmount = 0;

    private Vector3Int _goal;

    private List<Vector3Int> _path = new List<Vector3Int>();

    //cached references
    private ConsumerStructure _homeBase;
    private ProducerAccount _producerAccount;
    private Tilemap _tileMap;
    private AStarPathfinder _pathfinder;

    private void Awake()
    {
        _homeBase = GetComponentInParent<ConsumerStructure>();
        _tileMap = FindObjectOfType<Tilemap>();
        _pathfinder = FindObjectOfType<AStarPathfinder>();
    }

    private void OnEnable()
    {
        TeleportToHomeBase();
        _goal = _homeBase.CellPosition;
    }


/******************* custome methods ************************/
    public void Dispatch()
    {
        StartCoroutine(TraverseRoute());
    }



    /// <summary>
    /// Transport will move from homebase to closest producer, pickup resources
    /// and then return home to drop off resources
    /// </summary>
    /// <returns></returns>
    private IEnumerator TraverseRoute()
    {
        _producerAccount = _homeBase.GetClosestProducerAccount();
        _producerAccount.Producer.OutOfResources += AbortMission;

        _goal = _producerAccount.Producer.CellPosition;
        CalculatePath(_goal);
        yield return StartCoroutine(FollowPathRoutine(PickupResources));
        _producerAccount.Producer.OutOfResources -= AbortMission;

        _goal = _homeBase.CellPosition;
        CalculatePath(_goal);
        yield return StartCoroutine(FollowPathRoutine(DeliverResources));
    }



    /// <summary>
    /// Follows current path and performs an action when it reaches its destination
    /// </summary>
    /// <param name="finishAction">This is the method you wish to call when the transport
    /// reaches its goal</param>
    /// <returns></returns>
    private IEnumerator FollowPathRoutine(Action finishAction)
    {
        for (int index = 0; index < _path.Count; ++index)
        {
            //TODO update later to have truck move forward from position for smoother transitions
            //we want to travel to center of cell
            Vector3 nextPos = _tileMap.GetCellCenterWorld(_path[index]);

            RotateToDestination(nextPos);

            while (transform.position != nextPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, _movementSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        
        //make sure we only complete our finish action if we reached our goal
        if(_tileMap.WorldToCell(transform.position) == _goal)
            finishAction();

    }//end of function FollowPathRoutine



    private void CalculatePath(Vector3Int destinationCell)
    {
        _path.Clear();
        _path.AddRange(_pathfinder.GetPathToCell(_tileMap.WorldToCell(transform.position), destinationCell));
    }



    private void PickupResources()
    {
        _resourceAmount += _producerAccount.Producer.RequestResources(_resourceCapacity);
    }



    private void DeliverResources()
    {
        _homeBase.DepositResources(_resourceAmount);
        _resourceAmount = 0;
        gameObject.SetActive(false);
    }



    private void AbortMission()
    {
        StopAllCoroutines();

        _goal = _homeBase.CellPosition;
        CalculatePath(_goal);
        StartCoroutine(FollowPathRoutine(DeliverResources));
    }



    /********************** utility methods  ******************************/
    private void TeleportToHomeBase()
    {
        transform.position = _tileMap.GetCellCenterWorld(_homeBase.CellPosition);
    }



    private void RotateToDestination(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}//end of class transport
