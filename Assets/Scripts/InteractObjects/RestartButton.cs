using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        if (!button) return;
        button.onClick.AddListener(() =>
        {
            GameManager.instance.ReloadScene();

        });
    }
}
