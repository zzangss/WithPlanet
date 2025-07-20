# WithPlanet
KING 15기 하계방학 프로젝트 - WithPlanet

## 🤝 GitHub 협업 가이드

### 1. 협업 절차

1. **레포지터리 클론**  
   - 팀 레포를 클론하여 사용

2. **브랜치 생성**  
   - `main` 브랜치에서 직접 작업하지 않고, 반드시 새로운 브랜치를 생성하여 작업  
   - 브랜치 이름은 아래 네이밍 규칙을 따른다

3. **작업 및 커밋**  
   - 기능 개발, 버그 수정 등을 진행하고 커밋

4. **푸시 & Pull Request(PR)**  
   - 작업한 브랜치를 원격에 푸시한 뒤, `main` 브랜치로 PR을 생성 
   - PR에는 작업한 내용을 간단히 작성

5. **리뷰 & 머지**  
   - 리뷰한 후, 문제가 없다면 `main` 브랜치로 병합


### 2. 브랜치 전략

| 브랜치명           | 설명                     | 예시 |
|-------------------|--------------------------|--------------|
| `main`            | 배포 가능한 안정 버전     | |
| `develop`         | 개발 통합 브랜치         | |
| `feature/기능명`   | 개별 기능 개발 브랜치, 새로운 기능 추가     |`feature/login-page`   |
| `fix/`      | 버그 수정                        | `fix/login-error`      |
| `refactor/` | 리팩토링 (기능 변경 없음)        | `refactor/user-service`|


### 3. 커밋 메시지 규칙

| 태그       | 설명                          |
|------------|-------------------------------|
| `feat`     | 새로운 기능 추가               |
| `fix`      | 버그 수정                      |
| `docs`     | 문서 수정                      |
| `style`    | 포맷팅, 세미콜론 등 비기능 변경 |
| `refactor` | 코드 리팩토링                 |
| `chore`    | 설정, 문서 등 사소한 변경 |
| `test`     | 테스트 코드 추가              |
| `art`      | 아트 에셋 추가/수정           |

**📌 커밋 메시지 예시:**

- feat: 플레이어 점프 기능 추가
- fix: 게임 시작 시 NullReferenceException 해결

### 4. 폴더 구조

```plaintext
Assets/
├── Art/                # 🎨 아트 에셋
│   ├── Characters/     # 캐릭터 스프라이트, 모델 등
│   ├── Environment/    # 배경, 타일셋 등
│   ├── UI/             # UI 이미지 (버튼, 패널 등)
│   └── Icons/          # 인벤토리 아이콘 등
├── Audio/              # 🔊 사운드 리소스
│   ├── BGM/            # 배경 음악
│   └── SFX/            # 효과음
├── Materials/          # 머티리얼 파일
├── Animations/         # 🎞 애니메이션 관련
│   ├── Characters/     # 캐릭터 애니메이션
│   └── UI/             # UI 전환 애니메이션
├── Prefabs/            # 📦 재사용 가능한 프리팹
│   ├── Characters/
│   ├── Environment/
│   └── UI/
├── Scenes/             # 🎬 씬 파일
│   ├── [이름]Scene.unity  # 개발자 별 테스트 씬 (예: Zzangss_Scene.unity)
│   └── Shared/            # 공통 요소를 위한 씬
├── Scripts/            # 📜 코드 구성 (기능/역할별 정리)
│   ├── Core/           # GameManager, 상태 관리, 싱글톤 등 핵심 로직
│   ├── Systems/        # 입력, 시간, 세이브/로드 등 시스템 관리
│   ├── Characters/
│   │   ├── Player/     # 플레이어 동작/컨트롤
│   │   └── Enemy/      # 적 캐릭터 AI 등
│   ├── UI/             # UI 제어 및 상호작용
│   ├── Items/          # 아이템 로직 및 데이터
│   └── Utils/          # 공용 유틸 함수, 확장 메서드
└── Plugins/            # 외부 라이브러리 및 플러그인
```


## 💻 코드 컨벤션 (Avangarde Software 기준 강화판)

### 1. 네이밍 규칙

| 항목                  | 형식        | 예시                                    |
|-----------------------|-------------|-----------------------------------------|
| **Namespaces**        | PascalCase  | `WithPlanet.GamePlay.Player`           |
| **Classes / Methods** | PascalCase  | `PlayerController`, `MovePlayer()`      |
| **Fields**            | camelCase   | `playerSpeed`, `enemyCount`             |
| **Static Fields**     | PascalCase  | `GlobalSpeed`                           |
| **Properties**        | PascalCase  | `public string Name { get; set; }`      |
| **Parameters**        | camelCase   | `HighlightElement(bool isActive)`       |
| **Delegates**         | 접미사 Callback | `ProcessUserCallback`              |
| **Events / Actions**  | 접두사 On   | `OnDeath`, `OnStageClear`               |


### 2. 코드 스타일 규칙

- 네임스페이스 필수 사용 (코드 충돌 방지)
  
- **접근 제한자 명시 필수**
  ```c
  private int health;  // ✅ OK
  int health;          // ❌ 금지
  ```

- 한 줄당 하나의 변수 선언

  ```c
   private int score;
   private int lives;
  ```

- 중괄호는 항상 새 줄에 작성하며, 항상 사용
  ```c
   if (isDead)
   {
      Respawn();
   }
  ```
- 의미 있는 변수명 사용


## 🎮 Unity 특화 규칙

| 금지 사항                            | 대체/설명                                                  |
|-------------------------------------|-------------------------------------------------------------|
| `GameObject.Find()`                 | 성능 문제 → Inspector 참조 또는 스크립트에서 직접 연결      |
| `Update()` 내 `GetComponent()` 사용 | 사용 금지 → `Start()` 등에서 캐싱 후 사용                   |
| 인덱스 결합 배열 (`item[0]` 등)      | `Serializable` 클래스를 활용하여 데이터 구조화             |
| `Invoke()`, `SendMessage()` 사용     | `Coroutine`으로 대체                                       |
| `Singleton`                         | 매니저 클래스에만 사용 (예: `AudioManager`, `GameManager`) |

### 1.  Unity 권장 사항

- **Static 메서드 및 프로퍼티** 적극 활용  
  예시: `GameManager.Player`

- **필수 Unity 속성 사용**
  ```csharp
  [SerializeField] private int hp;

  [Serializable]
  public class ItemData
  {
      public string name;
      public int id;
  }

- **모든 코드는 namespace 내 작성**
  ```c
    namespace WithPlanet.GamePlay
  {
      public class PlayerController : MonoBehaviour
      {
          ...
      }
  }
