using System.Collections;
using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class LocomotionHandler : MonoBehaviour
    {
        public delegate void BounceImpact();
        public static event BounceImpact OnBounceImpact;

        [SerializeField] PlayerController _playerController;
        [SerializeField] CollisionHandler _collisionHandler;
        [SerializeField] GravityHandler _gravityHandler;
        FrameInput _input;
        [Header("CAMINATA")]
        [SerializeField] private float _aceleracion = 90f;
        [SerializeField] private float _moveClamp = 13f;
        [SerializeField] private float _deAceleracion = 60f;
        [SerializeField] private float _apexBonus = 2;

        private float _currentHorizontalSpeed;

        [Header("SALTO")]
        [SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private bool _onBounce = false;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;

        bool _justDied = false;

        private bool CanUseCoyote => _coyoteUsable && !_collisionHandler._colDown && _collisionHandler._timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _collisionHandler._colDown && _lastJumpPressed + _jumpBuffer > Time.time;

        [Header("REBOTE")]
        [SerializeField] private float _bounceHeight = 30;

        #region Event Subscription
        private void OnEnable()
        {
            Death.DeathHandler.OnRespawn += CancelMovement;
        }

        private void OnDisable()
        {
            Death.DeathHandler.OnRespawn -= CancelMovement;
        }
        #endregion
        private void Start()
        {
            _currentHorizontalSpeed = _playerController._currentHorizontalSpeed;
        }

        public void CalculateWalk()
        {
            if (_justDied)
                return;

            _input = _playerController.Input;
            if (_input.X != 0){
                // Settea la velocidad horizontal
                _currentHorizontalSpeed += _input.X * _aceleracion * Time.deltaTime;

                // La clampea por el maximo de movimiento por frame
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Aplica el bonus al momento de apex de un salto
                var apexBonus = Mathf.Sign(_input.X) * _apexBonus * _apexPoint;

                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else{
                // Si no hay input desaceleramos al personaje
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAceleracion * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _collisionHandler._colRight || _currentHorizontalSpeed < 0 && _collisionHandler._colLeft)
            {
                // Para no atravesar paredes
                _currentHorizontalSpeed = 0;
            }

            _playerController.SetCurrentHorizontalSpeed(_currentHorizontalSpeed);
        }

        public void CalculateJumpApex()
        {
            if (!_collisionHandler._colDown)
            {
                // Se vuelve mas fuerte mientras mas cerca este de la parte mas alta del salto
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(_playerController.Velocity.y));
                _gravityHandler.SetFallSpeed(Mathf.Lerp(_gravityHandler.GetMinFallSpeed(), _gravityHandler.GetMaxFallSpeed(),
                    _apexPoint));
            }
            else {
                _apexPoint = 0;
            }
        }

        public void CalculateJump(FrameInput input)
        {
            if (_justDied)
                return;

            // Saltar si: Esta grounded o en el threshold de coyote time || jump buffer suficiente
            if (input.JumpPressed && CanUseCoyote || HasBufferedJump)
            {
                _playerController.SetCurrentVerticalSpeed(_jumpHeight);
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _collisionHandler.SetTimeLeftGrounded(float.MinValue);
                _playerController.SetJumpingThisFrame(true);
            }
            else
            {
                _playerController.SetJumpingThisFrame(false);
            }

            // Terminar el salto antes si se suelta el boton
            if (!_collisionHandler._colDown && _input.JumpReleased && !_endedJumpEarly && _playerController.Velocity.y > 0 && !_onBounce)
            {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (_collisionHandler._colUp)
            {
                if (_playerController._currentVerticalSpeed > 0) _playerController.SetCurrentVerticalSpeed(0);
            }

        }

        public void Bounce(bool firstTime)
        {
            if (_justDied)
                return;

            if (firstTime)
            {
                //StartCoroutine(BounceCoroutine()); //Trying out this for game feel
                _playerController.SetCurrentVerticalSpeed(_bounceHeight);
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _collisionHandler.SetTimeLeftGrounded(float.MinValue);
                _playerController.SetJumpingThisFrame(true);
                _onBounce = true;
            }
            else
            {
                _playerController.SetJumpingThisFrame(false);
            }


            if (_collisionHandler._colUp)
            {
                if (_playerController._currentVerticalSpeed > 0) _playerController.SetCurrentVerticalSpeed(0);
            }
        }

        private IEnumerator BounceCoroutine()
        {
            OnBounceImpact();
            _playerController.SetPauseInput(true);
            yield return new WaitForSeconds(0.05f);
            _playerController.SetPauseInput(false);
            OnBounceImpact();

        }

        private void CancelMovement()
        {
            _justDied = true;
            _playerController.SetCurrentVerticalSpeed(0);
            _playerController.SetCurrentHorizontalSpeed(0);
        }
        /*
         * GETTER
         */
        public bool GetJustDied()
        {
            return _justDied;
        }

        public bool GetEndedJumpEarly()
        {
            return _endedJumpEarly;
        }

        public float GetEndedEarlyGravityModifier()
        {
            return _jumpEndEarlyGravityModifier;
        }


        /*
         * SETTERS
         */

        public void SetOnBounce(bool b) => _onBounce = b;
        public void SetCoyoteTimeUsable(bool b) => _coyoteUsable = b;

        public void SetLastJumpPressed(float f) => _lastJumpPressed = f;
        public void SetJustDied(bool b) => _justDied = b;
    }
}
