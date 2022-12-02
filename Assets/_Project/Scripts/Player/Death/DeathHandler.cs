using System.Collections;
using SilverWing.Enemies;
using SilverWing.Player.Controller;
using UnityEngine;

namespace SilverWing.Player.Death
{
    public class DeathHandler : MonoBehaviour
    {
        public delegate void Respawn();
        public static event Respawn OnRespawn;
        
        [SerializeField] PlayerController _playerController;
        [SerializeField] GameObject[] _sprites;

        [Header("Death Manager Settings")]
        [SerializeField] ParticleSystem _deathParticles;
        [SerializeField] Transform _lastCheckPoint;
        [SerializeField] float _waitForRespawn = 0.5f;

        WaitForSeconds _waitForTP;
        WaitForSeconds _waitForAvailable;
        bool _dead = false;
        private void Awake()
        {
            _waitForTP = new WaitForSeconds(1f);
            _waitForAvailable = new WaitForSeconds(_waitForRespawn);
        }

        #region Event Subscription
        private void OnEnable()
        {
            DamagePlayer.OnDamagePlayer += Damage;
            CheckPoint.CheckPointDetection.OnCheckPointDetection += UpdateCheckPoint;
        }

        private void OnDisable()
        {
            DamagePlayer.OnDamagePlayer -= Damage;
            CheckPoint.CheckPointDetection.OnCheckPointDetection -= UpdateCheckPoint;
        }
        #endregion
        private void Damage()
        {
            if (_dead)
                return;

            _dead = true;

            if (_playerController != null) _playerController.SetPauseInput(true);
            _deathParticles.Play();
            
            StartCoroutine(DespawnCoroutine());
        }

        private IEnumerator DespawnCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            foreach (GameObject _go in _sprites)
                _go.SetActive(false);
            yield return _waitForTP;


            StartCoroutine(RespawnCoroutine());
        }

        private IEnumerator RespawnCoroutine()
        {
            transform.position = _lastCheckPoint.position;
            _dead = false;

            yield return _waitForRespawn;
            foreach (GameObject _go in _sprites)
                _go.SetActive(true);

            OnRespawn?.Invoke();
            if (_playerController != null) _playerController.SetPauseInput(false);
        }

        private void UpdateCheckPoint(Transform checkPointTransform)
        {
            if (_lastCheckPoint == checkPointTransform)
                return;

            _lastCheckPoint = checkPointTransform;
        }
    }

}
