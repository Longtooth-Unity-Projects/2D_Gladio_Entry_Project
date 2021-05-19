using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerStructure : Structure
{
    public static ulong NumOfProducers { get; private set; }
    public ulong Id { get; private set; }

    [SerializeField] private float _produceResourceRate = 1f;

    private int _unallocatdResources = 0;

    private Queue<ConsumerStructure> consumerQ = new Queue<ConsumerStructure>();
    


    protected override void OnEnable()
    {
        base.OnEnable();
        Id = ++NumOfProducers;

        StartCoroutine(ProduceResourceRoutine());
    }


    private void OnDisable()
    {
        Id = 0;
        --NumOfProducers;
    }



    /*****************************custom methods********************************************/
    private IEnumerator ProduceResourceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_produceResourceRate);
            ++ResourceAmount;
            ++_unallocatdResources;

            if(_unallocatdResources > 0)
                NotifyNextConsumer();
        }
    }


    private void NotifyNextConsumer()
    {
        //we only want to go through the queue one time, otherwise if resources are available but there are
        //no transports available, we will be looping until a transport becomes available
        for(int notifications = 0; notifications < consumerQ.Count; ++notifications)
        {
            ConsumerStructure consumer = consumerQ.Dequeue();
            if (consumer.OrderFilled())
            {
                --_unallocatdResources;
                consumerQ.Enqueue(consumer);
                break;
            }
            consumerQ.Enqueue(consumer);
        }
        //TODO if resources are available but no transports, start a periodic check coroutine
    }


    public bool RegisterCustomer(ConsumerStructure consumer)
    {
        if (!consumerQ.Contains(consumer))
            consumerQ.Enqueue(consumer);

        return true;
    }


    public int RequestResources(int requestAmount)
    {
        int availableAmount = requestAmount < ResourceAmount ? requestAmount : ResourceAmount;
        ResourceAmount -= availableAmount;
        return availableAmount;
    }

}
