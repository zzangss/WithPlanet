using UnityEngine;

/// <summary>
/// Item�� �ִ� ���� �����տ� ������Ʈ�� �߰��ϰ� �ν����Ϳ� �������� �Ҵ��Ѵ�.
/// </summary>
public class ItemPickUp : MonoBehaviour
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

}