using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadBalanceMonth
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string textfile = string.Empty;
            string PathNamefile = "C:/Temp/Colombia_2019 12.xls";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            Worksheet workSheet = null;
            //Abre el libro de excel que tiene la plantilla
            workBook = app.Workbooks.Open(PathNamefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //Referencia la primera hoja de la plantilla
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets["PC"];
            bool nextread = true;
            int row = 4;
            int countnull = 1;
            //List<modelRpt1> list = new List<modelRpt1>();
            //bool doublecc = true;
            while (nextread)
            {
                var valu = (workSheet.Cells[row, 1] as Range).Value;
                if (valu == null)
                {
                    if (countnull == 7)
                    {
                        break;
                    }
                    row++; countnull++;
                    continue;
                }
                countnull = 1;
                textfile += row.ToString();
                dynamic di = (workSheet.Cells[row, 1] as Range).Interior.Color;
                string typerow = "";
                if (di.ToString().Contains("16777215"))
                {
                    typerow = "TOTAL";
                    textfile += ";TOTAL";
                }
                else
                {
                    typerow = "ITEM";
                    textfile += ";ITEM";
                }
                textfile += ";" + valu.ToString();

                if (typerow == "ITEM")
                {
                    Comment comm = (workSheet.Cells[row, 13] as Range).Comment;
                    if (comm != null)
                    {
                        string com = comm.Text().Replace(comm.Author, "").Replace(" ", "").Replace(":", "").Replace("+", "").Replace("-", "")
                            .Replace("DB", "");
                        com = com.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\t", "").Trim();
                        textfile += ";" + com;

                        //modelRpt1 rpt = new modelRpt1();
                        //rpt.name = valu.ToString();
                        //rpt.order = row; rpt.acconts = com;
                        //list.Add(rpt);
                    }
                    else
                    {

                        textfile += ";";
                    }
                }
                else
                {
                    dynamic comm = (workSheet.Cells[row, 13] as Range).Formula;
                    if (comm != null && comm.ToString().Contains("=") && row != 305)
                    {
                        string formula = comm.ToString();
                        formula = formula.Replace("=", "").Replace("SUM", "").Replace("M", "F").Replace("(", "").Replace(")", "");

                        textfile += ";" + formula;
                    }
                    else
                    {
                        textfile += ";";
                    }
                }
                //centros de operacion
                var valuco = (workSheet.Cells[row, 14] as Range).Value;
                if (valuco != null)
                {
                    textfile += ";" + valuco.ToString() ;
                }
                else
                {
                    textfile += ";";
                }
                //tercero
                var valuter = (workSheet.Cells[row, 15] as Range).Value;
                if (valuter != null)
                {
                    textfile += ";" + valuter.ToString() ;
                }
                else
                {
                    textfile += ";";
                }

                //if (valu.ToString().Trim() == "UTILIDAD (PERDIDA) ACUMULADAS")
                //{
                //    doublecc = false;
                //}
                textfile += Environment.NewLine;

                row++;
            }
         //   workBook.Save();
            workBook.Close(false, null, null);
            File.WriteAllText("resul.txt", textfile);
        }
    }
}
