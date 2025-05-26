using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class PreliquidationBussines
    {
        public AdeposDBContext context { get; set; }

        public IConfiguration configuration;
        public PreliquidationBussines(AdeposDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public NominaProgramation GeneratePlanos(NominaProgramation model)
        {
            List<LocationGeneric> listlocations = context.LocationGenerics.ToList();
            List<NominaNovedad> listNomins = context.NominaNovedads.Where(x => x.NominaProgramationId == model.NominaProgramationId
            && x.StateNovedad == 2).ToList();
            DataSet Datsetinterrup = CreateDatasetInterruptions();
            int countInterruptions = 1;
            DataTable tInterruptions = Datsetinterrup.Tables[0];
            foreach (NominaNovedad nom in listNomins)
            {
                nom.Tercero = context.Terceros.Where(x => x.TerceroId == nom.TerceroId).FirstOrDefault();
                nom.CodeNovedad = context.CodeNovedads.Where(x => x.CodeNovedadId == nom.CodeNovedadId).FirstOrDefault();
                if (nom.CodeNovedad.PlaneType == "TNL")
                {
                    DataRow newRowsInterruptions = tInterruptions.NewRow();
                    newRowsInterruptions["NMTNLTRM-CSQ"] = countInterruptions++.ToString().PadLeft(9, '0');
                    newRowsInterruptions["NMTNLTRM-TIPO-REG"] = "1";
                    newRowsInterruptions["NMTNLTRM-EMPLEADO"] = nom.Tercero.NumDocument.PadRight(13, ' ');
                    newRowsInterruptions["NMTNLTRM-SUC"] = "00";
                    newRowsInterruptions["NMTNLTRM-NDC"] = "00";
                    newRowsInterruptions["NMTNLTRM-CONS"] = "000";
                    newRowsInterruptions["NMTNLTRM-CPTO"] = nom.CodeNovedad.Syncode.PadLeft(3, '0');
                    newRowsInterruptions["NMTNLTRM-FECHA-INI"] = string.Concat(nom.DayInit.Year,
                            nom.DayInit.Month.ToString().PadLeft(2, '0'),
                            nom.DayInit.Day.ToString().PadLeft(2, '0'));
                    newRowsInterruptions["NMTNLTRM-FECHA-FIN"] = string.Concat(nom.DayEnd.Year,
                                                                                nom.DayEnd.Month.ToString().PadLeft(2, '0'),
                                                                                nom.DayEnd.Day.ToString().PadLeft(2, '0'));
                    TimeSpan Times = new TimeSpan(0, 0, 0, 0, 0);
                    if ((nom.DayEnd.Hour - nom.DayInit.Hour) >= 23 || (nom.DayEnd.DayOfYear - nom.DayInit.DayOfYear) > 1)
                    {
                        if (nom.DayEnd.Day > nom.DayInit.Day)
                        {
                            int days = (nom.DayEnd.Day - nom.DayInit.Day);
                            Times = new TimeSpan(days, 0, 0, 0, 0);
                            if (nom.DayEnd.Hour >= 8)
                            {
                                TimeSpan oneday = new TimeSpan(1, 0, 0, 0, 0);
                                Times = Times.Add(oneday);
                            }
                        }
                        else
                        {
                            Times = new TimeSpan(1, 0, 0, 0, 0);
                        }
                        newRowsInterruptions["NMTNLTRM-FECHA-DIAS"] = (Times.Days).ToString().PadLeft(5, '0');
                        newRowsInterruptions["NMTNLTRM-HORA-INI"] = "0000";
                        newRowsInterruptions["NMTNLTRM-HORA-FIN"] = "0000";
                    }
                    else
                    {
                        newRowsInterruptions["NMTNLTRM-FECHA-DIAS"] = "00000";

                        newRowsInterruptions["NMTNLTRM-HORA-INI"] = string.Concat(nom.DayInit.Hour.ToString().PadLeft(2, '0'),
                                                                    nom.DayInit.Minute.ToString().PadLeft(2, '0'));
                        newRowsInterruptions["NMTNLTRM-HORA-FIN"] = string.Concat(nom.DayEnd.Hour.ToString().PadLeft(2, '0'),
                                                                    nom.DayEnd.Minute.ToString().PadLeft(2, '0'));
                    }
                    newRowsInterruptions["NMTNLTRM-VALOR"] = "".PadRight(13, '0');
                    newRowsInterruptions["NMTNLTRM-BASE"] = "+".PadRight(14, '0');
                    newRowsInterruptions["NMTNLTRM-OBSERV"] = "+".PadRight(40, ' ');
                    tInterruptions.Rows.Add(newRowsInterruptions);
                }
            }
            DataSet DStLiquidateDay = CreateDatasetLiquidateFile();
            DataTable tLiquidateDay = DStLiquidateDay.Tables[0];
            foreach (NominaNovedad nom in listNomins)
            {
                nom.Tercero = context.Terceros.Where(x => x.TerceroId == nom.TerceroId).FirstOrDefault();
                nom.CodeNovedad = context.CodeNovedads.Where(x => x.CodeNovedadId == nom.CodeNovedadId).FirstOrDefault();
                if (nom.CodeNovedad.PlaneType == "HORAS LABORADAS")
                {
                    DataRow newRow2 = tLiquidateDay.NewRow();
                    if (nom.Tercero.NumDocument.Length <= 13)
                    {
                        newRow2["NMBATCHEMPL"] = nom.Tercero.NumDocument.PadRight(13, ' ');
                    }
                    else
                    {
                        throw new Exception("El numero de documento tiene mas de 13 caracteres");
                    }

                    newRow2["NMBATCHSUC"] = "00"; //sucursal
                    newRow2["NMBATCHCPTO"] = nom.CodeNovedad.Syncode.PadRight(3, (' '));//codigo concepto// hourLiquidateDay.HourType.idExtern.PadRight(3,(' '));

                    newRow2["NMBATCHCUMOV"] = "   ";

                    if (nom.Tercero.AreaId == 0)
                    {
                        newRow2["NMBATCHCOSTOSNMBATCHDES"] = "        ";
                    }
                    else
                    {
                        LocationGeneric loc = listlocations.Where(x => x.LocationGenericId == nom.Tercero.AreaId).FirstOrDefault();
                        if (loc.SyncCode.Length <= 8)
                        {
                            newRow2["NMBATCHCOSTOSNMBATCHDES"] = loc.SyncCode;
                        }
                        else
                        {
                            newRow2["NMBATCHCOSTOSNMBATCHDES"] = "        ";
                        }
                    }
                    newRow2["NMBATCHFECHA"] = nom.DayInit.Year.ToString() + nom.DayInit.Month.ToString().PadLeft(2, ('0')) + nom.DayInit.Day.ToString().PadLeft(2, ('0'));//fecha de movimiento

                    newRow2["NMBATCHFECINITNL"] = "        ";//fecha inicial del tiempo no laborado
                    newRow2["NMBATCHFECFINTNL"] = "        ";//fecha final del tiempo no laborado
                    newRow2["NMBATCHDIASTNL"] = "000";  //dias de tiempo no laborado 
                    newRow2["NMBATCHACTIVIDAD"] = "        ";//cantidad
                    newRow2["NMBATCHUBICACION"] = "        ";//Ubicacion

                    string timeofliquidate;
                    int Minutes = nom.HoursNovedad.Minutes;
                    timeofliquidate = nom.HoursNovedad.Hours.ToString().PadLeft(4, ('0')) + Minutes.ToString().PadLeft(2, ('0')) + "+";
                    newRow2["NMBATCHHORAS"] = timeofliquidate;//horas y minutos de la novedad

                    newRow2["NMBATCHVALOR"] = "0000000000000+";//valor de la novedad
                    newRow2["NMBATCHCANT"] = "0000000+";//cantidad
                    newRow2["NMBATCHCUOTANRO"] = "000";//prestamo numero de cuotas
                    newRow2["NMBATCHFECCPH"] = "        ";
                    if (nom.Tercero.NumDocument.Length <= 13)
                    {
                        Int64 IdentificationExtern = Convert.ToInt64(nom.Tercero.NumDocument);
                        newRow2["NMBATCHCED"] = IdentificationExtern.ToString("D13");
                    }
                    else
                    {
                        throw new Exception("El numero de documento tiene mas de 13 caracteres");
                    }
                    newRow2["NMBATCHPROJECT"] = "          ";
                    newRow2["NMBATCHUBICALAB"] = "        ";
                    tLiquidateDay.Rows.Add(newRow2);
                }
            }

            string pathapp = Directory.GetCurrentDirectory();
            string directoryDownload =  "/wwwroot/FilesApp/DocNovedades/";
            if (tInterruptions.Rows.Count > 0)
            {
                string pathfile = configuration.GetValue<string>("FilesFolderImportUnoeeNMTNLTRM");
                pathfile = pathfile + "/NMTNLTRM.DAT";
                model.PathFileTnlDownload = directoryDownload + "/NMTNLTRM.txt";
                readWriteFile(pathfile, Datsetinterrup, pathapp + model.PathFileTnlDownload);
            }
            if (tLiquidateDay.Rows.Count > 0)
            {
                string pathfile = configuration.GetValue<string>("FilesFolderImportUnoeeNMBATCH");
                pathfile = pathfile + "/NMBATCH.DAT";
                model.PathFileNmbatchDownload = directoryDownload + "/NMBATCH.txt";
                readWriteFile(pathfile, DStLiquidateDay, pathapp + model.PathFileNmbatchDownload );
            }


            return model;
        }



        DataSet CreateDatasetInterruptions()
        {
            try
            {
                DataSet MyDataSetInterruptionsType1 = new DataSet("dsInterruptionsType1");
                DataTable tInterruptions = new DataTable("dtInterruptions");
                DataColumn NMTNLTRMCSQ = new DataColumn("NMTNLTRM-CSQ");//Consecutivo
                NMTNLTRMCSQ.ReadOnly = false;
                DataColumn NMTNLTRMTIPOREG = new DataColumn("NMTNLTRM-TIPO-REG");//Tipo de registro
                NMTNLTRMTIPOREG.ReadOnly = false;
                DataColumn NMTNLTRMEMPLEADO = new DataColumn("NMTNLTRM-EMPLEADO");//Empleado
                NMTNLTRMEMPLEADO.ReadOnly = false;
                DataColumn NMTNLTRMSUC = new DataColumn("NMTNLTRM-SUC");//Sucursal
                NMTNLTRMEMPLEADO.ReadOnly = false;
                DataColumn NMTNLTRMNDC = new DataColumn("NMTNLTRM-NDC");//Contrato
                NMTNLTRMNDC.ReadOnly = false;
                DataColumn NMTNLTRMCONS = new DataColumn("NMTNLTRM-CONS");//Consecutivo
                NMTNLTRMCONS.ReadOnly = false;
                DataColumn NMTNLTRMCPTO = new DataColumn("NMTNLTRM-CPTO");//Concepto
                NMTNLTRMCPTO.ReadOnly = false;
                DataColumn NMTNLTRMFECHAINI = new DataColumn("NMTNLTRM-FECHA-INI");//Fecha Inicial
                NMTNLTRMFECHAINI.ReadOnly = false;
                DataColumn NMTNLTRMFECHAFIN = new DataColumn("NMTNLTRM-FECHA-FIN");//Fecha Inicial
                NMTNLTRMFECHAFIN.ReadOnly = false;
                DataColumn NMTNLTRMFECHADIAS = new DataColumn("NMTNLTRM-FECHA-DIAS");//Días del TNL
                NMTNLTRMFECHADIAS.ReadOnly = false;
                DataColumn NMTNLTRMHORAINI = new DataColumn("NMTNLTRM-HORA-INI");//Hora Inicial
                NMTNLTRMHORAINI.ReadOnly = false;
                DataColumn NMTNLTRMHORAFIN = new DataColumn("NMTNLTRM-HORA-FIN");//Hora Final
                NMTNLTRMHORAFIN.ReadOnly = false;
                DataColumn NMTNLTRMVALOR = new DataColumn("NMTNLTRM-VALOR");//Valor del TNL
                NMTNLTRMVALOR.ReadOnly = false;
                DataColumn NMTNLTRMBASE = new DataColumn("NMTNLTRM-BASE");//Base del TNL
                NMTNLTRMBASE.ReadOnly = false;
                DataColumn NMTNLTRMOBSERV = new DataColumn("NMTNLTRM-OBSERV");//Base del TNL
                NMTNLTRMOBSERV.ReadOnly = false;

                tInterruptions.Columns.Add(NMTNLTRMCSQ);
                tInterruptions.Columns.Add(NMTNLTRMTIPOREG);
                tInterruptions.Columns.Add(NMTNLTRMEMPLEADO);
                tInterruptions.Columns.Add(NMTNLTRMSUC);
                tInterruptions.Columns.Add(NMTNLTRMNDC);
                tInterruptions.Columns.Add(NMTNLTRMCONS);
                tInterruptions.Columns.Add(NMTNLTRMCPTO);
                tInterruptions.Columns.Add(NMTNLTRMFECHAINI);
                tInterruptions.Columns.Add(NMTNLTRMFECHAFIN);
                tInterruptions.Columns.Add(NMTNLTRMFECHADIAS);
                tInterruptions.Columns.Add(NMTNLTRMHORAINI);
                tInterruptions.Columns.Add(NMTNLTRMHORAFIN);
                tInterruptions.Columns.Add(NMTNLTRMVALOR);
                tInterruptions.Columns.Add(NMTNLTRMBASE);
                tInterruptions.Columns.Add(NMTNLTRMOBSERV);

                MyDataSetInterruptionsType1.Tables.Add(tInterruptions);
                return MyDataSetInterruptionsType1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        DataSet CreateDatasetLiquidateFile()
        {
            try
            {
                DataSet MyDataSetLiq = new DataSet("MyDataSetLiq");
                //nombre de la tabla
                DataTable tLiquidateDay = new DataTable("StringLiq");
                DataColumn NMBATCHEMPL = new DataColumn("NMBATCHEMPL");//documento identificacion del empleado 
                NMBATCHEMPL.ReadOnly = false;
                //columnas que van
                DataColumn NMBATCHSUC = new DataColumn("NMBATCHSUC");//sucursal en ceros 
                NMBATCHSUC.ReadOnly = false;
                DataColumn NMBATCHCPTO = new DataColumn("NMBATCHCPTO");//codigo del concepto 
                NMBATCHCPTO.ReadOnly = false;
                DataColumn NMBATCHCUMOV = new DataColumn("NMBATCHCUMOV");//centro de operacion
                NMBATCHCUMOV.ReadOnly = false;
                DataColumn NMBATCHCOSTOSNMBATCHDES = new DataColumn("NMBATCHCOSTOSNMBATCHDES");//centro de costos destino
                NMBATCHCOSTOSNMBATCHDES.ReadOnly = false;
                DataColumn NMBATCHFECHA = new DataColumn("NMBATCHFECHA");//fecha movimiento
                NMBATCHFECHA.ReadOnly = false;
                DataColumn NMBATCHFECINITNL = new DataColumn("NMBATCHFECINITNL");//fecha inicial de tiempo no laborado
                NMBATCHFECINITNL.ReadOnly = false;
                DataColumn NMBATCHFECFINTNL = new DataColumn("NMBATCHFECFINTNL");//fecha final del tiempo no laborado
                NMBATCHFECFINTNL.ReadOnly = false;
                DataColumn NMBATCHDIASTNL = new DataColumn("NMBATCHDIASTNL");//dias de tiempo no laborado
                NMBATCHDIASTNL.ReadOnly = false;
                DataColumn NMBATCHACTIVIDAD = new DataColumn("NMBATCHACTIVIDAD");//cantifad
                NMBATCHACTIVIDAD.ReadOnly = false;
                DataColumn NMBATCHUBICACION = new DataColumn("NMBATCHUBICACION");//ubicacion
                NMBATCHUBICACION.ReadOnly = false;
                DataColumn NMBATCHHORAS = new DataColumn("NMBATCHHORAS");//Horas
                NMBATCHHORAS.ReadOnly = false;
                DataColumn NMBATCHVALOR = new DataColumn("NMBATCHVALOR");//valor
                NMBATCHVALOR.ReadOnly = false;
                DataColumn NMBATCHCANT = new DataColumn("NMBATCHCANT");//Cantidad
                NMBATCHCANT.ReadOnly = false;
                DataColumn NMBATCHCUOTANRO = new DataColumn("NMBATCHCUOTANRO");//numero de cuotas o prestamo
                NMBATCHCUOTANRO.ReadOnly = false;
                DataColumn NMBATCHFECCPH = new DataColumn("NMBATCHFECCPH");//fecha pagada hasta
                NMBATCHFECCPH.ReadOnly = false;
                DataColumn NMBATCHCED = new DataColumn("NMBATCHCED");//cedula de la persona
                NMBATCHCED.ReadOnly = false;
                DataColumn NMBATCHPROJECT = new DataColumn("NMBATCHPROJECT");//proyecto
                NMBATCHPROJECT.ReadOnly = false;
                DataColumn NMBATCHUBICALAB = new DataColumn("NMBATCHUBICALAB");//proyecto
                NMBATCHPROJECT.ReadOnly = false;

                //numero de columnas que agrego al table
                tLiquidateDay.Columns.Add(NMBATCHEMPL);
                tLiquidateDay.Columns.Add(NMBATCHSUC);
                tLiquidateDay.Columns.Add(NMBATCHCPTO);
                tLiquidateDay.Columns.Add(NMBATCHCUMOV);
                tLiquidateDay.Columns.Add(NMBATCHCOSTOSNMBATCHDES);
                tLiquidateDay.Columns.Add(NMBATCHFECHA);
                tLiquidateDay.Columns.Add(NMBATCHFECINITNL);
                tLiquidateDay.Columns.Add(NMBATCHFECFINTNL);
                tLiquidateDay.Columns.Add(NMBATCHDIASTNL);
                tLiquidateDay.Columns.Add(NMBATCHACTIVIDAD);
                tLiquidateDay.Columns.Add(NMBATCHUBICACION);
                tLiquidateDay.Columns.Add(NMBATCHHORAS);
                tLiquidateDay.Columns.Add(NMBATCHVALOR);
                tLiquidateDay.Columns.Add(NMBATCHCANT);
                tLiquidateDay.Columns.Add(NMBATCHCUOTANRO);
                tLiquidateDay.Columns.Add(NMBATCHFECCPH);
                tLiquidateDay.Columns.Add(NMBATCHCED);
                tLiquidateDay.Columns.Add(NMBATCHPROJECT);
                tLiquidateDay.Columns.Add(NMBATCHUBICALAB);

                MyDataSetLiq.Tables.Add(tLiquidateDay);
                return MyDataSetLiq;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        static void readWriteFile(string name, DataSet source, string filedownload)
        {

            string fileName = name;
            FileInfo fi = new FileInfo(fileName);
            try
            {
                int line = 0;
                string String;
                StreamWriter swFromFileStream;
                if (!fi.Exists)
                {
                    FileStream fs = new FileStream(fileName,
                        FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    swFromFileStream = new StreamWriter(fs);
                }
                else
                {
                    File.Delete(fileName);
                    FileStream fs = new FileStream(fileName,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    swFromFileStream = new StreamWriter(fs);

                }
                String = "";
                foreach (DataRow x in source.Tables[0].Rows)
                {
                    string Row = string.Empty;
                    for (int y = 0; y < source.Tables[0].Columns.Count; y++)
                    {
                        Row = Row + x[y].ToString();
                    }
                    //if (version == "V2")
                    //{
                    //    string[] V2 = Row.Split(';');
                    //    string LineV2 = V2[0] + ";" + V2[1] + ";;" + V2[3] + ";" + V2[4].Replace("/", "-") + ";;;;;;;;;;" + V2[4].Replace("/", "-") + ";;;";
                    //    String = String + LineV2;
                    //}
                    //else
                    //{
                    String = String + Row;
                    //}
                    String = String + Environment.NewLine;
                }
                swFromFileStream.Write(String);
                swFromFileStream.Flush();
                swFromFileStream.Close();

                //para el archivo de descarga
                File.Delete(filedownload);
                FileStream filedownlo = new FileStream(filedownload,
                       FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                StreamWriter streamfile = new StreamWriter(filedownlo);
                streamfile.Write(String);
                streamfile.Flush();
                streamfile.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // Close the file so it can be deleted.
        }

    }
}
