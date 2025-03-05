using UnityEngine;

public class NinjaPigDashClone : MonoBehaviour
{
    [SerializeField] private Transform[] parts;
    private SpriteRenderer[] renderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        renderers = new SpriteRenderer[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            renderers[i] = parts[i].GetComponent<SpriteRenderer>();
        }
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void Recolor(Color col)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = col;
        }
    }

    public void Activate(Transform[] p, SpriteRenderer[] sprites, Color col)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (i == 0)
            {
                transform.position = p[i].position;
                transform.localScale = p[i].localScale;
            }

            if (i < sprites.Length)
            {
                renderers[i].sprite = sprites[i].sprite;
                renderers[i].sortingOrder = sprites[i].sortingOrder;
            }


            if (i > 0)
            {
                parts[i].transform.localPosition = p[i].localPosition;
                parts[i].transform.localScale = p[i].localScale;
                parts[i].transform.localRotation = p[i].localRotation;
            }
            Recolor(col);
            gameObject.SetActive(true);



        }

    }
}
