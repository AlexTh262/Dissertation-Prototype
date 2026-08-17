using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject Melee;

    //Melee
    bool isAttacking = false;
    float attackDuration = 0.3f;
    float attackTimer = 0;

    //Ranged
    public Transform aim;
    public GameObject bullet;
    public float fireForce = 10f;
    float shootCooldown = 0.25f;
    float shootTimer = 0.5f;

    //Counters
    int meleeCounter;
    int rangedCounter;

    // Update is called once per frame
    void Update()
    {
        CheckMeleeTimer();
        shootTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) 
        {
            //Attack
            OnAttack();
            meleeCounter++;
            PlayerData.meleeAttacks++;
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Shoot
            OnShoot();
            rangedCounter++;
            PlayerData.rangedAttacks++;
        }
    }

    void OnAttack()
    {
        if(!isAttacking)
        {
            Melee.SetActive(true);
            isAttacking = true;
        }
    }

    void OnShoot()
    {
        if (shootTimer > shootCooldown) 
        {
            shootTimer = 0;
            GameObject intBullet = Instantiate(bullet, aim.position, aim.rotation);
            intBullet.GetComponent<Rigidbody2D>().AddForce(-aim.up * fireForce, ForceMode2D.Impulse);
            Destroy(intBullet, 2f);
        }
    }

    void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration) 
            {
                attackTimer = 0;
                isAttacking= false;
                Melee.SetActive(false);
            }
        }
    }  
}