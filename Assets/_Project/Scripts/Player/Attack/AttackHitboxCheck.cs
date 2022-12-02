using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class AttackHitboxCheck : MonoBehaviour
    {
        public delegate void bounce();
        public static event bounce OnBounce;
        private bool _alreadyBounced = false;
        private bool _dontCheck = false;

        private void OnEnable()
        {
            _alreadyBounced = false;
            _dontCheck = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_dontCheck)
                return;

            if (_alreadyBounced)
                return;

            GameObject current = collision.gameObject;

            if (current == null)
                return;

            if (current.CompareTag("Bounceable"))
            {
                _dontCheck = true;
                OnBounce?.Invoke();
            }
        }
    }
}
