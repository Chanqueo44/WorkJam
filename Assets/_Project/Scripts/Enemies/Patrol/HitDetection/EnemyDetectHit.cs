using UnityEngine;

namespace SilverWing.Enemies.DetectHits
{
    public class EnemyDetectHit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject current = collision.gameObject;

            if (!current.CompareTag("WeaponHitbox"))
                return;

            OnGetHit();
        }

        public virtual void OnGetHit()
        {
            //HitFlash
        }
    }
}
