using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.InputSystem;

public class Structure : MonoBehaviour
{
    [SerializeField] private int _foundationRadius = 0;
    public int FoundationRadius { get => _foundationRadius; private set => _foundationRadius = value; }

    public Vector3Int CellPosition { get; private set; }

    private TextMeshPro resourceDisplay;


    private int _resourceAmount = 0;
    public int ResourceAmount
    {
        get { return _resourceAmount; }
        protected set
        {
            _resourceAmount = value;
            resourceDisplay.text = $"{_resourceAmount}";
        }
    }



    protected virtual void Awake()
    {
        resourceDisplay = GetComponentInChildren<TextMeshPro>();
        resourceDisplay.text = $"{_resourceAmount}";
    }

    protected virtual void OnEnable()
    {
        CellPosition = FindObjectOfType<Tilemap>().WorldToCell(transform.position);
    }


    protected virtual void Update()
    {
        ToggleLabels();
    }


    private void ToggleLabels()
    {
        //TODO change this to an input action map
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            resourceDisplay.enabled = !resourceDisplay.IsActive();
        }
    }
}
