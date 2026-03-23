using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeathVolume : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.Damage(player.health);
        }
    }
}
