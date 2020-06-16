using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSScriptLibrary;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Tang.Editor
{
    public class AnimatorLoadExcel
    {
        private static AnimatorLoadExcel instance;

        public static AnimatorLoadExcel Instance => instance ?? (instance = new AnimatorLoadExcel());
      
        public static JObject GetJObjectFromExcel(string excelPath)
        {
            List<string> sheetNameList = new List<string>()
                {"idle", "walk", "action", "hurt", "death", "run", "jump", "jumpSoftAttack", "jumpHardAttack", "dodge", "runSoftAttack", "runHardAttack"};

            JObject excelJObject = new JObject();
            foreach (var sheetName in sheetNameList)
            {
                JObject jObject = ConvertSheetToJObject(excelPath, sheetName, "StateName");
                excelJObject.Add(sheetName, jObject);
            }
            
            UnityEngine.Debug.Log(excelJObject.ToString());

            return excelJObject;
        }
        
        private static string PathESCReplace(string PathName)
        {
            if (!string.IsNullOrEmpty(PathName)) return PathName.Replace("\\", "/");
            EditorUtility.DisplayDialog("LoadExcel", "ExcelPath为空", "确定");
            throw new Exception("ExcelPath为空");
        }
        
        
        private static JObject ConvertSheetToJObject(string path, string sheetName, string keyName)
        {
            JObject retJObject = new JObject();
            
            path = PathESCReplace(path);
            EditorTools.AnalyzeExcelSheetsToJObject(path, (sheetName_, items) =>
            {
                if (sheetName == sheetName_)
                {
                    UnityEngine.Debug.Log(items.ToString());
                    
                    retJObject.Add(items[keyName].ToString(), items);
                }
            });

            return retJObject;
        }
    }
}