using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [Header("DEPENDENCIAS")]
        [SerializeField] CollisionHandler _collisionHandler;
        [SerializeField] LocomotionHandler _locomotionHandler;
        [SerializeField] GravityHandler _gravityHandler;
        [SerializeField] AttackHandler _attackHandler;

        private PlayerInputs _playerInputs;
        // External Hooks
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => false;//_colDown;

        private Vector3 _lastPosition;
        public float _currentHorizontalSpeed { get; private set; }
        public float _currentVerticalSpeed { get; private set; }

        private bool _active;
        private bool _shouldBounce = false;
        private bool _bounceFirstTime = true;
        public bool _pauseInput { get; private set; } = false;

        private InputHandler _inputHandler;
        void Awake() => Invoke(nameof(Activate), 0.5f);

        #region Event Subscription
        private void OnEnable()
        {
            AttackHitboxCheck.OnBounce += Bounce;
        }

        private void OnDisable()
        {
            AttackHitboxCheck.OnBounce -= Bounce;
        }
        #endregion

        void Activate()
        {
            _playerInputs = new PlayerInputs();
            _inputHandler = new InputHandler(_playerInputs);
            _active = true;
        } 

        private void Update()
        {
            if (!_active) return;

            if (_pauseInput) return;

            //Calculate Velocity
            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            Input = _inputHandler.GatherInput();

            if (Input.JumpPressed)
            {
                _locomotionHandler.SetLastJumpPressed(Time.time);
            }


            _collisionHandler.RunCollisionChecks();

            _locomotionHandler.CalculateWalk();
            _locomotionHandler.CalculateJumpApex();
            _gravityHandler.CalculateGravity();

            //print("X Direction: " + Input.X + ", JumpPerformed: " + Input.JumpPressed + ", JumpCanceled: " + Input.JumpReleased);
            _locomotionHandler.CalculateJump(Input);

            if (_inputHandler.GetAttackInput())
                _attackHandler.PerformAttack();

            if (_shouldBounce)
            {
                _locomotionHandler.Bounce(_bounceFirstTime);
                if (_bounceFirstTime)
                    _bounceFirstTime = false;
                if (_collisionHandler._colDown)
                {
                    _shouldBounce = false;
                    _locomotionHandler.SetOnBounce(false);
                }
            }
            if (_locomotionHandler.GetJustDied())
            {
                _locomotionHandler.SetJustDied(false);
                _attackHandler.SetIsAttacking(false);
            }
            MoveCharacter();
        }

        [Header("MOVIMIENTO")]
        [SerializeField, Tooltip("Aumentar este valor aumenta la accuracy de las colisiones por un costo en performance")]
        private int _freeColliderIterations = 10;

        // Los bounds nuevos se castean a futuro para evitar colisiones futuras
        private void MoveCharacter()
        {
            var pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Esto es lo que se utiliza externamente
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;

            // Chequea el movimiento mas lejano, si no choca nada se mueve y no hace chequeos extra
            var hit = Physics2D.OverlapBox(furthestPoint, _collisionHandler.GetCharacterBounds().size, 0, _collisionHandler.GetGroundLayer());
            if (!hit)
            {
                transform.position += move;
                return;
            }

            // En cualquier otra situacion incrementamos menos que la posicion actual y vemos cual es la posicion mas
            // Cercana a la que nos podemos mover
            var positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++)
            {
                // incrementamos para checkear todo menos el punto mas lejano pq eso ya lo hicimos
                var t = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, _collisionHandler.GetCharacterBounds().size, 0, _collisionHandler.GetGroundLayer()))
                {
                    transform.position = positionToMoveTo;

                    if (i == 1)
                    {
                        if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }

                    return;
                }

                positionToMoveTo = posToTry;
            }

        }

        private void Bounce()
        {
            _bounceFirstTime = true;
            _shouldBounce = true;
        }
        /*
         * GETTERS
         */

        /*
         * SETTERS
         */
        public void SetPauseInput(bool b) => _pauseInput = b;

        public void SetLandingFrame(bool b) => LandingThisFrame = b;
        public void SetJumpingThisFrame(bool b) => JumpingThisFrame = b;
        public void SetCurrentVerticalSpeed(float f) => _currentVerticalSpeed = f;
        public void SetCurrentHorizontalSpeed(float f) => _currentHorizontalSpeed = f;
    }
}
