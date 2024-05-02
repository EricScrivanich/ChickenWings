using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentMovement : MonoBehaviour
{
    [SerializeField] private List<GameObject> clouds;
    public PlayerID player;
    // Start is called before the first frame update

    // Update is called once per frame

    private void Start()
    {
        if (player.constantPlayerForceBool)
        {
            foreach (var obj in clouds)
            {
                obj.SetActive(false);
            }
        }
    }
    void Update()
    {
        transform.position = new Vector2(Camera.main.transform.position.x, 0);

    }
}
