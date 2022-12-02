using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace SilverWing.Enemies
{
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("Enemy Stats")]
        [SerializeField]
        float _patrolSpeed = 1f;


        [Header("Patrol Points Data")]
        [SerializeField, Tooltip("Cuanto tiempo va a esperar cuando llegue al punto de destino")] 
        float _waitTimeAtPatrolPoint = 1f;
        [SerializeField, Tooltip("El primer punto del array hacia el que se comenzara a dirigir el enemigo")] 
        int _firstPointIndex = 0;
        [SerializeField] List<Vector3> _patrolPositions = new List<Vector3>();

        
        Transform _transform;
        int _currentPointIndex = 0;
        int _indexModifier = 1;
        Vector3 _currentPoint;
        bool _isWaiting = false;

        float _failMargin = 0.2f;

        bool _waitingOnHit = false;
        
        private void Awake()
        {
            _transform = transform;
            _currentPointIndex = _firstPointIndex;
            if (_patrolPositions.Count > 0)
                _currentPoint = _patrolPositions[_currentPointIndex];
            CheckIndexMod();
        }

        private void Update()
        {
            Patrol();
        }

        private void Patrol()
        {
            if (_patrolPositions.Count <= 0)
                return;

            if (_isWaiting)
                return;

            if (_waitingOnHit)
                return;

            if (Vector3.Distance(_currentPoint, transform.position) <= _failMargin)
                StartCoroutine(WaitAtPatrolPoint());

            var directionVector = _currentPoint - _transform.position;
            directionVector.Normalize();
            
            var newPos = transform.position + directionVector * _patrolSpeed * Time.deltaTime;
            transform.position = newPos;
        }

        private void CheckIndexMod()
        {
            if (_currentPointIndex + _indexModifier >= _patrolPositions.Count)
                _indexModifier = -1;
            if (_currentPointIndex + _indexModifier < 0)
                _indexModifier = 1;
        }

        private IEnumerator WaitAtPatrolPoint()
        {
            _isWaiting = true;
            yield return new WaitForSeconds(_waitTimeAtPatrolPoint);

            CheckIndexMod();

            _currentPointIndex += _indexModifier;


            _currentPoint = _patrolPositions[_currentPointIndex];
            _isWaiting = false;
        }

        // SETTERS
        public void SetWaitingOnHit(bool b) => _waitingOnHit = b;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (_patrolPositions.Count <= 0)
                return;



            for (int i = 0; i < _patrolPositions.Count; i++)
            {
                var _current = _patrolPositions[i];
                Gizmos.DrawWireSphere(_current, 0.2f);
            }

        }
        private void OnValidate()
        {
            if (_firstPointIndex > _patrolPositions.Count - 1)
                _firstPointIndex = _patrolPositions.Count - 1;

            if (_firstPointIndex < 0)
                _firstPointIndex = 0;

            if (_waitTimeAtPatrolPoint < 0)
                _waitTimeAtPatrolPoint = 0;

        }

#endif
    }
}
