using UnityEngine;

/// <summary>
/// Item을 넣는 공간 프리팹에 컴포넌트로 추가하고 인스펙터에 아이템을 할당한다.
/// </summary>
public class ItemPickupPoint : MonoBehaviour
{
    [Header("해당 오브젝트에 할당되는 아이템")]
    [SerializeField] private Item mItem;
    public Item Item
    {
        get
        {
            return mItem; 
        }
    }

    [Header("해당 오브젝트에 상호작용시, 보여줄 인디케이터의 높이")]
    [SerializeField] private float mIndicatorHeight;
    /// <summary>
    /// 인디케이터의 높이
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

