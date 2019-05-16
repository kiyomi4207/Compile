using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System.Data.OleDb;
using System.Data;

namespace DBtoTableCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建PDF实例
            PdfDocument doc = new PdfDocument();
            //设置页边距
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(3.17f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;
            //创建一个空白页
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);
            //声明纵坐标
            float y = 10;

            //设置表的标题
            PdfBrush brush1 = PdfBrushes.Black;
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("宋体", 16f,FontStyle.Bold ), true);
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);
            page.Canvas.DrawString("用户表", font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);
            y = y + font1.MeasureString("用户表", format1).Height;
            y = y + 5;
            //创建数据表参数
            PdfTable table = new PdfTable();
            table.Style.CellPadding = 2;
            table.Style.BorderPen = new PdfPen(brush1, 0.75f);
            table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.SkyBlue;
            table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("宋体", 10f),true);
            table.Style.AlternateStyle = new PdfCellStyle();
            table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.LightYellow;
            table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("宋体", 10f),true);
            table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
            table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("宋体", 11f, FontStyle.Bold),true);
            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            table.Style.ShowHeader = true;
            //使用OleDbConnection（）方法获取数据表信息
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Administrator\Desktop\users.mdb";
                OleDbCommand command = new OleDbCommand();
                command.CommandText
                    = " select Name,Phone,Email from User_Info ";
                command.Connection = conn;
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    table.DataSourceType = PdfTableDataSourceType.TableDirect;
                    table.DataSource = dataTable;
                }
            }
            //根据数据表中已有行数定义表格列的宽度
            float width
                = page.Canvas.ClientSize.Width
                    - (table.Columns.Count + 1) * table.Style.BorderPen.Width;
            table.Columns[0].Width = width * 0.24f * width;
            table.Columns[0].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[1].Width = width * 0.21f * width;
            table.Columns[1].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[2].Width = width * 0.24f * width;
            table.Columns[2].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            //设置行高
            PdfLayoutResult result = table.Draw(page, new PointF(0, y));
            y = y + result.Bounds.Height + 5;
            //从表格中获取总行数
            PdfBrush brush2 = PdfBrushes.Gray;
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("宋体", 9f),true);
            page.Canvas.DrawString(String.Format("* {0} users in the list.", table.Rows.Count),
                font2, brush2, 5, y);

            //保存文件
            doc.SaveToFile("DataSource.pdf");
            doc.Close();

            //启动文件
            PDFDocumentViewer("DataSource.pdf");
        }

        private static void PDFDocumentViewer(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch { }
        }
    }
}
