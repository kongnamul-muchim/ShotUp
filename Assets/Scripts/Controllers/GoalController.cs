using UnityEngine;
using BallShotGame.Core;
using BallShotGame.Services;
using BallShotGame.Events;

namespace BallShotGame.Controllers
{
    /// <summary>
    /// 골인지점 충돌 감지 컨트롤러
    /// Goal Tilemap에 부착
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GoalController : MonoBehaviour
    {
        private IGoalService _goalService;

        private void Awake()
        {
            // Collider 설정 확인
            var collider = GetComponent<Collider2D>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning("Goal collider should be set to Trigger!");
            }
        }

        private void Start()
        {
            // DI에서 서비스 조회 (GameController Awake 이후)
            _goalService = GameService.Instance.Get<IGoalService>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Player 태그 또는 Ball 이름 확인
            if (other.CompareTag("Player") || other.name.Contains("Ball") || other.name.Contains("Player"))
            {
                Debug.Log("Goal triggered by: " + other.name);
                
                // 골인 처리
                _goalService?.SetGoalReached();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // 필요시 추가 처리
        }
    }
}
