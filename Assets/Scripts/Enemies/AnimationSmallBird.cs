using UnityEngine;

public class AnimationSmallBird : MonoBehaviour
{
    public Animator _animator;

    public AudioSource _audioSource;
    public AudioClip _wingFlapClip;
    public AudioClip _attackClip;
    public float _flapInterval = 0.4f;
    private float _flapTimer = 0f;

    float time = 0f;
    float soundVolume = 0.2f;

    int BIRD_ATTACK = Animator.StringToHash("Attack");
    int BIRD_HOLDING = Animator.StringToHash("Holding");
    int BIRD_FLYING = Animator.StringToHash("Flying");
    int BIRD_DIE = Animator.StringToHash("HitDeath");

    int _currentState;

    public Ammo _ammo;
    Ammo.State _BirdState; // gets updated by getting the enemyStateEnum 
    Ammo.State _currentBirdState; // only used for comparison

    void Start()
    {
        if (_animator == null)
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        if (_audioSource == null)
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }

        _currentState = BIRD_FLYING;
        _BirdState = Ammo.State.Alive;
        _currentBirdState = Ammo.State.Alive;
    }

    // Update is called once per frame
    void Update()
    {
        _BirdState = _ammo.GetBirdStateEnum();
        if (_BirdState != _currentBirdState)
        {
            switch (_BirdState)
            {
                case Ammo.State.Alive:
                    PlayFlying();
                    _currentBirdState = _BirdState;
                    break;
                case Ammo.State.Ammo:
                    //PlayAttackSound();
                    //PlayAttack();
                    _currentBirdState = _BirdState;
                    break;
                case Ammo.State.Dead:
                    PlayDie();
                    _currentBirdState = _BirdState;
                    break;
                default:
                    Debug.Log("Missing enemy state in animator!");
                    break;
            }
        }

        if (_currentBirdState == Ammo.State.Alive)
        {
            _flapTimer += Time.deltaTime;

            if (_flapTimer >= _flapInterval)
            {
                PlayWingFlapSound();
                _flapTimer = 0f;
            }
        }

    }

    private void PlayAttackSound()
    {
        if (_audioSource != null && _attackClip != null)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f) * soundVolume;
            _audioSource.volume = 1.0f;
            _audioSource.PlayOneShot(_attackClip);
        }
    }

    private void PlayWingFlapSound()
    {
        if (_audioSource != null && _wingFlapClip != null)
        {
            _audioSource.pitch = Random.Range(0.85f, 1.15f) * soundVolume;
            _audioSource.volume = Random.Range(0.8f, 1.0f) * soundVolume;

            _audioSource.PlayOneShot(_wingFlapClip);
        }
    }

    public void PlayHolding()
    {
        _currentState = BIRD_HOLDING;
        _animator.CrossFade(_currentState, 0.5f, 0);
    }
    public void PlayFlying()
    {
        _currentState = BIRD_FLYING;
        _animator.CrossFade(_currentState, 0.5f, 0);
    }

    public void PlayDie()
    {
        _currentState = BIRD_DIE;
        _animator.CrossFade(_currentState, 0.1f, 0);
    }
}
