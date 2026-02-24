using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour, IInItable, IPoolable
{
    [SerializeField] RoomDataSO data;
    [SerializeField] Transform monsterParent;
    [SerializeField] Transform blockParent;
    [SerializeField] Transform npcParent;

    Collider2D colider;

    List<GameObject> monsterList;
    List<GameObject> npcList;

    private void Awake()
    {
        colider = GetComponent<Collider2D>();
        monsterList = new List<GameObject>();
        npcList = new List<GameObject>();

        Initialize();
    }

    public void Initialize(InitData data = null)
    {
        colider.enabled = this.data.isEncount;

        foreach (var box in this.data.boxData)
        {
            GameObject obj = PoolManager.Instance.Instanciate(AddressKeys.TreasureBox, box);
            obj.transform.SetParent(npcParent, false);
            npcList.Add(obj);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
       
        SetBlock(true);
        MonsterEncounter();
        colider.enabled = false;
    }

    public virtual void MonsterEncounter()
    {
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

    public virtual void EncounterEnd()
    {
        SetBlock(false);
    }

    public void SetBlock(bool check)
    {
        blockParent.gameObject.SetActive(check);
    }

    public void OnSpawn()
    {
        monsterList.Clear();
        npcList.Clear();
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

        monsterList.Clear();
        npcList.Clear();
    }
}
