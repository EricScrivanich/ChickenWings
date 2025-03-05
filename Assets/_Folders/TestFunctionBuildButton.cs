using UnityEngine;
using UnityEngine.UI;

public class TestFunctionBuildButton : MonoBehaviour
{
    public EnemyPositioningLogic logic;
    public Image img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (EnemyPositioningLogic.useNewMove)
        //     img.color = Color.white;
        // else img.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PressButton()
    {
        // EnemyPositioningLogic.useNewMove = !EnemyPositioningLogic.useNewMove;
        // if (EnemyPositioningLogic.useNewMove)
        //     img.color = Color.white;
        // else img.color = Color.blue;

    }
}
