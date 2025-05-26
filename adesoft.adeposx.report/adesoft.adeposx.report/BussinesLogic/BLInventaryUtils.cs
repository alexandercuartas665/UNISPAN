using adesoft.adeposx.report.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.BussinesLogic
{
    public static class BLInventaryUtils
    {

        public static List<DTOInventary> ReadEquivalence85(string FilBase64)
        {
            List<DTOInventary> listdto = new List<DTOInventary>();
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                string[] base64cad = FilBase64.Split(',');
                string ext = base64cad[1].Contains("vnd.openxmlformats-officedocument.spreadsheetml.sheet") ? ".xlsx" : ".xls";
                byte[] arraydoc = Convert.FromBase64String(base64cad[1]);
                // string directorybase = Directory.GetCurrentDirectory(); no sirve para .net framework
                string directorybase = AppDomain.CurrentDomain.BaseDirectory;
                // evento.WriteEntry("Directorio Base: " + directorybase + " - Directorio 2 " + AppDomain.CurrentDomain.BaseDirectory, EventLogEntryType.Information);
                if (!Directory.Exists(directorybase + "FilesApp\\Temp\\"))
                {
                    Directory.CreateDirectory(directorybase + "FilesApp\\Temp\\");
                }
                string fullpath = directorybase + "FilesApp\\Temp\\" + Guid.NewGuid() + ext;
                FileStream stre = File.Create(fullpath);
                stre.Write(arraydoc, 0, arraydoc.Length);
                stre.Close();

                OleDbConnection Connection = new OleDbConnection();
                OleDbCommand Command = new OleDbCommand();
                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                DataSet DtsExcel = new DataSet();
                Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";
                Connection.Open();

                var sheets = Connection.GetSchema("TABLES").AsEnumerable().Select(x => x.Field<string>("TABLE_NAME")).ToList();

                Command.CommandText = "SELECT * FROM [" + sheets[0].Replace("'", "") + "]";  //A3:I29
                Command.Connection = Connection;
                DataAdapter.SelectCommand = Command;
                DataAdapter.Fill(DtsExcel);
                Connection.Close();

                foreach (DataRow cols in DtsExcel.Tables[0].Rows)
                {
                    if (cols[0].ToString().Trim() != "QUANTIFY"
                        && cols[1] != null && cols[1].ToString().Trim() != string.Empty)
                    {

                        DTOInventary dt = listdto.Where(x => x.Barcode == cols[0].ToString().Trim()).FirstOrDefault();
                        if (dt == null)
                        {
                            dt = new DTOInventary();
                            dt.Barcode = cols[0].ToString().Trim();
                            dt.Syncode = cols[1].ToString().Trim();
                            if (cols.ItemArray.Length > 2)
                            {
                                if (cols[2] != null && !string.IsNullOrEmpty(cols[2].ToString()))
                                {
                                    dt.PriceUnd = decimal.Parse(cols[2].ToString().Trim());
                                }
                            }
                            listdto.Add(dt);
                        }
                        else
                        {

                            dt.Barcode = cols[0].ToString().Trim();
                            dt.Syncode = cols[1].ToString().Trim();
                            dt.PriceUnd = decimal.Parse(cols[2].ToString().Trim());
                        }
                    }

                    //if (cols[0].ToString().Trim() != "CODIGO" && cols[0].ToString().Trim() != "C?digo".Trim()
                    //    && cols[1] != null && cols[1].ToString().Trim() != string.Empty)
                    //{
                    //    string barcoAux = cols[0].ToString().Trim();
                    //    DTOInventary x = Listcxi.Where(t => t.Barcode == barcoAux).FirstOrDefault();
                    //    if (x == null)
                    //    {
                    //        x = new DTOInventary();
                    //        x.Barcode = barcoAux;
                    //        x.ItemName = cols[1].ToString().Trim();
                    //        x.ItemName = x.ItemName.Replace("\"", "'");
                    //        x.CantInv = decimal.Parse(cols[3].ToString().Trim());
                    //        if (x.CantInv < 0)
                    //        {
                    //            x.CantInv = 0;
                    //        }
                    //        Listcxi.Add(x);
                    //    }
                    //    else
                    //    {
                    //        if (decimal.Parse(cols[3].ToString().Trim()) > 0)
                    //            x.CantInv += decimal.Parse(cols[3].ToString().Trim());
                    //        else
                    //        {

                    //        }
                    //    }
                    //}
                }


                File.Delete(fullpath);
                return listdto;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
                return listdto;
            }
        }

        /// <summary>
        /// Funcionalidad hecha para el cliente 
        /// </summary>
        /// <param name="PathNamefile"></param>
        /// <param name="bodega"></param>
        /// <returns></returns>
        public static List<DTOInventary> ReadInventaryOfDocumentOleDB(string PathNamefile, string bodega)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            try
            {


                List<DTOInventary> Listcxi = new List<DTOInventary>();
                string fulllPath = PathNamefile + "/" + bodega;
                string[] files = Directory.GetFiles(fulllPath);
                foreach (string file in files)
                {


                    if (file.ToLower().Contains(".xlsx") || file.ToLower().Contains(".xls"))
                    {
                        OleDbConnection Connection = new OleDbConnection();
                        OleDbCommand Command = new OleDbCommand();
                        OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                        DataSet DtsExcel = new DataSet();
                        Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";
                        Connection.Open();
                        Command.CommandText = "SELECT * FROM [Sheet$]";  //A3:I29
                        Command.Connection = Connection;
                        DataAdapter.SelectCommand = Command;
                        DataAdapter.Fill(DtsExcel);
                        Connection.Close();
                        //prueba
                        foreach (DataRow cols in DtsExcel.Tables[0].Rows)
                        {
                            if (cols[0].ToString().Trim() != "CODIGO" && cols[0].ToString().Trim() != "C?digo".Trim()
                                && cols[1] != null && cols[1].ToString().Trim() != string.Empty)
                            {
                                string barcoAux = cols[0].ToString().Trim();
                                DTOInventary x = Listcxi.Where(t => t.Barcode == barcoAux).FirstOrDefault();
                                if (x == null)
                                {
                                    x = new DTOInventary();
                                    x.Barcode = barcoAux;
                                    x.ItemName = cols[1].ToString().Trim();
                                    x.ItemName = x.ItemName.Replace("\"", "'");
                                    x.CantInv = decimal.Parse(cols[3].ToString().Trim());
                                    if (x.CantInv < 0)
                                    {
                                        x.CantInv = 0;
                                    }
                                    Listcxi.Add(x);
                                }
                                else
                                {
                                    if (decimal.Parse(cols[3].ToString().Trim()) > 0)
                                        x.CantInv += decimal.Parse(cols[3].ToString().Trim());
                                    else
                                    {

                                    }
                                }
                            }
                        }

                        File.Delete(file);
                    }
                }
                return Listcxi;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
                return null;
            }
        }
        //antigua opcion de parametro de contabilidad PC
        public static List<DTOReportBalanceMonth> ReadConfigReportDynamicOpc2(string PathNamefile)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                List<DTOReportBalanceMonth> Listcxi = new List<DTOReportBalanceMonth>();
                string fulllPath = PathNamefile;
                if (PathNamefile.ToLower().Contains(".xlsx") || PathNamefile.ToLower().Contains(".xls"))
                {
                    OleDbConnection Connection = new OleDbConnection();
                    OleDbCommand Command = new OleDbCommand();
                    OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                    DataSet DtsExcel = new DataSet();
                    Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathNamefile + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";
                    Connection.Open();
                    Command.CommandText = "SELECT * FROM [Hoja1$]";  //A3:I29
                    Command.Connection = Connection;
                    DataAdapter.SelectCommand = Command;
                    DataAdapter.Fill(DtsExcel);
                    Connection.Close();
                    int icont = 1;
                    //prueba
                    foreach (DataRow cols in DtsExcel.Tables[0].Rows)
                    {
                        if (cols[0].ToString().Trim() != "ID" && cols[0].ToString().Trim() != string.Empty)
                        {

                            DTOReportBalanceMonth rpt = new DTOReportBalanceMonth();
                            rpt.ReportDynamicId = 1;
                            rpt.PositionNum = icont++;
                            rpt.OrderNum = long.Parse(cols[0].ToString().Trim());
                            rpt.Type = cols[1].ToString().Trim();
                            rpt.Description = cols[2].ToString().Trim();
                            if (cols[3] != null && !string.IsNullOrEmpty(cols[3].ToString()))
                                rpt.Accounts = cols[3].ToString().Trim();
                            if (cols[4] != null && !string.IsNullOrEmpty(cols[4].ToString()))
                                rpt.Centros = cols[4].ToString().Trim();
                            if (cols[5] != null && !string.IsNullOrEmpty(cols[5].ToString()))
                                rpt.NitTercero = cols[5].ToString().Trim();
                            Listcxi.Add(rpt);
                        }
                    }
                }

                return Listcxi;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
                return null;
            }
        }


        public static List<DTOReportBalanceMonth> ReadConfigReportDynamic(string PathNamefile)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                List<DTOReportBalanceMonth> Listcxi = new List<DTOReportBalanceMonth>();
                if (PathNamefile.ToLower().Contains(".xlsx") || PathNamefile.ToLower().Contains(".xls"))
                {
                    string textfile = string.Empty;
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
                    int icont = 1;

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
                        DTOReportBalanceMonth rpt = new DTOReportBalanceMonth();
                        rpt.ReportDynamicId = 1;
                        rpt.PositionNum = icont++;

                        countnull = 1;
                        textfile += row.ToString();
                        rpt.OrderNum = row;
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
                        rpt.Type = typerow;
                        rpt.Description = valu.ToString();
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
                                rpt.Accounts = com;
                                //modelRpt1 rpt = new modelRpt1();
                                //rpt.name = valu.ToString();
                                //rpt.order = row; rpt.acconts = com;
                                //list.Add(rpt);
                            }
                            else
                            {
                                rpt.Accounts = "";
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
                                rpt.Accounts = formula;
                                textfile += ";" + formula.Trim();
                            }
                            else
                            {
                                rpt.Accounts = "";
                                textfile += ";";
                            }
                        }
                        //centros de operacion
                        var valuco = (workSheet.Cells[row, 14] as Range).Value;
                        if (valuco != null)
                        {
                            rpt.Centros = valuco.ToString().Trim();
                            textfile += ";" + valuco.ToString();
                        }
                        else
                        {
                            rpt.Centros = "";
                            textfile += ";";
                        }
                        //tercero
                        var valuter = (workSheet.Cells[row, 15] as Range).Value;
                        if (valuter != null)
                        {
                            rpt.NitTercero = valuter.ToString().Trim();
                            textfile += ";" + valuter.ToString();
                        }
                        else
                        {
                            rpt.NitTercero = "";
                            textfile += ";";
                        }
                        Listcxi.Add(rpt);
                        //if (valu.ToString().Trim() == "UTILIDAD (PERDIDA) ACUMULADAS")
                        //{
                        //    doublecc = false;
                        //}
                        textfile += Environment.NewLine;

                        row++;
                    }
                    //prueba
                    //foreach (DataRow cols in DtsExcel.Tables[0].Rows)
                    //{
                    //    if (cols[0].ToString().Trim() != "ID" && cols[0].ToString().Trim() != string.Empty)
                    //    {

                    //        DTOReportBalanceMonth rpt = new DTOReportBalanceMonth();
                    //        rpt.ReportDynamicId = 1;
                    //        rpt.PositionNum = icont++;
                    //        rpt.OrderNum = long.Parse(cols[0].ToString().Trim());
                    //        rpt.Type = cols[1].ToString().Trim();
                    //        rpt.Description = cols[2].ToString().Trim();
                    //        if (cols[3] != null && !string.IsNullOrEmpty(cols[3].ToString()))
                    //            rpt.Accounts = cols[3].ToString().Trim();
                    //        if (cols[4] != null && !string.IsNullOrEmpty(cols[4].ToString()))
                    //            rpt.Centros = cols[4].ToString().Trim();
                    //        if (cols[5] != null && !string.IsNullOrEmpty(cols[5].ToString()))
                    //            rpt.NitTercero = cols[5].ToString().Trim();
                    //        Listcxi.Add(rpt);
                    //    }
                    //}
                }

                return Listcxi;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
                return null;
            }
        }
        public static List<DTOTercero> ReadPersonOfDocumentOleDB(string PathNamefile)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                List<DTOTercero> Listcxi = new List<DTOTercero>();
                string fulllPath = PathNamefile + "\\Personal";
                string[] files = Directory.GetFiles(fulllPath);
                foreach (string file in files)
                {
                    if (file.ToLower().Contains(".xlsx") || file.ToLower().Contains(".xls"))
                    {
                        OleDbConnection Connection = new OleDbConnection();
                        OleDbCommand Command = new OleDbCommand();
                        OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                        DataSet DtsExcel = new DataSet();
                        Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file + ";Extended Properties='Excel 12.0 Xml;HDR=YES'";
                        Connection.Open();

                        var sheets = Connection.GetSchema("TABLES").AsEnumerable().Select(x => x.Field<string>("TABLE_NAME")).ToList();

                        Command.CommandText = "SELECT * FROM [" + sheets[0].Replace("'", "") + "]";  //A3:I29
                        Command.Connection = Connection;
                        DataAdapter.SelectCommand = Command;
                        DataAdapter.Fill(DtsExcel);
                        Connection.Close();
                        //prueba
                        foreach (DataRow cols in DtsExcel.Tables[0].Rows)
                        {
                            if (cols[0].ToString().Trim() != "Codigo del Empleado"
                                && cols[1] != null && cols[1].ToString().Trim() != string.Empty)
                            {
                                DTOTercero tercer = new DTOTercero();
                                tercer.NumDocument = cols[0].ToString().Trim();
                                string nombrecompleto = cols[1].ToString().Trim();
                                string[] nombres = nombrecompleto.Split(' ');
                                if (nombres.Length == 2)
                                {
                                    tercer.LastName = nombres[0];
                                    tercer.FirstName = nombres[1];
                                }
                                else if (nombres.Length == 3)
                                {
                                    tercer.LastName = nombres[0] + " " + nombres[1];
                                    tercer.FirstName = nombres[2];
                                }
                                else if (nombres.Length == 4)
                                {
                                    tercer.LastName = nombres[0] + " " + nombres[1];
                                    tercer.FirstName = nombres[2] + " " + nombres[3];
                                }
                                else
                                {
                                    tercer.LastName = nombres[0];
                                    tercer.FirstName = nombres[0];
                                }

                                tercer.Sexo = cols[3].ToString();
                                tercer.DateIn = DateTime.ParseExact(cols[4].ToString(), "d/M/yyyy", null);
                                tercer.DateBirth = DateTime.ParseExact(cols[5].ToString(), "d/M/yyyy", null);
                                tercer.CityBirth = cols[7].ToString();
                                tercer.CodeEnterprise = cols[8].ToString();
                                tercer.EnterpriseName = cols[9].ToString();
                                if (cols[10] != null)
                                    tercer.NDC = long.Parse(cols[10].ToString());
                                tercer.CodeArea = cols[11].ToString();
                                tercer.AreaName = cols[12].ToString();
                                tercer.CodeSucursal = cols[13].ToString();
                                tercer.SucursalName = cols[14].ToString();
                                tercer.DateContractStart = DateTime.ParseExact(cols[15].ToString(), "d/M/yyyy", null);
                                if (cols[16].ToString() != "99/99/9999")
                                    tercer.DateContractEnd = DateTime.ParseExact(cols[16].ToString(), "d/M/yyyy", null);

                                tercer.VacationUntil = DateTime.ParseExact(cols[17].ToString(), "d/M/yyyy", null);
                                if (cols[18] != null && !string.IsNullOrEmpty(cols[18].ToString().Trim()))
                                    tercer.DayPaysVacations = decimal.Parse(cols[18].ToString());
                                if (cols[19] != null && !string.IsNullOrEmpty(cols[19].ToString().Trim()) && cols[19].ToString() != "99/99/9999")
                                    tercer.DateRetirement = DateTime.ParseExact(cols[19].ToString(), "d/M/yyyy", null);
                                if (cols[20] != null)
                                    tercer.ReasonRetirement = cols[20].ToString().Replace("\"", "").Replace("}", "")
                                        .Replace("{", "");

                                string valuesalary = cols[23].ToString();
                                System.Globalization.NumberFormatInfo MyNFI = System.Globalization.NumberFormatInfo.CurrentInfo;
                                if (MyNFI.NumberDecimalSeparator.Equals("."))
                                {
                                    valuesalary = valuesalary.Replace(",", ".");
                                }
                                else if (MyNFI.NumberDecimalSeparator.Equals(","))
                                {
                                    valuesalary = valuesalary.Replace(".", ",");
                                }

                                tercer.Salary = decimal.Parse(valuesalary);
                                tercer.EpsCode = cols[24].ToString();
                                tercer.EpsName = cols[25].ToString();
                                tercer.AfpCode = cols[26].ToString();
                                tercer.AfpName = cols[27].ToString();
                                tercer.ArlCode = cols[28].ToString();
                                tercer.ArlName = cols[29].ToString();
                                tercer.CargoCode = cols[30].ToString();
                                tercer.CargoName = cols[31].ToString();
                                tercer.CajaCode = cols[32].ToString();
                                tercer.CajaCompesacionName = cols[33].ToString();
                                if (tercer.DateRetirement == null)
                                    tercer.IsActive = true;
                                Listcxi.Add(tercer);

                            }
                        }

                        File.Delete(file);
                    }
                }
                return Listcxi;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
                return null;
            }
        }

        public static DTOTransaction CreateFileAndReadOrderDistpatch(DTOTransaction dto)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            string[] base64cad = dto.AuxTest.Split(',');
            string ext = base64cad[1].Contains("vnd.openxmlformats-officedocument.spreadsheetml.sheet") ? ".xlsx" : ".xls";
            byte[] arraydoc = Convert.FromBase64String(base64cad[1]);
            // string directorybase = Directory.GetCurrentDirectory(); no sirve para .net framework
            string directorybase = AppDomain.CurrentDomain.BaseDirectory;
            // evento.WriteEntry("Directorio Base: " + directorybase + " - Directorio 2 " + AppDomain.CurrentDomain.BaseDirectory, EventLogEntryType.Information);



            if (!Directory.Exists(directorybase + "FilesApp\\Temp\\"))
            {
                Directory.CreateDirectory(directorybase + "FilesApp\\Temp\\");
            }
            string fullpath = directorybase + "FilesApp\\Temp\\" + Guid.NewGuid() + ext;
            try
            {
                FileStream stre = File.Create(fullpath);
                stre.Write(arraydoc, 0, arraydoc.Length);
                stre.Close();
                ReadOrderDistpatchOfDocumentInteropExcel(fullpath, dto);
                if (dto.TransactionIsOk)
                {
                    dto.Message = "La orden se importo correctamente : " + Environment.NewLine + dto.Message;
                }
                else
                {
                    //  dto.Message = "La orden no pudo ser importada : " + Environment.NewLine + dto.Message;
                    dto.Message = "La orden no pudo ser importada. ";
                }
            }
            catch (Exception ex)
            {
                dto.TransactionIsOk = false;
                dto.Message = "Error intentando importar orden de despacho: " + ex.ToString();
            }
            dto.AuxTest = null;
            //File.Delete(fullpath);
            return dto;
        }

        public static DTOTransaction CreateFileAnReaderItems(DTOTransaction dto)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            string[] base64cad = dto.AuxTest.Split(',');
            string ext = base64cad[1].Contains("vnd.openxmlformats-officedocument.spreadsheetml.sheet") ? ".xlsx" : ".xls";
            byte[] arraydoc = Convert.FromBase64String(base64cad[1]);

            // string directorybase = Directory.GetCurrentDirectory(); no sirve para .net framework
            string directorybase = AppDomain.CurrentDomain.BaseDirectory;
            // evento.WriteEntry("Directorio Base: " + directorybase + " - Directorio 2 " + AppDomain.CurrentDomain.BaseDirectory, EventLogEntryType.Information);
            if (!Directory.Exists(directorybase + "FilesApp\\Temp\\"))
            {
                Directory.CreateDirectory(directorybase + "FilesApp\\Temp\\");
            }
            string fullpath = directorybase + "FilesApp\\Temp\\" + Guid.NewGuid() + ext;
            try
            {
                FileStream stre = File.Create(fullpath);
                stre.Write(arraydoc, 0, arraydoc.Length);
                stre.Close();
                ReadItemsOfDocumentInteropExcel(fullpath, dto);
                if (dto.TransactionIsOk)
                {
                    dto.Message = "Los items se sincronizaron correctamente : " + Environment.NewLine + dto.Message;
                }
                else
                {
                    //  dto.Message = "La orden no pudo ser importada : " + Environment.NewLine + dto.Message;
                    dto.Message = "Los items no se sincronizaron. ";
                }
            }
            catch (Exception ex)
            {
                dto.TransactionIsOk = false;
                dto.Message = "Error intentando sincronizar items: " + ex.ToString();
            }
            dto.AuxTest = null;
            //File.Delete(fullpath);
            return dto;
        }


        public static DTOTransaction ReadItemsOfDocumentInteropExcel(string PathNamefile, DTOTransaction trans)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            //  evento.WriteEntry("archivo: " + PathNamefile, EventLogEntryType.Information);
            try
            {
                Application app = new Application();
                Workbook workBook = null;
                Worksheet workSheet = null;
                //Abre el libro de excel que tiene la plantilla
                workBook = app.Workbooks.Open(PathNamefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                //Referencia la primera hoja de la plantilla
                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets[1];

                string msgreturn = string.Empty;
                int countnull = 1;

                int rowitems = 2;//fila desde donde mas o menos empiezan los items

                bool newtable = false;
                if (trans.ListItems == null)
                    trans.ListItems = new List<DTOInventary>();
                while (true)
                {
                    var valu = (workSheet.Cells[rowitems, 2] as Range).Value;
                    if (valu == null || valu.ToString().Trim() == "")
                    {
                        newtable = false;
                        if (countnull == 10)
                        {
                            break;
                        }
                        rowitems++; countnull++;
                        continue;
                    }
                    countnull = 1;
                    var valuebarc = (workSheet.Cells[rowitems, 2] as Range).Value;
                    if (valuebarc != null && !string.IsNullOrEmpty(valuebarc.ToString()))
                    {
                        string barco = valuebarc.ToString().Trim();
                        DTOInventary deta = trans.ListItems.Where(x => x.Barcode == barco).FirstOrDefault();
                        if (deta == null)
                        {
                            try
                            {
                                deta = new DTOInventary();
                                deta.Barcode = barco;
                                deta.ItemName = (workSheet.Cells[rowitems, 3] as Range).Value.ToString().Trim();
                                deta.ItemName = deta.ItemName.Replace("\"", "'");
                                var area = (workSheet.Cells[rowitems, 5] as Range).Value;
                                if (area != null && !string.IsNullOrWhiteSpace(area.ToString()))
                                    deta.Area = decimal.Parse((workSheet.Cells[rowitems, 5] as Range).Value.ToString().Trim());
                                var Weight = (workSheet.Cells[rowitems, 4] as Range).Value;
                                if (Weight != null && !string.IsNullOrWhiteSpace(Weight.ToString()))
                                    deta.Weight = decimal.Parse((workSheet.Cells[rowitems, 4] as Range).Value.ToString().Trim());
                                deta.ZoneProduct = (workSheet.Cells[rowitems, 15] as Range).Value?.ToString().Trim() ?? "";
                                deta.WarehouseName = (workSheet.Cells[rowitems, 17] as Range).Value?.ToString().Trim() ?? "";
                            }
                            catch (Exception ex)
                            {

                            }
                            trans.ListItems.Add(deta);
                        }
                    }
                    rowitems++;
                }

                if (trans.ListItems == null || trans.ListItems.Count == 0)
                {
                    trans.TransactionIsOk = false;
                    msgreturn += "No se encontraron items en el excel." + Environment.NewLine;
                }
                else
                {
                    trans.TransactionIsOk = true;
                    msgreturn += "Se cargaron un total de " + trans.ListItems.Count + " items." + Environment.NewLine;
                }
                workBook.Close(false, null, null);
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
            }
            return trans;
        }


        public static DTOTransaction ReadOrderDistpatchOfDocumentInteropExcel(string PathNamefile, DTOTransaction trans)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            //  evento.WriteEntry("archivo: " + PathNamefile, EventLogEntryType.Information);
            try
            {
                Application app = new Application();
                Workbook workBook = null;
                Worksheet workSheet = null;
                //Abre el libro de excel que tiene la plantilla
                workBook = app.Workbooks.Open(PathNamefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                //Referencia la primera hoja de la plantilla
                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets[1];

                string msgreturn = string.Empty;
                int countnull = 1;

                //    evento.WriteEntry("Info: Lectura excel correcta", EventLogEntryType.Information);

                int rowitems = 30;//fila desde donde mas o menos empiezan los items

                bool newtable = false;
                if (trans.Details == null)
                    trans.Details = new List<DTOTransactionDetail>();
                while (true)
                {
                    var valu = (workSheet.Cells[rowitems, 1] as Range).Value;
                    if (valu == null || valu.ToString().Trim() == "")
                    {
                        newtable = false;
                        if (countnull == 80)
                        {
                            break;
                        }
                        rowitems++; countnull++;
                        continue;
                    }
                    if (valu.ToString().ToUpper().Trim().Contains("ITEM"))
                    {
                        countnull = 1;
                        rowitems++;
                        newtable = true;
                    }

                    if (newtable)
                    {
                        var valuebarc = (workSheet.Cells[rowitems, 2] as Range).Value;
                        if (valuebarc != null)
                        {
                            string barco = valuebarc.ToString().Trim();
                            DTOTransactionDetail deta = trans.Details.Where(x => x.ItemBarcode == barco).FirstOrDefault();
                            if (deta == null)
                            {
                                try
                                {
                                    deta = new DTOTransactionDetail();
                                    deta.ItemBarcode = barco;
                                    deta.ItemDescription = (workSheet.Cells[rowitems, 3] as Range).Value.ToString().Trim();
                                    deta.ItemDescription = deta.ItemDescription.Replace("\"", "'");

                                    string val = (workSheet.Cells[rowitems, 5] as Range).Value.ToString().Trim();
                                    deta.Cant = decimal.Parse(val);
                                }
                                catch (Exception ex)
                                {

                                }

                                trans.Details.Add(deta);
                            }
                            else
                            {
                                try
                                {
                                    string val = (workSheet.Cells[rowitems, 5] as Range).Value.ToString().Trim();
                                    deta.Cant += decimal.Parse(val);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    rowitems++;
                }

                if (trans.Details == null || trans.Details.Count == 0)
                {
                    trans.TransactionIsOk = false;
                    msgreturn += "No se encontraron items en el excel." + Environment.NewLine;
                }
                else
                {
                    trans.TransactionIsOk = true;
                    msgreturn += "Se cargaron un total de " + trans.Details.Count + " items." + Environment.NewLine;
                }
                workBook.Close(false, null, null);
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
            }

            return trans;
        }

        public static List<DTOInventary> ReadinventaryOfDocumentInteropExcel(string PathNamefile)
        {

            Application app = new Application();
            Workbook workBook = null;
            Worksheet workSheet = null;
            //Abre el libro de excel que tiene la plantilla
            workBook = app.Workbooks.Open(PathNamefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //Referencia la primera hoja de la plantilla
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets[1];
            bool nextread = true;
            int row = 1;
            int countnull = 1;
            List<DTOInventary> Listcxi = new List<DTOInventary>();
            Hashtable hashBodegas = new Hashtable();
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

                if (valu.ToString().Trim() != "CODIGO")
                {
                    string Barcodeaux = (workSheet.Cells[row, 1] as Range).Value.ToString().Trim();
                    string ItemNameaux = (workSheet.Cells[row, 2] as Range).Value.ToString().Trim();
                    foreach (int colm in hashBodegas.Keys)
                    {
                        DTOInventary x = new DTOInventary();
                        x.Barcode = Barcodeaux;
                        x.ItemName = ItemNameaux;
                        x.WarehouseName = hashBodegas[colm].ToString();
                        x.CantInv = decimal.Parse((workSheet.Cells[row, colm] as Range).Value.ToString().Trim());
                        Listcxi.Add(x);


                    }
                }
                else
                {
                    int numcell = 3;
                    while (true)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty((workSheet.Cells[row, numcell] as Range).Value.ToString().Trim()))
                            {
                                hashBodegas.Add(numcell, (workSheet.Cells[row, numcell] as Range).Value.ToString().Trim());
                            }
                            else
                            {
                                break;
                            }
                            numcell++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                row++;
            }
            workBook.Close(false, null, null);

            return Listcxi;
        }


        public static List<DTOParamContable> ReadParametersOfExcel(string PathNamefile)
        {

            Application app = new Application();
            Workbook workBook = null;
            Worksheet workSheet = null;
            //Abre el libro de excel que tiene la plantilla
            workBook = app.Workbooks.Open(PathNamefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            //Referencia la primera hoja de la plantilla
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets["Parametros"];
            bool nextread = true;
            int row = 1;
            int countnull = 1;
            List<DTOParamContable> Listcxi = new List<DTOParamContable>();
            Hashtable hashBodegas = new Hashtable();
            while (nextread)
            {

                var valu = (workSheet.Cells[row, 2] as Range).Value;
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
                if (valu.ToString().Trim().Contains("Informacion Economica"))
                {
                    long year = long.Parse(valu.ToString().ToUpper().Replace("INFORMACION ECONOMICA", "").Trim());
                    row = row + 4;
                    //string Barcodeaux = (workSheet.Cells[row, 1] as Range).Value.ToString().Trim();
                    //string ItemNameaux = (workSheet.Cells[row, 2] as Range).Value.ToString().Trim();
                    try
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            DTOParamContable cont = new DTOParamContable();
                            cont.year = year;
                            cont.month = i;
                            cont.ValueDolar = decimal.Parse((workSheet.Cells[row, i + 2] as Range).Value.ToString().Trim());
                            Listcxi.Add(cont);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    //foreach (int colm in hashBodegas.Keys)
                    //{
                    //    DTOInventary x = new DTOInventary();
                    //    x.Barcode = Barcodeaux;
                    //    x.ItemName = ItemNameaux;
                    //    x.WarehouseName = hashBodegas[colm].ToString();
                    //    x.CantInv = decimal.Parse((workSheet.Cells[row, colm] as Range).Value.ToString().Trim());
                    //    Listcxi.Add(x);
                    //}
                }
                row++;
            }
            workBook.Close(false, null, null);

            return Listcxi;
        }
    }
}