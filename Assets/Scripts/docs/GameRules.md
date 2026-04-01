# Ball Shot Game - 설계 문서

## 폴더 구조

```
Assets/Scripts/
├─── Core/
│   ├─── GameService.cs          # DI 컨테이너 (Service Locator)
│   ├─── EventBus.cs             # 제네릭 이벤트 버스
│   └─── IService.cs             # 서비스 인터페이스
├─── Events/
│   ├─── BallEvents.cs           # 공 관련 이벤트 정의
│   ├─── InputEvents.cs          # 입력 관련 이벤트 정의
│   └─── GameEvents.cs           # 게임 상태 이벤트 정의
├─── Services/
│   ├─── Interfaces/
│   │   ├─── IBallService.cs     # 공 서비스 인터페이스
│   │   ├─── IInputService.cs    # 입력 서비스 인터페이스
│   │   └─── IGoalService.cs     # 골 서비스 인터페이스
│   ├─── BallService.cs          # 공 물리/상태 관리
│   ├─── InputService.cs         # 마우스 입력 처리
│   └─── GoalService.cs          # 골인지점 관리
├─── Controllers/
│   ├─── BallController.cs       # 공 드래그/발사
│   ├─── GoalController.cs       # 골인지점 충돌 감지
│   └─── GameController.cs       # 게임 흘릌 제어 (리셋 등)
└─── Config/
    └─── GameConfig.cs           # 게임 설정값 (상수 관리)
```

---

## 게임 구조

### 1. 드래그 및 발사
- **마우스 클릭**: 드래그 시작 (공이 정지 상태일 때만 가능)
- **드래그 중**: 공을 당기는 방향으로 이동
- **마우스 놓기**: 반대 방향으로 발사
- **발사 세기**: 드래그 거리에 비례 (forceMultiplier = 10f)

### 2. 정지 상태 판정
- 속도 < 0.05f 이면 정지로 간주
- 정지 후에만 다음 드래그 가능

### 3. 골인 및 리셋
- Goal Tilemap 충돌 시 골인
- 3초 후 자동 리셋
- 리셋 위치: 원점 (0, 0, 0)

---

## 이벤트 구조 (제네릭)

### 공 이벤트 (BallEvents.cs)
```csharp
public struct BallDragStartedEvent { public Vector2 StartPosition; }
public struct BallDragUpdatedEvent { public Vector2 CurrentPosition; }
public struct BallLaunchedEvent { public Vector2 Force; }
public struct BallStoppedEvent { }
```

### 입력 이벤트 (InputEvents.cs)
```csharp
public struct InputMouseDownEvent { public Vector2 Position; }
public struct InputMouseDragEvent { public Vector2 Position; }
public struct InputMouseUpEvent { public Vector2 Position; }
```

### 게임 이벤트 (GameEvents.cs)
```csharp
public struct GoalReachedEvent { }
public struct ResetRequestedEvent { }
public struct GameResetCompletedEvent { }
```

---

## DI (Dependency Injection)

### GameService - 서비스 등록/조회
```csharp
// 등록
GameService.Register<IBallService>(new BallService());
GameService.Register<IInputService>(new InputService());

// 조회
var ballService = GameService.Get<IBallService>();
```

---

## 이벤트 흘릌

```
[드래그 시작]
InputService → EventBus.Publish(new BallDragStartedEvent())
              ↓
         BallController → 드래그 상태 저장

[발사]
InputService → EventBus.Publish(new BallLaunchedEvent(force))
              ↓
         BallController → BallService.Launch(force)

[정지 감지]
BallService → 속도 < 0.05f
           → EventBus.Publish(new BallStoppedEvent())

[골인]
GoalController → OnTriggerEnter2D
              → EventBus.Publish(new GoalReachedEvent())
              ↓
         GameController → 3초 대기 → EventBus.Publish(new ResetRequestedEvent())
```

---

## 설정값 (GameConfig.cs)

| 설정값 | 값 | 설명 |
|--------|-----|------|
| ForceMultiplier | 10f | 드래그 거리 → 발사력 변환 계수 |
| StopThreshold | 0.05f | 정지 상태 판정 속도 |
| ResetDelay | 3f | 골인 후 리셋 대기 시간 |
| ResetPosition | (0, 0, 0) | 공 리셋 위치 |
