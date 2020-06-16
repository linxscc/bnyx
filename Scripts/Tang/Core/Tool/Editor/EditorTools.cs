using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

namespace Tang.Editor
{
    public static class EditorTools
    {
        public static object GetDataTableData(DataTable dt, int row, int col)
        {
            if (dt.Rows.Count > row && dt.Columns.Count > col)
            {
                DataRow dr = dt.Rows[row];
                DataColumn dc = dt.Columns[col];

                return dr[dc];
            }

            Debug.LogError("找不到数据");
            return null;
        }

        public static void AnalyzeExcel(string excelPath, Action<List<KeyValuePair<string, object>>> callback)
        {
            try
            {
                FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
                {
                    DataSet dataset = excelReader.AsDataSet();

                    DataTable dt = dataset.Tables[0];
                    {
                        Dictionary<int, string> colToTitleDic = new Dictionary<int, string>();

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            DataRow dr = dt.Rows[row];

                            if (row == 1)
                            {
                                for (int col = 0; col < dt.Columns.Count; col++)
                                {
                                    DataColumn dc = dt.Columns[col];
                                    object value = dr[dc];
                                    colToTitleDic.Add(col, value.ToString());
                                }
                            }
                            else if (row >= 3)
                            {
                                DropItemsDataAsset.DropItem dropItem = new DropItemsDataAsset.DropItem();

                                List<KeyValuePair<string, object>> pairs = new List<KeyValuePair<string, object>>();


                                foreach (var iterator in colToTitleDic)
                                {
                                    object value = GetDataTableData(dt, row, iterator.Key);
                                    pairs.Add(new KeyValuePair<string, object>(iterator.Value, value));
                                }

                                callback(pairs);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("出错(一般是excel没有关闭)", e.ToString(), "确认");
            }
        }


        public static void AnalyzeExcelToDic(string excelPath, Action<Dictionary<string, object>> callback)
        {
            try
            {
                FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
                {
                    DataSet dataset = excelReader.AsDataSet();

                    DataTable dt = dataset.Tables[0];
                    {
                        Dictionary<int, string> colToTitleDic = new Dictionary<int, string>();

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            DataRow dr = dt.Rows[row];

                            if (row == 1)
                            {
                                for (int col = 0; col < dt.Columns.Count; col++)
                                {
                                    DataColumn dc = dt.Columns[col];
                                    object value = dr[dc];
                                    colToTitleDic.Add(col, value.ToString());
                                }
                            }
                            else if (row >= 3)
                            {
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                foreach (var iterator in colToTitleDic)
                                {
                                    object value = GetDataTableData(dt, row, iterator.Key);
                                    if (iterator.Value.ToString().IsNullOrEmpty() == false)
                                        dic.Add(iterator.Value, value);
                                }

                                callback(dic);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("出错(一般是excel没有关闭)", e.ToString(), "确认");
            }
        }


        public static void AnalyzeExcelSheetsToDic(string excelPath,
            Action<string, Dictionary<string, object>> callback)
        {
            try
            {
                FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
                {
                    DataSet dataset = excelReader.AsDataSet();

                    for (int i = 0; i < dataset.Tables.Count; i++)
                    {
                        DataTable dt = dataset.Tables[i];
                        {
                            Dictionary<int, string> colToTitleDic = new Dictionary<int, string>();

                            for (int row = 0; row < dt.Rows.Count; row++)
                            {
                                DataRow dr = dt.Rows[row];

                                if (row == 1)
                                {
                                    for (int col = 0; col < dt.Columns.Count; col++)
                                    {
                                        DataColumn dc = dt.Columns[col];
                                        object value = dr[dc];
                                        colToTitleDic.Add(col, value.ToString());
                                    }
                                }
                                else if (row >= 3)
                                {
                                    Dictionary<string, object> dic = new Dictionary<string, object>();
                                    foreach (var iterator in colToTitleDic)
                                    {
                                        object value = GetDataTableData(dt, row, iterator.Key);
                                        if (iterator.Value.ToString().IsNullOrEmpty() == false)
                                            dic.Add(iterator.Value, value);
                                    }

                                    callback(dataset.Tables[i].TableName, dic);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("出错(一般是excel没有关闭)", e.ToString(), "确认");
            }
        }

        public static void AnalyzeExcelSheetsToJObject(string excelPath, Action<string, JObject> callback)
        {
            FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
            {
                DataSet dataset = excelReader.AsDataSet();

                for (int i = 0; i < dataset.Tables.Count; i++)
                {
                    DataTable dt = dataset.Tables[i];
                    {
                        Dictionary<int, string> colToTitleDic = new Dictionary<int, string>();
                        Dictionary<int, string> colToTypeDic = new Dictionary<int, string>();

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            DataRow dr = dt.Rows[row];

                            if (row == 1)
                            {
                                for (int col = 0; col < dt.Columns.Count; col++)
                                {
                                    DataColumn dc = dt.Columns[col];
                                    object value = dr[dc];
                                    colToTitleDic.Add(col, value.ToString());
                                }
                            }
                            else if (row == 2)
                            {
                                for (int col = 0; col < dt.Columns.Count; col++)
                                {
                                    DataColumn dc = dt.Columns[col];
                                    object value = dr[dc];
                                    colToTypeDic.Add(col, value.ToString());
                                }
                            }
                            else if (row >= 3)
                            {
                                JObject jobj = new JObject();
                                foreach (var iterator in colToTitleDic)
                                {
                                    object value = GetDataTableData(dt, row, iterator.Key);
                                    if (iterator.Value.ToString().IsNullOrEmpty() == false)
                                    {
                                        try
                                        {
                                            switch (colToTypeDic[iterator.Key])
                                            {
                                                case "string":
                                                    jobj.Add(iterator.Value.ToString(), Convert.ToString(value));
                                                    break;
                                                case "int":
                                                    jobj.Add(iterator.Value.ToString(), Convert.ToInt32(value));
                                                    break;
                                                case "float":
                                                    jobj.Add(iterator.Value.ToString(), Convert.ToSingle(value));
                                                    break;
                                                case "vector3":
                                                    JObject MoveValue = new JObject();
                                                    JArray Value = JArray.Parse(Convert.ToString(value));
                                                    MoveValue.Add("x", Value[0]);
                                                    MoveValue.Add("y", Value[1]);
                                                    MoveValue.Add("z", Value[2]);

                                                    jobj.Add(iterator.Value.ToString(), MoveValue);
                                                    break;
                                                default:
                                                    Debug.LogError("找不到类型:" + iterator.Key);
                                                    break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }
                                }

                                callback(dataset.Tables[i].TableName, jobj);
                            }
                        }
                    }
                }
            }
        }
    }
}