using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.Audio.ControlContext;

public class BirdAnimation : MonoBehaviour
{
    public Animator _animator;

    float time = 0f;

    int BIRD_ATTACK = Animator.StringToHash("Attack");
    int BIRD_FLYING = Animator.StringToHash("Flying");
    int BIRD_DIE = Animator.StringToHash("HitDeath");

    int _currentState;
    float _lockedTill;

    void Start()
    {
        if(_animator == null)
        {
            _animator = gameObject.GetComponent<Animator>();
        }
        _currentState = BIRD_FLYING;
    }

    // Update is called once per frame
    void Update()
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
