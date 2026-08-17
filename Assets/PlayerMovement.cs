using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;

    public Transform aim;
    public bool isWalking = false;

    Vector2 lastMovedDirection;
    Vector2 movement;
    Vector2 movementLast;
    Vector2 lastPos;
    float distance;

    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if ((moveX == 0 && moveY == 0) && (movement.x != 0 || movement.y != 0)) //Four variables to account for direction moved and directiong facing
        {
            isWalking = false;
            lastMovedDirection = movement;
            Vector3 vector3 = Vector3.left * lastMovedDirection.x + Vector3.down * lastMovedDirection.y;
            aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3); //Set attacking direction when standing still to last direction moved
        }
        else if (movement.x != 0 || movement.y != 0)
        {
            isWalking = true;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x == 0 && movement.y == 0) //Setting direction player is facing when not moving to last direction moved
        {
            animator.SetFloat("HorizontalIdle", movementLast.x);
            animator.SetFloat("VerticalIdle", movementLast.y);
            
        }
       
        else
        {
            animator.SetFloat("HorizontalIdle", 0f);
            animator.SetFloat("VerticalIdle", 0f);
        } 

        if (movement.x == 0 && movement.y != 0)
        {
            animator.SetFloat("Horizontal", movementLast.x);
        } 

        if (movement.x != 0) movementLast.x = movement.x;
        if (movement.y != 0) movementLast.y = movement.y;

        if (lastPos != (Vector2)transform.position) //Keep track of total distance moved
        {
            distance += Vector2.Distance(transform.position, lastPos);
            lastPos = (Vector2)transform.position;
            Debug.Log(distance);
            PlayerData.totalDistance = distance;
        }

        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (isWalking) //Set direction of attack to direction of movement
        {
            Vector3 vector3 = Vector3.left * movement.x  + Vector3.down * movement.y;
            aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }
}