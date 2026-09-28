using UnityEngine;

public class EnemyParentClass : MonoBehaviour
{

    public int _hitPoints;

    public BalloonTree _balloonTree;
    public BalloonInfo target;

    virtual public void Awake()
    {
        
    }

    virtual public void Start()
    {
        _balloonTree = GameManager.instance.PlayerBaloonTree;
    }

    virtual public void Update()
    {
        
    }

    void OnEnable()
    {
        BalloonTree.OnBalloonPopped += HandleTargetChange;
    }

    void OnDisable()
    {
        BalloonTree.OnBalloonPopped -= HandleTargetChange;
    }

    protected void SetInitTarget()
    {
        target = _balloonTree.GetOutmostBalloon();
    }

    protected void GetNewTarget()
    {
        target = _balloonTree.GetClosestAvailableBalloon(gameObject.transform.position);
    }

    private void HandleTargetChange(BalloonInfo balloon)
    {
        if(balloon == target)
        {
            GetNewTarget();
        }
        
    }


}
