인벤토리 시스템 적용 방법

1. UI 프리팹을 Hierarchy로 옮기기

경로 : Assets/Prefabs/UI
-> 여기서 Inventory System & Inventory 옮기기

2. 스크립트 연결
Inventory System -> Inspector -> Inventory Main의 변수에 밑의 두 가지 넣기 

-> M Inventory Base : Inventory의 PARENT_InventoryBase
-> M Inventory Slosts Parent : GRIDLAYOUT_SlotsParent