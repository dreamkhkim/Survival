using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class IdleWeapon : WeaponTypes
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void InitSetting()
    {
        //weaponData.isAnim = GetComponentInParent<ThirdPersonController>();
        Debug.Log("장비 미 착용 ");
    }

    public override void GunSetOnAnim()
    {
        weaponData.isAnim._animator.SetTrigger("SwapCant");
    }

    public override void GunSetOffAnim()
    {
        weaponData.isAnim._animator.SetTrigger("Swap");
    }

    public override void AimAnim()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Reload()
    {
        throw new System.NotImplementedException();
    }
}
