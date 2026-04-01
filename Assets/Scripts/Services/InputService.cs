using UnityEngine;
using BallShotGame.Core;
using BallShotGame.Events;

namespace BallShotGame.Services
{
    /// <summary>
    /// 마우스 입력 처리 서비스
    /// </summary>
    public class InputService : IInputService
    {
        private bool _isDragging = false;
        private Vector2 _dragStartPosition;
        private float _dragStartTime;

        public void Initialize()
        {
            Debug.Log("InputService initialized");
        }

        public void Dispose()
        {
        }

        public bool CanDrag()
        {
            return !_isDragging;
        }

        public Vector2 GetDragStartPosition()
        {
            return _dragStartPosition;
        }

        public void SetDragging(bool isDragging)
        {
            _isDragging = isDragging;
            
            if (isDragging)
            {
                _dragStartPosition = GetMouseWorldPosition();
                _dragStartTime = Time.time;
                
                EventBus.Instance.Publish(new InputMouseDownEvent
                {
                    Position = _dragStartPosition,
                    ScreenPosition = Input.mousePosition
                });
            }
            else
            {
                EventBus.Instance.Publish(new InputMouseUpEvent
                {
                    Position = GetMouseWorldPosition(),
                    ScreenPosition = Input.mousePosition,
                    DragDuration = Time.time - _dragStartTime
                });
            }
        }

        public bool IsDragging()
        {
            return _isDragging;
        }

        /// <summary>
        /// 매 프레임 호출 - 입력 감지
        /// </summary>
        public void Update()
        {
            // 드래그 중일 때 마우스 위치 업데이트
            if (_isDragging)
            {
                EventBus.Instance.Publish(new InputMouseDragEvent
                {
                    Position = GetMouseWorldPosition(),
                    ScreenPosition = Input.mousePosition,
                    Delta = GetMouseWorldPosition() - _dragStartPosition
                });
            }
        }

        private Vector2 GetMouseWorldPosition()
        {
            if (Camera.main != null)
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            return Vector2.zero;
        }
    }
}
