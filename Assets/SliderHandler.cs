using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string HelpTitleText;
    [TextArea(3, 10)]
    public string HelpDescriptionText;
    public Sprite HelpImageSprite;
    public SettingsMenu menu;
    public void OnPointerEnter(PointerEventData eventData)
    {
        YourVoidFunction();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    // Define your void function here
    void YourVoidFunction()
    {
        if (!menu) menu = GameObject.FindObjectOfType<SettingsMenu>();
        menu.DisplayHelpMenu(HelpTitleText, HelpDescriptionText, HelpImageSprite);
    }
}
