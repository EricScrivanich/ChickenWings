
using UnityEngine;

public class groundExplosion : MonoBehaviour
{
    [SerializeField] private GameObject eggablePigYolk;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("EggableEntity"))
        {
            var o = collider.gameObject.GetComponent<EggableCollider>();
            SetYolkTransform(o.GetTransform());

            o.KillOnGround();



            return;
        }
        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(true);
        }

    }
    public void SetYolkTransform(Transform t)
    {
        if (t != null)
        {
            var y = Instantiate(eggablePigYolk, t.position, t.rotation);
            Debug.Log("Setting yolk transform: " + t.position + " " + t.rotation);
           



            y.transform.localScale = t.lossyScale;
            y.GetComponent<PigEggableYolk>().Initialize();

        }
        else
        {
            Debug.LogWarning("EggableCollider transform is null, cannot spawn yolk.");
        }


    }
}
