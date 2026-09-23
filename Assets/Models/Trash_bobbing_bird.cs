using UnityEngine;

public class Trash : MonoBehaviour
{

    float _randomTimeOffset;
    float _amplitude;
    float _speed;
    Vector3 _startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _randomTimeOffset = Random.value;
        _amplitude = Random.value + 1.0f;
        _startPos = transform.position;
        _speed = Random.value + 1f;
    }

    // Update is called once per frame
    void Update()
    {
        float yOffset = Mathf.Sin((Time.time + _randomTimeOffset) * _speed) * _amplitude;
        transform.position = new Vector3(_startPos.x, _startPos.y + yOffset, _startPos.z);
    }
}
