using System.Collections;
using UnityEngine;


public class RecordableObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [ExposedScriptableObject]
    public PigsScriptableObject pigID;

    [SerializeField] private SpriteRenderer xArrow;
    [SerializeField] private SpriteRenderer yArrow;
    [SerializeField] private GameObject timeStampPrefab;
    private Transform[] timeStamps;


    [field: SerializeField] public short ID { get; private set; }

    [SerializeField] private Color selectedOutlinesColor;
    [SerializeField] private SpriteRenderer[] selectedOutlines;
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private SpriteRenderer icon;



    [SerializeField] private float lineRendTimeStep;
    [SerializeField] private Transform touchColl;

    private Rigidbody2D rb;
    [SerializeField] private LevelCreatorColors colorSO;
    private IRecordableObject obj;
    private LineRenderer line;

    public RecordedDataStructDynamic Data;

    public enum EditableData
    {
        Speed,
        Size,
        Jump_Height,
        Jump_Offset,
        Drop_Delay,
        Drop_Offset,
        Fart_Delay,
        Fart_Offset,
        Glide_Height,
        Glide_Offset,
        Rotation
    }

    [Header("Ranges and Default Values")]
    [field: SerializeField] public Vector3 SpeedValues { get; private set; }
    [field: SerializeField] public Vector3 SizeValues { get; private set; }
    [field: SerializeField] public Vector3 MagnitideValues { get; private set; }
    [field: SerializeField] public Vector3 TimeIntervalValues { get; private set; }
    [field: SerializeField] public Vector3 DelayOrPhaseOffsetValues { get; private set; }
    [field: SerializeField] public Vector3Int TypeValues { get; private set; }



    public EditableData[] editedData;

    [Header("Speed, Size, Magnitude, Time Interval, Delay, Type")]





    public ushort spawnedTimeStep { get; private set; }
    public ushort unspawnedTimeStep { get; private set; }

    private bool isSelected = false;

    // private bool movingRight = false;

    private bool rbActive = true;
    public int speedChanger { get; private set; } = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        RestoreArrowsDefault();


        if (!isSelected)
        {
            xArrow.gameObject.SetActive(false);
            yArrow.gameObject.SetActive(false);
        }





        // Time.timeScale = 0;




    }

    public void SetIsSelected(bool selected)
    {
        isSelected = selected;

        if (isSelected)
        {
            SetLineColor(colorSO.SelectedLineColor);
            foreach (var s in selectedOutlines)
            {
                s.enabled = true;
            }
            xArrow.gameObject.SetActive(true);
            yArrow.gameObject.SetActive(true);
            fill.enabled = true;

            return;
        }

        else if (LevelRecordManager.CurrentTimeStep == spawnedTimeStep)
        {
            SetLineColor(colorSO.NormalLineColor);


        }
        else if (LevelRecordManager.CurrentTimeStep > spawnedTimeStep)
        {
            SetLineColor(colorSO.PassedLineColor);

        }

        foreach (var s in selectedOutlines)
        {
            s.enabled = false;
        }
        fill.enabled = false;

        xArrow.gameObject.SetActive(false);
        yArrow.gameObject.SetActive(false);


    }

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        obj = prefab.GetComponent<IRecordableObject>();
        rb = prefab.GetComponent<Rigidbody2D>();

        timeStamps = new Transform[colorSO.timeStampColors.Length];

        if (obj.ShowLine())
        {
            for (int i = 0; i < colorSO.timeStampColors.Length; i++)
            {
                if (i == 0)
                {

                    timeStamps[0] = timeStampPrefab.transform;

                }
                else
                {
                    timeStamps[i] = Instantiate(timeStampPrefab, transform).transform;
                }

                timeStamps[i].GetComponent<SpriteRenderer>().color = colorSO.timeStampColors[i];

            }
        }
        else
        {
            timeStampPrefab.SetActive(false);
        }



        foreach (var s in selectedOutlines)
        {
            s.color = selectedOutlinesColor;
            s.enabled = false;
        }
    }

    public void CreateNew()
    {

        Data = new RecordedDataStructDynamic(ID, transform.position, SpeedValues.z, 1, MagnitideValues.z, TimeIntervalValues.z, DelayOrPhaseOffsetValues.z, (short)TypeValues.z, LevelRecordManager.CurrentTimeStep, 0);
        Debug.Log("NEw object with scale of " + Data.scale);
        spawnedTimeStep = LevelRecordManager.CurrentTimeStep;
        unspawnedTimeStep = 40000;




        UpdateObjectData();


    }

    public string FormatEnumName(EditableData data)
    {
        return data.ToString().Replace("_", " ");
    }

    public void SetActiveFromList()
    {
        Debug.Log("Setting active from list");
        gameObject.SetActive(true);
        UpdateObjectData();
    }

    // private IEnumerator CheckForUnspawnedTime()
    // {
    //     while (true)
    //     {

    //     }
    // }

    public void LoadAssetFromSave(RecordedDataStructDynamic data)
    {

        Data = data;
        transform.position = data.startPos;
        spawnedTimeStep = data.spawnedStep;

        unspawnedTimeStep = 40000;

        UpdateObjectData();


    }

    public void HandleClick(bool clicked, int arrowType)
    {
        if (clicked)
        {
            if (arrowType == 0)
            {
                xArrow.color = colorSO.ArrowNullColor;
                yArrow.color = colorSO.ArrowNullColor;
            }
            else if (arrowType == 1)
            {
                xArrow.color = colorSO.ArrowXSelectedColor;
                xArrow.transform.localScale = colorSO.arrowSelectedScale;

            }
            else if (arrowType == 2)
            {
                yArrow.color = colorSO.ArrowYSelectedColor;
                yArrow.transform.localScale = colorSO.arrowSelectedScale;

            }
        }
        else
            RestoreArrowsDefault();

    }

    private void RestoreArrowsDefault()
    {
        xArrow.color = colorSO.ArrowXDefaultColor;
        yArrow.color = colorSO.ArrowYDefaultColor;
        xArrow.transform.localScale = colorSO.arrowNormalScale;
        yArrow.transform.localScale = colorSO.arrowNormalScale;
    }

    public void SetSelectedObject(LevelDataEditors[] floatSliders)
    {
        int checkedInt = 0;

        SetIsSelected(true);




        for (int i = 0; i < floatSliders.Length; i++)
        {
            // if (minMaxFloatsAndDefualt[i] == Vector3.zero)
            // {
            //     checkedInt++;

            // }
            // if (minMaxFloatsAndDefualt.Length > i + checkedInt && minMaxFloatsAndDefualt[i + checkedInt] == Vector3.zero)
            // {
            //     checkedInt++;

            // }
            // if (minMaxFloatsAndDefualt.Length > i + checkedInt && minMaxFloatsAndDefualt[i + checkedInt] == Vector3.zero)
            // {
            //     checkedInt++;

            // }
            if (i >= editedData.Length)
            {
                floatSliders[i].gameObject.SetActive(false);

            }
            else
            {
                floatSliders[i].gameObject.SetActive(true);
                // floatSliders[i].SetDataForFloatSlider(FormatEnumName(editedData[i]), this, minMaxFloatsAndDefualt[i + checkedInt].x, minMaxFloatsAndDefualt[i + checkedInt].y, minMaxFloatsAndDefualt[i + checkedInt].z);
                floatSliders[i].SetDataForFloatSlider(FormatEnumName(editedData[i]), false);
            }

        }

    }

    private void OnEnable()
    {
        LevelRecordManager.SetGlobalTime += UpdateObjectPosition;


        // UpdateObjectData();
        // UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);
    }

    private void OnDisable()
    {
        LevelRecordManager.SetGlobalTime -= UpdateObjectPosition;
        line.positionCount = 0;

    }

    // Update is called once per frame
    public void UpdateObjectData(bool isScale = false)
    {
        obj.ApplyCustomizedData(Data);
        if ((Data.ID == 0 || Data.ID == 2) && isScale)
        {
            prefab.GetComponent<ScaleAdjuster>().SetScales();
        }
        UpdateLineRenderer();
        UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);


    }

    private void UpdateObjectPosition(ushort timeStep, float realTime)
    {

        if (realTime == 0 && timeStep < spawnedTimeStep)
        {
            gameObject.SetActive(false);
            return;
        }


        Vector2 pos = transform.position;
        float offset = obj.ReturnPhaseOffset(transform.position.x);
        float t;
        if (realTime != 0)
        {
            if (rbActive)
            {
                SetLineColor(colorSO.NormalLineColor);
                // rb.en
                // touchColl.gameObject.SetActive(false);
                rb.simulated = false;
                rbActive = false;
            }
            t = realTime;
        }
        else
        {
            if (isSelected)
            {
                SetLineColor(colorSO.SelectedLineColor);
            }

            else if (timeStep == spawnedTimeStep)
            {
                SetLineColor(colorSO.NormalLineColor);
            }
            else if (timeStep > spawnedTimeStep)
            {
                SetLineColor(colorSO.PassedLineColor);
            }
            if (!rbActive)
            {
                Debug.Log("Enabling rb with real time: " + realTime);

                rb.simulated = true;

                // touchColl.gameObject.SetActive(true);
                rbActive = true;
            }
            t = timeStep * LevelRecordManager.TimePerStep;
        }
        // if (timeStep > unspawnedTimeStep || timeStep < spawnedTimeStep)
        // {
        //     gameObject.SetActive(false);
        //     return;
        // }
        var p = obj.PositionAtRelativeTime(t - (spawnedTimeStep * LevelRecordManager.TimePerStep), pos, offset);
        float ts = colorSO.TimeStampTimeSpacing;

        if (obj.ShowLine())
        {
            for (int c = 0; c < timeStamps.Length; c++)
            {

                Vector2 p2 = obj.PositionAtRelativeTime(ts + (t - (spawnedTimeStep * LevelRecordManager.TimePerStep)), pos, offset);
                timeStamps[c].transform.position = p2;
                ts += colorSO.TimeStampTimeSpacing;

            }

        }


        if ((Data.speed > 0 && p.x < BoundariesManager.leftBoundary) || (Data.speed < 0 && p.x > BoundariesManager.rightBoundary))
        {
            if (timeStep == 0)
            {

                unspawnedTimeStep = LevelRecordManager.CurrentTimeStep;

            }
            else
                unspawnedTimeStep = timeStep;



            gameObject.SetActive(false);
            return;
        }




        prefab.transform.position = p;

        // if (rbActive)
        // {
        //     touchColl.position = p;
        // }

    }

    private void SetLineColor(Color col)
    {
        line.startColor = col;
        line.endColor = col;
    }

    public void UpdateBasePosition(Vector2 pos)
    {
        transform.position = pos;
        if (Data.speed > 0 && pos.x < 0)
        {
            Data.speed *= -1;
            speedChanger = -1;
            UpdateObjectData();
        }
        else if (Data.speed < 0 && pos.x > 0)
        {
            Data.speed *= -1;
            speedChanger = 1;

            UpdateObjectData();
        }
        Data.startPos = pos;
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (!obj.ShowLine()) return;
        // Start with zero points
        bool movingRight = false;
        if (transform.position.x < 0) movingRight = true;
        else movingRight = false;
        Vector2 pos = transform.position;
        float offset = obj.ReturnPhaseOffset(transform.position.x);
        float checkTime = LevelRecordManager.CurrentTimeStep * LevelRecordManager.TimePerStep;
        int currentTimeStep = LevelRecordManager.CurrentTimeStep;

        for (int i = 0; i < 1000; i++)
        {
            Vector2 p = obj.PositionAtRelativeTime(checkTime - (LevelRecordManager.CurrentTimeStep * LevelRecordManager.TimePerStep), pos, offset);

            // Add new point to LineRenderer
            if (line.positionCount <= i)
                line.positionCount++;

            line.SetPosition(i, p);
            if (ID == 1)
            {
                line.positionCount = 2;
                float x = BoundariesManager.leftBoundary;

                if (movingRight)
                {
                    x = BoundariesManager.rightBoundary;
                }
                line.SetPosition(1, new Vector2(x, transform.position.y));

                break;
            }

            else
                checkTime += LevelRecordManager.TimePerStep;
            currentTimeStep++;

            // Stop adding points if it reaches boundary or max line length
            if ((movingRight && p.x > BoundariesManager.rightBoundary) ||
                (!movingRight && p.x < BoundariesManager.leftBoundary))
            {
                line.positionCount = i + 1;
                break;
            }
        }


    }
}
