using UnityEngine;

public class SnoleAttack : MonoBehaviour
{
    [SerializeField] AnimationClip damageClip;
    [SerializeField] int damage;
    [SerializeField] float speed;
    private Animator animator;
    private Vector3 direction;
    private Transform child;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        child = transform.GetChild(0);

        animator = child.gameObject.GetComponent<Animator>();
        animator.Play(damageClip.name);

        setDirectionAndRotation();
    }

    private void setDirectionAndRotation()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        direction = (playerPos - transform.position).normalized;

        float rot = Vector3.Angle(Vector3.left, direction);
        Debug.Log($"rotation: {rot}");
        Quaternion rotation = Quaternion.Euler(0, 0, rot);
        child.rotation = rotation;
        if (playerPos.y > transform.position.y)
        {
            child.rotation = Quaternion.Euler(-child.rotation.eulerAngles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * direction * Time.deltaTime;
    }

    public void setVals(int d, float s)
    {
        damage = d;
        speed = s;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        Debug.Log($"Collided with {gameObject.name}");

        if (gameObject.tag != "PlayerCollider" && gameObject.tag != "Walls")
        {
            return;
        }

        if (gameObject.tag == "PlayerCollider")
        {
            gameObject.transform.parent.GetComponent<EntityStatus>().Hurt(damage);
        }

        Destroy(this.gameObject);
    }
}
