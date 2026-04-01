# 구현 계획 (Implementation Plan)

## 개요
DI(Dependency Injection) + Event Bus 패턴을 활용한 볼 슛 게임 구현

---

## Phase 1: Core 인프라 구축

### Task 1.1: IService 인터페이스
- [ ] `Assets/Scripts/Core/IService.cs` 생성
- [ ] 빈 인터페이스 정의 (마커 인터페이스)

### Task 1.2: GameService (DI 컨테이너)
- [ ] `Assets/Scripts/Core/GameService.cs` 생성
- [ ] 싱글톤 패턴 구현
- [ ] `Register<T>()` 메서드 구현
- [ ] `Get<T>()` 메서드 구현
- [ ] 서비스 생명주기 관리 (Initialize/Dispose)

### Task 1.3: EventBus (제네릭 이벤트 버스)
- [ ] `Assets/Scripts/Core/EventBus.cs` 생성
- [ ] `Subscribe<T>()` 메서드 구현
- [ ] `Unsubscribe<T>()` 메서드 구현
- [ ] `Publish<T>()` 메서드 구현
- [ ] 이벤트 핸들러 관리 (Dictionary 기반)

---

## Phase 2: 이벤트 정의

### Task 2.1: BallEvents
- [ ] `Assets/Scripts/Events/BallEvents.cs` 생성
- [ ] `BallDragStartedEvent` 구조체 정의
- [ ] `BallDragUpdatedEvent` 구조체 정의
- [ ] `BallLaunchedEvent` 구조체 정의
- [ ] `BallStoppedEvent` 구조체 정의

### Task 2.2: InputEvents
- [ ] `Assets/Scripts/Events/InputEvents.cs` 생성
- [ ] `InputMouseDownEvent` 구조체 정의
- [ ] `InputMouseDragEvent` 구조체 정의
- [ ] `InputMouseUpEvent` 구조체 정의

### Task 2.3: GameEvents
- [ ] `Assets/Scripts/Events/GameEvents.cs` 생성
- [ ] `GoalReachedEvent` 구조체 정의
- [ ] `ResetRequestedEvent` 구조체 정의
- [ ] `GameResetCompletedEvent` 구조체 정의

---

## Phase 3: Config 설정

### Task 3.1: GameConfig
- [ ] `Assets/Scripts/Config/GameConfig.cs` 생성
- [ ] `ForceMultiplier` 상수 정의 (10f)
- [ ] `StopThreshold` 상수 정의 (0.05f)
- [ ] `ResetDelay` 상수 정의 (3f)
- [ ] `ResetPosition` 상수 정의 (Vector3.zero)

---

## Phase 4: 서비스 구현

### Task 4.1: 서비스 인터페이스 정의
- [ ] `Assets/Scripts/Services/Interfaces/IBallService.cs` 생성
  - Initialize(), Launch(), IsStopped(), Reset() 메서드 정의
- [ ] `Assets/Scripts/Services/Interfaces/IInputService.cs` 생성
  - Initialize(), Update() 메서드 정의
- [ ] `Assets/Scripts/Services/Interfaces/IGoalService.cs` 생성
  - Initialize(), CheckGoal() 메서드 정의

### Task 4.2: InputService 구현
- [ ] `Assets/Scripts/Services/InputService.cs` 생성
- [ ] 마우스 입력 감지 (Input.GetMouseButton)
- [ ] 화면 좌표 → 월드 좌표 변환 (Camera.main.ScreenToWorldPoint)
- [ ] 이벤트 발행 (InputEvents)
- [ ] 공이 정지 상태일 때만 입력 처리

### Task 4.3: BallService 구현
- [ ] `Assets/Scripts/Services/BallService.cs` 생성
- [ ] Rigidbody2D 참조 관리
- [ ] Launch() 메서드: AddForce로 발사
- [ ] IsStopped() 메서드: 속도 체크 (GameConfig.StopThreshold)
- [ ] Update()에서 정지 상태 감지 및 BallStoppedEvent 발행
- [ ] Reset() 메서드: 위치/속도 초기화

### Task 4.4: GoalService 구현
- [ ] `Assets/Scripts/Services/GoalService.cs` 생성
- [ ] Goal Tilemap 참조 관리
- [ ] 골인 상태 플래그 관리

---

## Phase 5: 컨트롤러 구현

### Task 5.1: BallController 구현
- [ ] `Assets/Scripts/Controllers/BallController.cs` 생성
- [ ] MonoBehaviour 상속
- [ ] Awake(): GameService에서 서비스 조회
- [ ] 이벤트 구독 설정 (Start())
  - BallDragStartedEvent → 드래그 시작 위치 저장
  - BallDragUpdatedEvent → 드래그 거리 계산
  - BallLaunchedEvent → BallService.Launch() 호출
  - ResetRequestedEvent → ResetBall() 호출
- [ ] 드래그 중 발사 방향/세기 계산
- [ ] OnDestroy(): 이벤트 구독 해제

### Task 5.2: GoalController 구현
- [ ] `Assets/Scripts/Controllers/GoalController.cs` 생성
- [ ] MonoBehaviour 상속
- [ ] OnTriggerEnter2D() 구현
- [ ] GoalReachedEvent 발행
- [ ] Goal Tilemap에 Trigger Collider 설정 (Unity Editor)

### Task 5.3: GameController 구현
- [ ] `Assets/Scripts/Controllers/GameController.cs` 생성
- [ ] MonoBehaviour 상속
- [ ] Awake(): GameService 초기화 및 서비스 등록
  - BallService, InputService, GoalService 등록
- [ ] Start(): 이벤트 구독
  - GoalReachedEvent → StartCoroutine(ResetAfterDelay())
- [ ] ResetAfterDelay() 코루틴 구현
  - 3초 대기 (GameConfig.ResetDelay)
  - ResetRequestedEvent 발행

---

## Phase 6: Unity 씬 설정

### Task 6.1: 컴포넌트 추가
- [ ] GameController 컴포넌트를 씬의 빈 GameObject에 추가
- [ ] BallController 컴포넌트를 Player(Ball) GameObject에 추가
- [ ] GoalController 컴포넌트를 Goal Tilemap GameObject에 추가
- [ ] Player에 Rigidbody2D 컴포넌트 확인 (있어야 함)
- [ ] Player에 CircleCollider2D 컴포넌트 확인 (있어야 함)

### Task 6.2: 충돌 설정
- [ ] Goal Tilemap에 TilemapCollider2D + Trigger 활성화
- [ ] 또는 BoxCollider2D 2D + Is Trigger 체크
- [ ] Physics2D 레이어 설정 확인

### Task 6.3: 카메라 설정
- [ ] Camera.main이 존재하는지 확인
- [ ] 카메라가 Orthographic 모드인지 확인

---

## Phase 7: 테스트 및 디버깅

### Task 7.1: 단위 테스트
- [ ] GameService 등록/조회 테스트
- [ ] EventBus 구독/발행 테스트
- [ ] BallService Launch/Reset 테스트

### Task 7.2: 통합 테스트
- [ ] 드래그 → 발사 흐름 테스트
- [ ] 공 정지 감지 테스트
- [ ] 골인 → 리셋 흐름 테스트 (3초 타이머)

### Task 7.3: 엣지 케이스
- [ ] 공이 멈추지 않을 때 드래그 시도
- [ ] 연속 클릭 테스트
- [ ] 빠른 드래그/발사 테스트

---

## 완료 조건 (Definition of Done)

- [ ] 모든 스크립트 컴파일 에러 없음
- [ ] 드래그 거리에 비례하여 공 발사됨
- [ ] 공이 완전히 멈출 때까지 재입력 불가
- [ ] Goal 도달 시 3초 후 자동 리셋
- [ ] 리셋 시 공이 원점(0,0)으로 이동
- [ ] 이벤트 버스를 통한 의존성 분리 확인
- [ ] DI 패턴 적용 확인

---

## 예상 소요 시간

| Phase | 예상 시간 |
|-------|----------|
| Phase 1: Core 인프라 | 30분 |
| Phase 2: 이벤트 정의 | 20분 |
| Phase 3: Config | 10분 |
| Phase 4: 서비스 구현 | 45분 |
| Phase 5: 컨트롤러 구현 | 60분 |
| Phase 6: 씬 설정 | 20분 |
| Phase 7: 테스트 | 30분 |
| **총계** | **~3.5시간** |
