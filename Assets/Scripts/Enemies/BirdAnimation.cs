using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.Audio.ControlContext;

public class BirdAnimation : MonoBehaviour
{
    
    public Animator _animator;

    public AudioSource _audioSource;
    public AudioClip _wingFlapClip;
    public AudioClip _attackClip;
    public float _flapInterval = 0.4f;
    private float _flapTimer = 0f;
    
    float time = 0f;

    int BIRD_ATTACK = Animator.StringToHash("Attack");
    int BIRD_FLYING = Animator.StringToHash("Flying");
    int BIRD_DIE = Animator.StringToHash("HitDeath");

    int _currentState;
    float _lockedTill;

    public EnemyBird _enemyBird;
    EnemyBird.BirdStateEnum _enemyBirdState; // gets updated by getting the enemyStateEnum 
    EnemyBird.BirdStateEnum _currentEnemyBirdState; // only used for comparison

    void Start()
    {
        if(_animator == null)
        {
            _animator = gameObject.GetComponent<Animator>();
        }
        
        if (_audioSource == null)
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }
        
        _currentState = BIRD_FLYING;
        _enemyBirdState = EnemyBird.BirdStateEnum.Flying;
        _currentEnemyBirdState = EnemyBird.BirdStateEnum.Flying;
    }

    // Update is called once per frame
    void Update()
    {
        _enemyBirdState = _enemyBird.GetBirdStateEnum();
        if (_enemyBirdState != _currentEnemyBirdState) 
        {
            switch (_enemyBirdState) 
            {
                case EnemyBird.BirdStateEnum.Flying:
                    PlayFlying();
                    _currentEnemyBirdState = _enemyBirdState;
                    break;
                case EnemyBird.BirdStateEnum.Attacking:
                    PlayAttackSound();
                    PlayAttack();
                    _currentEnemyBirdState = _enemyBirdState;
                    break;
                case EnemyBird.BirdStateEnum.Dead:
                    PlayDie();
                    _currentEnemyBirdState = _enemyBirdState;
                    break;
                default:
                    Debug.Log("Missing enemy state in animator!");
                    break;
            }
        }
        
        if (_currentEnemyBirdState == EnemyBird.BirdStateEnum.Flying)
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
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.volume = 1.0f;
            _audioSource.PlayOneShot(_attackClip);
        }
    }
    
    private void PlayWingFlapSound()
    {
        if (_audioSource != null && _wingFlapClip != null)
        {
            _audioSource.pitch = Random.Range(0.85f, 1.15f);
            _audioSource.volume = Random.Range(0.8f, 1.0f);

            _audioSource.PlayOneShot(_wingFlapClip);
        }
    }

    private void TestAnimations()
    {
        time += Time.deltaTime;

        if (time > 3f)
        {
            int anim_temp = Random.Range(0, 3);
            switch (anim_temp)
            {
                case 0:
                    PlayAttack();
                    break;
                case 1:
                    PlayDie();
                    break;
                case 2:
                    PlayFlying();
                    break;
                default:
                    Debug.Log("Animation somthing went wrong");
                    break;
            }
            time = 0f;
        }
    }

    public void PlayAttack()
    {
        _currentState = BIRD_ATTACK;
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


    private int GetState() // Tarodev program like a animator logic
    {
        if (Time.time < _lockedTill) return _currentState;

        // Priorities
        //if (InputManager.JumpIsHeld) return OnGround_BeforeJump;
        //if (InputManager.GrindWasPressed) return LockState(Grinding, 0.5f);
        //if (InputManager.FlipWasPressed) return LockState(FlipKickFlip, 0.5f);
        //if (InputManager.GrabWasPressed) return LockState(GrabRight, 0.5f);
        //if (InputManager.LandWasPressed) return LockState(HardLanding, 0.5f);
        //if (InputManager.DebugWasPressed) return LockState(Wallride, 0.5f);
        //if (InputManager.Direction.y < 0f) return Manual;

        return BIRD_FLYING;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }

}
