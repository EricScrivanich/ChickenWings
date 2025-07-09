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
    [field: SerializeField] public short DataType { get; private set; }
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
        Frequency,
        Laser_Spacing


    }







    [SerializeField] private string typeTitle;
    [SerializeField] private string[] typeOptions;

    [Header("Speed, Size, Magnitude, Time Interval, Delay, Type")]





    public ushort spawnedTimeStep { get; private set; }
    public ushort unspawnedTimeStep { get; private set; }

    public bool isSelected { get; private set; } = false;

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
        AI,
        Position

    }
    private Transform cage;
    private CageAttatchment cageAttatchment;

    [SerializeField] private PostionType _pType;

    [field: SerializeField] public bool canAttachCage { get; private set; } = true;

    [SerializeField] private SpriteRenderer[] sprites;

    [Header("Set Potisition Object Settings")]
    [SerializeField] private bool setPositionObject = false;
    public RecordedDataStructTweensDynamic positionData { get; private set; }
    public RecordedDataStructTweensDynamic rotationData { get; private set; }
    public RecordedDataStructTweensDynamic timerData { get; private set; }
    private Ease[] eases = new Ease[] { Ease.Linear, Ease.InSine, Ease.OutSine, Ease.InOutSine };

    private List<float> startingTimes = new List<float>();
    private List<float> endingTimes = new List<float>();

    public enum PositionerType

    {
        Positions,
        Rotations,
        Timers

    }
    private ObjectPositioner objectPositioner;
    public PositionerType[] positionerTypes;

    private int lastSavedType = 0;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        RestoreArrowsDefault();




        if (!isSelected)
        {
            xArrow.gameObject.SetActive(false);
            yArrow.gameObject.SetActive(false);
        }
        if (Title != "Bomber")
        {
            MonoBehaviour mb = obj as MonoBehaviour;
            if (mb != null)
            {
                mb.enabled = false;
            }
        }




        // Time.timeScale = 0;




    }
    public void UpdateTimeStep(int step, bool multipleSelect = false)
    {
        spawnedTimeStep = (ushort)step;
        Data.spawnedStep = spawnedTimeStep;

        if (_pType == PostionType.Position)
        {
            if (positionData != null)
            {
                positionData.AdjustBaseTimeStep(spawnedTimeStep);
            }
            if (rotationData != null)
            {
                rotationData.AdjustBaseTimeStep(spawnedTimeStep);
            }
            if (timerData != null)
            {
                timerData.AdjustBaseTimeStep(spawnedTimeStep);
            }

            CustomTimeSlider.instance.CreateTweenHandles(DynamicValueAdder.instance.data);
            DynamicValueAdder.instance.UpdateSpawnTimeData();
            // if (timerData != null)
            // {
            //     timerData.UpdateTimeStep(spawnedTimeStep);
            // }
        }

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


        if (_pType == PostionType.Position)
        {
            objectPositioner = prefab.GetComponent<ObjectPositioner>();
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
        lastSavedType = type;

        if (_pType == PostionType.Position)
        {
            for (int i = 0; i < positionerTypes.Length; i++)
            {
                switch (positionerTypes[i])
                {
                    case PositionerType.Positions:
                        positionData = new RecordedDataStructTweensDynamic(ID, 0, LevelRecordManager.CurrentTimeStep);


                        break;
                    case PositionerType.Rotations:
                        rotationData = new RecordedDataStructTweensDynamic(ID, 1, LevelRecordManager.CurrentTimeStep);

                        break;
                    case PositionerType.Timers:
                        timerData = new RecordedDataStructTweensDynamic(ID, 2, LevelRecordManager.CurrentTimeStep);
                        break;

                }
            }



        }



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
        lastSavedType = data.type;
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
        else if (Title == "Balloon" && Data.type == 1)
        {
            skipProjectileLines = true;

        }

        if (_pType == PostionType.Position)
        {
            if (Title == "Laser")
            {
                Debug.Log("Loading Laser Data");
                Data.type = data.positionerData.type;
                Data.startPos = data.positionerData.startPos;
                Data.float1 = data.positionerData.startRot;
                Data.float2 = data.positionerData.perecnt;
            }
            for (int i = 0; i < positionerTypes.Length; i++)
            {
                switch (positionerTypes[i])
                {
                    case PositionerType.Positions:
                        positionData = new RecordedDataStructTweensDynamic(ID, 0, data.spawnedStep);
                        data.positionerData.SetDataForRecording(positionData);

                        Debug.Log("added postioner data: " + positionData.startingSteps.Count);


                        break;
                    case PositionerType.Rotations:
                        rotationData = new RecordedDataStructTweensDynamic(ID, 1, data.spawnedStep);
                        data.positionerData.SetDataForRecording(rotationData);

                        break;
                    case PositionerType.Timers:
                        timerData = new RecordedDataStructTweensDynamic(ID, 2, data.spawnedStep);
                        data.positionerData.SetDataForRecording(timerData);
                        break;

                }
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

        // else
        //     return;



        ValueEditorManager.instance.ResetRectList();




        if (editedData != null && editedData.Length > 0)

            for (int i = 0; i < editedData.Length; i++)
            {
                int index = i;
                if (i == editedData.Length - 1) ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, true);
                else ValueEditorManager.instance.SendFloatValues(FormatEnumName(editedData[i]), index, false);
            }
        else ValueEditorManager.instance.DeactivateFloatEditors();


        ValueEditorManager.instance.SendTypeValues(typeTitle, typeOptions);

        if (positionerTypes != null && positionerTypes.Length > 0)
            for (int i = 0; i < positionerTypes.Length; i++)
            {
                ValueEditorManager.instance.SendPositionerValues(positionerTypes[i].ToString(), i, positionerTypes.Length - 1 == i);
            }





        ValueEditorManager.instance.OrderRectList(Title);
        if (Title == "Balloon" && Data.type == 1)
        {

            ValueEditorManager.instance.SetNewFloatSize(-2);

        }

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
                s.color = colorSO.SelectedPigOutlineColor;
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

    private void CheckSpecialData(int t, bool b)
    {
        switch (t)
        {
            case 0:
                if (b)
                {
                    if (canAttachCage)
                    {
                        foreach (var s in selectedOutlines)
                        {
                            s.color = colorSO.CanAttachCageOutlineColor;
                            s.enabled = true;
                        }

                    }
                }
                else
                {
                    if (canAttachCage)
                        foreach (var s in selectedOutlines)
                        {
                            s.enabled = false;
                        }
                }
                break;

        }

    }

    public void SetCageAttachment(bool attach)
    {
        Data.hasCageAttachment = attach;

        if (!attach)
        {
            if (cage != null)
            {
                Debug.Log("Deleting Cage");

                Destroy(cage.gameObject);
                cage = null;
            }
            if (cageAttatchment != null)
            {
                cageAttatchment = null;

            }
        }

    }



    private void OnEnable()
    {
        LevelRecordManager.SetGlobalTime += UpdateObjectPosition;
        LevelRecordManager.CheckViewParameters += ChangeViewParameters;
        LevelRecordManager.OnSendSpecialDataToActiveObjects += CheckSpecialData;

        iconSprites[1].gameObject.SetActive(true);


        // UpdateObjectData();
        // UpdateObjectPosition(LevelRecordManager.CurrentTimeStep, 0);
    }

    private void OnDisable()
    {
        LevelRecordManager.SetGlobalTime -= UpdateObjectPosition;
        LevelRecordManager.CheckViewParameters -= ChangeViewParameters;
        LevelRecordManager.OnSendSpecialDataToActiveObjects -= CheckSpecialData;


        if (isSelected) LevelRecordManager.instance.UnactivateSelectedObject();

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

        if (Data.hasCageAttachment && cage != null)
        {
            cage.gameObject.SetActive(false);

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

    public void UpdateBasePosition(Vector2 pos, bool usingOffset = false)
    {
        // if (Title == "Windmill") obj.ApplyCustomizedData(Data);

        if (usingOffset) pos = LevelRecordManager.instance.ReturnRoundedPosition(Data.startPos - pos, true);
        if (_pType == PostionType.Grounded)
        {
            if (pos.x < BoundariesManager.leftViewBoundary) return;
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
        else if (_pType == PostionType.Position)
        {
            if (positionData != null)
            {
                Data.startPos = pos;
                positionData.positions[0] = pos;
            }
            transform.position = pos;
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


        if (lastSavedType != Data.type && Title == "Balloon")
        {
            if (Data.type == 0)
            {
                skipProjectileLines = false;
                ValueEditorManager.instance.SetNewFloatSize(2);
            }
            else if (Data.type == 1)
            {
                skipProjectileLines = true;
                ValueEditorManager.instance.SetNewFloatSize(-2);


            }


            SetProjectileLines();

            lastSavedType = Data.type;
            return;
        }
        lastSavedType = Data.type;



        if ((Data.ID == 0 || Data.ID == 2) && isScale)
        {
            prefab.GetComponent<ScaleAdjuster>().SetScales();
        }

        if (_pType == PostionType.Position)
        {

            if (Title == "Laser")
            {
                rotationData.values[0] = Data.float1;
            }



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

        if (realTime == 0 && timeStep < spawnedTimeStep && !(LevelRecordManager.instance.multipleObjectsSelected && isSelected))
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

        if (_pType == PostionType.Position)
        {
            float calculatedStep;
            if (realTime != 0)
            {
                calculatedStep = realTime / LevelRecordManager.TimePerStep;
            }
            else
            {
                calculatedStep = timeStep;
            }







            float elapsedLifetime = t - (spawnedTimeStep * LevelRecordManager.TimePerStep);

            for (int i = 0; i < positionerTypes.Length; i++)
            {


                switch (positionerTypes[i])
                {
                    case PositionerType.Positions:
                        if (positionData.startingSteps == null || positionData.startingSteps.Count <= 1)
                        {

                            continue;
                        }


                        for (int j = 1; j < positionData.startingSteps.Count; j++)
                        {
                            if (calculatedStep >= positionData.startingSteps[j] && calculatedStep < positionData.endingSteps[j])
                            {

                                float startTime = positionData.startingSteps[j] * LevelRecordManager.TimePerStep;
                                float endTime = positionData.endingSteps[j] * LevelRecordManager.TimePerStep;
                                float totalDuration = endTime - startTime;

                                float currentTime = calculatedStep * LevelRecordManager.TimePerStep;
                                float timeIntoTween = currentTime - startTime;

                                float percent = Mathf.Clamp01(timeIntoTween / totalDuration);
                                Vector2 val = DOVirtual.EasedValue(positionData.positions[j - 1], positionData.positions[j], percent, eases[positionData.easeTypes[j]]);
                                prefab.transform.position = val;
                                break;


                            }
                            else if (calculatedStep >= positionData.endingSteps[j] && (j == positionData.endingSteps.Count - 1 || calculatedStep < positionData.startingSteps[j + 1]))
                            {

                                prefab.transform.position = positionData.positions[j];
                                break;
                            }
                        }
                        break;
                    case PositionerType.Rotations:
                        if (rotationData.startingSteps == null)
                        {

                            continue;

                        }
                        else if (rotationData.startingSteps.Count <= 1)
                        {
                            prefab.transform.eulerAngles = new Vector3(0, 0, rotationData.values[0]);
                            continue;
                        }


                        for (int j = 1; j < rotationData.startingSteps.Count; j++)
                        {

                            if (calculatedStep >= rotationData.startingSteps[j] && calculatedStep < rotationData.endingSteps[j])
                            {
                                // please debug log the values

                                float startTime = rotationData.startingSteps[j] * LevelRecordManager.TimePerStep;
                                float endTime = rotationData.endingSteps[j] * LevelRecordManager.TimePerStep;
                                float totalDuration = endTime - startTime;

                                float currentTime = calculatedStep * LevelRecordManager.TimePerStep;
                                float timeIntoTween = currentTime - startTime;

                                float percent = Mathf.Clamp01(timeIntoTween / totalDuration);
                                // int flipIntitial = 1;
                                // int flipFinal = 1;

                                // if (rotationData.other[j - 1] == 0) flipIntitial = -1;
                                // if (rotationData.other[j] == 0) flipFinal = -1;
                                // float initialRot = rotationData.values[j - 1] * flipIntitial;
                                // float finalRot = rotationData.values[j] * flipFinal;




                                // float percent = ((calculatedStep * LevelRecordManager.TimePerStep) - elapsedLifetime) / (((rotationData.endingSteps[j] * LevelRecordManager.TimePerStep) - (rotationData.startingSteps[j] * LevelRecordManager.TimePerStep)) - elapsedLifetime);

                                float val = DOVirtual.EasedValue(rotationData.values[j - 1], rotationData.values[j], percent, eases[rotationData.easeTypes[j]]);
                                prefab.transform.eulerAngles = new Vector3(0, 0, val);

                                break;


                            }
                            else if (calculatedStep >= rotationData.endingSteps[j] && (j == rotationData.endingSteps.Count - 1 || calculatedStep < rotationData.startingSteps[j + 1]))
                            {

                                // int flipFinal = 1;


                                // if (rotationData.other[j] == 0) flipFinal = -1;

                                // float finalRot = 
                                prefab.transform.eulerAngles = new Vector3(0, 0, rotationData.values[j]);
                                break;
                            }

                        }
                        break;

                    case PositionerType.Timers:
                        if (timerData.startingSteps == null)
                        {

                            continue;

                        }
                        else if (timerData.startingSteps.Count <= 1)
                        {
                            objectPositioner.SetPercentForLaserShooting(0);
                            continue;
                        }


                        for (int j = 1; j < timerData.startingSteps.Count; j++)
                        {

                            if (calculatedStep >= timerData.startingSteps[j] && calculatedStep < timerData.endingSteps[j])
                            {
                                float startTime = timerData.startingSteps[j] * LevelRecordManager.TimePerStep;
                                float endTime = timerData.endingSteps[j] * LevelRecordManager.TimePerStep;
                                float totalDuration = endTime - startTime;

                                float currentTime = calculatedStep * LevelRecordManager.TimePerStep;
                                float timeIntoTween = currentTime - startTime;

                                if (timeIntoTween < LevelRecordManager.instance.ConvertLevelTimeToRealTime(LaserParent.cooldownDuration))
                                {
                                    float warnUpPercent = Mathf.Clamp01(timeIntoTween / LevelRecordManager.instance.ConvertLevelTimeToRealTime(LaserParent.cooldownDuration));
                                    objectPositioner.SetPercentForLaserShooting(warnUpPercent);
                                }
                                else
                                {
                                    float percent = -Mathf.Lerp(1, 0, Mathf.Clamp01((timeIntoTween - LevelRecordManager.instance.ConvertLevelTimeToRealTime(LaserParent.cooldownDuration)) / (totalDuration - LevelRecordManager.instance.ConvertLevelTimeToRealTime(LaserParent.cooldownDuration))));
                                    objectPositioner.SetPercentForLaserShooting(percent);
                                }
                                break;
                            }

                            else if (calculatedStep >= timerData.endingSteps[j] && (j == timerData.endingSteps.Count - 1 || calculatedStep < timerData.startingSteps[j + 1]))
                            {

                                objectPositioner.SetPercentForLaserShooting(0);
                                break;
                            }
                            else
                            {
                                objectPositioner.SetPercentForLaserShooting(0);
                            }

                        }
                        break;

                }


            }
            SetCagePosition();
            return;
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

            if (!(LevelRecordManager.instance.multipleObjectsSelected && isSelected))
            {
                gameObject.SetActive(false);
                return;
            }



        }




        prefab.transform.position = p;
        SetCagePosition();



        if (Title == "Bomber")
        {
            offset = obj.ReturnPhaseOffset(p.x);

            if (offset < 0 && !(LevelRecordManager.instance.multipleObjectsSelected && isSelected))
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

    #region ObjectPostionerTypeFunctions



    #endregion

    private void SetCagePosition()
    {
        if (!Data.hasCageAttachment)
        {


            return;


        }
        if (cage == null)
        {
            cage = LevelRecordManager.instance.ReturnSpecialObject(0).GetComponent<Transform>();
            cage.GetComponent<ChicAndCageRecordable>().SetData(this);
        }
        if (cageAttatchment == null)
        {
            if (prefab.GetComponent<CageAttatchment>() == null)
            {
                if (prefab.GetComponentInChildren<CageAttatchment>() != null)
                {
                    cageAttatchment = prefab.GetComponentInChildren<CageAttatchment>();
                }
                else
                {
                    Debug.LogError("No Cage Attatchment found on prefab: " + prefab.name);
                    return;
                }
            }
            else
                cageAttatchment = prefab.GetComponent<CageAttatchment>();
        }
        if (!cage.gameObject.activeInHierarchy) cage.gameObject.SetActive(true);

        cage.position = cageAttatchment.ReturnPosition();




    }

    private void SetLineColor(Color col)
    {
        line.startColor = col;
        line.endColor = col;
    }
    private bool skipProjectileLines = false;
    private List<GameObject> projectileLines = new List<GameObject>();
    private void SetProjectileLines()
    {

        if (isSelected && (ID == 10 || ID == 8) && !skipProjectileLines)
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
    public bool IsPositionerObject()
    {
        return _pType == PostionType.Position;
    }

    public RecordedObjectPositionerDataSave ReturnPositionerDataSave()
    {

        List<ushort> sizeByType = new List<ushort>();
        List<ushort> startSteps = new List<ushort>();
        List<ushort> endSteps = new List<ushort>();
        List<ushort> eases = new List<ushort>();
        List<Vector2> positions = new List<Vector2>();
        List<float> values = new List<float>();
        ushort[] sizesByTypes = new ushort[3];

        float startRot = 0;
        float percent = 0;

        if (Title == "Laser")
        {
            startRot = Data.float1;
            percent = Data.float2;
        }

        for (int i = 0; i < positionerTypes.Length; i++)
        {
            switch (positionerTypes[i])
            {
                case PositionerType.Positions:

                    for (int j = 1; j < positionData.startingSteps.Count; j++)
                    {
                        startSteps.Add(positionData.startingSteps[j]);
                        endSteps.Add(positionData.endingSteps[j]);
                        eases.Add(positionData.easeTypes[j]);
                        positions.Add(positionData.positions[j]);

                    }
                    sizesByTypes[0] = (ushort)(positionData.startingSteps.Count - 1);


                    break;
                case PositionerType.Rotations:
                    for (int j = 1; j < rotationData.startingSteps.Count; j++)
                    {
                        startSteps.Add(rotationData.startingSteps[j]);
                        endSteps.Add(rotationData.endingSteps[j]);
                        eases.Add(rotationData.easeTypes[j]);
                        values.Add(rotationData.values[j]);
                    }
                    sizesByTypes[1] = (ushort)(rotationData.startingSteps.Count - 1);

                    break;
                case PositionerType.Timers:
                    for (int j = 1; j < timerData.startingSteps.Count; j++)
                    {
                        startSteps.Add(timerData.startingSteps[j]);
                        endSteps.Add(timerData.endingSteps[j]);
                        // eases.Add(timerData.easeTypes[j]);
                        // values.Add(timerData.values[j]);
                    }


                    sizesByTypes[2] = (ushort)(timerData.startingSteps.Count - 1);
                    break;

            }
        }

        return new RecordedObjectPositionerDataSave((short)ID, Data.type, sizesByTypes, percent, new Vector3(Data.startPos.x, Data.startPos.y, startRot), positions.ToArray(), values.ToArray(), eases.ToArray(), startSteps.ToArray(), endSteps.ToArray(), Data.spawnedStep, 0);



    }

    private void CheckForPreloadTimeStep(int checkedStep)
    {


    }
}
