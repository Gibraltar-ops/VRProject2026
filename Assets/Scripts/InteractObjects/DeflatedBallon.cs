using System.Collections;
using UnityEngine;

public class DeflatedBallon : MonoBehaviour
{

    public Transform _attachmentPoint;
    public Transform _inflatableTransform;
    public float _timer;
    public bool _isInflating = false;
    private bool _interactable = true;
    private BalloonTree PlayerBalloonTree;
    Vector3 _targetScale1 = Vector3.one * 0.3f;
    Vector3 _targetScale2 = Vector3.one * 1.2f;

    public float _firstPhaseTime = 2f;
    public float _secondPhaseTime = 8f;
    public float destoryAfterTime = 4f;
    Vector3 _startScale;
    State state;

    enum State
    {
        Deflated,
        Attached,
        Complete
    }

    public GameObject _vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startScale = _inflatableTransform.localScale;
        state = State.Deflated;
        PlayerBalloonTree = GameManager.instance.PlayerBaloonTree;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_interactable) return;
        if (_isInflating)
        {
            _timer += Time.deltaTime;

            if (_timer < _firstPhaseTime)
            {
                float t = _timer / _firstPhaseTime;
                _inflatableTransform.localScale = Vector3.Lerp(_startScale, _targetScale1, _timer);
            }
            else if (_timer > _firstPhaseTime && _timer < (_secondPhaseTime + _firstPhaseTime))
            {
                float t = (_timer - _firstPhaseTime) / _secondPhaseTime;
                _inflatableTransform.localScale = Vector3.Lerp(_targetScale1, _targetScale2, t);
            }
            else if (_timer > (_firstPhaseTime + _secondPhaseTime) && state == State.Deflated)
            {
                state = State.Complete;
            }
            else if (_timer > (_firstPhaseTime + _secondPhaseTime + destoryAfterTime))
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

    public void OnRelease()
    {
        if (state == State.Deflated)
        {

            StartCoroutine(RapidDeflate());
            _isInflating = false;
            Debug.Log("Dropped Half-filled balloon.");
        }
        else if (state == State.Complete)
        {
            BalloonInfo slot = PlayerBalloonTree.GetFreeBalloonSlot();
            _isInflating = false;
            StartCoroutine(AddBalloonToCanopy(slot));
            Debug.Log("Dropped Full Balloon.");
        }
    }

    IEnumerator RapidDeflate()
    {
        _interactable = false;
        state = State.Deflated;
        float timeToDeflate = 0.2f;
        float timer = 0f;
        float t;
        Vector3 targetScale = _startScale;
        Vector3 originScale = new Vector3(_inflatableTransform.localScale.x, _inflatableTransform.localScale.y, _inflatableTransform.localScale.z);

        while (timer < timeToDeflate)
        {
            t = timer / timeToDeflate;
            _inflatableTransform.localScale = Vector3.Lerp(originScale, targetScale, t);
            _inflatableTransform.localPosition = _attachmentPoint.localPosition + new Vector3(_inflatableTransform.localScale.x / 2f, 0f, 0f);
            timer += Time.deltaTime;
            yield return null;
        }
        _timer = 0f;
        _interactable = true;
    }

    IEnumerator AddBalloonToCanopy(BalloonInfo slot)
    {
        float moveTime = 2f;
        float timer = 0f;
        float t;

        Vector3 startPosition = new Vector3(_inflatableTransform.position.x, _inflatableTransform.position.y, _inflatableTransform.position.z);
        Vector3 endPosition = slot.transform.position;

        while (timer < moveTime)
        {
            t = timer / moveTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
        PlayerBalloonTree.AddBalloon(slot);
        Destroy(gameObject);
    }
}
