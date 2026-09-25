using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BalloonInfo
{
    public Transform transform;
    public int ID;
    public float DistanceFromCenter;
    public bool active;
    public bool available;
}
public class BalloonTree : MonoBehaviour
{
    public Transform CenterPoint;
    public Transform BalloonParent;
    private List<BalloonInfo> _balloons;
    private float _popInterval = 1.5f;
    private float _timer = 0;
    //public Transform TargetOrigin;
    public GameObject PopParticles;

    public static event Action<GameObject> OnBalloonPopped;

    void OnEnable()
    {
        BasicGun.OnGunHit += HandleBalloonHit;
    }
    void OnDisable()
    {
        BasicGun.OnGunHit -= HandleBalloonHit;
    }

    void Start()
    {
        List<BalloonInfo> unsorted = new();
        _balloons = new();
        foreach (Transform balloon in BalloonParent)
        {
            BalloonInfo newBalloon = new();

            newBalloon.transform = balloon;
            newBalloon.DistanceFromCenter = GetDistanceFromCenter(balloon, CenterPoint);
            unsorted.Add(newBalloon);
        }

        List<BalloonInfo> sorted = unsorted.OrderBy(dist => dist.DistanceFromCenter).ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].ID = i;
            sorted[i].active = true;
            sorted[i].available = true;
            _balloons.Add(sorted[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        _timer += dt;

        if (_timer >= _popInterval)
        {
            //PopBalloon(GetClosestAvailableBalloon(TargetOrigin.position));
            //PopBalloon(GetOutmostBalloon());
            _timer = 0f;
        }


    }

    BalloonInfo GetOutmostBalloon()
    {

        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].active && _balloons[i].available)
            {
                return _balloons[i];
            }
        }
        return null;
    }

    public BalloonInfo GetClosestAvailableBalloon(Vector3 origin)
    {
        int closest = -1;
        float closestDist = float.MaxValue;

        for (int i = 0; i < _balloons.Count; i++)
        {
            if (!_balloons[i].active || !_balloons[i].available)
                continue;

            float dist = Vector3.SqrMagnitude(_balloons[i].transform.position - origin);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = i;
            }
        }
        if (closest < 0) return null;
        _balloons[closest].available = !_balloons[closest].available;
        return GetBalloonFromIndex(closest);
    }

    void PopBalloon(int index)
    {
        _balloons[index].active = false;
        _balloons[index].available = false;
        _balloons[index].transform.gameObject.SetActive(false);
        Instantiate(PopParticles, _balloons[index].transform.position, quaternion.identity, transform);
        OnBalloonPopped?.Invoke(_balloons[index].transform.gameObject);

    }

    public void PopBalloon(BalloonInfo balloon)
    {
        if (balloon == null) return;
        balloon.active = false;
        balloon.available = false;
        balloon.transform.gameObject.SetActive(false);
        Instantiate(PopParticles, balloon.transform.position, quaternion.identity, transform);
        OnBalloonPopped?.Invoke(balloon.transform.gameObject);
        Debug.Log("Popped Balloon with id: " + balloon.ID);

    }

    public void PopBalloon(GameObject balloonGameObject)
    {
        if (balloonGameObject == null) return;
        foreach (BalloonInfo balloonInfo in _balloons)
        {
            if (balloonInfo.transform.gameObject == balloonGameObject)
            {
                PopBalloon(balloonInfo);
                OnBalloonPopped?.Invoke(balloonGameObject);
                return;
            }
        }
    }

    void HandleBalloonHit(GameObject gameObject)
    {
        if (!gameObject.activeSelf) return;
        PopBalloon(gameObject);
    }

    void AddBalloon()
    {
        for (int i = 0; i < _balloons.Count; i++)
        {
            if (!_balloons[i].active)
            {
                _balloons[i].active = true;
                _balloons[i].available = true;
                _balloons[i].transform.gameObject.SetActive(true);
                return;
            }
        }
    }

    BalloonInfo GetBalloonFromIndex(int index)
    {
        if (index < 0) return null;
        return _balloons[index];
    }

    float GetDistanceFromCenter(Transform balloon, Transform center)
    {
        return Vector3.Distance(balloon.position, center.position);
    }


}
