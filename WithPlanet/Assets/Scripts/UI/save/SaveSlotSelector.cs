using UnityEngine;
using UnityEngine.UI;

public class SaveSlotSelector : MonoBehaviour
{
    // 현재 선택된 슬롯 번호를 저장하는 변수
    private int selectedSlot = -1;

    // 슬롯 버튼들을 연결할 배열 (인스펙터에서 설정)
    public Button[] slotButtons;

    // 선택되었을 때의 색상과 기본 색상
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    // 슬롯을 선택했을 때 호출되는 함수
    public void SelectSlot(int slotIndex)
    {
        // 이전에 선택된 슬롯의 색상을 기본값으로 되돌림
        if (selectedSlot != -1 && selectedSlot <= slotButtons.Length)
        {
            slotButtons[selectedSlot - 1].image.color = defaultColor;
        }

        // 새로운 슬롯을 선택하고 색상을 변경
        selectedSlot = slotIndex;
        slotButtons[selectedSlot - 1].image.color = selectedColor;

        Debug.Log($"슬롯 {selectedSlot}이 선택되었습니다.");
    }

    // 현재 선택된 슬롯 번호를 반환하는 함수
    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}