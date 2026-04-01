using UnityEngine;

namespace BallShotGame.Controllers
{
    /// <summary>
    /// 카메라가 플레이어를 따라가는 컨트롤러
    /// Main Camera에 부착
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";
        
        [Header("Follow Settings")]
        [Tooltip("낮을수록 더 부드러움 (0.02 ~ 0.15 추천)")]
        [SerializeField] private float smoothSpeed = 0.08f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [Tooltip("카메라 최대 이동 속도 (흔들림 방지)")]
        [SerializeField] private float maxCameraSpeed = 15f;
        
        private Vector3 _currentVelocity;
        private Rigidbody2D _targetRb;
        
        [Header("Bounds")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private void Start()
        {
            // 타겟이 설정되지 않았으면 태그로 찾기
            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    _targetRb = target.GetComponent<Rigidbody2D>();
                    Debug.Log($"Camera target found: {target.name}");
                }
                else
                {
                    Debug.LogWarning($"Camera target with tag '{targetTag}' not found!");
                }
            }
            else
            {
                _targetRb = target.GetComponent<Rigidbody2D>();
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 타겟 위치 + 오프셋
            Vector3 desiredPosition = target.position + offset;
            
            // Bounds 적용 (선택적)
            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            // SmoothDamp 사용 (Lerp보다 더 부드러움)
            // smoothTime을 타겟 속도에 따라 조정 (빠를수록 더 부드럽게)
            float dynamicSmoothTime = smoothSpeed;
            if (_targetRb != null)
            {
                float targetSpeed = _targetRb.linearVelocity.magnitude;
                // 속도가 빠를수록 smoothTime 증가 (더 부드럽게)
                dynamicSmoothTime = smoothSpeed + (targetSpeed * 0.005f);
                dynamicSmoothTime = Mathf.Clamp(dynamicSmoothTime, smoothSpeed, smoothSpeed * 2f);
            }
            
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position, 
                desiredPosition, 
                ref _currentVelocity, 
                dynamicSmoothTime,
                maxCameraSpeed  // 최대 속도 제한
            );

            transform.position = smoothedPosition;
        }

        /// <summary>
        /// 타겟 수동 설정
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
    }
}
