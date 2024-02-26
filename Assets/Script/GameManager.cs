using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ThirdPersonController player;
    [SerializeField]
    private float healthTimer = 0f;

    public static GameManager instance = null;

    public TMP_Text ammoText;


    public Image playerHp;
    public Image playerDead;
    public RawImage weaponRifle;
    public RawImage weaponPistal;
    public RawImage weaponShotGun;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponRifle.gameObject.SetActive(false);
        weaponPistal.gameObject.SetActive(false);
        weaponShotGun.gameObject.SetActive(false);
        playerDead.gameObject.SetActive(false);
    }

    // 업데이트가 끝난 후 호출되는 생명주기 
    private void LateUpdate()
    {
        if(player.currentWeapon == player.idleWeapon)
            ammoText.gameObject.SetActive(false);
        
        else 
           ammoText.gameObject.SetActive(true);
         
       
        if(player.currentWeapon == player.rifle)
        {
            weaponRifle.gameObject.SetActive(true);
            weaponPistal.gameObject.SetActive(false);
            weaponShotGun.gameObject.SetActive(false);
        }
        else if(player.currentWeapon == player.pistal)
        {
            weaponShotGun.gameObject.SetActive(false);
            weaponRifle.gameObject.SetActive(false);
            weaponPistal.gameObject.SetActive(true);
        }

        else if(player.currentWeapon == player.shotGun)
        {
            weaponPistal.gameObject.SetActive(false);
            weaponRifle.gameObject.SetActive(false);
            weaponShotGun.gameObject.SetActive(true);
        }
        ammoText.text = player.currentWeapon.weaponData.currentBulletCount + "/" + player.currentWeapon.weaponData.carryBullet;
        
    }

   

    // Update is called once per frame
    void Update()
    {
        
        playerHp.fillAmount = player.PlayerHp * 0.01f;

        if(player.PlayerHp < 100)
        {
            healthTimer += Time.deltaTime;

            if(healthTimer >= 2f)
            {
                healthTimer = 0f;
                player.PlayerHp += 1;

                player.PlayerHp = Mathf.Clamp(player.PlayerHp, 0, 101);
            }
        }
        
    }

    public void PlayerDeadScene()
    {
        playerDead.gameObject.SetActive(true);
        
    }
}
