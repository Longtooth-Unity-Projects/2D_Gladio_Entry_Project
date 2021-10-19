using System;
using System.Collections.Generic;
using UnityEngine;

public class ProducerAccount : IComparable
{
    public ProducerStructure Producer { get; private set; }
    public Stack<Vector3Int> PathToProducer { get; private set; }
    public int Distance { get; private set; }


    //********** constructors **************
    public ProducerAccount(ProducerStructure producer, Stack<Vector3Int> path)
    {
        Producer = producer;
        PathToProducer = path;
        Distance = path.Count;
    }



    //********************* IComparable implmentation **********************************
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        ProducerAccount otherAccount = obj as ProducerAccount;
        if (otherAccount != null)
            return this.Distance.CompareTo(otherAccount.Distance);
        else
            throw new ArgumentException("Object is not a Producer Account");
    }
}
