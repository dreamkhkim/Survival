using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;



public enum ItemType
{
    Rifle, Pistal, ShotGun, Health, None
}

public abstract class ItemStrategy
{
    protected Item item;

    public ItemStrategy(Item item)
    {
        this.item = item;
    }

    public abstract void ItemUse();
}

public class RifleItem : ItemStrategy
{
    public RifleItem(Item item) : base(item)
    {
        this.item = item;
    }

    public override void ItemUse()
    {

        item.owner.Owner.rifle.weaponData.carryBullet += item.GetValue;

    }
}

public class PistalItem : ItemStrategy
{
    public PistalItem(Item item) : base(item)
    {
        this.item = item;
    }

    public override void ItemUse()
    {

        item.owner.Owner.pistal.weaponData.carryBullet += item.GetValue;

    }
}

public class ShotGunItem : ItemStrategy
{
    public ShotGunItem(Item item) : base(item)
    {
        this.item = item;
    }

    public override void ItemUse()
    {

        item.owner.Owner.shotGun.weaponData.carryBullet += item.GetValue;

    }
}

public class HealthItem : ItemStrategy
{
    public HealthItem(Item item) : base(item)
    {
        this.item = item;
    }

    public override void ItemUse()
    {
        item.owner.Owner.PlayerHp += item.GetValue;
    }
}


public class Item : EnemyPool
{
    //[SerializeField]
    public ItemType type;


    public IPlayerInteractable owner;
    

    [SerializeField]
    private int itemValue;
    public int GetValue
    {
        get => itemValue;
        set => itemValue = value;
    }

    [SerializeField]
    private ItemStrategy strategy;

    public ItemType GetItemNum(ItemType value)
    {
        type = value;

        return type;
    }

    private void Awake()
    {
       
        switch(type)
        {
            case ItemType.Rifle:
                strategy = new RifleItem(this);
                break;
            case ItemType.Pistal:
                strategy = new PistalItem(this);
                break;
            case ItemType.ShotGun:
                strategy = new ShotGunItem(this);
                break;
            case ItemType.Health:
                strategy = new HealthItem(this);
                break;

        }
    }

  
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<IPlayerInteractable>(out IPlayerInteractable player);

        if (player != null)
        {
            owner = player;
            strategy.ItemUse();

            ReleaseObject();
            //Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //strategy.ItemUse();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

}
