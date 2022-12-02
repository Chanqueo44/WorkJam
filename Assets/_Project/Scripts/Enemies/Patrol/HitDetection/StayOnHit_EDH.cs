using UnityEngine;

namespace SilverWing.Enemies.DetectHits
{
    public class StayOnHit_EDH : EnemyDetectHit
    {
        [SerializeField] StayOnHit _stayOnHitScript;

        public override void OnGetHit()
        {
            base.OnGetHit();
            _stayOnHitScript.PerformWaiting();
        }
    }
}
