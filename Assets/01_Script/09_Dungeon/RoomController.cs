using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour, IInItable, IPoolable
{
    [SerializeField] RoomDataSO data;

    [SerializeField] Transform monsterParent;
    [SerializeField] Transform blockParent;
    [SerializeField] Transform npcParent;
    [SerializeField] Transform portalParent;

    Collider2D colider;

    List<GameObject> monsterList;
    List<GameObject> npcList;
    List<Portal> portalList;

    private void Awake()
    {
        colider = GetComponent<Collider2D>();
        monsterList = new List<GameObject>();
        npcList = new List<GameObject>();
        portalList = new List<Portal>();

        Initialize();
    }

    public void Initialize(InitData data = null)
    {
        colider.enabled = this.data.isEncount;
        CreateObject();
    }

    /// <summary>
    /// 오브젝트 생성
    /// </summary>
    private void CreateObject()
    {
        foreach (var box in this.data.boxData)
        {
            GameObject obj = PoolManager.Instance.Instanciate(AddressKeys.TreasureBox, box);
            obj.transform.SetParent(npcParent, false);
            npcList.Add(obj);
        }

        foreach (var arg in this.data.portalData)
        {
            GameObject obj = PoolManager.Instance.Instanciate(AddressKeys.Door, arg);
            obj.transform.SetParent(portalParent, false);
            Portal portal = obj.GetComponent<Portal>();
            portalList.Add(portal);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
       
        colider.enabled = false;
        MonsterEncounter();
    }

    /// <summary>
    /// 몬스터 인카운터
    /// </summary>
    public virtual void MonsterEncounter()
    {
        // 길을 막는다.
        SetBlock(true);

        // 포탈을 막는다.
        foreach (var portal in portalList)
        {
            portal.PortalOpen(false);
        }

        // 몬스터 소환
        foreach (var monData in this.data.spawnData)
        {
            GameObject obj = PoolManager.Instance.Instanciate(monData.id, new MonsterArg { position = monData.localPosition});
            obj.transform.SetParent(monsterParent, false);

            Monster mon = obj.GetComponent<Monster>();
            mon.OnDie -= MonsterDie;
            mon.OnDie += MonsterDie;

            monsterList.Add(obj);
        }
    }

    /// <summary>
    ///  몬스터 카운팅
    /// </summary>
    /// <param name="mon"></param>
    private void MonsterDie(Monster mon)
    {
        if (mon == null)
            return;

        mon.OnDie -= MonsterDie;

        if (monsterList.Contains(mon.gameObject))
        {
            monsterList.Remove(mon.gameObject);
        }

        if (monsterList.Count <= 0)
        {
            EncounterEnd();
        }
    }

    /// <summary>
    /// 인카운터 종료
    /// </summary>
    public virtual void EncounterEnd()
    {
        foreach(var portal in portalList)
        {
            portal.PortalOpen(true);
        }
        SetBlock(false);
    }

    /// <summary>
    /// 길목을 막는 벽 활성화
    /// </summary>
    /// <param name="check"></param>
    public void SetBlock(bool check)
    {
        blockParent.gameObject.SetActive(check);
    }

    public void OnSpawn()
    {
        monsterList.Clear();
        npcList.Clear();
        portalList.Clear();

        SetBlock(false);
        colider.enabled = false;
    }

    public void OnDespawn()
    {
        foreach(var mon in monsterList)
        {
            PoolManager.Instance.Destroy(mon);
        }

        foreach(var npc in  npcList)
        {
            PoolManager.Instance.Destroy(npc);
        }

        foreach (var portal in portalList)
        {
            PoolManager.Instance.Destroy(portal.gameObject);
        }

        monsterList.Clear();
        npcList.Clear();
        portalList.Clear();
    }
}
