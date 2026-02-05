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

        image.sprite = activeImage;
        ItemImage.sprite = item.data.image;

        if (item.data.maxCount == 1)
            text.text = "";
        else
        {
            text.text = $"{item.data.maxCount}";
        }

    }
    /// <summary>
    /// 클릭 처리 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // up down 동시
        onItemClicked.Invoke(index);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //down 되었을 시
    }

    /// <summary>
    /// 드래그 드롭 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.item == null)
            return;

        if (eventData.pointerEnter == null)
        {
            if(!(item.data.id == ItemId.None))
                onItemDrop.Invoke(index);
            Debug.Log($"Slot UI : {index}");
            return;
        }
        
        SlotUI otherSlot = eventData.pointerEnter.gameObject.GetComponent<SlotUI>();

        if (otherSlot == null)
            return;
        if (otherSlot.item == null)
            return;

        onItemSwap.Invoke(index, otherSlot.index);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}

