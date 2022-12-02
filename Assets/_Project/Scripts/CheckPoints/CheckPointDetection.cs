using UnityEngine;

namespace SilverWing.CheckPoint
{
    public class CheckPointDetection : MonoBehaviour
    {
        public delegate void CheckPointDeteciton(Transform transform);
        public static event CheckPointDeteciton OnCheckPointDetection;

        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Color _enabledColor;
        [SerializeField] Color _disabledColor;
        bool _enabled;

        #region Event Subscription
        private void OnEnable()
        {
            OnCheckPointDetection += Detect;
        }

        private void OnDisable()
        {
            OnCheckPointDetection += Detect;
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject current = collision.gameObject;

            if (!collision.CompareTag("Player"))
                return;

            OnCheckPointDetection?.Invoke(transform);
        }

        private void Detect(Transform trans)
        {
            if (trans.Equals(gameObject.transform) && !_enabled)
            {
                _enabled = true;
                _spriteRenderer.color = _enabledColor;
            }

            if (!trans.Equals(gameObject.transform) && _enabled)
            {
                _spriteRenderer.color = _disabledColor;
                _enabled = false;
            }


        }
    }
}
