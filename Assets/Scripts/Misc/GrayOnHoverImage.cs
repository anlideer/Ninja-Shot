using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrayOnHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }
}
