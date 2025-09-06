using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SaveSlotSelector : MonoBehaviour
{
    private int selectedSlot = -1;
    public Button[] slotButtons;
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    // 세이브 컨트롤러와 슬롯 텍스트 배열
    public SaveController saveController;
    public TextMeshProUGUI[] playTimeTexts;
    public TextMeshProUGUI[] dateTexts;

    private void Start()
    {
        RefreshUI(); // 시작 시 UI를 새로고침
    }

    // UI를 새로고침하여 현재 세이브 파일을 반영하는 함수
    public void RefreshUI()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1; // 슬롯 번호 (1부터 시작)

            // 세이브 파일이 존재하는지 확인
            bool hasFile = saveController.HasSaveFile(slotIndex);

            slotButtons[i].gameObject.SetActive(hasFile); // 파일이 있으면 버튼 활성화

            if (hasFile)
            {
                // 파일이 있으면 플레이 시간과 날짜 로드
                float loadedPlayTime = saveController.GetPlayTime(slotIndex);
                string lastSavedDate = saveController.GetLastSavedDate(slotIndex);

                playTimeTexts[i].text = FormatPlayTime(loadedPlayTime);
                dateTexts[i].text = lastSavedDate;
            }
            else
            {
                // 파일이 없으면 텍스트도 비워둠
                playTimeTexts[i].text = "";
                dateTexts[i].text = "";
            }
        }
    }

    // 슬롯 선택 함수
    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;

        // 모든 버튼의 색상을 기본 색상으로 되돌립니다.
        for (int i = 0; i < slotButtons.Length; i++)
        {
            slotButtons[i].GetComponent<Image>().color = defaultColor;
        }

        // 선택된 버튼(slotIndex는 1부터 시작)의 색상만 변경합니다.
        if (slotIndex > 0 && slotIndex <= slotButtons.Length)
        {
            slotButtons[slotIndex - 1].GetComponent<Image>().color = selectedColor;
        }
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
    public void SetSelectedSlot(int slotnum)
    {
        selectedSlot=slotnum;
    }

   // (float)를 "00h 00m 00s" 형식으로 변환하는 도우미 함수
    private string FormatPlayTime(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}h {1:D2}m {2:D2}s", t.Hours, t.Minutes, t.Seconds);
    }
}