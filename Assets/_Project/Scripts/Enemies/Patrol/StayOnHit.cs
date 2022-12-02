using System.Collections;
using UnityEngine;

namespace SilverWing.Enemies
{
    public class StayOnHit : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField][Tooltip("El tiempo (En segundos) que se va a quedar esperando el obstaculo una vez el jugador le pegue")]
        float _waitTimeOnHit = 0.5f;

        EnemyPatrol _patrolScript;
        bool _isWaitingOnHit = false;

        private void Awake()
        {
            _patrolScript = GetComponent<EnemyPatrol>();
        }

        public void PerformWaiting()
        {
            if (_isWaitingOnHit)
                StopAllCoroutines();

            StartCoroutine(WaitTimeOnHit());
        }

        private IEnumerator WaitTimeOnHit()
        {
            _isWaitingOnHit = true;
            _patrolScript.SetWaitingOnHit(_isWaitingOnHit);
            yield return new WaitForSeconds(_waitTimeOnHit);
            _isWaitingOnHit = false;
            _patrolScript.SetWaitingOnHit(_isWaitingOnHit);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_waitTimeOnHit < 0f)
                _waitTimeOnHit = 0f;
        }
#endif
    }
}
