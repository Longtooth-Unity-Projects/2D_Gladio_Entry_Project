using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;



public class BoardManager : MonoBehaviour
{
    //settings
    [SerializeField] private int _rows = 256;
    [SerializeField] private int _columns = 256;
    [SerializeField] private int _bordersize = 1;
    [SerializeField] private int _numberOfProducers = 2;
    [SerializeField] private int _numberOfConsumers = 5;

    public int Rows { get { return _rows; }  }
    public int Columns { get { return _columns; } }

    private Dictionary<TileBase, ScriptableTileWrapper> dataFromTilesDict;

    //cached references
    [SerializeField] private Tile _grassTile;
    [SerializeField] private Tile _roadTile;
    [SerializeField] private Tile _foundationTile;

    [SerializeField] private ProducerStructure _producerPrefab;
    [SerializeField] private ConsumerStructure _consumerPrefab;
    private ProducerStructure[] _producerArray;
    private ConsumerStructure[] _consumerArray;

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private List<ScriptableTileWrapper> _scriptableTileDataList;

    private AStarPathfinder pathfinder;
    private List<Vector3Int> availableTilesList;

    //debugging variables
    public Color pathColor;




    //********************* methods **********************************
    private void Awake()
    {
        BuildTiledataDictionary();

        availableTilesList = new List<Vector3Int>(_rows * _columns);
        _producerArray = new ProducerStructure[_numberOfProducers];
        _consumerArray = new ConsumerStructure[_numberOfConsumers];

        pathfinder = FindObjectOfType<AStarPathfinder>();

        SetupRandomField();
        BuildRoads();
    }


    
    /**************************** CUSTOM METHODS *********************************/
    private void BuildTiledataDictionary()
    {
        //associate tiles with scriptable data, using tile as key for lookup
        dataFromTilesDict = new Dictionary<TileBase, ScriptableTileWrapper>();

        foreach (ScriptableTileWrapper tileData in _scriptableTileDataList)
            foreach (TileBase tileBase in tileData.tilesWithTheseTraits)
                dataFromTilesDict.Add(tileBase, tileData);
    }



    private void SetupRandomField()
    {
        int borderMin = _bordersize - 1;
        int rowMaxValue = _rows - _bordersize;
        int colMaxValue = _columns - _bordersize;

        //lay base tiles
        for (int x = 0; x < _columns; x++)
            for (int y = 0; y < _rows; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                _tilemap.SetTile(tilePos, _grassTile);

                if (x > borderMin && x < colMaxValue && y > borderMin && y < rowMaxValue)
                    availableTilesList.Add(tilePos);
            }

        
        //set up producers
        for (int i = 0; i < _numberOfProducers; ++i)
        {
            Vector3Int spawnCell = availableTilesList[Random.Range(0, availableTilesList.Count - 1)];
            _producerArray[i] = SpawnStructure(_producerPrefab, spawnCell) as ProducerStructure;
        }

        //set up consumers
        for (int i = 0; i < _numberOfConsumers; ++i)
        {
            Vector3Int spawnCell = availableTilesList[Random.Range(0, availableTilesList.Count - 1)];
            _consumerArray[i] = SpawnStructure(_consumerPrefab, spawnCell) as ConsumerStructure;
        }
    }



    public Structure SpawnStructure(Structure structureToSpawn, Vector3 spawnPosition)
    {
        //we want a foundation of walkable tiles dictated by the structures foundation radius
        Vector3Int foundationCenterCell = _tilemap.WorldToCell(spawnPosition);
        int radius = structureToSpawn.FoundationRadius;

        if(radius == 0)
            _tilemap.SetTile(foundationCenterCell, _foundationTile);
        else
        {
            for(int x = foundationCenterCell.x - radius; x <= foundationCenterCell.x + radius; ++x)
                for(int y = foundationCenterCell.y - radius; y <= foundationCenterCell.y + radius; ++y)
                {
                    Vector3Int tileCell = new Vector3Int(x, y, foundationCenterCell.z);
                    _tilemap.SetTile(tileCell, _foundationTile);
                    availableTilesList.Remove(tileCell);
                }
        }

        availableTilesList.Remove(foundationCenterCell);

        Structure structure = Instantiate(structureToSpawn);
        structure.transform.position = spawnPosition;
        structure.CellPosition = _tilemap.WorldToCell(spawnPosition);

        return structure;
    }



    public Structure SpawnStructure(Structure structureToSpawn, Vector3Int spawnPosition)
    {
        return SpawnStructure(structureToSpawn, _tilemap.GetCellCenterWorld(spawnPosition));
    }



    private void BuildRoads()
    {
        foreach (ConsumerStructure consumer in _consumerArray)
            foreach (ProducerStructure producer in _producerArray)
            {
                //we want to lay all road in the x direction and then the y direction for asthetic reasons
                int xDirection = producer.CellPosition.x < consumer.CellPosition.x ? -1 : 1;
                int yDirection = producer.CellPosition.y < consumer.CellPosition.y ? -1 : 1;
                Vector3Int currentPos = consumer.CellPosition;

                while (currentPos.x != producer.CellPosition.x)
                {
                    currentPos.x += xDirection;
                    if (_tilemap.GetTile(currentPos).name != _foundationTile.name)
                        _tilemap.SetTile(currentPos, _roadTile);

                }
                while (currentPos.y != producer.CellPosition.y)
                {
                    currentPos.y += yDirection;
                    if (_tilemap.GetTile(currentPos).name != _foundationTile.name)
                        _tilemap.SetTile(currentPos, _roadTile);
                }
            }
    }





    /************************** UTILITY METHODS **************************************************/
    public Vector3 GetCenterGridWorldPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int gridPosition = _tilemap.WorldToCell(mousePosition);
        Vector3 worldCenterPos = _tilemap.GetCellCenterWorld(gridPosition);
        return worldCenterPos;
    }



    public ScriptableTileWrapper GetTileData(Vector3Int tilePosition)
    {
        TileBase tile = _tilemap.GetTile(tilePosition);

        if (tile == null)
            return null;
        else
            return dataFromTilesDict[tile];
    }



    public float GetTileMovementSpeed(Vector2 worldPosition)
    {
        Vector3Int gridPosition = _tilemap.WorldToCell(worldPosition);
        TileBase tileAtPos = _tilemap.GetTile(gridPosition);

        if (tileAtPos == null)
            return 1f;

        return dataFromTilesDict[tileAtPos].movementMultiplier;
    }



    public bool GetTileIsWalkable(Vector3Int gridPosition)
    {
        TileBase tileAtPos = _tilemap.GetTile(gridPosition);

        if (tileAtPos == null)
            return false;

        return dataFromTilesDict[tileAtPos].isWalkable;
    }








    //*********************Debugging Code***************************

    private void DebugInits()
    {
        _producerArray = FindObjectsOfType<ProducerStructure>();
        _consumerArray = FindObjectsOfType<ConsumerStructure>();
    }



    private void DebugPath()
    {
        foreach (ConsumerStructure consumer in _consumerArray)
            foreach (ProducerStructure producer in _producerArray)
            {
                Stack<Vector3Int> path = pathfinder.GetPathToCell(consumer.CellPosition, producer.CellPosition);

                if (path != null)
                    foreach (Vector3Int cellPosition in path)
                    {
                        _tilemap.SetTileFlags(cellPosition, TileFlags.None);
                        _tilemap.SetColor(cellPosition, pathColor);
                        _tilemap.SetTileFlags(cellPosition, TileFlags.LockColor);
                    }
            }
    }



    private void DebugInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridPosition = _tilemap.WorldToCell(mousePosition);
            Vector3 worldCenterPos = _tilemap.GetCellCenterWorld(gridPosition);
            Vector3 localCenterPos = _tilemap.GetCellCenterLocal(gridPosition);

            TileBase clickedTile = _tilemap.GetTile(gridPosition);
            if (clickedTile == null) return;

            //this is how we access data for specific tile types
            float speedHinderance = dataFromTilesDict[clickedTile].movementMultiplier;

            Debug.Log($"mousePos: {mousePosition} worldCenterPos: {worldCenterPos} localCenterPos: {localCenterPos} ");
            Debug.Log($"Grid postion {gridPosition} Tile: {clickedTile} SpeedHinderance: {speedHinderance}");

        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Vector3 spawnPosition = GetCenterGridWorldPosition();
            SpawnStructure(_consumerPrefab, spawnPosition);
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Vector3 spawnPosition = GetCenterGridWorldPosition();
            SpawnStructure(_producerPrefab, spawnPosition);
        }

    }//end of method DebugInput

}//end of class BoardManager
