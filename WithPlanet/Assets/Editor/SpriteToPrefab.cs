using UnityEngine;
using UnityEditor;
using System.IO; // 파일 경로

public class SpriteToPrefab
{
    // Assets 메뉴에 새 기능을 추가
    [MenuItem("Assets/Create Prefabs From Selected Sprites")]
    private static void CreatePrefabsFromSprites()
    {
        // --- 1. 현재 선택한 폴더 경로 가져오기 
        string prefabFolder;
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("오류", "먼저 프로젝트 창에서 프리팹을 저장할 '폴더'를 선택해주세요.", "확인");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedObject);

        // 선택한 것이 폴더가 맞는지 확인
        if (AssetDatabase.IsValidFolder(path))
        {
            prefabFolder = path;
        }
        // (만약 폴더가 아닌 '파일'을 선택했다면, 그 파일이 속한 폴더를 경로로 사용)
        else if (!string.IsNullOrEmpty(path))
        {
            prefabFolder = Path.GetDirectoryName(path);
        }
        else
        {
            EditorUtility.DisplayDialog("오류", "프로젝트 창에서 유효한 폴더나 파일을 선택해주세요.", "확인");
            return;
        }
        // --- 경로 설정 끝 ---


        // 프로젝트 창에서 선택한 모든 오브젝트를 가져오기.
        Object[] selectedAssets = Selection.objects;

        int createdCount = 0;

        foreach (Object asset in selectedAssets)
        {
            // 선택한 것이 스프라이트가 맞는지 확인.
            if (asset is Sprite sprite)
            {
                // 1. 스프라이트 이름으로 새 게임 오브젝트 생성
                string gameObjectName = sprite.name;
                GameObject go = new GameObject(gameObjectName);

                // 2. SpriteRenderer 추가 및 스프라이트 할당
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;


                /* 필요한 경우 사용. 
                //  1. 기울기 설정
                go.transform.rotation = Quaternion.Euler(45, 45, 0);

                //  2. 사이즈 조절
                go.transform.localScale = new Vector3(0.05f, 0.05f, 1f);
                */

                // 3. 프리팹으로 저장
                // 현재 선택한 폴더 경로 + 스프라이트 이름으로 최종 경로 설정
                string prefabPath = prefabFolder + "/" + gameObjectName + ".prefab";

                // 중복 방지
                prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

                // 4. 씬에 임시로 만들었던 게임 오브젝트 삭제
                GameObject.DestroyImmediate(go);

                createdCount++;
            }
        }

        Debug.Log($"스프라이트 {createdCount}개로부터 프리팹을 자동 생성했습니다. (저장위치: {prefabFolder})");
    }
}