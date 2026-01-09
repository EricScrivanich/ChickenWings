using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveCreator : MonoBehaviour, IBoxSliderListener
{

    public static WaveCreator instance;
    [SerializeField] private BoxSlider[] boxSliderArray;
    [SerializeField] private BoxTypeChanger boxTypeChanger;
    [SerializeField] private TextMeshProUGUI pointIndexText;



    private int currentMin;
    private int currentMax;
    private int currentTotalWaveIndex;


    public int numberOfSubWaves;

    [SerializeField] private GameObject linePrefab;
    private List<LineRenderer> lines;

    private List<RecordableObjectPlacer> objects;
    private List<RecordableObjectPlacer> objectsInSubWave;

    private List<short> usedRNGIndicesList;
    private int currentWaveIndex;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false);


    }
    void Start()
    {
        for (int i = 0; i < boxSliderArray.Length; i++)
        {
            boxSliderArray[i].Initialize(this);
        }
        pointIndexText.enabled = false;
        boxTypeChanger.Initialize(this);

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Open(int waveIndex)
    {
        this.gameObject.SetActive(true);
        LevelRecordManager.instance.OpenWaveEditor(true, waveIndex);
        currentWaveIndex = waveIndex;



    }

    public void SelectObject(RecordedDataStructDynamic data)
    {
        pointIndexText.enabled = true;
        pointIndexText.text = data.randomSpawnData.z.ToString();
        Vector2Int range = LevelRecordManager.instance.ReturnRandomSpawnRange(data.RandomWaveIndex);
        currentTotalWaveIndex = data.RandomWaveIndex;
        currentMin = range.x;
        currentMax = range.y;
        boxSliderArray[0].SetValue((float)range.x / 100f);
        boxSliderArray[1].SetValue((float)range.y / 100f);
        objectsInSubWave = new List<RecordableObjectPlacer>();
        foreach (var obj in objects)
        {
            if (obj.Data.randomSpawnData.y == data.randomSpawnData.y)
            {
                objectsInSubWave.Add(obj);
            }
        }
        boxTypeChanger.SetData("Position", 1, data.usedRNGIndices[1] + 1);



    }
    public void ChangeBoxTypeData(string title, int indexToEdit)
    {
        boxTypeChanger.SetData(title, indexToEdit, objectsInSubWave[0].Data.usedRNGIndices[indexToEdit] + 1);

    }

    public void SendObjects(List<RecordableObjectPlacer> objs)
    {
        objects = new List<RecordableObjectPlacer>();

        this.objects = objs;
        numberOfSubWaves = 0;

        foreach (var obj in objects)
        {
            if (obj.Data.randomSpawnData.y >= numberOfSubWaves)
            {
                numberOfSubWaves = obj.Data.randomSpawnData.y + 1;
            }

        }




        InstantiateLines();
    }

    public void AddObject(RecordableObjectPlacer obj)
    {
        this.objects.Add(obj);
        InstantiateLines();
    }


    private void InstantiateLines()
    {
        // Clear existing lines
        if (lines != null)
        {
            foreach (var line in lines)
            {
                Destroy(line.gameObject);
            }
        }

        lines = new List<LineRenderer>();

        // Create new lines for each object

        int waveIndex = -1;

        for (int i = 0; i < objects.Count; i++)
        {
            if (waveIndex != objects[i].Data.randomSpawnData.y)
            {
                waveIndex = objects[i].Data.randomSpawnData.y;
                var l = Instantiate(linePrefab);
                var lineRenderer = l.GetComponent<LineRenderer>();
                lines.Add(lineRenderer);
                lineRenderer.positionCount = 0;

                for (int j = 0; j < objects.Count; j++)
                {
                    if (objects[j].Data.randomSpawnData.y == waveIndex)
                    {
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, objects[j].transform.position);
                    }



                }


            }
            // var l = Instantiate(linePrefab);
            // var lineRenderer = l.GetComponent<LineRenderer>();
            // lines.Add(lineRenderer);
            // lineRenderer.positionCount = objects.Count;
            // for (int i = 0; i < objects.Count; i++)
            // {
            //     lineRenderer.SetPosition(i, objects[i].transform.position);
            // }

        }
    }
    public void Close()
    {
        LevelRecordManager.instance.OpenWaveEditor(false, currentWaveIndex);
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        this.gameObject.SetActive(false);
    }

    public void UpdateLine()
    {
        if (!gameObject.activeInHierarchy) return;

        for (int i = 0; i < objects.Count; i++)
        {
            lines[0].SetPosition(i, objects[i].transform.position);
        }

    }

    public void OnBoxSliderValueChanged(int type, float valueSliderHorizontal)
    {
        switch (type)
        {
            case 0:


                currentMin = Mathf.RoundToInt(valueSliderHorizontal * 100);
                if (currentMin > currentMax)
                {
                    currentMin = currentMax;
                    valueSliderHorizontal = (float)currentMin / 100f;

                }
                boxSliderArray[0].SetValue(valueSliderHorizontal);
                LevelRecordManager.instance.SetNewRandomSpawnRange(currentTotalWaveIndex, new Vector2Int(currentMin, currentMax));
                break;
            case 1:


                currentMax = Mathf.RoundToInt(valueSliderHorizontal * 100);
                if (currentMax < currentMin)
                {

                    currentMax = currentMin;
                    valueSliderHorizontal = (float)currentMax / 100f;

                }
                boxSliderArray[1].SetValue(valueSliderHorizontal);
                LevelRecordManager.instance.SetNewRandomSpawnRange(currentTotalWaveIndex, new Vector2Int(currentMin, currentMax));

                break;

        }

        LevelRecordManager.instance.SetNewRandomSpawnRange(currentTotalWaveIndex, new Vector2Int(currentMin, currentMax));
    }

    public void OnBoxTypeChanged(int type, int value)
    {

        for (int i = 0; i < objectsInSubWave.Count; i++)
        {
            objectsInSubWave[i].Data.usedRNGIndices[type] = (short)(value - 1);

        }


    }

    public void OnBoxSliderTimeValueChanged(int type, ushort valueSliderHorizontal)
    {
        throw new System.NotImplementedException();
    }

    public void OnBoxSliderByteValueChanged(int type, byte value)
    {
        // Not used in WaveCreator
    }

    // Update is called once per frame

}
