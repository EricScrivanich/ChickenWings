using UnityEngine;
using PathCreation;
using System.Collections;
using DG.Tweening;

public class ObjectPathFollower : MonoBehaviour
{
    private PathCreator pathCreator;
    [ExposedScriptableObject]
    [SerializeField] private AnimationCurveSO speedCurve;
    [SerializeField] private float speed = 5f;
    private AimRotation aimScript;

    private float timer;
    private float duration;
    private Rigidbody2D rb;
    private Transform parentTransform;
    [SerializeField] private float allowedRotOffset;
    [SerializeField] private float globalRotOffset;

    [SerializeField] private Vector2 startTargetFollow;
    [SerializeField] private Vector2 endTargetFollow;

    [field: SerializeField] public Transform targetFollow { get; private set; }

    public void InitializePath(int flip)
    {

        if (transform.parent.localScale.x != flip)
        {
            transform.parent.localScale = new Vector3(flip, 1, 1);
        }
        targetFollow.position = startTargetFollow * transform.localScale.y;


    }
    void Awake()
    {
        if (pathCreator == null) pathCreator = GetComponent<PathCreator>();

        this.enabled = false;
    }

    public Vector2 GetFirstPosition()
    {
        return pathCreator.path.GetPointAtDistance(0);
    }

    public void EnableAndDoPath(float dur, Rigidbody2D target, AimRotation aim)
    {

        rb = target;
        timer = 0;
        duration = dur;

        if (aim != null)
        {
            aimScript = aim;
            Vector3 normalAtDistance = pathCreator.path.GetNormal(0);
            aimScript.SetPathAngleAndAllowedOffset(Mathf.Atan2(normalAtDistance.y, normalAtDistance.x) * Mathf.Rad2Deg, globalRotOffset, allowedRotOffset);
        }


        this.enabled = true;



        targetFollow.DOLocalMove(endTargetFollow * transform.localScale.y, dur);

    }

    public void DisablePath()
    {
        DOTween.Kill(targetFollow);
        this.enabled = false;

    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        float perc = timer / duration;
        if (perc > 1f)
        {
            this.enabled = false;

        }
        else
        {
            float dist = speedCurve.ReturnValue(perc);
            Vector2 point = pathCreator.path.GetPointAtTime(dist);
            rb.MovePosition(point);
            if (aimScript != null)
            {
                Vector3 normalAtDistance = pathCreator.path.GetNormal(dist);
                aimScript.SetPathAngleAndAllowedOffset(Mathf.Atan2(normalAtDistance.y, normalAtDistance.x) * Mathf.Rad2Deg, globalRotOffset, allowedRotOffset);



            }
        }
    }
    // Update is called once per frame

}
