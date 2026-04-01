using UnityEngine;
using BallShotGame.Core;
using BallShotGame.Events;

namespace BallShotGame.Services
{
    /// <summary>
    /// 골인지점 관리 서비스
    /// </summary>
    public class GoalService : IGoalService
    {
        private bool _isGoalReached = false;

        public void Initialize()
        {
            Debug.Log("GoalService initialized");
        }

        public void Dispose()
        {
        }

        public bool IsGoalReached()
        {
            return _isGoalReached;
        }

        public void SetGoalReached()
        {
            if (_isGoalReached) return;
            
            _isGoalReached = true;
            
            EventBus.Instance.Publish(new GoalReachedEvent
            {
                GoalPosition = Vector2.zero, // 필요시 실제 위치로 수정
                StageNumber = 1
            });

            Debug.Log("Goal reached!");
        }

        public void ResetGoal()
        {
            _isGoalReached = false;
        }
    }
}
