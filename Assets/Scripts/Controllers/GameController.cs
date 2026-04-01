using UnityEngine;
using System.Collections;
using BallShotGame.Core;
using BallShotGame.Services;
using BallShotGame.Events;
using BallShotGame.Config;

namespace BallShotGame.Controllers
{
    /// <summary>
    /// 게임 흐름 제어 컨트롤러
    /// 씬의 빈 오브젝트에 부착
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private static bool _isInitialized = false;

        private void Awake()
        {
            // 중복 초기화 방지
            if (_isInitialized)
            {
                Debug.LogWarning("GameController already initialized!");
                return;
            }

            InitializeServices();
            _isInitialized = true;
        }

        private void Start()
        {
            // 이벤트 구독
            EventBus.Instance.Subscribe<GoalReachedEvent>(OnGoalReached);
            
            Debug.Log("GameController started");
        }

        /// <summary>
        /// DI 서비스 등록
        /// </summary>
        private void InitializeServices()
        {
            // 서비스 등록
            GameService.Instance.Register<IBallService>(new BallService());
            GameService.Instance.Register<IInputService>(new InputService());
            GameService.Instance.Register<IGoalService>(new GoalService());
            
            Debug.Log("All services registered");
        }

        /// <summary>
        /// 골인 이벤트 처리
        /// </summary>
        private void OnGoalReached(GoalReachedEvent evt)
        {
            Debug.Log($"Goal reached at {evt.GoalPosition}! Starting reset coroutine...");
            
            // 3초 후 리셋
            StartCoroutine(ResetAfterDelay(GameConfig.ResetDelay));
        }

        /// <summary>
        /// 지연 리셋 코루틴
        /// </summary>
        private IEnumerator ResetAfterDelay(float delay)
        {
            Debug.Log($"Resetting in {delay} seconds...");
            
            yield return new WaitForSeconds(delay);
            
            // 리셋 이벤트 발행
            EventBus.Instance.Publish(new ResetRequestedEvent
            {
                Reason = ResetReason.GoalReached,
                Delay = 0f
            });
            
            // 골 상태 리셋
            var goalService = GameService.Instance.Get<IGoalService>();
            goalService?.ResetGoal();
            
            Debug.Log("Game reset completed");
        }

        /// <summary>
        /// 수동 리셋 (버튼 등에서 호출)
        /// </summary>
        public void ManualReset()
        {
            EventBus.Instance.Publish(new ResetRequestedEvent
            {
                Reason = ResetReason.Manual,
                Delay = 0f
            });
        }

        /// <summary>
        /// 게임 재시작 (완전 초기화)
        /// </summary>
        public void RestartGame()
        {
            // 모든 서비스 종료
            GameService.Instance.DisposeAll();
            
            // 초기화 플래그 리셋
            _isInitialized = false;
            
            // 다시 초기화
            InitializeServices();
            
            Debug.Log("Game restarted");
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<GoalReachedEvent>(OnGoalReached);
        }
    }
}
