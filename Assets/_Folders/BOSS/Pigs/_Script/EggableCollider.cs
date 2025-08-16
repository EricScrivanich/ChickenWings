using UnityEngine;

public class EggableCollider : MonoBehaviour
{
    [SerializeField] private IEggable eggable;

    public void GetEgged()
    {
        eggable.OnEgged();
    }
}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    // Update is called once per frame
   

