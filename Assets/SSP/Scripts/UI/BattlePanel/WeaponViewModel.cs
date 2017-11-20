using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponViewModel : MonoBehaviour
{
    public InventoryType type;
    [SerializeField] private Image image;
    [SerializeField] private Slider slider;
    [SerializeField] private Image background;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color deactiveColor;

    public void SetImage(Sprite _image)
    {
        image.sprite = _image;
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void SwapBackgroundColor(bool active)
    {
        background.color = active ? activeColor : deactiveColor;
    }
}
