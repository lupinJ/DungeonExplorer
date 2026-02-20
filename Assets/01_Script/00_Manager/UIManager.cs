using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum UIName
{
    Inventory,
    PlayerHpBar,
}

public class UIManager : Singleton<UIManager>
{
    
    public Transform canvas; //캔버스의 위치

    public Dictionary<string, UIBase> UIDic = new();

    public void ShowPanel(string uiName) 
    {
        if(UIDic.TryGetValue(uiName, out UIBase popupUI))
        {
            popupUI.gameObject.SetActive(true);
            return;
        }

        if(AssetManager.Instance.TryGetAsset<GameObject>(uiName, out GameObject obj))
        {
            UIBase uiBase = Instantiate(obj).GetComponent<UIBase>() as UIBase;
            
            if(uiBase != null)
            {
                UIDic.Add(uiName, uiBase);
                uiBase.transform.parent = canvas;
                uiBase.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"is not UIBase {uiName}");
            }
                
        }
        else
        {
            Debug.Log($"failed Get Asset {uiName}");
        }

    }
    public void HidePanel(string uiName) 
    {
        if(UIDic.TryGetValue(uiName, out UIBase popupUI))
        {
            popupUI.gameObject.SetActive(false);
            return;
        }

        Debug.LogWarning($"UIManager.HidePanel() : {uiName} is null");
    }

    /// <summary>
    /// UI 반환 함수 (string 접근)
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="panel"></param>
    /// <returns></returns>
    public bool TryGetPanel(string uiName, out UIBase panel)
    {
        UIBase uiBase = null;

        if(UIDic.TryGetValue(uiName, out uiBase))
        {
            panel = uiBase;
            return true;
        }

        if (AssetManager.Instance.TryGetAsset<GameObject>(uiName, out GameObject obj))
        {
            uiBase = Instantiate(obj).GetComponent<UIBase>() as UIBase;

            if (uiBase != null)
            {
                UIDic.Add(uiName, uiBase);
                uiBase.transform.parent = canvas;
                panel = uiBase;
                return true;
            }
            else
            {
                Debug.Log($"is not UIBase {uiName}");
            }

        }
        else
        {
            Debug.Log($"failed Get Asset {uiName}");
        }

        panel = uiBase;    
        return false;
    }

    /// <summary>
    /// UI 반환 함수 (enum 접근)
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="panel"></param>
    /// <returns></returns>
    public bool TryGetPanel(UIName uiName, out UIBase panel)
    {
        if (DataManager.Instance.TryGetUIPath(uiName, out string key))
        {
           return TryGetPanel(key, out panel);
        }

        panel = null;
        return false;
    }
    
    /// <summary>
    /// Scene에 진입 시 UI 캐싱 처리
    /// </summary>
    /// <param name="list"></param>
    public void OnSceneLoadedCreate(List<string> list)
    {
        OnSceneUnLoadDestroy();

        // canvas 생성
        AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.GameCanvas, out GameObject canvasObj);
        canvas = Instantiate(canvasObj).transform;

        // UI 생성
        foreach (string key in list)
        {
            if(AssetManager.Instance.TryGetAsset<GameObject>(key, out GameObject obj))
            {
                UIBase uiBase = Instantiate(obj).GetComponent<UIBase>() as UIBase;
                
                if (uiBase != null)
                {
                    uiBase.transform.SetParent(canvas, false);
                    UIDic.Add(key, uiBase);
                    
                }
                else
                {
                    Debug.Log($"is not UIBase {key}");
                }
            }
            else
            {
                Debug.Log($"failed Get Asset {key}");
            }
        }
    }

    /// <summary>
    /// Scene 종료시 제거 처리
    /// </summary>
    public void OnSceneUnLoadDestroy()
    {
        foreach(var pair in UIDic)
        {
            if(pair.Value != null)
                Destroy(pair.Value.gameObject);
        }

        UIDic.Clear();

        if (canvas != null)
        {
            Destroy(canvas.gameObject);
            canvas = null;
        }
    }

    /// <summary>
    /// UI 전체 초기화
    /// </summary>
    public void OnSceneLoadedInit()
    {
        foreach(var pair in UIDic)
        {
            if(pair.Value is IInItable initAction)
            {
                initAction.Initialize();
            }
        }
    }
}
