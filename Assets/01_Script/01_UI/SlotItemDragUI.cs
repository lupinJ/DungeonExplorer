using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemDragUI : UIBase
{
    RectTransform rectTransform;
    Image image;
    CancellationTokenSource ct;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        image.enabled = false;
        image.raycastTarget = false;
    }

    public void DragStart(Item item)
    {
        ct?.Cancel();
        ct?.Dispose();
        ct = new CancellationTokenSource();

        image.sprite = item.Image;
        image.enabled = true;
        DragAsync(ct.Token).Forget();
    }

    public void DragEnd() 
    {
        ct?.Cancel();
        ct?.Dispose();
        ct = null;
        image.enabled = false;
    }

    private async UniTaskVoid DragAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                rectTransform.position = Input.mousePosition;


                await UniTask.NextFrame(ct);
            }
        }
        catch (System.OperationCanceledException)
        { }
        
    }

    private void OnDisable()
    {
        DragEnd();
    }

}
