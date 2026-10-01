using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private Collider2D _collectCollider;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.TryGetComponent(out ICollectable collectable))
        {
            if(!collider.IsTouching(_collectCollider))
                return;

            collectable.Collect(GetComponent<Player>());
        }
    }
}
