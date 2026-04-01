using UnityEngine;

namespace BallShotGame.Config
{
    /// <summary>
    /// 게임 설정값 상수 관리
    /// 전역 설정값 중앙 관리
    /// </summary>
    public static class GameConfig
    {
        #region Physics Settings
        
        /// <summary>
        /// 드래그 거리 → 발사력 변환 계수
        /// 드래그 거리 × 이 값 = 발사 힘
        /// </summary>
        public const float ForceMultiplier = 10f;
        
        /// <summary>
        /// 정지 상태 판정 속도 threshold
        /// 이 값 이하이면 정지로 간주하고 속도를 즉시 0으로 설정
        /// </summary>
        public const float StopThreshold = 0.3f;
        
        /// <summary>
        /// 최대 발사 힘 (제한값)
        /// </summary>
        public const float MaxLaunchForce = 18f;
        
        /// <summary>
        /// 최소 발사 힘 (너무 약한 드래그 무시)
        /// </summary>
        public const float MinLaunchForce = 0.5f;
        
        /// <summary>
        /// 속도 감쇄 계수 (0~1, 1에 가까울수록 천천히 멈춤)
        /// 매 프레임 속도에 곱해짐
        /// </summary>
        public const float VelocityDamping = 0.97f;
        
        /// <summary>
        /// 마찰력 (0~1, 높을수록 마찰이 커서 빨리 멈춤)
        /// </summary>
        public const float Friction = 0.02f;
        
        #endregion
        
        #region Game Settings
        
        /// <summary>
        /// 골인 후 리셋 대기 시간 (초)
        /// </summary>
        public const float ResetDelay = 3f;
        
        /// <summary>
        /// 공 리셋 위치
        /// </summary>
        public static readonly Vector3 ResetPosition = Vector3.zero;
        
        /// <summary>
        /// 화면 밖 이탈 판정 거리
        /// 이 거리 이상 멀어지면 리셋
        /// </summary>
        public const float OutOfBoundsDistance = 50f;
        
        #endregion
        
        #region Input Settings
        
        /// <summary>
        /// 드래그 가능한 최대 거리
        /// </summary>
        public const float MaxDragDistance = 5f;
        
        /// <summary>
        /// 마우스 클릭으로 인식하는 최대 이동 거리
        /// </summary>
        public const float ClickThreshold = 0.1f;
        
        /// <summary>
        /// 공 클릭 인식 범위 (반지름)
        /// </summary>
        public const float BallClickRadius = 1f;
        
        /// <summary>
        /// 땅으로 인식하는 최대 각도 (도)
        /// 이 각도 이내이면 땅으로 간주하여 감속 적용
        /// </summary>
        public const float GroundDetectionAngle = 45f;
        
        #endregion
        
        #region Layer Settings
        
        /// <summary>
        /// Ground 레이어 마스크
        /// </summary>
        public const int GroundLayer = 6;
        
        /// <summary>
        /// Goal 레이어 마스크
        /// </summary>
        public const int GoalLayer = 7;
        
        #endregion
    }
}
