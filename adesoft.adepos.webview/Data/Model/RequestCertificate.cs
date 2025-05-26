using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class RequestCertificate : BaseEntity
    {
        readonly List<string> ListTypeCertificates_ = new List<string>() { "CERTIFICADO LABORAL"};
        public RequestCertificate()
        {

        }
        [Key]
        public long RequestCertificateId { get; set; }
        [NotMapped]
        public string NumberDocument { get; set; }

        public long TerceroId { get; set; }

        public string TypeCertificate { get; set; }

        public DateTime DateRecord { get; set; }


        public string PathDocumentoAdjunto { get; set; }
        [NotMapped]
        public string UrlPathDocumentoAdjunto { get; set; }

        [NotMapped]
        public string NameFile
        {
            get
            {
                if (string.IsNullOrEmpty(PathDocumentoAdjunto))
                {
                    return "";
                }
                else
                {
                    return System.IO.Path.GetFileName(PathDocumentoAdjunto);
                }
            }
        }

        [NotMapped]
        public List<string> ListTypeCertificates
        {
            get
            {
                return ListTypeCertificates_;
            }
        }
        /// <summary>
        /// 1 PENDIENTE , 2 APROBADO , 3 NO APROBADO
        /// </summary>
        public long StateRequestCertificateId { get; set; }

        [NotMapped]
        public string StateLabel
        {
            get
            {
                if (StateRequestCertificateId == 1)
                {
                    return "PENDIENTE";
                }
                else if (StateRequestCertificateId == 2)
                {
                    return "APROBADO";
                }
                else if (StateRequestCertificateId == 3)
                {
                    return "NO APROBADO";
                }
                else
                {
                    return "";
                }
            }
        }


        [NotMapped]
        public string FechaSolicitud
        {
            get
            {
                return DateRecord.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }

        [NotMapped]
        public Tercero Tercero { get; set; }

        [NotMapped]
        public string ColorStateNovedad
        {
            get
            {
                if (StateRequestCertificateId == 1)
                {
                    return "Orange";
                }
                else if (StateRequestCertificateId == 2)
                {
                    return "Green";
                }
                else if (StateRequestCertificateId == 3)
                {
                    return "Red";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
