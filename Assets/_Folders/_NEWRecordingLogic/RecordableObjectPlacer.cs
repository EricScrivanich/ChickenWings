using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecordableObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private GameObject baseIcon;
    [field: SerializeField] public string Title { get; private set; }

    [ExposedScriptableObject]
    public PigsScriptableObject pigID;

    [SerializeField] private SpriteRenderer xArrow;
    [SerializeField] private SpriteRenderer yArrow;
    [SerializeField] private GameObject timeStampPrefab;
    [SerializeField] private GameObject timeStampProjectilePrefab;
    [SerializeField] private bool useProjectile = false;
    private Transform[] timeStamps;


    [field: SerializeField] public short ID { get; private set; }


    [SerializeField] private SpriteRenderer[] selectedOutlines;
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private SpriteRenderer icon;

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
        Rotation,
        Blade_Speed,
        Start_Rotation,
        Fart_Length

    }

    [Header("Ranges and Default Values")]
    [field: SerializeField] public Vector3 SpeedValues { get; private set; }
    [field: SerializeField] public Vector3 SizeValues { get; private set; }
    [field: SerializeField] public Vector3 MagnitideValues { get; private set; }
    [field: SerializeField] public Vector3 TimeIntervalValues { get; private set; }
    [field: SerializeField] public Vector3 DelayOrPhaseOffsetValues { get; private set; }
    [field: SerializeField] public Vector3Int TypeValues { get; private set; }



    public EditableData[] editedData;

    [SerializeField] private string typeTitle;
    [SerializeField] private string[] typeOptions;

    [Header("Speed, Size, Magnitude, Time Interval, Delay, Type")]





    public ushort spawnedTimeStep { get; private set; }
    public ushort unspawnedTimeStep { get; private set; }

    private bool isSelected = false;

    private bool movingRight = false;
    public int speedChanger
    {
        get
        {
            if (!movingRight) return 1;
            return -1;
        }
    }

    private bool rbActive = true;


    [Header("Postioning Settings, is goudned adds gorund to minY")]

    [SerializeField] private Vector2 minMaxY;
    [SerializeField] private float minX;

    public enum PostionType
    {
        AnySide,
        RightSideOnly,

        Grounded,
        CenterOnly
    }

    [SerializeField] private PostionType _pType;

    private SpriteRenderer sprite;


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



    public void HideBaseIcon(bool hide)
    {
        baseIcon.SetActive(!hide);
    }

    public void SetIsSelected(bool selected)
    {
        isSelected = selected;

        SetProjectileLines();


        if (isSelected)
        {
            SetLineColor(colorSO.SelectedLineColor);
            foreach (var s in selectedOutlines)
            {
                s.enabled = true;
            }
            xArrow.gameObject.SetActive(true);
            if (_pType != PostionType.Grounded || _pType != PostionType.CenterOnly)
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
        if (Title == "Ring")
        {
            sprite = prefab.GetComponent<SpriteRenderer>();
        }


        if (obj.ShowLine())
        {
            timeStamps = new Transform[colorSO.timeStampColors.Length];
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
            s.color = colorSO.SelectedPigOutlineColor;
            s.enabled = false;
        }
    }

    public void CreateNew()
    {

        Data = new RecordedDataStructDynamic(ID, transform.position, SpeedValues.z, SizeValues.z, MagnitideValues.z, TimeIntervalValues.z, DelayOrPhaseOffsetValues.z, (short)TypeValues.z, LevelRecordManager.CurrentTimeStep, 0);

        Debug.Log("NEw object with scale of " + Data.scale);
        spawnedTimeStep = LevelRecordManager.CurrentTimeStep;
        unspawnedTimeStep = 40000;

        UpdateBasePosition(Data.startPos);


        // UpdateObjectData();


    }

    public void LoadAssetFromSave(RecordedDataStructDynamic data)
    {

        Data = data;
        transform.position = data.startPos;
        spawnedTimeStep = data.spawnedStep;

        if (data.speed < 0) movingRight = true;

        unspawnedTimeStep = 40000;
        UpdateBasePosition(Data.startPos);

        // UpdateObjectData();


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


    public void HandleClick(bool clicked, int arrowType)
    {
        if (clicked)
        {
            if (_pType == PostionType.Grounded || _pType == PostionType.CenterOnly) yArrow.gameObject.SetActive(false);
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

        if (_pType == PostionType.Grounded || _pType == PostionType.CenterOnly) yArrow.gameObject.SetActive(false);
    }

    public void SetSelectedObject()
    {
        int checkedInt = 0;

        SetIsSelected(true);




        for (int i = 0; i < editedData.Length; i++)
        {

            int index = i;

            if (i == editedData.Length - 1) ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, true);

            else ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, false);

        }
        ValueEditorManager.instance.SendTypeValues(typeTitle, typeOptions);

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

        if (projectileLines != null && projectileLines.Count > 0)
        {
            for (int i = 0; i < projectileLines.Count; i++)
            {

                if (projectileLines[i] != null)

                    LevelRecordManager.instance.ReturnPooledObjectToQ(projectileLines[i], 1);

            }
            projectileLines = new List<GameObject>();
        }

    }

    public void DestroySelf()
    {
        for (int i = 0; i < projectileLines.Count; i++)
        {
            projectileLines[i].transform.parent = null;
            LevelRecordManager.instance.ReturnPooledObjectToQ(projectileLines[i], 1);

        }
        projectileLines.Clear();
        Destroy(this.gameObject);
    }

    public void UpdateBasePosition(Vector2 pos)
    {
        // if (Title == "Windmill") obj.ApplyCustomizedData(Data);
        Debug.Log("Updating base position");

        if (_pType == PostionType.Grounded)
        {
            transform.position = new Vector2(pos.x, BoundariesManager.GroundPosition + minMaxY.x);
            Data.startPos = transform.position;

            UpdateObjectData();


            return;

        }
        else if (_pType == PostionType.AnySide)
        {
            // pos.y = Mathf.Clamp(pos.y, minMaxY.x, minMaxY.y);
            // if (pos.x <)

        }
        else if (_pType == PostionType.CenterOnly)
        {
            pos.y = Mathf.Clamp(pos.y, minMaxY.x, minMaxY.y);
            pos.x = Mathf.Clamp(pos.x, -minX, minX);
            transform.position = pos;
            Data.startPos = pos;
            UpdateObjectData();

            return;
        }

        transform.position = pos;
        Data.startPos = pos;
        if (Data.speed > 0 && pos.x < 0)
        {
            Data.speed *= -1;
            movingRight = true;


            return;
        }
        else if (Data.speed < 0 && pos.x > 0)
        {
            Data.speed *= -1;
            movingRight = false;




            return;

        }
        UpdateObjectData();
        UpdateLineRenderer();
        SetProjectileLines();
    }

    public void UpdateObjectData(bool isScale = false)
    {
        obj.ApplyCustomizedData(Data);

        if ((Data.ID == 0 || Data.ID == 2) && isScale)
        {
            prefab.GetComponent<ScaleAdjuster>().SetScales();
        }

        if (Title == "Ring")
        {
            sprite.color = colorSO.RingColors[Data.type];
        }

        UpdateLineRenderer();
        SetProjectileLines();

        // if (Title == "Windmill")
        // {
        //     Vector2 nun = obj.PositionAtRelativeTime((LevelRecordManager.CurrentTimeStep - spawnedTimeStep) * LevelRecordManager.TimePerStep, transform.position, 0);

        // }
        UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);


    }

    private void UpdateObjectPosition(ushort timeStep, float realTime)
    {

        if (realTime == 0 && timeStep < spawnedTimeStep)
        {
            Debug.Log("Time step is less than spawned time step");
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
                if (timeStamps != null)
                    foreach (var o in timeStamps)
                    {
                        o.gameObject.SetActive(false);
                    }
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
                if (timeStamps != null)
                    foreach (var o in timeStamps)
                    {
                        o.gameObject.SetActive(true);
                    }

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


        if ((Data.speed >= 0 && p.x < BoundariesManager.leftBoundary) || (Data.speed < 0 && p.x > BoundariesManager.rightBoundary))
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
        offset = obj.ReturnPhaseOffset(transform.position.x);

        if (Title == "Bomber" && offset < 0)
        {
            unspawnedTimeStep = LevelRecordManager.CurrentTimeStep;
            gameObject.SetActive(false);

            return;
        }

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
    private List<GameObject> projectileLines = new List<GameObject>();
    private void SetProjectileLines()
    {

        if (isSelected && (ID == 10 || ID == 8))
        {
            float offset = obj.ReturnPhaseOffset(transform.position.x);
            Vector2 pos = transform.position;
            bool returnExtra = false;
            for (int i = 0; i < 70; i++)
            {
                Vector2 p = obj.PositionAtRelativeTime(obj.TimeAtCreateObject(i), pos, offset);

                if ((movingRight && p.x > BoundariesManager.rightBoundary) || (!movingRight && p.x < BoundariesManager.leftBoundary))
                    returnExtra = true;

                if (projectileLines.Count <= i)
                {
                    if (returnExtra) return;
                    projectileLines.Add(LevelRecordManager.instance.ReturnPooledRecordingObject(1));


                }

                if (returnExtra)
                {
                    int start = i;
                    int end = i;
                    for (int n = i; n < projectileLines.Count; n++)
                    {
                        projectileLines[n].transform.parent = null;
                        projectileLines[n].SetActive(false);
                        LevelRecordManager.instance.ReturnPooledObjectToQ(projectileLines[n], 1);
                        end = n;


                    }

                    projectileLines.RemoveRange(start, end - start);
                }

                projectileLines[i].transform.position = p;
                // projectileLines[i].transform.parent = this.transform;
                projectileLines[i].SetActive(true);
            }


        }
        else if (projectileLines != null && projectileLines.Count > 0)
        {
            for (int i = 0; i < projectileLines.Count; i++)
            {

                if (projectileLines[i] != null)

                    LevelRecordManager.instance.ReturnPooledObjectToQ(projectileLines[i], 1);

            }
            projectileLines = new List<GameObject>();
        }

    }


    private void UpdateLineRenderer()
    {
        if (!obj.ShowLine()) return;
        // Start with zero points

        Vector2 pos = transform.position;
        float offset = obj.ReturnPhaseOffset(transform.position.x);
        float checkTime = LevelRecordManager.CurrentTimeStep * LevelRecordManager.TimePerStep;


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
                checkTime += .07f;


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
