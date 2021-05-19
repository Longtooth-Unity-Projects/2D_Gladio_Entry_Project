using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, Input_Actions.IPlayerActions
{
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _zoomIncrement = 0.5f;
    [SerializeField] private float _minCameraSize = 5.0f;
    [SerializeField] private float _maxCameraSize = 60.0f;
    [SerializeField] private float _widthToHeightRatio = 1.78f;

    private float boardRows;
    private float boardColumns;


    //used for movement
    private float minX = 0f;
    private float minY = 0f;
    private float maxY = 0f;
    private float maxX = 0f;
    private float deltaX = 0f;
    private float deltaY = 0f;
    private bool _isMoving = false;
    private Vector3 _newPosition = new Vector3(0,0,0);

    //cached references
    private Input_Actions _controls;
    private Camera _myCamera;


    private void Awake()
    {
        if (_controls == null)
            _controls = new Input_Actions();
        _controls.Player.SetCallbacks(this);

        _myCamera = Camera.main;
    }


    private void Start()
    {
        BoardManager boardManager = FindObjectOfType<BoardManager>();

        boardRows = (float)boardManager.Rows;
        boardColumns = (float)boardManager.Columns;

        SetInitialCam();
    }


    private void OnEnable()
    {
        _controls.Enable();
    }


    private void Update()
    {
        if (_isMoving)
            Move();
    }


    private void OnDisable()
    {
        _controls.Disable();
    }



    /******************* custom methods **********************/
    private void SetInitialCam()
    {
        
        float halfRowHeight = boardRows / 2f;

        _myCamera.orthographicSize = (halfRowHeight < _maxCameraSize) ? halfRowHeight : _maxCameraSize;

        transform.position = new Vector3((float) boardColumns / 2f, (float) boardRows / 2f, transform.position.z);

        SetBoundaries(_myCamera.orthographicSize);
    }


    private void SetBoundaries(float currentHeight)
    {
        minY = 0 + currentHeight;
        maxY = boardRows - currentHeight;
        minX = 0 + _widthToHeightRatio * currentHeight;
        maxX = boardColumns - _widthToHeightRatio * currentHeight ;
    }

    private void Move()
    {

        Vector3 direction = _controls.Player.Move.ReadValue<Vector2>();

        //we want the camera to move faster the more zoomed out we are
        deltaX = direction.x * Time.deltaTime * _movementSpeed * _myCamera.orthographicSize;
        deltaY = direction.y * Time.deltaTime * _movementSpeed * _myCamera.orthographicSize;

        _newPosition.x = Mathf.Clamp(transform.position.x + deltaX, minX, maxX);
        _newPosition.y = Mathf.Clamp(transform.position.y + deltaY, minY, maxY);
        _newPosition.z = transform.position.z;

        transform.position = _newPosition;
    }

    /************************ Input Methods ************************/
    public void OnMove(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                _isMoving = true;
                break;
            case InputActionPhase.Performed:
                _isMoving = true;
                break;
            case InputActionPhase.Canceled:
                _isMoving = false;
                break;
            default:
                break;
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollValueY = context.ReadValue<Vector2>().y;

            if (scrollValueY < 0.0f && _myCamera.orthographicSize <= _minCameraSize)
                return;
            if (scrollValueY > 0.0f && _myCamera.orthographicSize >= _maxCameraSize)
                return;

            _myCamera.orthographicSize += _zoomIncrement * scrollValueY;
            SetBoundaries(_myCamera.orthographicSize);
        }
    }
}//end of class CameraController
