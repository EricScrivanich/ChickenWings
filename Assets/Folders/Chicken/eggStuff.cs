using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggStuff : MonoBehaviour
{
    private PlayerMovement pm;
    private eggMovement em;

    [SerializeField] private  GameObject egg_Regular;
    [SerializeField] private  GameObject egg_Bullet;

    private GameObject eggDropSpawn;
    private Transform _transform;

    private StatsManager statsMan;

    public enum AmmoType
    {
        Regular,
        Explosive,
        Bullet,
        Incendiary
    }

    public AmmoType currentAmmoType;

    void Start()
    {
        _transform = GetComponent<Transform>();
        var spawnX = _transform.position.x;
        var spawnY = _transform.position.y;
        pm = gameObject.GetComponent<PlayerMovement>();
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
    }

    // Update is called once per frame
   
    public void DropEgg()
    {
        int ammo = statsMan.GetAmmo(); 

        if (pm.disableKeyPress)
        {
            return;
        }
  
       if (ammo > 0 && currentAmmoType == AmmoType.Regular)
       {

    GameObject newEgg = Instantiate(egg_Regular, new Vector3(_transform.position.x,_transform.position.y - .2f,0 ), Quaternion.identity);
    statsMan.UseAmmo(1);
       }
       
       if (ammo > 0 && currentAmmoType == AmmoType.Bullet)
       {

    GameObject newEgg = Instantiate(egg_Bullet, new Vector3(_transform.position.x,_transform.position.y - .2f,0 ), Quaternion.identity);
    statsMan.UseAmmo(1);
       }
    }
    
    private void KeyPress()
    {
    if (pm.disableKeyPress)
        {
            return;
        }
    
   

    
}
}


