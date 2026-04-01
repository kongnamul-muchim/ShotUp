using UnityEngine;
using BallShotGame.Core;
using BallShotGame.Services;
using BallShotGame.Events;
using BallShotGame.Config;

namespace BallShotGame.Controllers
{
    /// <summary>
    /// 공 드래그 및 발사 컨트롤러
    /// Player 오브젝트에 부착
    /// </summary>
    public class BallController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        
        private IBallService _ballService;
        private IInputService _inputService;
        
        private bool _isDragging = false;
        private Vector2 _dragStartPos;
        private Vector2 _currentMousePos;

        private void Awake()
        {
            // Rigidbody 설정
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            
            // 탄성 설정 (벽에 튕기도록)
            if (rb != null)
            {
                rb.sharedMaterial = new PhysicsMaterial2D("BallMaterial")
                {
                    friction = 0.1f,
                    bounciness = 0.6f  // 0~1, 높을수록 더 튕김
                };
            }
        }

        private void Start()
        {
            // DI에서 서비스 조회 (GameController Awake 이후)
            _ballService = GameService.Instance.Get<IBallService>();
            _inputService = GameService.Instance.Get<IInputService>();
            
            _ballService?.SetRigidbody(rb);
            
            // 이벤트 구독
            EventBus.Instance.Subscribe<InputMouseDownEvent>(HandleMouseDownEvent);
            EventBus.Instance.Subscribe<InputMouseDragEvent>(HandleMouseDragEvent);
            EventBus.Instance.Subscribe<InputMouseUpEvent>(HandleMouseUpEvent);
            EventBus.Instance.Subscribe<ResetRequestedEvent>(HandleResetRequested);
        }

        private void Update()
        {
            // 서비스 업데이트
            if (_ballService is BallService ballService)
            {
                ballService.Update();
            }
            
            // 마우스 입력 처리
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            // 서비스 초기화 확인
            if (_ballService == null) return;
            
            // 공이 정지 상태일 때만 드래그 가능
            if (!_ballService.IsStopped()) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = GetMouseWorldPosition();
                
                // 공을 클릭했는지 확인
                float distance = Vector2.Distance(mousePos, transform.position);
                if (distance < GameConfig.BallClickRadius) // 클릭 범위
                {
                    StartDrag(mousePos);
                }
            }
            else if (_isDragging && Input.GetMouseButton(0))
            {
                UpdateDrag(GetMouseWorldPosition());
            }
            else if (_isDragging && Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        private void StartDrag(Vector2 position)
        {
            _isDragging = true;
            _dragStartPos = position;
            _currentMousePos = position;
            
            _inputService?.SetDragging(true);
            
            EventBus.Instance.Publish(new BallDragStartedEvent
            {
                StartPosition = _dragStartPos,
                BallPosition = transform.position
            });
        }

        private void UpdateDrag(Vector2 position)
        {
            _currentMousePos = position;
            
            EventBus.Instance.Publish(new BallDragUpdatedEvent
            {
                CurrentPosition = _currentMousePos,
                StartPosition = _dragStartPos
            });
        }

        private void EndDrag()
        {
            _isDragging = false;
            
            _inputService?.SetDragging(false);
            
            // 발사 힘 계산
            Vector2 dragVector = _dragStartPos - _currentMousePos;
            float dragDistance = dragVector.magnitude;
            
            // 최소/최대 힘 적용
            if (dragDistance < GameConfig.MinLaunchForce)
            {
                Debug.Log("Drag too short, not launching");
                return;
            }
            
            // 힘 계산: 드래그 거리 × 계수
            Vector2 force = dragVector.normalized * Mathf.Min(dragDistance * GameConfig.ForceMultiplier, GameConfig.MaxLaunchForce);
            
            _ballService?.Launch(force);
        }

        private void HandleMouseDownEvent(InputMouseDownEvent evt)
        {
            // 이벤트 기반 처리 (필요시)
        }

        private void HandleMouseDragEvent(InputMouseDragEvent evt)
        {
            // 이벤트 기반 처리 (필요시)
        }

        private void HandleMouseUpEvent(InputMouseUpEvent evt)
        {
            // 이벤트 기반 처리 (필요시)
        }

        private void HandleResetRequested(ResetRequestedEvent evt)
        {
            _ballService?.Reset();
        }

        private Vector2 GetMouseWorldPosition()
        {
            if (Camera.main != null)
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            return Vector2.zero;
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            EventBus.Instance.Unsubscribe<InputMouseDownEvent>(HandleMouseDownEvent);
            EventBus.Instance.Unsubscribe<InputMouseDragEvent>(HandleMouseDragEvent);
            EventBus.Instance.Unsubscribe<InputMouseUpEvent>(HandleMouseUpEvent);
            EventBus.Instance.Unsubscribe<ResetRequestedEvent>(HandleResetRequested);
        }

        /// <summary>
        /// 충돌 감지 - 땅에 닿을 때만 느려짐
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[COLLISION] Hit: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
            
            // 충돌 지점 확인
            if (collision.contacts.Length == 0)
            {
                Debug.LogWarning("[COLLISION] No contact points!");
                return;
            }
            
            // 충돌 각도 계산 (위쪽으로부터의 각도)
            Vector2 normal = collision.contacts[0].normal;
            float angle = Vector2.Angle(normal, Vector2.up);
            
            Debug.Log($"[COLLISION] Normal: {normal}, Angle from UP: {angle:F1}°");
            
            // 땅바닥인지 확인 (위쪽 노멀 벡터에 가까움 = 땅)
            // 벽은 옆쪽(90도), 천장은 아래쪽(180도)
            bool isGround = angle < GameConfig.GroundDetectionAngle;  // 땅으로 간주
            
            if (isGround)
            {
                // 땅에 닿았을 때만 저항 적용
                Debug.Log($"[COLLISION] ★ GROUND HIT! Applying damping...");
                _ballService?.OnCollision();
            }
            else
            {
                // 벽이나 천장 - 저항 없음
                Debug.Log($"[COLLISION] Wall/Ceiling hit (no damping)");
            }
        }
    }
}
