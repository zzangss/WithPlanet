using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템을 드래그 할 경우 임시로 DragSlot에 아이템을 저장한다.
/// </summary>
public class DragSlot : Singleton<DragSlot>
{
    [HideInInspector] public InventorySlot CurrentSlot; // 드래그한 아이템의 슬롯 데이터를 가져와 저장 
    [HideInInspector] public bool IsShiftMode; // shift 사용 여부 확인. 아이템 반만 옮기기 구현 
    [SerializeField] private Image mItemImage; // 드래그한 아이템의 이미지 저장 

    public void DragSetImage(Image _itemImage)
    {
        mItemImage.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float alpha)
    {
        Color color = mItemImage.color;
        color.a = alpha;
        mItemImage.color = color;
    }
}