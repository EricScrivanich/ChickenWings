using System.Collections.Generic;
using UnityEngine;

public class PlayerEggDrop : PlayerSystem
{
    [SerializeField] private GameObject egg_Regular;
    // [SerializeField] private GameObject egg_Bullet;
    // [SerializeField] private GameObject eggYolkPrefab;
    [SerializeField] private int eggPoolAmount = 10;
    [SerializeField] private int yolkPoolAmount = 10;

    private List<GameObject> eggPool;
    public List<GameObject> yolkPool {get; private set;}

    private int ammo;

    // Start is called before the first frame update
    void Start()
    {
        ammo = player.ID.Ammo;
        

        // Initialize eggPool and yolkPool
        eggPool = new List<GameObject>();
        yolkPool = new List<GameObject>();

        for (int i = 0; i < eggPoolAmount; i++)
        {
            GameObject egg = Instantiate(egg_Regular);
            egg.SetActive(false);
            eggPool.Add(egg);
        }

        // for (int i = 0; i < yolkPoolAmount; i++)
        // {
        //     GameObject yolk = Instantiate(eggYolkPrefab);
        //     yolk.SetActive(false);
        //     yolkPool.Add(yolk);
        // }
    
   
    }

    public GameObject GetPooledObject(List<GameObject> pool)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    private void EggDrop()
{
    if (player.ID.Ammo > 0)
    {
        GameObject egg = GetPooledObject(eggPool);

        if (egg != null)
        {
            egg.transform.position = new Vector2(_transform.position.x, _transform.position.y - .25f);
            
            
            egg.SetActive(true);
            // player.ID.globalEvents.eggVelocity?.Invoke(rb.velocity.x);
        }
            player.ID.Ammo -= 1;
            player.ID.globalEvents.OnUpdateAmmo?.Invoke();
    }
}
private void getPlayerVelocity()
{
   
}


    // private void AddAmmo(int amount)
    // {
    //     ammo += amount;
    //     player.ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    // }

    void OnEnable()
    {
        player.ID.events.OnEggDrop += EggDrop;
        // player.ID.globalEvents.OnAddAmmo += AddAmmo;
        // player.ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    }

    void OnDisable()
    {
        player.ID.events.OnEggDrop -= EggDrop;
        // player.ID.globalEvents.OnAddAmmo -= AddAmmo;
    }
}
