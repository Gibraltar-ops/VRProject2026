using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class BalloonInfo
{
    public enum BalloonState
    {
        Alive,
        Dead,
        MarkedForDeath,
        MarkedForSpawn
    }
    public Transform transform;
    public int ID;
    public float DistanceFromCenter;

    public BalloonState state;

}

[RequireComponent(typeof(AudioSource))]
public class BalloonTree : MonoBehaviour
{
    public Transform CenterPoint;
    public Transform BalloonParent;
    
    public AudioClip[] poppingSounds;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    private AudioSource source;
    public AudioMixerGroup sfxGroup;


    public List<BalloonInfo> _balloons { get; private set; }
    public int ActiveBalloons { get; private set; }
    [SerializeField] private int startBalloons = 7;

    private float _popInterval = 1.5f;
    private float _timer = 0;
    //public Transform TargetOrigin;
    public GameObject PopParticles;

    public static event Action<BalloonInfo> OnBalloonPopped;

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
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f;

        if (sfxGroup != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
        }
        
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
            if (i < startBalloons)
            {
                sorted[i].state = BalloonInfo.BalloonState.Alive;
                sorted[i].transform.gameObject.SetActive(true);
            }
            else
            {
                sorted[i].state = BalloonInfo.BalloonState.Dead;
                sorted[i].transform.gameObject.SetActive(false);
            }
            _balloons.Add(sorted[i]);
        }

        int counter = 0;
        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].state == BalloonInfo.BalloonState.Alive)
            {
                counter++;
            }
        }
        ActiveBalloons = counter;
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

    public BalloonInfo GetOutmostBalloon()
    {

        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].state == BalloonInfo.BalloonState.Alive)
            {
                _balloons[i].state = BalloonInfo.BalloonState.MarkedForDeath;
                return _balloons[i];
            }
        }
        return null;
    }

    public void SetBalloonState(BalloonInfo balloon, BalloonInfo.BalloonState state)
    {
        balloon.state = state;
    }

    public BalloonInfo GetClosestAvailableBalloon(Vector3 origin)
    {
        int closest = -1;
        float closestDist = float.MaxValue;

        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].state != BalloonInfo.BalloonState.Alive)
                continue;

            float dist = Vector3.SqrMagnitude(_balloons[i].transform.position - origin);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = i;
            }
        }
        if (closest < 0) return null;
        _balloons[closest].state = BalloonInfo.BalloonState.MarkedForDeath;
        return GetBalloonFromIndex(closest);
    }


    public void PopBalloon(BalloonInfo balloon)
    {
        if (balloon == null) return;
        balloon.state = BalloonInfo.BalloonState.Dead;
        PlayPoppingSound(poppingSounds);
        balloon.transform.gameObject.SetActive(false);
        Instantiate(PopParticles, balloon.transform.position, quaternion.identity, transform);
        OnBalloonPopped?.Invoke(balloon);
        Debug.Log("Popped Balloon with id: " + balloon.ID);
        ActiveBalloons--;

    }

    public void PopBalloon(GameObject balloonGameObject)
    {
        if (balloonGameObject == null) return;
        foreach (BalloonInfo balloonInfo in _balloons)
        {
            if (balloonInfo.transform.gameObject == balloonGameObject)
            {
                PopBalloon(balloonInfo);
                OnBalloonPopped?.Invoke(balloonInfo);
                return;
            }
        }
    }

    void HandleBalloonHit(GameObject gameObject)
    {
        if (!gameObject.activeSelf) return;
        PopBalloon(gameObject);
    }

    public BalloonInfo GetFreeBalloonSlot()
    {
        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].state == BalloonInfo.BalloonState.Dead)
            {
                _balloons[i].state = BalloonInfo.BalloonState.MarkedForSpawn;
                return _balloons[i];
            }
        }
        return null;
    }

    // Call the zero-parameter version if you just want to add a balloon instantly, using the pool of dead balloons.
    public void AddBalloon()
    {
        for (int i = 0; i < _balloons.Count; i++)
        {
            if (_balloons[i].state == BalloonInfo.BalloonState.Dead)
            {
                _balloons[i].state = BalloonInfo.BalloonState.Alive;
                _balloons[i].transform.gameObject.SetActive(true);
                ActiveBalloons++;
                return;
            }
        }
    }
    // Pass along a specific balloon if you have received and freed a slot earlier using GetFreeBalloonSlot.
    public void AddBalloon(BalloonInfo balloon)
    {
        if (balloon.state == BalloonInfo.BalloonState.MarkedForSpawn)
        {
            balloon.state = BalloonInfo.BalloonState.Alive;
            balloon.transform.gameObject.SetActive(true);
            ActiveBalloons++;
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

    private void PlayPoppingSound(AudioClip[] audioArray)
    {
        if (audioArray == null || audioArray.Length == 0) return;
        
        source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        
        int randomIndex = UnityEngine.Random.Range(0, audioArray.Length);
        AudioClip selectedClip = audioArray[randomIndex];
        
        source.PlayOneShot(selectedClip);
    }

}
