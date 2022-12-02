using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class AttackHandler : MonoBehaviour
    {
        bool _isAttacking = false;
        [SerializeField] Animator _animator;
        [SerializeField] GameObject _attackHitbox;

        #region Event Subscription
        private void OnEnable()
        {
            LocomotionHandler.OnBounceImpact += PauseAnimation;
        }

        private void OnDisable()
        {
            LocomotionHandler.OnBounceImpact -= PauseAnimation;
        }
        #endregion

        public void Awake()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();
        }
        public void PerformAttack()
        {
            if (_isAttacking)
                return;

            _animator.SetTrigger("Attack");
            _isAttacking = true;
        }

        public void BeginCollisionCheck()
        {
            if (_attackHitbox == null)
                return;

            _attackHitbox.SetActive(true);
        }

        public void EndCollisionCheck()
        {
            if (_attackHitbox == null)
                return;

            _attackHitbox.SetActive(false);
        }

        public void EndAttack()
        {
            _isAttacking = false;
        }

        private void PauseAnimation()
        {
            if (_animator.speed == 0f)
                _animator.speed = 1f;
            else
                _animator.speed = 0f;
        }


        // SETTERS

        public void SetIsAttacking(bool b) => _isAttacking = b;

    }
}
