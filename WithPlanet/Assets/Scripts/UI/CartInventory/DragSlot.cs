using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������� �巡�� �� ��� �ӽ÷� DragSlot�� �������� �����Ѵ�.
/// </summary>
public class DragSlot : Singleton<DragSlot>
{
    [HideInInspector] public InventorySlot CurrentSlot; // �巡���� �������� ���� �����͸� ������ ���� 
    [HideInInspector] public bool IsShiftMode; // shift ��� ���� Ȯ��. ������ �ݸ� �ű�� ���� 
    [SerializeField] private Image mItemImage; // �巡���� �������� �̹��� ���� 

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