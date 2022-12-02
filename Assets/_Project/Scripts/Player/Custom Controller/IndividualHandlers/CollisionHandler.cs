using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SilverWing.Player.Controller
{
    public class CollisionHandler : MonoBehaviour
    {
        [SerializeField] PlayerController _playerController;
        [SerializeField] LocomotionHandler _locomotionHandler;

        [Header("COLISIONES")]
        [SerializeField] private Bounds _characterBounds;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField][Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f;

        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        public bool _colUp { get; private set; }
        public bool _colRight { get; private set; }
        public bool _colDown { get; private set; }
        public bool _colLeft { get; private set; }

        public float _timeLeftGrounded { get; private set; }

        //Utilizacion de checkeo por raycast para informacion pre colision
        public void RunCollisionChecks()
        {
            // Calcula el rango de los rays
            CalculateRayRanged();

            // Ground
            _playerController.SetLandingFrame(false);
            var groundedCheck = RunGroundDetection(_raysDown);

            //print(groundedCheck);

            if (_colDown && !groundedCheck)
                _timeLeftGrounded = Time.time;
            else if (!_colDown && groundedCheck) {
                _locomotionHandler.SetCoyoteTimeUsable(true);
                _playerController.SetLandingFrame(true);
            }

            _colDown = groundedCheck;

            // Las otras colisiones
            _colUp = RunGroundDetection(_raysUp);
            _colLeft = RunGroundDetection(_raysLeft);
            _colRight = RunGroundDetection(_raysRight);
        }

        private void CalculateRayRanged()
        {
            var b = new Bounds(transform.position, _characterBounds.size);

            _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
            _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
            _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
        }

        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
            for (var i = 0; i < _detectorCount; i++) {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                CalculateRayRanged();

                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft}){
                    foreach (var point in EvaluateRayPositions(range)) {
                        Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                    }
                }


                if (!Application.isPlaying) return;

                Gizmos.color = Color.red;
                var move = new Vector3(_playerController._currentHorizontalSpeed, _playerController._currentVerticalSpeed) * Time.deltaTime;
                Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
            }
        }

        bool RunGroundDetection(RayRange range)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
        }

        /*
         * GETTERS
         */
        public Bounds GetCharacterBounds()
        {
            return _characterBounds;
        }

        public LayerMask GetGroundLayer()
        {
            return _groundLayer;
        }

        /*
         * SETTERS
         */

        public void SetTimeLeftGrounded(float f) => _timeLeftGrounded = f;
    }
}
