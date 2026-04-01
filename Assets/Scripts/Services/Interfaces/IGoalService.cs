using UnityEngine;
using BallShotGame.Core;

namespace BallShotGame.Services
{
    /// <summary>
    /// 골 관리 서비스 인터페이스
    /// </summary>
    public interface IGoalService : IService
    {
        /// <summary>
        /// 골인 상태 확인
        /// </summary>
        /// <returns>골인 여부</returns>
        bool IsGoalReached();
        
        /// <summary>
        /// 골인 처리
        /// </summary>
        void SetGoalReached();
        
        /// <summary>
        /// 골인 상태 리셋
        /// </summary>
        void ResetGoal();
    }
}
