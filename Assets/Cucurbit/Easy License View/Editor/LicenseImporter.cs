using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class LicenseImporter : AssetPostprocessor {
    private static readonly string filePath = "Assets/Cucurbit/Easy License View/ExcelData/license.xlsx";
    private static readonly string exportPath = "Assets/Cucurbit/Easy License View/ExcelData/license.asset";
    private static readonly string[] sheetNames = { "licenses", };
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) {
            if (!filePath.Equals (asset))
                continue;
                
            EntityLicense data = (EntityLicense)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EntityLicense));
            if (data == null) {
                data = ScriptableObject.CreateInstance<EntityLicense> ();
                AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
                data.hideFlags = HideFlags.NotEditable;
            }
            
            data.sheets.Clear ();
            using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                IWorkbook book = null;
                if (Path.GetExtension (filePath) == ".xls") {
                    book = new HSSFWorkbook(stream);
                } else {
                    book = new XSSFWorkbook(stream);
                }
                
                foreach(string sheetName in sheetNames) {
                    ISheet sheet = book.GetSheet(sheetName);
                    if( sheet == null ) {
                        Debug.LogError("[QuestData] sheet not found:" + sheetName);
                        continue;
                    }

                    EntityLicense.Sheet s = new EntityLicense.Sheet ();
                    s.name = sheetName;
                
                    for (int i=1; i<= sheet.LastRowNum; i++) {
                        IRow row = sheet.GetRow (i);
                        ICell cell = null;
                        
                        EntityLicense.Param p = new EntityLicense.Param ();
                        
                        bool isComment = false;
                        ICell tmp = row.GetCell(0);
                        if (tmp != null) {
                            if (tmp.CellType == CellType.String) {
                                if (tmp.StringCellValue == "End" || tmp.StringCellValue == "end") {
                                    //強制終了
                                    break;
                                }
                                else if (tmp.StringCellValue[0] == '#' || (tmp.StringCellValue[0] == '/' && tmp.StringCellValue[1] == '/') || tmp.StringCellValue.Length == 0) {
                                    //コメント行とみなす
                                    isComment = true;
                                }
                            }
                            else if (tmp.CellType == CellType.Blank) {
                                isComment = true;
                            }
                        }
                        else if (tmp == null) {
                            isComment = true;
                        }

                        if (!isComment) {
                            cell = row.GetCell(0); p.Title = (cell == null ? "" : cell.StringCellValue);
                            cell = row.GetCell(1); p.Provision = (cell == null ? "" : cell.StringCellValue);
                            s.list.Add (p);
                        }
                    }
                    data.sheets.Add(s);
                }
            }

            ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
            EditorUtility.SetDirty (obj);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
