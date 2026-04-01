using UnityEngine;
using BallShotGame.Core;

namespace BallShotGame.Services
{
    /// <summary>
    /// 공 관리 서비스 인터페이스
    /// </summary>
    public interface IBallService : IService
    {
        /// <summary>
        /// 공 발사
        /// </summary>
        /// <param name="force">발사 힘</param>
        void Launch(Vector2 force);
        
        /// <summary>
        /// 공이 정지 상태인지 확인
        /// </summary>
        /// <returns>정지 여부</returns>
        bool IsStopped();
        
        /// <summary>
        /// 공 리셋
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 현재 속도 가져오기
        /// </summary>
        /// <returns>속도</returns>
        float GetCurrentVelocity();
        
        /// <summary>
        /// 충돌 감지 시 호출
        /// </summary>
        void OnCollision();
        
        /// <summary>
        /// Rigidbody2D 참조 설정
        /// </summary>
        /// <param name="rb">Rigidbody2D</param>
        void SetRigidbody(Rigidbody2D rb);
    }
}
