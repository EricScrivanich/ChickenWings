
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
            var t = o.GetTransform();
            if (t != null)
            {
                var y = Instantiate(eggablePigYolk, t.position, t.rotation);
                o.KillOnGround();

                y.transform.localScale = t.lossyScale;
                y.GetComponent<PigEggableYolk>().Initialize();
            }
            else
            {
                Debug.LogWarning("EggableCollider transform is null, cannot spawn yolk.");
            }



            return;
        }
        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(true);
        }

    }
}
