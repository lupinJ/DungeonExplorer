using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

struct DropItemArg : InitData {

    public Vector3 position;
    public Item item;
}

public class DropItem : MonoBehaviour, IInItable, IInteractable
{
    SpriteRenderer sprite;
    Item item;
    CancellationTokenSource cts;

    public void Initialize(InitData idata = default)
    {
        if(idata is DropItemArg data)
        {
            transform.position = data.position;
            this.item = data.item;
            sprite.sprite = item.Image;
        }
        else
        {
            Debug.Log("DropItem Initialize Error");
        }

        StartFloat();
    }

    void StartFloat()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        transform.DOMoveY(transform.position.y + 0.2f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .WithCancellation(cts.Token);
    }

    public void Interact()
    {
        GameManager.Instance.player.inventory.AddItem(item);
        Destroy(this.gameObject);
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

}
