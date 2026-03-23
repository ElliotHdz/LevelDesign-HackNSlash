using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    public int health = 5;
    private int maxHealth;
    private bool isAlive = true;

    public float speed = 4f;
    [Space(10), Header("Dash"), Space(7)]
    public float dashLength = 15f;
    public float dashCoolDown = 0.5f;
    private bool canDash = true;
    private float tDash;
    public LayerMask wallMask;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 movement = new Vector2();
    private Vector3 lastDirection = Vector3.forward;

    private PlayerActions actions;

    [Space(10), Header("Attack"), Space(7)]
    public int damage = 2;
    public float attackRadius = 0.3f;
    public float attackCoolDown = 0.3f;
    [Range(0, 1)]
    public float attackDistance = 0.3f;
    private float tAttack;
    private bool canAttack = true;
    public LayerMask enemyLayer;

    public GameObject playerCam;

    public bool drawGizmos = false;

    private Vector3 checkpoint;

    void Awake()
    {
        actions = new PlayerActions();
        actions.Game.Enable();
        
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        var cam = Instantiate(playerCam);
        CameraTarget target = new CameraTarget
        {
            TrackingTarget = transform,
            CustomLookAtTarget = true
        };
        cam.GetComponent<CinemachineCamera>().Target = target;

        checkpoint = transform.position;
        maxHealth = health;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        movement = actions.Game.Move.ReadValue<Vector2>();

        if (movement.magnitude > 0)
        {
            lastDirection.x = movement.x;
            lastDirection.z = movement.y;

        }
        animator.SetBool("IsMoving", movement.magnitude > 0);

        transform.rotation = Quaternion.LookRotation(lastDirection.normalized, Vector3.up);

        if (canAttack)
        {
            if (actions.Game.Attack.WasPressedThisFrame())
            {
                Attack();
            }
        } 
        else
        {
            tAttack -= Time.deltaTime;
            if (tAttack <= 0)
            {
                tAttack = 0;
                canAttack = true;
            }
        }

        if (canDash)
        {
            if (actions.Game.Dash.WasPressedThisFrame())
            {
                Dash();
            }
        } 
        else
        {
            tDash -= Time.deltaTime;
            if (tDash <= 0)
            {
                tDash = 0;
                canDash = true;
            }
        }

    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            rb.AddForce(new Vector3(movement.x, 0, movement.y).normalized * speed, ForceMode.Impulse);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void Dash()
    {
        Ray ray = new Ray(transform.position + (Vector3.up * 0.5f), lastDirection.normalized);
        if (Physics.Raycast(ray, out RaycastHit  hit , dashLength, wallMask))
        {
            var point = hit.point;
            var dir = (transform.position - point).normalized;
            point = point + (dir * 0.5f);
            transform.position = new Vector3(point.x, transform.position.y, point.z);
        } 
        else
        {
            transform.position = transform.position + (lastDirection.normalized * dashLength);
        }
        canDash = false;
        tDash = dashCoolDown;
    }
    private void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        tAttack = attackCoolDown;

        var point = transform.position + (transform.forward.normalized * attackDistance);
        point.y = transform.position.y + 0.5f; 
        var enemies = Physics.OverlapSphere(point, attackRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            Debug.Log("Attacking");
            foreach (var enemy in enemies)
            {
                enemy.gameObject.GetComponent<Enemy>().Hit(damage);
            }
        }
    }
    public void Damage(int damage)
    {
        if (!isAlive)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            isAlive = false;
            animator.SetTrigger("Die");
            StartCoroutine(nameof(EndGame));
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = checkpoint;
        isAlive = true;
        health = maxHealth;

    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpoint = pos;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }
        var point = transform.position + (transform.forward.normalized * attackDistance);
        point.y = transform.position.y + 0.5f; 
        Gizmos.DrawSphere(point, attackRadius);
    }
}
