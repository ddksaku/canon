using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

/// <summary>
/// Summary description for MemXlsWriter.
/// </summary>
public class MemXlsWriter: System.IO.StreamWriter
{
    public string CultureToSaveDecimals { get; set; }

	public MemXlsWriter(string filename, bool isA): base(filename, isA)
	{
	}

	/// <summary>
	/// Writes a header of XLS file
	/// </summary>
	public void XlsWriteHeader()
	{
		this.WriteLine("<?xml version=\"1.0\"?>");
		this.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
		this.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
		this.WriteLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
		this.WriteLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
		this.WriteLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
		this.WriteLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
        this.WriteLine(" <Styles>");
        this.WriteLine(" <Style ss:ID=\"s21\"><Font /></Style>");
        this.WriteLine(" <Style ss:ID=\"s23\"><Font /><NumberFormat ss:Format=\"Short Date\"/></Style>");
        this.WriteLine(" <Style ss:ID=\"s22\"><Font ss:Color=\"#FF0000\" /><Alignment ss:Vertical=\"Bottom\" ss:Rotate=\"90\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/></Style>");
        this.WriteLine(" <Style ss:ID=\"s41\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Borders/><Font ss:FontName=\"Verdana\" x:Family=\"Swiss\" ss:Size=\"18\" ss:Color=\"#FF0000\" ss:Bold=\"1\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
        this.WriteLine(" <Style ss:ID=\"s42\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Borders/><Font ss:Color=\"#FF0000\" ss:Bold=\"1\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
        this.WriteLine(" </Styles>");
        this.WriteLine(" <Worksheet ss:Name=\"Data\">");
		this.WriteLine("  <Table>");
	}

    /// <summary>
    /// Writes a header of XLS file
    /// </summary>
    public void XlsWriteHeaderManySheets()
    {
        this.WriteLine("<?xml version=\"1.0\"?>");
        this.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
        this.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
        this.WriteLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
        this.WriteLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
        this.WriteLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
        this.WriteLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
        this.WriteLine(" <Styles>");
        this.WriteLine(" <Style ss:ID=\"s21\"><Font ss:Bold=\"1\"/></Style>");
        this.WriteLine(" <Style ss:ID=\"s23\"><Font ss:Bold=\"1\"/><NumberFormat ss:Format=\"Short Date\"/></Style>");
        this.WriteLine(" <Style ss:ID=\"s22\"><Font ss:Color=\"#FF0000\" ss:Bold=\"1\"/><Alignment ss:Vertical=\"Bottom\" ss:Rotate=\"90\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/></Style>");
        this.WriteLine(" <Style ss:ID=\"s41\"><Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\"/><Borders/><Font ss:FontName=\"Verdana\" x:Family=\"Swiss\" ss:Size=\"18\" ss:Color=\"#FF0000\" ss:Bold=\"1\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
        this.WriteLine(" <Style ss:ID=\"s42\"><Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\"/><Borders/><Font ss:Color=\"#FF0000\" ss:Bold=\"1\"/><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
        this.WriteLine(" <Style ss:ID=\"s45\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Borders/><Font /><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat ss:Format=\"#,##0.00\\ [$Kč]\"/><Protection/></Style>");
        this.WriteLine(" <Style ss:ID=\"s46\"><Alignment ss:Horizontal=\"Right\" ss:Vertical=\"Center\"/><Borders/><Font /><Interior ss:Color=\"#FFFFFF\" ss:Pattern=\"Solid\"/><NumberFormat ss:Format=\"Percent\"/><Protection/></Style>");
        this.WriteLine(" </Styles>");
    }

	/// <summary>
	/// Writes a footer of XLS file
	/// </summary>
	public void XlsWriteFooter()
	{
		this.WriteLine("  </Table>");
		this.WriteLine(" </Worksheet>");
		this.WriteLine("</Workbook>");
	}

    /// <summary>
    /// Writes a footer of XLS file
    /// </summary>
    public void XlsWriteFooterManySheets()
    {
        this.WriteLine("</Workbook>");
    }

    /// <summary>
    /// Writes all DataTable to xls file including header and footer
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public bool XlsWriteDataTable(System.Data.DataTable dt)
    {
        try
        {
            this.XlsWriteHeader();
            this.XlsWriteTableHeader(dt.Columns, 0);
            for (int i = 0; i < dt.Rows.Count; i++)
                this.XlsWriteTableRow(dt.Rows[i]);
            this.XlsWriteFooter();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Can't write xls file", ex);
        }
    }

    /// <summary>
    /// write table on excel page, 5 rows under table is set select
    /// </summary>
    /// <param name="table"></param>
    /// <param name="setting"></param>
    /// <param name="select"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public bool XlsWriteDataTable(System.Data.DataTable table, int setting, string select, string page)
    {
        try
        {
            this.WriteLine(" <Worksheet ss:Name=\"" + page + "\">");
            this.WriteLine("  <Table>");
            this.XlsWriteTableHeader(table.Columns, setting);
            for (int i = 0; i < table.Rows.Count; i++)
                this.XlsWriteTableRow(table.Rows[i]);
            for (int i = 0; i < 5; i++)
            {
                this.WriteLine("   <Row>");
                this.WriteLine("   </Row>");
            }
            this.WriteLine("   <Row>");
            this.WriteLine("    <Cell><Data ss:Type=\"String\">" + StripHtml(select) + "</Data></Cell>");
            this.WriteLine("   </Row>");
            this.WriteLine("  </Table>");
            this.WriteLine(" </Worksheet>");
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Can't write xls file "+ex.Message, ex);
        }
    }
    /// <summary>
    /// Writes all DataTables to xls file including header and footer
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public bool XlsWriteDataTables(string[] sheetNames, int[] settings, params System.Data.DataTable[] list)
    {
        try
        {
            this.XlsWriteHeaderManySheets();
            for (int j = 0; j < list.Length; j++)
            {
                this.WriteLine(" <Worksheet ss:Name=\"" + sheetNames[j] + "\">");
                this.WriteLine("  <Table>");
                this.XlsWriteTableHeader(list[j].Columns, settings[j]);
                for (int i = 0; i < list[j].Rows.Count; i++)
                    this.XlsWriteTableRow(list[j].Rows[i]);
                this.WriteLine("  </Table>");
                this.WriteLine(" </Worksheet>");
            }
            this.XlsWriteFooterManySheets();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Can't write xls file", ex);
        }
    }

	/// <summary>
	/// Writes header row of a worksheet in XLS file
	/// </summary>
	/// <param name="dcc"></param>
	public void XlsWriteTableHeader(System.Data.DataColumnCollection dcc, int settings)
	{
		for (int i=0; i < dcc.Count; i++)
		{
            System.Data.DataColumn dc = dcc[i];
            string width = (this.StripHtml(dc.ColumnName).Length * 7).ToString();
            if (settings == 1) width = "120";
            if (i == 0) width = "180";
			this.WriteLine("<Column ss:Index=\"" + (i+1).ToString() +
				"\" ss:AutoFitWidth=\"0\" ss:Width=\"" + width + "\"/>");
		}
		this.WriteLine("   <Row ss:AutoFitHeight=\"0\" ss:Height=\"90\">");
		for (int i=0; i < dcc.Count; i++)
		{
			System.Data.DataColumn dc = dcc[i];
            string text  = this.StripHtml(dc.ColumnName);
            string style = "s42";
            if (settings == 1) style = "s22";
            if (i == 0)
            {
                style = "s41";
                text = "CANON";
            }
			this.WriteLine("    <Cell ss:StyleID=\"" + style + "\"><Data ss:Type=\"String\">" + 
                text + "</Data></Cell>");
		}
		this.WriteLine("   </Row>");
	}

	/// <summary>
	/// Writes a row into XLS file
	/// </summary>
	/// <param name="dr"></param>
	public void XlsWriteTableRow(System.Data.DataRow dr)
	{
		this.WriteLine("   <Row>");
		for (int i=0; i < dr.ItemArray.Length; i++)
		{
            string textToWrite = StripHtml(dr.ItemArray[i].ToString());
            string type = "String";
            string style = "s21";
            int parseRes;
            decimal parseRes2;
            if (dr.ItemArray[i].GetType().ToString() == "System.DateTime") 
            {
                textToWrite = ((DateTime)dr.ItemArray[i]).ToString("yyyy-MM-ddT00:00:00.000");
                type = "DateTime";
                style = "s23";
            }
            if ((int.TryParse(textToWrite, out parseRes)) && (i != 0))
            {
                type = "Number";
                textToWrite = textToWrite.Trim();
            }
            else if ((decimal.TryParse(textToWrite, 
                                       NumberStyles.Any, 
                                       CultureInfo.CreateSpecificCulture("cs-CZ"), out parseRes2)) && (i != 0))
            {
                type = "Number";
                CultureInfo ci = null;
                if (!string.IsNullOrEmpty(this.CultureToSaveDecimals))
                    ci = CultureInfo.CreateSpecificCulture(CultureToSaveDecimals);
                else
                    ci = CultureInfo.CreateSpecificCulture("cs-CZ");
                
                textToWrite = parseRes2.ToString(ci);
                if (((i>=2)&&(i%2 != 1))||(i==1))
                    style = "s45";
                else if ((i >= 2) && (i % 2 == 1))
                {
                    textToWrite = (parseRes2/100).ToString(ci);
                    style = "s46";
                }
            }

            if (i == 0) style = "s42";
            this.WriteLine("    <Cell ss:StyleID=\"" + style + "\"><Data ss:Type=\"" + type + "\">" + textToWrite + "</Data></Cell>");
		}
		this.WriteLine("   </Row>");
	}

	/// <summary>
	/// Writes a table's content to xls name
	/// </summary>
	public void XlsWriteHtmlTable(Table tbl)
	{
		for (int j=0; j < tbl.Rows.Count; j++)
		{
			this.WriteLine("   <Row>");
			TableRow tr = tbl.Rows[j];
			for (int i=0; i < tr.Cells.Count; i++)
			{
				int    colspan = tr.Cells[i].ColumnSpan;
				int    rowspan = tr.Cells[i].RowSpan;
				string valueToWrite = StripHtml(tr.Cells[i].Text);
				string colspanToWrite = (colspan == 0)?"":" ss:MergeAcross=\"" + (colspan-1).ToString() + "\"";
				string rowspanToWrite = ((rowspan == 0)||(i > 1))?"":" ss:MergeDown=\"" + (rowspan-1).ToString() + "\"";
				string startIndex = "";
				try
				{
					if ((tbl.Rows[j-1].Cells[0].RowSpan > 0)&&(i == 0))
						startIndex = " ss:Index=\"2\"";
					if ((tbl.Rows[j-1].Cells[0].RowSpan > 0)&&(tbl.Rows[j-1].Cells[1].RowSpan > 0)&&(i == 0))
						startIndex = " ss:Index=\"3\"";
				}
				catch (Exception)
				{
					startIndex = "";
				}
				this.WriteLine("    <Cell " + colspanToWrite + rowspanToWrite + startIndex + "><Data ss:Type=\"String\">" + valueToWrite + "</Data></Cell>");
			}
			this.WriteLine("   </Row>");
		}
	}

	/// <summary>
	/// Writes a custom array
	/// </summary>
	public void XlsWriteCustomArray(string[] values, bool isBold)
	{
		this.WriteLine("   <Row>");
		for (int i=0; i < values.Length; i++)
		{
			string style = (isBold)?" ss:StyleID=\"s21\"":"";
			this.WriteLine("    <Cell" + style + "><Data ss:Type=\"String\">" + values[i] + "</Data></Cell>");
		}
		this.WriteLine("   </Row>");
	}

	protected string StripHtml(string html)
	{
		if (html == null || html == string.Empty)
			return string.Empty;
        string converted = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", string.Empty);
        return converted.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&nbsp;", "");
	}

}
