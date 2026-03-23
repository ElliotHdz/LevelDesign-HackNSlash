using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BarrierTrigger : MonoBehaviour
{
    public GameObject[] barriers;
    public bool debug = false;

    void Awake()
    {
        gameObject.GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            foreach (var barrier in barriers)
            {
                barrier.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }

        void OnDrawGizmos()
    {
        if (!debug)
        {
            return;
        }
        Gizmos.color = Color.red;
        var col = gameObject.GetComponent<BoxCollider>();
        Gizmos.DrawCube(transform.position + col.center, col.size);
    }
}
