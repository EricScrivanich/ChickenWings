using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class PositionerObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    private SpriteRenderer mainSprite;
    private Rigidbody2D rb;
    private int index = -1;
    private bool isSelected = false;

    private SortingGroup sortGroup;


    [SerializeField] private SpriteRenderer xArrow;
    [SerializeField] private SpriteRenderer yArrow;




    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainSprite = GetComponent<SpriteRenderer>();
        sortGroup = GetComponent<SortingGroup>();

    }

    public void SetNumber(int index)
    {


    }

    public void Activate(int index, Vector2 pos)
    {
        text.text = index.ToString();
        this.index = index;
        transform.position = pos;
        if (index == 0) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    public void MovePosition(Vector2 newPos)
    {
        transform.position = newPos;

    }
    public int ReturnIndex()
    {
        return index;
    }
    public void SetSelectedIndex(int i)
    {
        if (index == i)
        {
            // rb.simulated = true;
            sortGroup.sortingOrder = 0;
            transform.localScale = Vector3.one * .9f;
            mainSprite.enabled = true;
            isSelected = true;
            xArrow.gameObject.SetActive(true);
            yArrow.gameObject.SetActive(true);
        }
        else
        {
            // rb.simulated = false;
            sortGroup.sortingOrder = -2;
            transform.localScale = Vector3.one * .8f;
            mainSprite.enabled = false;
            isSelected = false;
            xArrow.gameObject.SetActive(false);
            yArrow.gameObject.SetActive(false);

        }


    }

    public void Press(int arrowType)
    {



        if (arrowType == 3)
        {
            xArrow.color = LevelRecordManager.instance.colorSO.ArrowXSelectedColor;
            xArrow.transform.localScale = LevelRecordManager.instance.colorSO.arrowSelectedScale;

        }
        else if (arrowType == 4)
        {
            yArrow.color = LevelRecordManager.instance.colorSO.ArrowYSelectedColor;
            yArrow.transform.localScale = LevelRecordManager.instance.colorSO.arrowSelectedScale;

        }
        else
        {
            xArrow.color = LevelRecordManager.instance.colorSO.ArrowNullColor;
            yArrow.color = LevelRecordManager.instance.colorSO.ArrowNullColor;
        }

    }

    public void ReleaseClick()
    {
        xArrow.color = LevelRecordManager.instance.colorSO.ArrowXDefaultColor;
        yArrow.color = LevelRecordManager.instance.colorSO.ArrowYDefaultColor;
        xArrow.transform.localScale = LevelRecordManager.instance.colorSO.arrowNormalScale;
        yArrow.transform.localScale = LevelRecordManager.instance.colorSO.arrowNormalScale;
    }
    public void Deactivate()
    {

    }


    // Update is called once per frame

}
