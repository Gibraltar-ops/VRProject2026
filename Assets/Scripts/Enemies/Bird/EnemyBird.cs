using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public GameObject target;

    private float speed = 5f;



    private BirdAttackState birdAttackState;
    private BirdCircleState birdCircleState;
    private BirdDisengageState birdDisengageState;


    void Awake()
    {
        
    }
}
