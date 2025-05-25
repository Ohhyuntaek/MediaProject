# 미디어 프로젝트 - Defeat the Darks (Dawnfall)
## 팀명: Eightone
- 팀원: 윤지석, 양경덕, 오현택, 문 혁(외부 인원)
  
---
Youtube: https://www.youtube.com/watch?v=PAZx04rv7VU&ab_channel=%EC%9C%A4%EC%A7%80%EC%84%9D (인디크래프트 공모전 제출 당시 영상)

### 1. 프로젝트 소개

![dawnfall title](https://github.com/user-attachments/assets/4c0556bc-9809-4f86-bb41-e9630aaa1bbf)

- Unity Engine을 기반으로 제작된 2D 로그라이크 게임 제작 프로젝트입니다.

### 2. 프로젝트 개요

- 개발 기간: 2025.03.10. ~ 2025.05.25. (이 후 추가 개발 진행 예정)
  
- 개발 도구: Unity 6000.0.32f1

- 개발 언어: C#

- 씬 구조:

  | 이름 | 연결된 다른 씬 |
  |-----|----------------|
  | TitleScene | LoadingScene, MainScene |
  | MainScene | TitleScene, LoadingScene, MapScene |
  | MapScene | MainScene, LoadingScene, Shop, InStage |
  | InStage | MapScene, LoadingScene |
  | Shop | MapScene, LoadingScene |

### 3. 기술

- Unity
  
  - FSM: Dark, Ally, Dawn.
    
  - Object Pool: Spawn the Allies.
    
  - Scriptable Object: DarkData, AllyData, DawnData, etc.
    
  - Singletone: RuntimeDataManager, etc.

  - Async: LoadingScene

- Git
  
  - Git-Flow 형식의 Branch 관리
    
  - Ryder
 
- 생성형 AI
  
  - SUNO AI Music
    
  - Chat GPT

- 디자인
  
  - adobe photoshop
    
  - itch.io

### 4. 플레이 설명

- 조작
  
  > 마우스 클릭
  
  > Space bar (Dawn Active Skill)

### 5. Ally

**5-1. 개요**

  - 유닛은 기본적인 특성항목을 공유하며 (공격력 , 사거리 등) 각 항목 당 수치는 상이합니다. 각 유닛의 역할군, 종족, 혹은 유닛 자체에 따라 고유의 항목이 존재합니다.

**5-2. Ally Status**

  - 유닛 사거리 : 유닛 사거리는 숫자로 표기합니다. 유닛 사거리는 기본적으로 자신앞에 있는 적을 포함하며 적혀있는 숫자만큼 적이 스폰되는 쪽에 가까워집니다. 

  - 유닛 공격 방식 : 유닛 공격 방식은 2가지 범주로 나뉩니다. 원/근거리, 단일/범위 공격. 
    
    - 단일 공격은 단 하나의 적 유닛만을 공격하며, 범위 공격은 한 칸에 있는 적 유닛을 전부 공격합니다.
    
    - 예외적으로 다수를 노리는 공격이 있습니다. 이 경우는 여러 유닛을 공격하지만, 한 칸에 있는 적을 모두 공격하지 않을 수도, 또한 공격하는 적이 여러 칸에 나뉘어 있을 수도 있습니다.

  - 유닛 종족 : 각 유닛마다 종족이 지정되어 있습니다. 각 종족은 상성을 가지고 있습니다. 적 유닛 역시 여러 범주로 나뉘어 있는데, 쉽게 말해 포켓몬스터의 타입 개념이라고 생각하면 됩니다. 추후 적 유닛을 추가하면서 이 부분도 추가하겠습니다. 현재는 단순하게 종족만 지정되어있고 상성은 지정되지 않은 상태입니다. 

  - 발판 강화 : 각 유닛이 해당 유닛의 역할군에 맞는 위치에 존재한다면, 발판 강화보너스가 발동됩니다. 이 경우 유닛 특성에 맞는 추가 효과가 발생합니다. 역할군마다 특화되어 있는 효과가 다릅니다. 일반적으로 다음 기준을 따라갑니다.
  
    a. 전열 : 아군에게 도움을 주거나 적들에게 이동 방해를 거는 유형
    
    b. 중열 : 적에게 강력한 데미지를 주는 유형
    
    c. 후열 : 적에게 디버프를 주거나, 아군에게 버프를 주는 유형

  - 일반 공격 : 모든 유닛은 사정거리 안의 적 중 가장 가까운 적에게 우선 공격합니다. 만일 같은 위치에 있다면 둘 중 더 먼저 스폰된 몬스터에게 공격합니다. 

**5-3. Ally 리스트**

| 이미지 | 이름 | 역할군 | 링크 |
|--------|------|--------|------|
|![Joandarc](https://github.com/user-attachments/assets/083edb28-8560-490d-9ff9-61f7640f3b87) | Joan Dark | Front Lain | [Asset Site](https://otsoga.itch.io/swordswoman) |
|![Rogue](https://github.com/user-attachments/assets/1030e892-e520-4955-8a8a-7bcfd26838ba) | Rogue | Front Lain | [Asset Site](https://pixelthewise.itch.io/character-animation-set) |
|![NightLord](https://github.com/user-attachments/assets/8dba4548-5f6f-4cb5-adaf-3a89db9e10ee) | Night Lord | Mid Lain | [Asset Site](https://otsoga.itch.io/night-lord) |
|![BountyHunter](https://github.com/user-attachments/assets/0a27baa3-2f61-434f-9b6b-aa9d0ba9f3d0) | The Bounty Hunter | Mid Lain | [Asset Site](https://clembod.itch.io/bounty-h)|
|![CentaurLady](https://github.com/user-attachments/assets/8e83d5df-ee5b-4b77-a05e-4e2d64758031) | Centaur Lady | Rear Lain | [Asset Site](https://otsoga.itch.io/centaur-lady) |
|![SalamanderWitch](https://github.com/user-attachments/assets/ebf33175-a45f-4f71-85c9-22f60c253566) | Salamander Witch | Rear Lain | [Asset Site](https://otsoga.itch.io/eleonore) |

**5-4. Ally Parameter**

| 한국어 | 영어 | 설명 | 수치 범위 | 자료형 | 초기화 수치 | 강화 여부 |
|--------|------|------|-----------|--------|-------------|-----------|
| 공격력 | attack | 아군 유닛의 기본 공격력 | 2 ~ 20 | float | - | O |
| 사거리 | range | 아군 유닛의 사거리 | - | - | - | - |
| 공격 속도 | attackSpeed | 아군 유닛의 공격속도 | 1 ~ 3 | float | - | O |
| 지속 시간 | duration | 아군 유닛이 필드에 존재하는 시간 | 5 ~ 10 | float | 5 | O |
| 소환 코스트 | cost | 아군 유닛을 소환하기 위해 사용되는 빛 조각의 개수 | 2 ~ 5 | int | - | O |

### 6. Dawn (플레이어 캐릭터)

- 플레이어 캐릭터는 Ally 중 하나를 선택하여 사용합니다. Ally 상태와는 다른 스킬을 보유합니다.

- 액티브 스킬은 Space bar를 누르면 사용할 수 있습니다.

  a. Joan Dark
    
    - 빛을 다루는 수호자
    
    - 광역 버프 & 유닛 유지력
    
    - 패시브: **Twilight Aura(황혼의 기운)**
 
      - 매 3번째 아군이 배치될 때, 인접한(같은 열의 좌 우) 유닛들에게 1초동안 군중 제어 면역 효과를 부여
 
    - 액티브: **Daybreak(새벽의 여명)**
 
      - 필드의 모든 Ally의 유지 시간을 최대로 회복
     
### 7. Dark

**7-1. 개요**

  - Dark는 기본, 특수, 보스로 나뉩니다.
  
  - 각 Dark는 서로 다른 개체 값을 가지고 있으며, Dark에 따라 아군 Aly와의 상성을 가지고 있습니다. (추가 예정)

**7-2. Dark Types**

  a. 기본

    - 기본 Dark는 특별한 능력을 가지고 있지 않습니다.

  b. 특수

    - 특수 Dark는 고유의 이동 혹은 공격 매커니즘을 가지고 있습니다.

  c. 보스

    - 보스 Dark는 Boss Stage에 등장합니다. 여러 개의 패턴을 보유하고 있습니다.

**7-3. Dark Table**

| 이름 | 종류 | 링크 |
|------|-----|-------|
| Dark Dusts | 기본 및 특수 Dark | [Asset Site](https://penusbmic.itch.io/the-dark-series-top-down-monster-pack-1) |
| Boss | 보스 Dark | [Asset Site](https://penusbmic.itch.io/the-dark-series-the-tarnished-widow-boss) |

### 8. Stage

**8-1. 개요**

![image](https://github.com/user-attachments/assets/8edb357a-11b1-4131-9fcb-e3ecb664ab87)

- MapScene 진입 시, 여러 경로와 노드로 나뉜 것을 볼 수 있습니다.

  - 경로는 유저가 이동할 수 있는 길을 의미합니다.
 
  - 노드는 유저가 진입할 수 있는 스테이지를 의미합니다.
 
**8-2. Stage Type**

| 이름 | 타입 | 역할 |
|------|------|------|
| Normal Stage | StageType.Normal | 기본 Dark가 몰려오는 일반적인 Stage |
| Shop | StageType.Shop | 아이템을 구매할 수 있는 Stage |
| Boss | StageType.Boss | Boss Dark가 몰려오는 Boss Stage |

### 9. Ally 카드, 강화 카드, 아이템

**9-1. Ally 카드**

![image](https://github.com/user-attachments/assets/cd3e1dc7-977b-4dc4-9b30-dd09ec8fedb5)

-  카드 이미지와 일치하는 Ally를 

**9-2. 강화 카드**

- 

**9-3. 아이템**

- 

### 10. UI 및 기타 에셋

| 이름 | UI 오브젝트 | 씬 | 링크 |
|------|------------|----|-----|
| Isometric Asset Jumpstart Pack | InStage 타일 맵 디자인 | InStage | [Asset Site](https://philtacular.itch.io/pixel-art-tileset-isometric-starter-pack) |
| Super Asset Bundle #5 : Mini Pocket Status | Book, Map, Deck, Card 등 기타 UI | MainScene, MapScene, InStage, Shop | [Asset Site](https://humblepixel.itch.io/super-asset-bundle-5-mini-pocket-status) |
| Holy Spell Effect | Ally 소환 이펙트 | InStage | [Asset Site](https://pimen.itch.io/holy-spell-effect) |
| Pixel Reward Series #1: Coins | 클리어 이벤트 | InStage | [Asset Site](https://humblepixel.itch.io/pixel-reward-series-1-coins/download/fIlxm_Cgp1MJ6Vb_5ElAc8WreZR_pAEXSEF231C1) |
| LockVenture #5: Weather Forecast | 강화 카드 | InStage | [Asset Site](https://humblepixel.itch.io/lockventure-5-weather-forecast/download/0DKYZnHA1QVmTlqxfpVcZBBj6NpXrEu62xwLCAtK) |
| Pixel Buttons | 설정 및 돌아가기 버튼 | MapScene | [Asset Site](https://humblepixel.itch.io/pixel-buttons/download/TRDpBhWBfraVOOssJHAVYF6S2_cPOmR6nY6mPKcj) |
| Free Pixel Font - Thaleah | 폰트 | TitleScene, MainScene, MapScene, InStage | [Asset Site](https://tinyworlds.itch.io/free-pixel-font-thaleah) |
| Silver: Premere pixel font for games | 폰트 | TitleScene, MainScene, MapScene, InStage, Shop | [Asset Site](https://poppyworks.itch.io/silver) |
| 8000+ Raven Fantasy Icons | 아이콘 | MapScene, Shop | [Asset Site](https://clockworkraven.itch.io/raven-fantasy-icons) |
| All in 1 Vfx Toolkit | 공격 이펙트 | InStage | [Asset Site](https://assetstore.unity.com/packages/vfx/all-in-1-vfx-toolkit-206665) |
| All in 1 Sprite Shader | 피격 이펙트 | InStage | [Asset Site](https://assetstore.unity.com/packages/vfx/shaders/all-in-1-sprite-shader-156513) |
| DOTween Pro | 카메라 흔들림 | InStage | [Asset Site](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) |
