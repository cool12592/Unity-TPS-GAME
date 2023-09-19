using UnityEngine;
using System.Data;
using System;
using System.IO;
using ExcelDataReader;
using System.Collections.Generic;


public static class ExcelReader
{
    public static void ReaderTable(out Dictionary<string, string> tableDic, string filePath, int level)
    {
#if UNITY_EDITOR
        ReaderTableAsExcel(out tableDic, filePath, level);
       
#else
        ReaderTableAsCSV(out tableDic, filePath, level);

#endif
    }

    public static void ReaderTableAsCSV(out Dictionary<string, string> tableDic, string filePath, int level)
    {
        tableDic = new Dictionary<string, string>();
        filePath = "DataFile/Excel/" + filePath;

        List<Dictionary<string, object>> data = CSVReader.Read(filePath);
        level -= 1;

        foreach (var d in data[level])
        {
            tableDic[d.Key] = d.Value.ToString();
        }

      

        

    }

    public static void ReaderTableAsExcel(out Dictionary<string, string> tableDic, string filePath, int level)
    {
        // filePath = "Assets/Excel/Stat.xlsx";

        tableDic = new Dictionary<string, string>();
        filePath = Application.dataPath + "/Resources/DataFile/Excel/" + filePath+ ".xlsx";//Application.streamingAssetsPath + "/"+ filePath;
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
           
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var table = reader.AsDataSet().Tables[0];

                var columnNameRow = table.Rows[0];
                var row = table.Rows[level];

                int col = 0;
                foreach(var colName in columnNameRow.ItemArray)
                {
                    var data = row[col];
                    tableDic[colName.ToString()] = data.ToString();
                    col++;
                }

          
            }
        }

    }
}
