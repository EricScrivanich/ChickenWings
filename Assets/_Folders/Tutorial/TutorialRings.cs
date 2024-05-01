using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialRings : MonoBehaviour
{

    [SerializeField] private GameObject jumpRing;

    public static Action<int> Section;


    private void ChangeSection(int sec)
    {

    }

    private void JumpSection()
    {

    }

    private void OnEnable()
    {
        Section += ChangeSection;

    }

    private void OnDisable()
    {
        Section -= ChangeSection;

    }
    // Start is called before the first frame update

}
