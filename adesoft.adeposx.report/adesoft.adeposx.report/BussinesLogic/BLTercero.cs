using adesoft.adeposx.report.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.BussinesLogic
{
    public class BLTercero
    {
        public BLTercero()
        {

        }


        public static DTOTercero GenerateCertificate(DTOTercero tercero)
        {
            try
            {
                string filecertificated = string.Empty;
                if (tercero.IsActive)
                {
                    filecertificated = "CERTIFICACION_ACTIVOS.doc";
                }
                else
                {
                    filecertificated = "CERTIFICADO_RETIRADOS.doc";
                }
                //https://gist.github.com/ArthurEzenwanne/c443e8bbc67af312aea05c632652ab56
                Application app = new Application();
                Object m = System.Reflection.Missing.Value;
                String ruta = string.Empty;
                object filesave = Guid.NewGuid();
                object oformat = WdSaveFormat.wdFormatPDF;

                string directorybase = AppDomain.CurrentDomain.BaseDirectory + "FilesApp\\Certificados\\";

                if (!Directory.Exists(directorybase))
                {
                    Directory.CreateDirectory(directorybase);
                }
                string FullPathFile = directorybase + filecertificated;
                string FullPathFileCopy = directorybase + filesave + ".doc";
                object FullPathFilePdf = directorybase + filesave + ".pdf";
                string RelativePathFilePdf = "FilesApp\\Certificados\\" + filesave + ".pdf";
                File.Copy(FullPathFile, FullPathFileCopy);
                Document doc = app.Documents.Open(FullPathFileCopy,
                                           ref m, ref m, ref m,
                                           ref m, ref m, ref m,
                                           ref m, ref m, ref m,
                                           ref m, false, ref m,
                                           ref m, ref m, ref m);
                doc.Activate();
                string documentId = string.Empty;
                try
                {
                    documentId = long.Parse(tercero.NumDocument.Trim()).ToString("N0");
                }
                catch(Exception ex)
                {
                    documentId = tercero.NumDocument;
                }
                if (tercero.IsActive)
                {
                    doc.Bookmarks["NombreEmpleado"].Range.Text = tercero.FullName;
                    doc.Bookmarks["Cargo"].Range.Text = tercero.CargoName;
                    doc.Bookmarks["Identificacion"].Range.Text = documentId;
                    doc.Bookmarks["FechaDocumento"].Range.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-CO"));
                    doc.Bookmarks["Salario"].Range.Text = tercero.Salary.ToString("C2", CultureInfo.GetCultureInfo("Es-co"));
                    doc.Bookmarks["Fechalabor"].Range.Text = tercero.DateContractStart.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-CO"));
                }
                else
                {
                    doc.Bookmarks["NombreEmpleado"].Range.Text = tercero.FullName;
                    doc.Bookmarks["Cargo"].Range.Text = tercero.CargoName;
                    doc.Bookmarks["Identificacion"].Range.Text = documentId;
                    doc.Bookmarks["FechaDocumento"].Range.Text = Util.UtilNumbers.NumeroALetras(DateTime.Now.Day).ToLower() + " (" + DateTime.Now.Day + ") días" + DateTime.Now.ToString(" 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-CO"));
                    doc.Bookmarks["Salario"].Range.Text = tercero.Salary.ToString("C2", CultureInfo.GetCultureInfo("Es-co"));
                    doc.Bookmarks["Fechalabor"].Range.Text = tercero.DateContractStart.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-CO"));
                    if (tercero.DateRetirement != null)
                        doc.Bookmarks["Fechafin"].Range.Text = tercero.DateRetirement.Value.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-CO"));
                    else
                        doc.Bookmarks["Fechafin"].Range.Text = "";
                }
                doc.SaveAs(ref FullPathFilePdf, ref oformat, ref m, ref m,
             ref m, ref m, ref m, ref m, ref m,
             ref m, ref m, ref m, ref m, ref m, ref m, ref m
             );
                // Always close Word.exe.
                doc.Close();
                app.Quit(ref m, ref m, ref m);
                File.Delete(FullPathFileCopy);
                DTOTercero terc = new DTOTercero();
                terc.FirstName = RelativePathFilePdf;
                return terc;
            }
            catch (Exception ex)
            {
                return new DTOTercero();
            }
        }

    }
}