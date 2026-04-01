using UnityEngine;
using BallShotGame.Core;
using BallShotGame.Events;
using BallShotGame.Config;

namespace BallShotGame.Services
{
    /// <summary>
    /// 공 물리/상태 관리 서비스
    /// </summary>
    public class BallService : IBallService
    {
        private Rigidbody2D _rigidbody;
        private bool _isStopped = true;
        private bool _hasCollided = false;  // 충돌 여부 플래그
        private int _collisionCount = 0;     // 충돌 횟수

        public void Initialize()
        {
            Debug.Log("BallService initialized");
        }

        public void Dispose()
        {
            _rigidbody = null;
        }

        public void SetRigidbody(Rigidbody2D rb)
        {
            _rigidbody = rb;
        }

        public void Launch(Vector2 force)
        {
            if (_rigidbody == null) return;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
            _isStopped = false;
            _hasCollided = false;  // 발사 시 충돌 플래그 리셋
            _collisionCount = 0;

            EventBus.Instance.Publish(new BallLaunchedEvent
            {
                Force = force,
                Direction = force.normalized,
                Magnitude = force.magnitude
            });

            Debug.Log($"Ball launched with force: {force}");
        }

        public bool IsStopped()
        {
            if (_rigidbody == null) return true;

            // 수평 속도만 체크
            float horizontalVelocity = Mathf.Abs(_rigidbody.linearVelocity.x);
            return horizontalVelocity < GameConfig.StopThreshold;
        }

        public void Reset()
        {
            if (_rigidbody == null) return;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.position = GameConfig.ResetPosition;
            _isStopped = true;

            EventBus.Instance.Publish(new BallStoppedEvent
            {
                StopPosition = _rigidbody.position,
                FinalVelocity = 0f
            });

            Debug.Log("Ball reset");
        }

        public float GetCurrentVelocity()
        {
            return _rigidbody?.linearVelocity.magnitude ?? 0f;
        }

        /// <summary>
        /// 매 프레임 호출 - 정지 상태 감지 및 속도 감쇄
        /// </summary>
        public void Update()
        {
            if (_rigidbody == null || _isStopped) return;

            // 충돌 후에만 속도 감쇄 적용 (수평 방향만)
            if (_hasCollided)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                float horizontalSpeed = Mathf.Abs(velocity.x);
                
                // 매우 약한 감속만 적용 (관성 유지)
                // 0.995 = 0.5% 감소 (천천히 멈춤)
                float damping = 0.995f;
                
                // 속도가 매우 낮아질 때만 조금 더 강하게
                if (horizontalSpeed < 0.5f)
                {
                    damping = 0.98f;  // 2% 감소
                }
                if (horizontalSpeed < 0.3f)
                {
                    damping = 0.95f;  // 5% 감소 (급격히)
                }
                
                // X축만 감쇄, Y축(중력)은 유지
                velocity.x *= damping;
                _rigidbody.linearVelocity = velocity;
                
                // 각속도도 천천히 감쇄
                _rigidbody.angularVelocity *= 0.99f;
            }

            // 수평 속도만 체크 (Y축은 제외)
            float horizontalVelocity = Mathf.Abs(_rigidbody.linearVelocity.x);
            if (horizontalVelocity < GameConfig.StopThreshold)
            {
                _isStopped = true;
                
                // 수평 속도만 0으로, 수직은 유지
                Vector2 finalVelocity = _rigidbody.linearVelocity;
                finalVelocity.x = 0f;
                _rigidbody.linearVelocity = finalVelocity;
                _rigidbody.angularVelocity = 0f;
                
                EventBus.Instance.Publish(new BallStoppedEvent
                {
                    StopPosition = _rigidbody.position,
                    FinalVelocity = horizontalVelocity
                });

                Debug.Log("Ball stopped");
            }
        }

        /// <summary>
        /// 충돌 감지 - BallController에서 호출
        /// </summary>
        public void OnCollision()
        {
            if (!_hasCollided)
            {
                _hasCollided = true;
                _collisionCount++;
                Debug.Log($"First collision! Damping will be applied. Total collisions: {_collisionCount}");
            }
        }
    }
}
