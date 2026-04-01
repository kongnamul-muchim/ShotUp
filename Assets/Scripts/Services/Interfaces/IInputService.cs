using UnityEngine;
using BallShotGame.Core;

namespace BallShotGame.Services
{
    /// <summary>
    /// 입력 관리 서비스 인터페이스
    /// </summary>
    public interface IInputService : IService
    {
        /// <summary>
        /// 드래그 가능한지 확인
        /// </summary>
        /// <returns>드래그 가능 여부</returns>
        bool CanDrag();
        
        /// <summary>
        /// 드래그 시작 위치 가져오기
        /// </summary>
        /// <returns>드래그 시작 위치</returns>
        Vector2 GetDragStartPosition();
        
        /// <summary>
        /// 드래그 상태 설정
        /// </summary>
        /// <param name="isDragging">드래그 중 여부</param>
        void SetDragging(bool isDragging);
        
        /// <summary>
        /// 드래그 중인지 확인
        /// </summary>
        /// <returns>드래그 중 여부</returns>
        bool IsDragging();
    }
}
