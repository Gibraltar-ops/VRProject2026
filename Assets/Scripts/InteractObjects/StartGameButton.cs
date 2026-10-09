using UnityEngine;

public class StartGameButton : MonoBehaviour
{

    [SerializeField] private GameObject MainMenuPanel;

    void Start()
    {
        GameManager.instance.SetRunning(false);
    }

    public void StartGame()
    {
        GameManager.instance.SetRunning(true);
        MainMenuPanel.SetActive(false);
        GameManager.instance.TriggerStartGameEvent();
    }
}
