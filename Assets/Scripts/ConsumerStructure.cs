using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerStructure : Structure
{
    public static ulong NumOfConsumers { get; private set; }

    public ulong Id { get; private set; }

    private List<ProducerAccount> _producerAccounts = new List<ProducerAccount>();
    private ObjectPool _transportPool;




    private void OnEnable()
    {
        Id = ++NumOfConsumers;
    }


    private void Start()
    {
        _transportPool = GetComponent<ObjectPool>();
        GenerateProducerAccounts();
        RegisterWithProducer();
    }


    private void OnDisable()
    {
        Id = 0;
        --NumOfConsumers;
    }



    /*****************************custom methods********************************************/
    private void GenerateProducerAccounts()
    {
        ProducerStructure[] producers = FindObjectsOfType<ProducerStructure>();
        AStarPathfinder pathfinder = FindObjectOfType<AStarPathfinder>();

        foreach (ProducerStructure producer in producers)
        {
            Stack<Vector3Int> path = pathfinder.GetPathToCell(this.CellPosition, producer.CellPosition);
            _producerAccounts.Add(new ProducerAccount(producer, path));
        }

        _producerAccounts.Sort();
    }


    private void RegisterWithProducer()
    {
        //currently, we only want to register with the closest customer
        _producerAccounts[0].Producer.RegisterCustomer(this);
        _producerAccounts[0].Producer.OutOfResources += FindNewProducer;
    }


    public bool OrderFilled()
    {
        //we need to let producer know if we are going to pickup

        GameObject transportObj = _transportPool.EnableObjectInPool();
        if (transportObj != null)
        {
            transportObj.GetComponent<Transport>().Dispatch();
            return true;
        }
        return false;
    }

    
    public ProducerAccount GetClosestProducerAccount()
    {
        if (_producerAccounts.Count > 0)
            return _producerAccounts[0];
        else
            return null;
    }


    public void DepositResources(int amountToDeposit)
    {
        ResourceAmount += amountToDeposit;
    }


    private void FindNewProducer()
    {
        Debug.Log($"FindNew: {_producerAccounts.Count}");
        _producerAccounts[0].Producer.OutOfResources -= FindNewProducer;
        _producerAccounts.RemoveAt(0);
        Debug.Log($"FindNew: {_producerAccounts.Count}");

        GenerateProducerAccounts();
        RegisterWithProducer();
    }

    /********************** utility methods ********************************/


}//end of class StructureConsumer
