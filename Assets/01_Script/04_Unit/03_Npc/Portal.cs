using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PortalArg : InitData
{
    public Vector2 position;
    public bool isOpen;
    public string nextScene;
}

public class Portal : MonoBehaviour, IInItable
{
    [SerializeField] bool isOpen; // 포탈 개방 여부
    [SerializeField] string nextScene; // 목적지

    [SerializeField] GameObject OpenPortal;
    [SerializeField] GameObject ClosePortal;

    private void Awake()
    {
        PortalOpen(isOpen);
    }

    public void Initialize(InitData data = null)
    {
        if (data is not PortalArg arg)
            return;

        transform.localPosition = arg.position;
        isOpen = arg.isOpen;
        nextScene = arg.nextScene;
        PortalOpen(isOpen);
    }

    public void PortalOpen(bool open)
    {
        isOpen = open;
        OpenPortal.SetActive(open);
        ClosePortal.SetActive(!open);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isOpen)
            return;

        LoadManager.Instance.LoadSceneAsync(nextScene);
    }
   
}
