using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class AddressableKeyGenerator : MonoBehaviour
{
    // 유니티 상단 메뉴에 항목을 추가합니다.
    [MenuItem("Tools/Addressables/Generate Key Constants")]
    public static void Generate()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings를 찾을 수 없습니다. Addressables를 먼저 설정해주세요.");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// 이 파일은 AddressableKeyGenerator에 의해 자동으로 생성되었습니다. 수동으로 수정하지 마세요!\n");
        sb.AppendLine("public static class AddressKeys");
        sb.AppendLine("{\n");

        // 변수명 중복 체크를 위한 리스트
        HashSet<string> usedVariableNames = new HashSet<string>();

        foreach (var group in settings.groups)
        {
            // 기본 그룹이나 빈 그룹은 건너뜁니다.
            if (group.ReadOnly) continue;

            foreach (var entry in group.entries)
            {
                // 전체 경로 가져오기
                string fullPath = entry.address;

                // 경로에서 파일 이름만 추출
                // ex) "Characters/Player/Knight" -> "Knight"
                string fileName = Path.GetFileNameWithoutExtension(fullPath);

                // 변수명으로 부적절한 문자 제거 및 정리
                string varName = SanitizeVariableName(fileName);

                // 중복 이름 처리 (Knight, Knight_1, Knight_2..)
                string finalVarName = varName;
                int counter = 1;
                while (usedVariableNames.Contains(finalVarName))
                {
                    finalVarName = $"{varName}_{counter}";
                    counter++;
                }
                usedVariableNames.Add(finalVarName);

                sb.AppendLine($"    public static readonly string {finalVarName} = \"{entry.address}\";\n");
            }
        }

        sb.AppendLine("}");

        // 저장 경로 
        string directoryPath = Path.Combine(Application.dataPath, "01_Script/99_Utility");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        string filePath = Path.Combine(directoryPath, "AddressKeys.cs");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        // 유니티가 새로 생성된 파일을 인식하도록 리프레시
        AssetDatabase.Refresh();

        Debug.Log($"<color=green><b>Addressable Keys 생성 완료!</b></color> 경로: {filePath}");
    }

    private static string SanitizeVariableName(string name)
    {
        // 공백, 슬래시, 대시 등을 언더바로 치환하고 특수문자 제거
        string sanitized = name.Replace(" ", "_").Replace("-", "_").Replace(".", "_").Replace("/", "_");

        // 숫자로 시작하면 앞에 언더바 추가
        if (char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        return sanitized;
    }
}
