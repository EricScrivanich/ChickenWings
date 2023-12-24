using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEggScript : MonoBehaviour
{
    public PlayerID ID;
    private Rigidbody2D rb;
    [SerializeField] private GameObject egg_Regular;
    // [SerializeField] private GameObject egg_Bullet;
    // [SerializeField] private GameObject eggYolkPrefab;
    [SerializeField] private int eggPoolAmount = 10;
    

    private List<GameObject> eggPool;
    
   
    private int ammo;

    // Start is called before the first frame update
    void Start()
    {
        
        ammo = ID.Ammo;
        rb = GetComponent<Rigidbody2D>();
        

        // Initialize eggPool and yolkPool
        eggPool = new List<GameObject>();
      

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
    
    ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
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
    if (ammo > 0)
    {
        GameObject egg = GetPooledObject(eggPool);

        if (egg != null)
        {
            ID.AddEggVelocity = rb.velocity.x / 2.7f;
            egg.transform.position = new Vector2(transform.position.x, transform.position.y - .25f);
            
            
            egg.SetActive(true);
            // ID.globalEvents.eggVelocity?.Invoke(rb.velocity.x);
        }
        ammo -= 1;
        ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    }
}
private void getPlayerVelocity()
{
   
}


    private void AddAmmo(int amount)
    {
        ammo += amount;
        ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    }

    void OnEnable()
    {
        ID.events.OnEggDrop += EggDrop;
        ID.globalEvents.OnAddAmmo += AddAmmo;
        ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    }

    void OnDisable()
    {
        ID.events.OnEggDrop -= EggDrop;
        ID.globalEvents.OnAddAmmo -= AddAmmo;
    }
}


