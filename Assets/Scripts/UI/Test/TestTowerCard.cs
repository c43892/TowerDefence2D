using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TestTowerCard : MonoBehaviour, IPointerDownHandler
{
    public string Type { get; set; }
    public Action<TestTowerCard> OnSelected = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSelected?.Invoke(this);
    }
}
