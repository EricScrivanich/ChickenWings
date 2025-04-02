using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RecordableObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private SpriteRenderer[] iconSprites;

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
    [field: SerializeField] public ushort DataType { get; private set; }
    [field: SerializeField] public Vector3Int TypeValues { get; private set; }

    public EditableData[] editedData;
    [field: SerializeField] public Vector3[] FloatValues { get; private set; }

    [SerializeField] private bool floatOneIsSpeed;
    private float speed;




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
        Fart_Length,
        Frequency

    }

    [Header("Ranges and Default Values")]
    [field: SerializeField] public Vector3 SpeedValues { get; private set; }
    [field: SerializeField] public Vector3 SizeValues { get; private set; }
    [field: SerializeField] public Vector3 MagnitideValues { get; private set; }
    [field: SerializeField] public Vector3 TimeIntervalValues { get; private set; }
    [field: SerializeField] public Vector3 DelayOrPhaseOffsetValues { get; private set; }






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
        CenterOnly,
        AI
    }

    [SerializeField] private PostionType _pType;

    [SerializeField] private SpriteRenderer[] sprites;


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
    public void UpdateTimeStep(int step)
    {
        spawnedTimeStep = (ushort)step;
        Data.spawnedStep = spawnedTimeStep;
        // UpdateObjectData();
        ValueEditorManager.instance.UpdateSpawnTime(spawnedTimeStep);


    }


    public void HideBaseIcon(bool hide)
    {
        baseIcon.SetActive(!hide);
    }



    void Awake()
    {
        line = GetComponent<LineRenderer>();
        obj = prefab.GetComponent<IRecordableObject>();
        rb = prefab.GetComponent<Rigidbody2D>();
        // if (Title == "Ring")
        // {
        //     sprite = prefab.GetComponent<SpriteRenderer>();
        // }


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

        iconSprites[1].color = colorSO.iconOutlineColor;
        iconSprites[2].color = colorSO.iconFillColor;
    }

    public void CreateNew(int typeOvveride)
    {

        ushort type = (ushort)TypeValues.z;

        if (typeOvveride >= 0) type = (ushort)typeOvveride;

        float[] floatValues = new float[5];

        for (int i = 0; i < FloatValues.Length; i++)
        {
            floatValues[i] = FloatValues[i].z;

        }





        Data = new RecordedDataStructDynamic(ID, type, transform.position, floatValues[0], floatValues[1], floatValues[2], floatValues[3], floatValues[4], LevelRecordManager.CurrentTimeStep, 0);

        if (floatOneIsSpeed) speed = floatValues[0];
        else speed = 0;

        if (Title == "Ring")
        {
            foreach (var s in sprites)
            {
                s.color = colorSO.RingColors[Data.type];
            }

        }
        spawnedTimeStep = LevelRecordManager.CurrentTimeStep;
        unspawnedTimeStep = 40000;

        UpdateBasePosition(Data.startPos);


        // UpdateObjectData();


    }
    private Sequence fadeIconSeq;
    private bool iconsFaded = false;
    public void FadeIconImages(bool fade)
    {
        if (fadeIconSeq != null && fadeIconSeq.IsPlaying()) fadeIconSeq.Kill();
        iconsFaded = fade;
        fadeIconSeq = DOTween.Sequence();
        if (fade)
        {
            fadeIconSeq.Append(xArrow.DOFade(.2f, .2f));
            fadeIconSeq.Join(yArrow.DOFade(.2f, .2f));

            foreach (var s in iconSprites)
            {
                fadeIconSeq.Join(s.DOFade(.2f, .2f));
            }

        }
        else
        {
            fadeIconSeq.Append(xArrow.DOFade(1, .2f));
            fadeIconSeq.Join(yArrow.DOFade(1, .2f));
            foreach (var s in iconSprites)
            {
                fadeIconSeq.Join(s.DOFade(1f, .2f));
            }
        }
        fadeIconSeq.Play().SetUpdate(true);

    }

    public void LoadAssetFromSave(RecordedDataStructDynamic data)
    {

        Data = data;
        transform.position = data.startPos;
        spawnedTimeStep = data.spawnedStep;
        if (floatOneIsSpeed) speed = data.float1;
        else speed = 0;

        if (speed < 0) movingRight = true;

        unspawnedTimeStep = 40000;

        if (Title == "Ring")
        {
            foreach (var s in sprites)
            {
                s.color = colorSO.RingColors[Data.type];
            }

        }
        UpdateBasePosition(Data.startPos);

        // UpdateObjectData();


    }

    public Sprite GetIcon()
    {
        return iconSprites[0].sprite;
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
        {
            if (iconsFaded) FadeIconImages(false);
            RestoreArrowsDefault();
        }


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
        if (isSelected) return;

        SetIsSelected(true);




        for (int i = 0; i < editedData.Length; i++)
        {

            int index = i;

            if (i == editedData.Length - 1) ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, true);

            else ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, false);

        }
        ValueEditorManager.instance.SendTypeValues(typeTitle, typeOptions);

    }
    public void SetIsSelected(bool selected)
    {
        isSelected = selected;

        SetProjectileLines();


        if (isSelected)
        {
            SetLineColor(colorSO.SelectedLineColor);
            iconSprites[1].gameObject.SetActive(true);

            ValueEditorManager.instance.UpdateSpawnTime(spawnedTimeStep);
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

        if (LevelRecordManager.CurrentTimeStep > spawnedTimeStep + 3)
            iconSprites[1].gameObject.SetActive(false);

        if (!LevelRecordManager.ShowLines & obj.ShowLine())
        {
            line.positionCount = 0;
            foreach (var s in timeStamps)
            {
                s.gameObject.SetActive(false);
            }

        }
        else if (!LevelRecordManager.ShowSpeeds && obj.ShowLine())
        {
            foreach (var s in timeStamps)
            {
                s.gameObject.SetActive(false);
            }
        }



        if (LevelRecordManager.CurrentTimeStep == spawnedTimeStep)
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

    private void ChangeViewParameters()
    {
        UpdateObjectData();

    }

    private void OnEnable()
    {
        LevelRecordManager.SetGlobalTime += UpdateObjectPosition;
        LevelRecordManager.CheckViewParameters += ChangeViewParameters;

        iconSprites[1].gameObject.SetActive(true);


        // UpdateObjectData();
        // UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);
    }

    private void OnDisable()
    {
        LevelRecordManager.SetGlobalTime -= UpdateObjectPosition;
        LevelRecordManager.CheckViewParameters -= ChangeViewParameters;

        if (isSelected && LevelRecordManager.instance != null) LevelRecordManager.instance.UnactivateSelectedObject();

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
        if (Data.float1 > 0 && pos.x < 0 && floatOneIsSpeed)
        {
            Data.float1 *= -1;
            movingRight = true;


            return;
        }
        else if (Data.float1 < 0 && pos.x > 0 && floatOneIsSpeed)
        {
            Data.float1 *= -1;
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



        UpdateLineRenderer();
        SetProjectileLines();

        // if (Title == "Windmill")
        // {
        //     Vector2 nun = obj.PositionAtRelativeTime((LevelRecordManager.CurrentTimeStep - spawnedTimeStep) * LevelRecordManager.TimePerStep, transform.position, 0);

        // }
        UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);


    }
    public bool CheckForPoolSizes(ushort timeStep)
    {
        float t = timeStep * LevelRecordManager.TimePerStep;
        float offset = obj.ReturnPhaseOffset(transform.position.x);
        float addedX = 0;

        if (Title == "Windmill")
        {
            addedX = 2;
        }

        var p = obj.PositionAtRelativeTime(t - (spawnedTimeStep * LevelRecordManager.TimePerStep), transform.position, offset);

        if (_pType != PostionType.CenterOnly && _pType != PostionType.AI && (Data.float1 >= 0 && p.x < BoundariesManager.leftBoundary + addedX) || (Data.float1 < 0 && p.x > BoundariesManager.rightBoundary - addedX))
        {




            return false;
        }







        if (Title == "Bomber")
        {
            offset = obj.ReturnPhaseOffset(p.x);

            if (offset < 0) return false;
        }

        return true;
    }

    private void UpdateObjectPosition(ushort timeStep, float realTime)
    {

        if (realTime == 0 && timeStep < spawnedTimeStep)
        {
            Debug.LogError("Time step is less than spawned time step");

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
                if ((LevelRecordManager.ShowLines && LevelRecordManager.ShowSpeeds) || isSelected)
                {
                    if (timeStamps != null)
                        foreach (var o in timeStamps)
                        {
                            o.gameObject.SetActive(true);
                        }
                }
                else
                {
                    if (timeStamps != null)
                        foreach (var o in timeStamps)
                        {
                            o.gameObject.SetActive(false);
                        }
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
        float calculatedTime = t - (spawnedTimeStep * LevelRecordManager.TimePerStep);
        var p = obj.PositionAtRelativeTime(calculatedTime, pos, offset);
        float ts = colorSO.TimeStampTimeSpacing;

        if (obj.ShowLine() && ((LevelRecordManager.ShowLines && LevelRecordManager.ShowSpeeds) || isSelected))
        {
            for (int c = 0; c < timeStamps.Length; c++)
            {

                Vector2 p2 = obj.PositionAtRelativeTime(ts + (t - (spawnedTimeStep * LevelRecordManager.TimePerStep)), pos, offset);
                if ((movingRight && p2.x > BoundariesManager.rightBoundary) || (!movingRight && p2.x < BoundariesManager.leftBoundary))
                {
                    timeStamps[c].gameObject.SetActive(false);
                    ts += colorSO.TimeStampTimeSpacing;
                    continue;
                }
                if (timeStamps[c].gameObject.activeInHierarchy == false)
                {
                    timeStamps[c].gameObject.SetActive(true);
                }
                timeStamps[c].transform.position = p2;
                ts += colorSO.TimeStampTimeSpacing;

            }

        }
        else if (timeStamps != null)
        {
            foreach (var o in timeStamps)
            {
                o.gameObject.SetActive(false);
            }
        }

        if (_pType != PostionType.CenterOnly && calculatedTime > .54f && !isSelected)
        {
            iconSprites[1].gameObject.SetActive(false);
        }
        else if (iconSprites[1].gameObject.activeInHierarchy == false)
        {
            iconSprites[1].gameObject.SetActive(true);
        }



        if (_pType != PostionType.CenterOnly && _pType != PostionType.AI && (Data.float1 >= 0 && p.x < BoundariesManager.leftBoundary) || (Data.float1 < 0 && p.x > BoundariesManager.rightBoundary))
        {

            // if (CustomTimeSlider.instance.ChangingObjectTime && !CustomTimeSlider.instance.OverObjectTime)
            // {
            //     CustomTimeSlider.instance.RetractTime(true);
            //     Debug.Log("Retracting time with speed: " + Data.speed + " and pos of: " + p.x + " and time of: " + calculatedTime);

            //     return;
            // }
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


        if (Title == "Bomber")
        {
            offset = obj.ReturnPhaseOffset(p.x);

            if (offset < 0)
            {
                unspawnedTimeStep = LevelRecordManager.CurrentTimeStep;
                gameObject.SetActive(false);
                return;
            }


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

        if (!LevelRecordManager.ShowLines && !isSelected)
        {
            line.positionCount = 0;
            return;
        }
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
