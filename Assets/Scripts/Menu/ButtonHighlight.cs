using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    Button thisButton;
    [SerializeField] Image[] targetImages;

    private void Start()
    {
        thisButton = GetComponent<Button>();
    }



    public void ToggleImages(bool trigger)
    {
        if (trigger)
        {
            foreach (Image image in targetImages)
            {
                image.color = Color.white;
            }
        }
        else
        {
            foreach (Image image in targetImages)
            {
                image.color = new Color(0, 0, 0, 0);
            }
        }
    }
}
