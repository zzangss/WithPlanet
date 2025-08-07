using UnityEngine;

/// <summary>
/// Item�� �ִ� ���� �����տ� ������Ʈ�� �߰��ϰ� �ν����Ϳ� �������� �Ҵ��Ѵ�.
/// </summary>
public class ItemPickupPoint : MonoBehaviour
{
    [Header("�ش� ������Ʈ�� �Ҵ�Ǵ� ������")]
    [SerializeField] private Item mItem;
    public Item Item
    {
        get
        {
            return mItem; 
        }
    }

    [Header("�ش� ������Ʈ�� ��ȣ�ۿ��, ������ �ε��������� ����")]
    [SerializeField] private float mIndicatorHeight;
    /// <summary>
    /// �ε��������� ����
    /// /// </summary>
    /// <value></value>
    public float IndicatorHeight
    {
        get
        {
            return mIndicatorHeight;
        }
    }
}

