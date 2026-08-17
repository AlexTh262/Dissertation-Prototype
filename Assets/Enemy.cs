using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    float health, maxHealth = 3f;
    EnemyCounter enemyCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.Find("Player").transform;
        health = maxHealth;
        enemyCounter = GameObject.Find("EnemyCounter").GetComponent<EnemyCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target && InRange() == true)
        {
           Vector3 direction =(target.position - transform.position).normalized;
            moveDirection = direction;           
        } else
        {
            moveDirection = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if(target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            enemyCounter.AddToCounter();
            Destroy(gameObject);
        }
    }

    public bool InRange()
    {
        float dist = Vector2.Distance(target.position, rb.position);
        if (dist < 8)
        {
            return true;
        } else 
        {
            return false; 
        }
    }
}