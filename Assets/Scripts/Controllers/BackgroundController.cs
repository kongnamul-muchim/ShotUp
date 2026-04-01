using UnityEngine;

namespace BallShotGame.Controllers
{
    /// <summary>
    /// 배경이 플레이어를 따라가는 컨트롤러
    /// BackGround 오브젝트에 부착
    /// </summary>
    public class BackgroundController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";
        
        [Header("Follow Settings")]
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private bool lockZ = true;
        [SerializeField] private float zPosition = 0f;
        
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            // 타겟이 설정되지 않았으면 태그로 찾기
            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(targetTag);
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    Debug.Log($"Background target found: {target.name}");
                }
                else
                {
                    Debug.LogWarning($"Target with tag '{targetTag}' not found!");
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 타겟 위치 계산 (offset 적용)
            Vector2 desiredPosition = (Vector2)target.position + offset;
            
            // 부드러운 이동 (Lerp)
            Vector2 smoothedPosition = Vector2.Lerp(
                (Vector2)_transform.position, 
                desiredPosition, 
                smoothSpeed
            );

            // Z축 처리
            float z = lockZ ? zPosition : _transform.position.z;
            
            // 위치 적용
            _transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, z);
        }

        /// <summary>
        /// 타겟 수동 설정
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// 오프셋 설정
        /// </summary>
        public void SetOffset(Vector2 newOffset)
        {
            offset = newOffset;
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position, 0.5f);
        }
    }
}
