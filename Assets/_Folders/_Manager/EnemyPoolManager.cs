using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [Header("Pool Sizes")]
    public SpawnSetupEnemy setup;

    [SerializeField] private int jetPackPigPoolSize;
    [SerializeField] private int tenderizerPigPoolSize;
    [SerializeField] private int normalPigPoolSize;
    [SerializeField] private GameObject jetPackPigPrefab; // Reference to the JetPackPig prefab
    [SerializeField] private GameObject tenderizerPigPrefab; // Reference to the JetPackPig prefab
    [SerializeField] private GameObject normalPigPrefab; // Reference to the JetPackPig prefab

    private int jetPackPigIndex;
    private int normalPigIndex;
    private int tenderizerPigIndex;
    private JetPackPigMovement[] jetPackPig;
    private TenderizerPig[] tenderizerPig;
    private PigMovementBasic[] normalPig;

    // Start is called before the first frame update
    void Start()
    {
        jetPackPigIndex = 0;

        // Initialize the pool array
        jetPackPig = new JetPackPigMovement[jetPackPigPoolSize];

        for (int i = 0; i < jetPackPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(jetPackPigPrefab);
            obj.SetActive(false);

            // Get the JetPackPigMovement component and store it in the array
            jetPackPig[i] = obj.GetComponent<JetPackPigMovement>();
        }

        tenderizerPig = new TenderizerPig[tenderizerPigPoolSize];

        for (int i = 0; i < tenderizerPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(tenderizerPigPrefab);
            obj.SetActive(false);

            // Get the tenderizerPigMovement component and store it in the array
            tenderizerPig[i] = obj.GetComponent<TenderizerPig>();
        }

        normalPig = new PigMovementBasic[normalPigPoolSize];

        for (int i = 0; i < normalPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(normalPigPrefab);
            obj.SetActive(false);

            // Get the normalPigMovement component and store it in the array
            normalPig[i] = obj.GetComponent<PigMovementBasic>();
        }


    }
    public void GetNormalPig(Vector2 pos, Vector3 scale, float speed)
    {
        if (normalPigIndex >= normalPig.Length) normalPigIndex = 0;
        var script = normalPig[normalPigIndex];
        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.xSpeed = speed;
        script.gameObject.SetActive(true);
        normalPigIndex++;
    }
    public void GetJetPackPig(Vector2 pos, Vector3 scale, float speed)
    {
        if (jetPackPigIndex >= jetPackPig.Length) jetPackPigIndex = 0;
        var script = jetPackPig[jetPackPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);
        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.speed = speed;
        script.gameObject.SetActive(true);
        jetPackPigIndex++;
    }
    public void GetTenderizerPig(Vector2 pos, Vector3 scale, float speed, bool hasHammer)
    {
        if (tenderizerPigIndex >= tenderizerPig.Length) tenderizerPigIndex = 0;
        var script = tenderizerPig[tenderizerPigIndex];
        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.speed = speed;
        script.hasHammer = hasHammer;
        script.gameObject.SetActive(true);
        tenderizerPigIndex++;
    }



}