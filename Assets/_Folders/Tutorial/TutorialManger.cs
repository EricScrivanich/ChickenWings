using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class TutorialManger : MonoBehaviour
{
    public PlayerID player;
    public RingID ring;

    public int currentMech;
    private int passesNeeded;
    [SerializeField] private List<string> poolNames;

    private int finalRingSize = 26;




    [SerializeField] private GameObject jumpRing;
    public Pool pools;
    private bool ringBool;
    private bool hasShownJump;
    public List<GameObject> buttons;
    public GameObject jump;
    public GameObject rings;
    public GameObject flip;
    public GameObject dash;
    public GameObject drop;

    public GameObject final;

    public GameObject menu;
    public GameObject egg;

    [SerializeField] private TutorialCavas canvas;


    private Coroutine Spawner;

    public int RingsHit;
    // Start is called before the first frame update
    void Start()
    {
        RingsHit = 0;
        if (currentMech == 0)
        {
            hasShownJump = false;
        }
        else
        {
            hasShownJump = true;
        }

        foreach (var but in buttons)
        {
            but.SetActive(false);
        }
        ShowMech(currentMech);

    }

    // Update is called once per frame
    private void ShowJump(bool show)
    {
        if (currentMech != 0 || hasShownJump)
        {
            return;

        }
        buttons[0].SetActive(true);


        Pause(show);
        jump.SetActive(show);

        if (show == false)
        {
            passesNeeded = 4;
            ResetRingsHit(passesNeeded);
            Spawner = StartCoroutine(WaitForRingStats());
            hasShownJump = true;

        }



    }

    private IEnumerator WaitForRingStats()
    {
        yield return new WaitForSeconds(.5f);
        canvas.AnimateStats();

        yield return new WaitForSeconds(1.8f);
        ShowRings(true);
    }

    public void ShowRings(bool show)
    {
        ringBool = show;



        Pause(show);
        rings.SetActive(show);

        if (show == false)
        {

            Spawner = StartCoroutine(SpawnRings(poolNames[0], -.2f, 4.1f));


        }



    }

    private void ShowFlips(bool show)
    {
        if (currentMech != 1)
        {
            return;

        }

        ShowButtons(2);

        Pause(show);
        flip.SetActive(show);

        if (show == false)
        {
            passesNeeded = 3;
            ResetRingsHit(passesNeeded);
            Spawner = StartCoroutine(SpawnRings(poolNames[1], -.1f, 4.2f));
            player.events.OnFlipLeft -= HitFlip;
            player.events.OnFlipRight -= HitFlip;

        }

    }

    private void ShowDash(bool show)
    {
        if (currentMech != 2)
        {
            return;

        }

        ShowButtons(3);

        Pause(show);
        dash.SetActive(show);

        if (show == false)
        {
            passesNeeded = 3;
            ResetRingsHit(passesNeeded);
            Spawner = StartCoroutine(SpawnRings(poolNames[2], .5f, 4.1f));
            player.events.OnDash -= HitDash;


        }

    }


    private void ShowDrop(bool show)
    {
        if (currentMech != 3)
        {
            return;
        }
        ShowButtons(4);

        Pause(show);
        drop.SetActive(show);

        if (show == false)
        {
            passesNeeded = 3;
            ResetRingsHit(passesNeeded);
            Spawner = StartCoroutine(SpawnRings(poolNames[3], 0, 0));
            player.events.OnDrop -= HitDrop;
        }
    }

    public void ShowFinal(bool show)
    {
        if (currentMech != 4)
        {
            return;
        }
        ShowButtons(4);
        if (show == false)
        {
            ring.ResetVariables();
        }

        Pause(show);

        final.SetActive(show);


        if (show == false)
        {
            passesNeeded = finalRingSize;
            ResetRingsHit(finalRingSize);

        }
    }

    public void ShowMenu(bool show)
    {
        if (currentMech != 5)
        {
            return;
        }
        ShowButtons(4);


        Pause(show);

        menu.SetActive(show);


        // if (show == false)
        // {
        //     passesNeeded = finalRingSize;
        //     ResetRingsHit(finalRingSize);

        // }
    }



    private void ShowButtons(int amount)

    {
        for (int i = 0; i <= amount; i++)
        {
            buttons[i].SetActive(true);
        }

    }

    private IEnumerator SpawnRings(string name, float miny, float maxy)
    {
        if (name == "RingDash")
        {
            yield return new WaitForSeconds(2.7f);
        }

        while (true)
        {
            Transform ring = pools.Spawn(name, new Vector2(BoundariesManager.rightBoundary, Random.Range(miny, maxy)), transform.rotation);

            yield return new WaitForSeconds(3.2f);

        }


    }

    private void ShowMech(int type)
    {
        if (type == 0)
        {
            ShowJump(true);
        }
        else if (type == 1)
        {
            ShowFlips(true);
        }
        else if (type == 2)
        {
            ShowDash(true);
        }
        else if (type == 3)
        {
            ShowDrop(true);

        }
        else if (type == 4)
        {
            ShowFinal(true);

        }
        else if (type == 5)
        {
            ShowMenu(true);

        }
        if (type > 0 && type < 4)
        {
            pools.DespawnAll(poolNames[type - 1]);

        }



    }

    private void RandomY(int index)
    {

    }

    private void ResetRingsHit(int passesNeededVar)
    {
        canvas.UpdateRings(0, passesNeededVar);
        RingsHit = 0;


    }

    private void CheckRealRingProgress(bool didComplete, int none)
    {
        if (!didComplete)
        {
            ResetRingsHit(finalRingSize);

        }
    }

    private void AddRingHit()
    {

        RingsHit++;
        canvas.UpdateRings(RingsHit, passesNeeded);

        if (RingsHit == passesNeeded)
        {
            RingsHit = 0;
            StopCoroutine(Spawner);

            currentMech++;
            StartCoroutine(NextTutorialDelay());


        }

    }

    private void AddRealRingHit()
    {
        RingsHit++;
        canvas.UpdateRings(RingsHit, finalRingSize);

        if (RingsHit >= finalRingSize)
        {
            currentMech++;
            StartCoroutine(NextTutorialDelay());

        }




    }
    private IEnumerator NextTutorialDelay()
    {
        yield return new WaitForSeconds(.5f);
        ShowMech(currentMech);

    }




    private void Pause(bool isPuased)
    {
        if (isPuased)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;

        }
    }

    private void HitJump()
    {
        ShowJump(false);


    }
    private void HitFlip(bool holding)
    {
        if (holding)
        ShowFlips(false);
    }

    private void HitDash(bool pass)
    {
        ShowDash(false);

    }
    private void HitDrop()
    {
        ShowDrop(false);
    }

    private void OnEnable()
    {
        player.events.OnJump += HitJump;
        player.events.OnFlipLeft += HitFlip;
        player.events.OnFlipRight += HitFlip;
        player.events.OnDash += HitDash;
        player.events.OnDrop += HitDrop;

        ring.ringEvent.OnCheckOrder += AddRealRingHit;
        ring.ringEvent.OnCreateNewSequence += CheckRealRingProgress;




        ring.ringEvent.tutorialRingPass += AddRingHit;

    }
    private void OnDisable()
    {
        player.events.OnJump -= HitJump;
        player.events.OnFlipLeft -= HitFlip;
        player.events.OnFlipRight -= HitFlip;
        player.events.OnDash -= HitDash;
        player.events.OnDrop -= HitDrop;

        ring.ringEvent.OnCheckOrder -= AddRealRingHit;
        ring.ringEvent.OnCreateNewSequence -= CheckRealRingProgress;




        ring.ringEvent.tutorialRingPass -= AddRingHit;



    }
}
