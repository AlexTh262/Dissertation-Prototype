using UnityEngine;
using UnityEngine.Tilemaps;

public class Weapon : MonoBehaviour
{

    public float damage = 1f;
    public enum WeaponType { Melee, Bullet}
    public WeaponType weaponType;
    private void OnTriggerEnter2D(Collider2D collision)
    {
    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    TilemapCollider2D tm = collision.gameObject.GetComponentInParent<TilemapCollider2D>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
        if (tm != null)
        {
            if (weaponType == WeaponType.Bullet)
            {
                Destroy(gameObject);
            }
        }
    }
}