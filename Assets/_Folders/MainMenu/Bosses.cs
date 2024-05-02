using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosses : MonoBehaviour
{
    [SerializeField] private GameObject intro;
    [SerializeField] private List<GameObject> Tank;
    [SerializeField] private List<GameObject> Pig;
    [SerializeField] private List<GameObject> DeviledEgg;
    [SerializeField] private List<GameObject> Heli;
    private List<GameObject> activeList;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Return()
    {
        Invoke("Reset", .65f);

    }
    private void Reset()
    {
        if (activeList != null)
        {
            foreach (var obj in activeList)
            {
                obj.SetActive(false);
            }
            intro.SetActive(true);
        }
    }

    public void Switch(int n)
    {

        if (activeList != null)
        {
            foreach (var obj in activeList)
            {
                obj.SetActive(false);
            }
        }
        intro.SetActive(false);


        switch (n)
        {
            case 1:
                SetListActive(Tank);
                activeList = Tank;
                break;
            case 2:
                SetListActive(Pig);
                activeList = Pig;


                break;
            case 3:
                SetListActive(DeviledEgg);
                activeList = DeviledEgg;


                break;
            case 4:
                SetListActive(Heli);
                activeList = Heli;

                break;

            default:

                break;
        }


    }

    void SetListActive(List<GameObject> l)
    {
        foreach (var obj in l)
        {
            obj.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
