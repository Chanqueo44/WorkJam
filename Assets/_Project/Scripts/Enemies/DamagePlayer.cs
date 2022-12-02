using UnityEngine;

namespace SilverWing.Enemies
{
    public class DamagePlayer : MonoBehaviour
    {
        public delegate void InflictDamage();
        public static event InflictDamage OnDamagePlayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject current = collision.gameObject;

            if (!current.CompareTag("Player"))
                return;

            OnDamagePlayer?.Invoke();
        }
    }
}
