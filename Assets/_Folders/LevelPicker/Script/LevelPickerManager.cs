using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;

public class LevelPickerManager : MonoBehaviour
{
    private InputController controls;
    [SerializeField] private PathCreator[] paths;
    [SerializeField] private PlayerLevelPickerPathFollwer playerPathFollower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new InputController();


        controls.LevelCreator.CursorClick.performed += ctx =>
        {


            HandleClickObject(Mouse.current.position.ReadValue());

        };

        controls.LevelCreator.Finger1Press.performed += ctx =>
        {
            HandleClickObject(Pointer.current.position.ReadValue());

        };
        //     controls.LevelCreator.Finger1Press.canceled += ctx =>
        //    {

        //        HandleReleaseClick();
        //    };

        //     controls.LevelCreator.CursorClick.canceled += ctx =>
        //     {

        //         HandleReleaseClick();
        //     };

        //     controls.LevelCreator.Finger1Pos.performed += ctx =>
        //   {
        //       Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        //       HandleMoveObject(pos);

        //   };

        //     controls.LevelCreator.CursorPos.performed += ctx =>
        //     {
        //         Vector2 pos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        //         HandleMoveObject(pos);
        //     };


    }
    private int currentPlayerPath;
    private ILevelPickerPathObject currentTarget;
    private void HandleClickObject(Vector2 screenPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPosition);

        var obj = GetObjectFromTouchPosition(worldPoint);

        if (obj != null && obj != currentTarget)
        {
            currentTarget = obj;


            Vector3Int data = obj.Return_Type_PathIndex_Order();

            float d = paths[data.y].path.GetClosestDistanceAlongPath(obj.ReturnLinePostion());
            playerPathFollower.DoPathToPoint(paths[data.y], d);







        }
        else
        {
            Debug.Log("No object found at the clicked position.");

        }
    }


    private ILevelPickerPathObject GetObjectFromTouchPosition(Vector2 worldPoint)
    {



        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);



        if (hits.Length == 0)
        {
            Debug.Log("NO OBJECTS FOUND");
            return null;
        }


        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<ILevelPickerPathObject>() != null)
            {
                return hit.collider.gameObject.GetComponent<ILevelPickerPathObject>();
            }
        }

        return null;
    }

    private void OnEnable()
    {
        controls.LevelCreator.Enable();
        // LevelRecordManager.AddNewObject += SetObjectToBeAdded;

    }

    private void OnDisable()
    {
        controls.LevelCreator.Disable();

        // timeSlider.onValueChanged.RemoveAllListeners();
        // LevelRecordManager.AddNewObject -= SetObjectToBeAdded;





    }



    // Update is called once per frame
    void Update()
    {

    }
}
