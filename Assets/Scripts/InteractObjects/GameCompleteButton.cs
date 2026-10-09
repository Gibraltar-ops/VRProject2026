using UnityEngine;

public class GameCompleteButton : MonoBehaviour
{
    [SerializeField] private GameObject FinishGamePanel;

    void Start()
    {
        //FinishGamePanel.SetActive(false);
        //GameManager.instance.OnGameComplete += ActivatePanel;
    }

    void OnEnable()
    {
    }
    void OnDisable()
    {
        //GameManager.instance.OnGameComplete -= ActivatePanel;
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
