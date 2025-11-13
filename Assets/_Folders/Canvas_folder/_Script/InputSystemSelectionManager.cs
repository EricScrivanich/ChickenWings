using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class InputSystemSelectionManager : MonoBehaviour
{
    public static InputSystemSelectionManager instance;
    private InputSystemUIInputModule inputModule;

    [SerializeField] private CanvasGroup[] menuGroups;
    private GameObject lastSelected;
    private GameObject lastSelectedByMenu;
    private string checkTag = "none";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Try to find the Input System UI Module (usually on EventSystem)
        inputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            Debug.LogError("No InputSystemUIInputModule found on the current EventSystem!");
            return;
        }

        // Subscribe to the move action (D-pad / joystick navigation)
        inputModule.move.action.performed += OnNavigatePerformed;
    }

    void OnDestroy()
    {
        if (inputModule != null)
            inputModule.move.action.performed -= OnNavigatePerformed;
    }

    private void OnNavigatePerformed(InputAction.CallbackContext ctx)
    {
        // 🔹 Navigation just happened (D-pad or left stick)

        var direction = ctx.ReadValue<Vector2>();
        if (direction == Vector2.zero)
            return; // No movement
        lastSelected = EventSystem.current.currentSelectedGameObject;


        if (CheckIfDirection(direction))
        {
            Debug.Log("Direction matched: " + direction + " Selecting: " + nextSelectedByDirection.gameObject);
            StartCoroutine(WaitAndSetSelected(nextSelectedByDirection));
            // nextSelectedByDirection = null;
            checkDirection = Vector2.zero;

        }

        Debug.Log("Navigation input: " + direction);

        // Check if current is invalid, and skip if needed
        // if (current == null || ShouldSkip(current))
        // {
        //     GameObject next = FindNextValidSelectable(current);
        //     if (next != null)
        //         EventSystem.current.SetSelectedGameObject(next);
        // }
    }
    private bool isSetting = false;
    private IEnumerator WaitAndSetSelected(GameObject obj)
    {
        if (isSetting) yield break;
        isSetting = true;

        yield return null; // Wait one frame
        EventSystem.current.SetSelectedGameObject(obj);
        lastSelected = obj;
        isSetting = false;
    }

    private bool ShouldSkip(GameObject obj)
    {
        if (obj == null) return true;
        if (checkTag == "none") return false;
        else if (obj.CompareTag(checkTag)) return false;

        return true;
    }

    private GameObject FindNextValidSelectable(GameObject start)
    {
        if (start == null) return null;
        Selectable sel = start.GetComponent<Selectable>();
        if (sel == null) return null;

        Selectable[] directions =
        {
            sel.FindSelectableOnRight(),
            sel.FindSelectableOnLeft(),
            sel.FindSelectableOnDown(),
            sel.FindSelectableOnUp()
        };

        foreach (Selectable s in directions)
        {
            if (s != null && !ShouldSkip(s.gameObject))
                return s.gameObject;
        }

        return null;
    }


    public void SetNewWindow(INavigationUI newWindow, string tag = "none")
    {
        checkTag = tag;

        EventSystem.current.SetSelectedGameObject(newWindow.GetFirstSelected());
        lastSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void SetNextSelected(GameObject nextObj)
    {
        if (nextObj != null)
        {
            EventSystem.current.SetSelectedGameObject(nextObj);
            lastSelected = nextObj;
        }
    }

    private Vector2 checkDirection = Vector2.zero;
    private GameObject nextSelectedByDirection = null;

    public bool CheckIfDirection(Vector2 rawInput)
    {
        Vector2 c = Vector2.zero;

        if (rawInput != Vector2.zero)
        {
            if (rawInput.x > 0.5f)
                c = Vector2.right;
            else if (rawInput.x < -0.5f)
                c = Vector2.left;
            else if (rawInput.y > 0.5f)
                c = Vector2.up;
            else if (rawInput.y < -0.5f)
                c = Vector2.down;
            else
                c = Vector2.zero;
        }

        Debug.Log("CheckIfDirection: " + c + " vs " + checkDirection);


        if (checkDirection == Vector2.zero || c != checkDirection || nextSelectedByDirection == null)
            return false;
        else
            return true;
    }
    public void HandleGroup(int index, bool enable)
    {
        if (menuGroups == null || index < 0 || index >= menuGroups.Length) return;

        if (enable && lastSelectedByMenu != null)
            EventSystem.current.SetSelectedGameObject(lastSelectedByMenu);
        else
        {
            lastSelectedByMenu = EventSystem.current.currentSelectedGameObject;

        }

        menuGroups[index].blocksRaycasts = enable;
        menuGroups[index].interactable = enable;
    }
    public void SetNextSelectedByDirection(GameObject nextObj, Vector2 direction)
    {
        Debug.Log("Set next selected by direction: " + direction);
        nextSelectedByDirection = nextObj;
        checkDirection = direction;
    }
}