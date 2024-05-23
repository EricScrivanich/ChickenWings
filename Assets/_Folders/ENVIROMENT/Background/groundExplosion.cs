
using UnityEngine;

public class groundExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collider)
    {
        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(true);
        }
    }
}
