using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HSSF.Util;
using System.Reflection;

namespace TestProject
{
    public class OperateExcel
    {
        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="list"></param>
        /// <param name="file"></param>
        /// <param name="block"></param>
        /// <param name="other">其他参数自用</param>
        /// <param name="columns">表头</param>
        public static void TableToExcel<T>(List<T> list, string file, int block, string[] other = null, string[] columns = null)
        {
            var dt = ListToDataTable(list, block);
            var fileExt = Path.GetExtension(file).ToLower();
            IWorkbook workbook = fileExt switch
            {
                ".xlsx" => new XSSFWorkbook(),
                ".xls" => new HSSFWorkbook(),
                _ => null
            };
            if (workbook == null) { return; }
            var sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            //样式
            var contentStyle = GetCellStyle(workbook, 14f, HorizontalAlignment.Center, VerticalAlignment.Center);

            var headerStyle = GetCellStyle(workbook, 22f, HorizontalAlignment.Center, VerticalAlignment.Center);

            var standerStyle = GetCellStyle(workbook, 14f, HorizontalAlignment.Center, VerticalAlignment.Center,
                new HSSFColor.Red());

            //第一行
            //合并，先合并再写值
            var company = sheet.CreateRow(0).CreateCell(0);
            MergeCell(sheet, 0, 0, 0, block);
            company.SetCellValue(other[0]);
            company.CellStyle = headerStyle;

            //第二行
            var timeCon = sheet.CreateRow(1).CreateCell(0);
            MergeCell(sheet, 1, 1, 0, block);
            timeCon.SetCellValue(other[1]);
            timeCon.CellStyle = headerStyle;

            //表头  
            var row = sheet.CreateRow(2);
            for (var i = 0; i < columns.Length; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(columns[i]);
                cell.CellStyle = headerStyle;
            }

            //数据  
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row1 = sheet.CreateRow(i + 3);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                    // ReSharper disable once PossibleNullReferenceException
                    // 超标写红
                    cell.CellStyle = dt.Rows[i][j].ToString().Equals("超标") ? standerStyle : contentStyle;
                }
            }

            //宽度自适应
            AutoColumnWidth(sheet, block);

            //转为字节数组  
            var stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(buf, 0, buf.Length);
            fs.Flush();
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="firstCol"></param>
        /// <param name="lastCol"></param>
        private static void MergeCell(ISheet sheet, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            var merge = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            sheet.AddMergedRegion(merge);
        }

        /// <summary>
        /// 宽度自适应
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cols"></param>
        private static void AutoColumnWidth(ISheet sheet, int cols)
        {
            for (var col = 0; col <= cols; col++)
            {
                sheet.AutoSizeColumn(col);//自适应宽度，但是其实还是比实际文本要宽
                var columnWidth = sheet.GetColumnWidth(col) / 256;//获取当前列宽度
                for (var rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    var cell = row?.GetCell(col);
                    var contextLength = Encoding.UTF8.GetBytes(cell?.ToString() ?? string.Empty).Length;//获取当前单元格的内容宽度
                    columnWidth = columnWidth < contextLength ? contextLength : columnWidth;

                }
                sheet.SetColumnWidth(col, columnWidth * 220);
            }
        }

        /// <summary>
        /// 返回单元格样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="fontSize"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static ICellStyle GetCellStyle(IWorkbook workbook, double fontSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, HSSFColor color = null)
        {
            var style = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.FontHeightInPoints = fontSize;
            if (color != null) font.Color = color.Indexed;
            style.VerticalAlignment = verticalAlignment;
            style.Alignment = horizontalAlignment;
            style.SetFont(font);
            return style;
        }

        private static DataTable ListToDataTable<T>(List<T> list, int block)
        {
            var dataTable = new DataTable();
            var num1 = 0;
            ++block;
            foreach (var property in typeof(T).GetProperties())
            {
                if (num1 > block) continue;
                dataTable.Columns.Add(new DataColumn(property.Name, GetNullableType(property.PropertyType)));
                ++num1;
            }
            var num2 = 0;
            foreach (var obj in list)
            {
                var row = dataTable.NewRow();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (num2 > block) continue;
                    if (!IsNullableType(property.PropertyType))
                        row[property.Name] = property.GetValue(obj, null);
                    else
                        row[property.Name] = property.GetValue(obj, null) ?? DBNull.Value;
                    ++num2;
                }
                num2 = 0;
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static Type GetNullableType(Type t)
        {
            var type = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(t);
            return type;
        }

        private static bool IsNullableType(Type type)
        {
            if (type == typeof(string) || type.IsArray)
                return true;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        /// <summary>
        /// 将Excel导入DataTable
        /// </summary>
        /// <param name="filepath">导入的文件路径（包括文件名）</param>
        /// <param name="sheetname">工作表名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>DataTable</returns>
        public static DataTable ExcelToDataTable(string filepath, string sheetname, bool isFirstRowColumn)
        {
            ISheet sheet = null;//工作表
            DataTable data = new DataTable();
            FileStream fs = null;
            IWorkbook workbook = null;

            var startrow = 0;
            using (fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    if (filepath.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (filepath.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                    if (sheetname != null)
                    {
                        sheet = workbook.GetSheet(sheetname);
                        if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstrow = sheet.GetRow(0);
                        int cellCount = firstrow.LastCellNum; //行最后一个cell的编号 即总的列数
                        if (isFirstRowColumn)
                        {
                            for (int i = firstrow.FirstCellNum; i < cellCount; i++)
                            {
                                ICell cell = firstrow.GetCell(i);
                                if (cell != null)
                                {
                                    string cellvalue = cell.StringCellValue;
                                    if (cellvalue != null)
                                    {
                                        DataColumn column = new DataColumn(cellvalue);
                                        data.Columns.Add(column);
                                    }
                                }
                            }
                            startrow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startrow = sheet.FirstRowNum;
                        }
                        //读数据行
                        int rowcount = sheet.LastRowNum;
                        for (int i = startrow; i <= rowcount; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue; //没有数据的行默认是null
                            }
                            DataRow datarow = data.NewRow();//具有相同架构的行
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    var result = row.GetCell(j).ToString();
                                    if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result) || result.Length <= 0)
                                        continue;
                                    datarow[j] = result;
                                }
                            }
                            data.Rows.Add(datarow);
                        }
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return null;
                }
                finally { fs.Close(); fs.Dispose(); }
            }
        }

        /// <summary>
        /// DataTable转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToDataList<T>(DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("字段[" + info.Name + "]转换出错," + ex.Message);
                        }
                    }
                }
                if(AnyPropertyIsNull(s))
                    list.Add(s);
            }
            return list;
        }


        /// <summary>
        /// 泛型类校验是否有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>

        private static bool AnyPropertyIsNull<T>(T t) 
        {
            PropertyInfo[] rs = t.GetType().GetProperties();
            foreach (var item in rs)
            {
                var info = typeof(T).GetProperty(item.Name);
                object value = info.GetValue(t);
                if (value == null || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString()))
                    return false;
                return true;
            }
            return true;
        }
    }
}
