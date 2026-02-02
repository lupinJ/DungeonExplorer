using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    
    public Transform canvas; //캔버스의 위치

    public Dictionary<string, PopupUI> popupUIDictionary = new();
    
    protected override void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ShowPanel(string uiName) 
    {
        if(popupUIDictionary.TryGetValue(uiName, out PopupUI popupUI))
        {
            popupUI.ShowPanel();
            return;
        }
        
        Debug.LogWarning($"UIManager.ShowPanel() : {uiName} is null");
    }

    public void HidePanel(string uiName) 
    {
        if(popupUIDictionary.TryGetValue(uiName, out PopupUI popupUI))
        {
            popupUI.HidePanel();
            return;
        }

        Debug.LogWarning($"UIManager.HidePanel() : {uiName} is null");
    }

    public PopupUI GetPanel(string uiName)
    {
        if(popupUIDictionary.TryGetValue(uiName, out PopupUI popupUI))
        {
            return popupUI;
        }

        Debug.LogWarning($"UIManager.GetPanel() : {uiName} is null");
        return null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas = GameObject.Find("PopupCanvas").transform;
        
        if(canvas == null)
        {
            Debug.LogWarning($"UIManager.OnSceneLoaded() : canvas is null");
        }

        PopupUI[] popupUIs = canvas.GetComponentsInChildren<PopupUI>(true); 
        popupUIDictionary.Clear();
        
        foreach (PopupUI popupUI in popupUIs)
        {

            if (!popupUIDictionary.ContainsKey(popupUI.name))
            {
                popupUIDictionary.Add(popupUI.name, popupUI); 
            }
            else
            {
                Debug.LogWarning($"중복 키 : {popupUI.name}");
            }
        }
        
        
    }
}
