using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class SlotUI : UIBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    // 클릭(사용), 스위칭, 버림 event 필요
    [SerializeField] InventoryUI inventoryUI;

    public int index;
    public Image image;

    public Image ItemImage;
    public TextMeshProUGUI text;

    public Item item;
    public Sprite activeImage;
    public Sprite inActiveImage;

    public event Action<int> onItemClicked;
    public event Action<int, int> onItemSwap;
    public event Action<int> onItemDrop;
    public event Action<Item> onDragStart;
    public event Action onDragEnd;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// slot 갱신 함수
    /// </summary>
    public void RefreshUI()
    {
        if (item == null)
        {
            image.sprite = inActiveImage;
            ItemImage.enabled = false;
            text.text = "";
            return;
        }
        else if (item.data.id == ItemId.None)
        {
            ItemImage.enabled = false;
        }
        else
        {
            ItemImage.enabled = true;
        }

        // Item 표시
        image.sprite = activeImage;
        ItemImage.sprite = item.data.image;

        // text 표시
        if (item is CountableItem cItem)
        {
            text.text = $"{cItem.Count}";
        }
        else if(item is WeaponItem WItem)
        {
            if (WItem.Is_equip)
                text.text = "E";
            else
                text.text = "";
        }
        else
            text.text = "";

    }

    /// <summary>
    /// 클릭 처리 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // up down 동시
        onItemClicked?.Invoke(index);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //down 되었을 시
        if (item == null)
            return;
        if (item.data.id == ItemId.None)
            return;

        onDragStart?.Invoke(this.item);
    }

    /// <summary>
    /// 드래그 드롭 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.item == null)
            return;

        onDragEnd?.Invoke();

        if (eventData.pointerEnter == null)
        {
            if(!(item.data.id == ItemId.None))
                onItemDrop?.Invoke(index);
            return;
        }
        
        SlotUI otherSlot = eventData.pointerEnter.gameObject.GetComponent<SlotUI>();

        if (otherSlot == null)
            return;
        if (otherSlot.item == null)
            return;

        onItemSwap?.Invoke(index, otherSlot.index);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(1f, 1f, 0.64f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }

    private void OnDisable()
    {
        image.color = Color.white;
    }
}

