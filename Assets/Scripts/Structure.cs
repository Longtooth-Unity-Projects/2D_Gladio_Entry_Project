using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Structure : MonoBehaviour
{
    [SerializeField] private int _foundationRadius = 0;
    public int FoundationRadius { get => _foundationRadius; private set => _foundationRadius = value; }

    public Vector3Int CellPosition { get; set; }

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



    //********************* methods **********************************
    protected virtual void Awake()
    {
        resourceDisplay = GetComponentInChildren<TextMeshPro>();
        resourceDisplay.text = $"{_resourceAmount}";
    }



    protected virtual void Update()
    {
        ToggleLabels();
    }





    /************************* custom methods *******************************/
    private void ToggleLabels()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            resourceDisplay.enabled = !resourceDisplay.IsActive();
        }
    }
}//end of class Structure
