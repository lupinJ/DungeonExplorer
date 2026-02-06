using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DropItemArg : InitData {

    public Vector3 position;
    public Item item;
}

public class DropItem : MonoBehaviour, IInItable, IInteractable
{
    SpriteRenderer sprite;
    Item item;

    public void Initialize(InitData idata = default)
    {
        if(idata is DropItemArg data)
        {
            transform.position = data.position;
            this.item = data.item;
            sprite.sprite = item.data.image;
        }
        else
        {
            Debug.Log("DropItem Initialize Error");
        }
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

    
}
