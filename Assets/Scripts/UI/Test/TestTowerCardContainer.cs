using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;
using System.Linq;
using Swift;
using Swift.Math;
using UnityEngine.EventSystems;

public class TestTowerCardContainer : MonoBehaviour, IDragHandler
{
    public Transform TowerContainer;
    public BattleMapUnitModel TowerModel;
    public TestTowerCard CardModel;

    public Func<string[]> AllValidTowerTypes;
    public Func<Vec2, bool> IsValidTowerPos;

    public Func<Vector3, Vec2> WorldPos2BattleMapPos = null;
    public Func<Vec2, Vector3> BattleMapPos2UnitObjLocalPos = null;

    public TestTowerCard SelectedCard { get; set; }
    public BattleMapUnitModel SelectedTower { get; set; }

    private readonly List<TestTowerCard> AllCardsCreated = new();

    public Action<string, Vec2> OnCreatingTower = null;

    public void OnDrag(PointerEventData eventData)
    {
        if (CardModel == null)
            return;

        if (SelectedTower == null)
        {
            SelectedTower = Instantiate(TowerModel);
            SelectedTower.transform.SetParent(TowerContainer);
            SelectedTower.gameObject.SetActive(true);

            SelectedTower.Unit = Tower.Create(ConfigManager.GetTowerConfig(SelectedCard.Type));
            SelectedTower.Play("Idle");

            SelectedCard.gameObject.SetActive(false);
        }

        var wPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var towerPos = WorldPos2BattleMapPos(wPos);

        SelectedTower.GetComponent<SpriteRenderer>().color = !IsValidTowerPos(towerPos) ? Color.red : Color.white;
        SelectedTower.Unit.Pos = towerPos;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        OnCardDropped();
    }


    void OnCardDropped()
    {
        if (SelectedTower == null)
        {
            if (SelectedCard != null)
                SelectedCard.gameObject.SetActive(true);

            return;
        }

        Destroy(SelectedTower.gameObject);

        var wPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var towerPos = WorldPos2BattleMapPos(wPos);

        if (IsValidTowerPos(towerPos))
            OnCreatingTower?.Invoke(SelectedCard.Type, towerPos);
        else if (SelectedCard != null)
            SelectedCard.gameObject.SetActive(true);
    }

    public void Refresh()
    {
        AllCardsCreated.ForEach((card) => Destroy(card.gameObject));
        AllCardsCreated.Clear();

        AllValidTowerTypes().Travel(type =>
        {
            var card = Instantiate(CardModel);
            card.Type = type;
            card.transform.SetParent(transform);
            card.gameObject.SetActive(true);
            card.OnSelected += (c) => SelectedCard = c;

            AllCardsCreated.Add(card);
        });
    }
}
