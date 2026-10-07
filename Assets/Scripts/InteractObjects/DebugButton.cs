using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DebugButton : MonoBehaviour
{
    //XRPokeInteractor interactor;
    public TMP_Text debug_text;

    void Start()
    {
        //interactor = GetComponent<XRPokeInteractor>();
    }

    public void HoverEntered()
    {
        debug_text.text = "Hover Enter";
    }
    public void HoverExit()
    {
        debug_text.text = "Hover Exit";
    }
    public void SelectEntered()
    {
        debug_text.text = "Select";
    }
    public void SelectExited()
    {
        debug_text.text = "Select Exited";
    }
    public void UIHover()
    {
        debug_text.text = "UI Hover Exit";
    }
    public void UIHoverExit()
    {
        debug_text.text = "UI Hover Exit";
    }
}
