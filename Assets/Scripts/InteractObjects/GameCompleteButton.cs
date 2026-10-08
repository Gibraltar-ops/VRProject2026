using UnityEngine;

public class GameCompleteButton : MonoBehaviour
{
    [SerializeField] private GameObject FinishGamePanel;

    void Start()
    {
        FinishGamePanel.SetActive(false);
    }

    void OnEnable()
    {
        GameManager.instance.GameComplete += ActivatePanel;
    }
    void OnDisable()
    {
        GameManager.instance.GameComplete -= ActivatePanel;
    }

    private void ActivatePanel()
    {
        FinishGamePanel.SetActive(true);
    }

    private void DeactivatePanel()
    {
        FinishGamePanel.SetActive(false);
    }

    public void FinishGame()
    {
        GameManager.instance.ReloadScene();
    }
}
