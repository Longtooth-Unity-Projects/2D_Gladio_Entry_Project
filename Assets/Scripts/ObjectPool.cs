using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _objectPrefab;

    [Range(0, 500)]
    [SerializeField] private int _poolSize = 5;
    private GameObject[] _objectPool;





    private void Awake()
    {
        PopulatePool();
    }



    /************** custom methods *************************/
    private void PopulatePool()
    {
        _objectPool = new GameObject[_poolSize];
        for (int i = 0; i < _objectPool.Length; ++i)
        {
            _objectPool[i] = Instantiate(_objectPrefab, transform);
            _objectPool[i].SetActive(false);
        }
    }


    public int NumOfObjectsAvailable()
    {
        int numAvailable = 0;

        foreach (GameObject obj in _objectPool)
            if (obj.activeInHierarchy)
                numAvailable++;

        return numAvailable;
    }


    public GameObject EnableObjectInPool()
    {
        for (int i = 0; i < _objectPool.Length; i++)
            if (_objectPool[i].activeInHierarchy == false)
            {
                _objectPool[i].SetActive(true);
                return _objectPool[i];
            }
        return null;
    }



}//end of class object pool
