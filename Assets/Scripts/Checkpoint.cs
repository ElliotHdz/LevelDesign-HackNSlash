using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public bool debug = false;

    void Awake()
    {
        gameObject.GetComponent<Collider>().isTrigger = true;       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.SetCheckpoint(transform.position);
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (!debug)
        {
            return;
        }
        Gizmos.color = Color.green;
        var col = gameObject.GetComponent<BoxCollider>();
        Gizmos.DrawCube(transform.position + col.center, col.size);
    }
}
