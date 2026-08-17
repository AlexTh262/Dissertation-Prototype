using UnityEngine;
using UnityEngine.Tilemaps;
using static Weapon;

public class BulletCollision : MonoBehaviour
{
    public enum WeaponType { Melee, Bullet }
    public WeaponType weaponType;
    private void OnTriggerEnter2D(Collider2D collision)
    {



    }
}
