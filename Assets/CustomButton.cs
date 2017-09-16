using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler
{
	public CustomButton()
	{

	}

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(" Was Clicked.");
    }
}
