using TMPro;
using UnityEngine;

public class BirdDebug : MonoBehaviour
{
    public TextMeshPro debugText;
    public EnemyBird _enemyBird;
    public string _debugString = string.Empty;
    public Transform _faceCameraTransform;

    

    // Update is called once per frame
    void Update()
    {
        _debugString = _enemyBird.GetCurrentStateDebug() + "\n" + _enemyBird.GetCurrentAlive();
        debugText.text = _debugString;

        _faceCameraTransform.transform.LookAt(Camera.main.transform);
    }
}
