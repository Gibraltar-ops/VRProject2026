using UnityEngine;

public class DeflatedBallon : MonoBehaviour
{
    public Transform _attachmentPoint;
    public Transform _inflatableTransform;
    public float _timer;
    public bool _isInflating = false;
    Vector3 _targetScale1 = Vector3.one * 0.3f;
    Vector3 _targetScale2 = Vector3.one * 1.2f;

    public float _firstPhaseTime = 2f;
    public float _secondPhaseTime = 8f;
    public float destoryAfterTime = 4f;
    Vector3 _startScale;

    public GameObject _vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startScale = _inflatableTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        if( _isInflating)
        {
            _timer += Time.deltaTime;

            if (_timer < _firstPhaseTime)
            {
                float t = _timer/ _firstPhaseTime;
                _inflatableTransform.localScale = Vector3.Lerp(_startScale, _targetScale1, _timer);
            }
            else if( _timer > _firstPhaseTime && _timer < (_secondPhaseTime + _firstPhaseTime))
            {
                float t = (_timer - _firstPhaseTime) / _secondPhaseTime;
                _inflatableTransform.localScale = Vector3.Lerp(_targetScale1, _targetScale2, t);
            }
            else if(_timer > (_firstPhaseTime+_secondPhaseTime+destoryAfterTime) )
            {
                Instantiate(_vfx, _inflatableTransform.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            // translate in world space?
            _inflatableTransform.localPosition = _attachmentPoint.localPosition + new Vector3(_inflatableTransform.localScale.x / 2f, 0f, 0f);
        }
    }

    public void AttachToHelium()
    {
        _isInflating = true;
    }

    public void DetachFromHelium()
    {
        _isInflating = false;
    }
}
