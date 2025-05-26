using BlazorInputFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class NominaNovedad : BaseEntity
    {
        public NominaNovedad()
        {

        }

        public long NominaNovedadId { get; set; }

        //public long CantHours { get; set; }

        //public long CantMinutes { get; set; }
        /// <summary>
        /// DEDUCCION...
        /// </summary>
        public string TypeNovedadName { get; set; }

        public long CodeNovedadId { get; set; }

        public TimeSpan HoursNovedad { get; set; }

     

        public DateTime DayInit { get; set; }

        public DateTime DayEnd { get; set; }

        public long TerceroId { get; set; }


        public bool FullDay { get; set; }
        /// <summary>
        ///1 PENDIENTE ,2 APROBADO ,3 NO APROBADO ,4 REVISADO.
        /// </summary>
        public long StateNovedad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HoursNovedad2 { get; set; }
        [JsonIgnore]
        [NotMapped]
        public string StateNovedadLabel
        {
            get
            {
                if (NominaProgramationId != 0)
                {
                    if (StateNovedad == 1)
                    {
                        return "PENDIENTE";
                    }
                    else if (StateNovedad == 2)
                    {
                        return "APROBADO";
                    }
                    else if (StateNovedad == 3)
                    {
                        return "NO APROBADO";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {//modulo de produccion
                    if (StateNovedad == 2)
                    {
                        return "ACTIVO";
                    }
                    else if (StateNovedad == 3)
                    {
                        return "ANULADO";
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
        [JsonIgnore]
        [NotMapped]
        public string ColorStateNovedad
        {
            get
            {

                if (StateNovedad == 1)
                {
                    return "Orange";
                }
                else if (StateNovedad == 2)
                {
                    return "Green";
                }
                else if (StateNovedad == 3)
                {
                    return "Red";
                }
                else
                {
                    return "";
                }

            }
        }
        public string Observation { get; set; }

        public long NominaProgramationId { get; set; }

        public string ConsecutiveMobileId { get; set; }

        [NotMapped]
        public Tercero Tercero { get; set; }

        [JsonIgnore]
        [NotMapped]
        public CodeNovedad CodeNovedad { get; set; }


        [NotMapped]
        public string DayInitLabel
        {
            get
            {
                return DayInit.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }
        [NotMapped]
        public string DayEndLabel
        {
            get
            {
                return DayEnd.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }



        [NotMapped]
        public string DateInitEndLabel
        {
            get
            {
                if (DayEnd.Subtract(DayInit).Days > 0)
                {
                    return DayInit.ToString("MMMM dd", CultureInfo.GetCultureInfo("ES-co")) + " - " +
             DayEnd.ToString("MMMM dd", CultureInfo.GetCultureInfo("ES-co"));
                }
                else
                {
                    return DayInit.ToString("MMMM dd. hh:mm tt", CultureInfo.GetCultureInfo("ES-co")) + " - " +
               DayEnd.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ES-co"));
                }
            }
        }
        [NotMapped]
        public string DateInitEndLabelProduction
        {
            get
            {
                if (DayEnd.Subtract(DayInit).Days > 0)
                {
                    return DayInit.ToString("MMMM dd. yyyy", CultureInfo.GetCultureInfo("ES-co")) + " Hasta " +
             DayEnd.ToString("MMMM dd. yyyy", CultureInfo.GetCultureInfo("ES-co"));
                }
                else
                {
                    return DayInit.ToString("MMMM dd. yyyy", CultureInfo.GetCultureInfo("ES-co"));
                }
            }
        }

        public string PathDocumentoAdjunto { get; set; }

        [NotMapped]
        public string NameFile { get; set; }
        [JsonIgnore]
        [NotMapped]
        public IFileListEntry FileEntry { get; set; }
        [JsonIgnore]
        [NotMapped]
        public byte[] FileBuffer { get; set; }

        [NotMapped]
        public string TerceroFullName { get; set; }
        
    }
}
