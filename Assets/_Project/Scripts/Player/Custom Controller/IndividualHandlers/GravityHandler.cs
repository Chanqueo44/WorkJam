using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class GravityHandler : MonoBehaviour
    {
        [SerializeField] PlayerController _playerController;
        [SerializeField] CollisionHandler _collisionHandler;
        [SerializeField] LocomotionHandler _locomotionHandler;
        [Header("GRAVEDAD")]
        [SerializeField] private float _fallClamp = -40f;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        public float _fallSpeed { get; private set; }

        public void CalculateGravity()
        {
            if (_collisionHandler._colDown)
            {
                if (_playerController._currentVerticalSpeed < 0) _playerController.SetCurrentVerticalSpeed(0);
            }
            else
            {
                // Añade fuerza hacia abajo mientras asciende si terminamos el salto antes
                var fallSpeed = _locomotionHandler.GetEndedJumpEarly() && _playerController._currentVerticalSpeed > 0 ? _fallSpeed * _locomotionHandler.GetEndedEarlyGravityModifier() : _fallSpeed;
            
                // Caida
                _playerController.SetCurrentVerticalSpeed(_playerController._currentVerticalSpeed - fallSpeed * Time.deltaTime);

                // Clampeo
                if (_playerController._currentVerticalSpeed < _fallClamp) _playerController.SetCurrentVerticalSpeed(_fallClamp);
            }
        }

        /*
         * GETTERS
         */
        public float GetMinFallSpeed() {
            return _minFallSpeed;
        }

        public float GetMaxFallSpeed() {
            return _maxFallSpeed;
        }


        /*
         * SETTERS
         */

        public void SetFallSpeed(float f) => _fallSpeed = f;
    }
}
