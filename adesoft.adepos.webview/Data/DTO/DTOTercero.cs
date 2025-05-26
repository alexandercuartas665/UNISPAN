using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOTercero
    {
        public long TerceroId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ComercialName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + (string.IsNullOrEmpty(LastName) ? "" : LastName);
            }
        }

        public string NumDocument { get; set; }

        public string Adress1 { get; set; }
        public string Adress2 { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }
        public long TypeTerceroId { get; set; }

        public int TypePersonId { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public string Photo1 { get; set; }


        public string Photo2 { get; set; }

        public string PhotoBase64 { get; set; }

        public string Photo1Base64 { get; set; }

        public string Photo2Base64 { get; set; }

        #region ContactoFamiliar

        public string ContactNameFamily1 { get; set; }

        public string ContactPhoneFamily1 { get; set; }

        public string ContactAdressFamily1 { get; set; }

        public string ContactParentescoFamily1 { get; set; }

        public int[] PhotosChanged { get; set; }

        #endregion

        public string Sexo { get; set; }
        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        public DateTime DateBirth { get; set; }
        /// <summary>
        /// Fecha de ingreso
        /// </summary>
        public DateTime DateIn { get; set; }
        /// <summary>
        /// Ciudad de nacimiento
        /// </summary>
        public string CityBirth { get; set; }
        /// <summary>
        /// NDC
        /// </summary>
        public long NDC { get; set; }

        /// <summary>
        /// localizaciones genericas , la tabla es LocationGeneric
        /// </summary>
        public string EnterpriseName { get; set; }

        public string CodeEnterprise { get; set; }
        /// <summary>
        /// localizaciones genericas , la tabla es LocationGeneric
        /// </summary>
        public string AreaName { get; set; }

        public string CodeArea { get; set; }
        /// <summary>
        /// localizaciones genericas , la tabla es LocationGeneric
        /// </summary>
        public string SucursalName { get; set; }

        public string CodeSucursal { get; set; }
        /// <summary>
        /// Fecha de inicio de contrato
        /// </summary>
        public DateTime DateContractStart { get; set; }
        /// <summary>
        /// Fecha de fin de contrato
        /// </summary>
        public DateTime? DateContractEnd { get; set; }

        /// <summary>
        /// Vacaciones hasta
        /// </summary>
        public DateTime VacationUntil { get; set; }
        /// <summary>
        /// Dias pagados vacaciones
        /// </summary>
        public decimal DayPaysVacations { get; set; }

        /// <summary>
        /// Fecha de retiro
        /// </summary>
        public DateTime? DateRetirement { get; set; }

        /// <summary>
        /// razon del retiro
        /// </summary>
        public string ReasonRetirement { get; set; }

        /// <summary>
        /// Salario
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Entidad promotora de salud, la tabla es LocationGeneric
        /// </summary>
        public string EpsName { get; set; }


        public string EpsCode { get; set; }
        /// <summary>
        /// Administradora de fondos pensionales, la tabla es LocationGeneric
        /// </summary>
        public string AfpName { get; set; }
        public string AfpCode { get; set; }
        /// <summary>
        /// Asegurador riesgos laborales , la tabla es LocationGeneric
        /// </summary>
        public string ArlName { get; set; }
        public string ArlCode { get; set; }
        /// <summary>
        /// Cargo , la tabla es LocationGeneric
        /// </summary>
        public string CargoName { get; set; }

        public string CargoCode { get; set; }
        /// <summary>
        /// Caja de compesacion ,LocationGeneric
        /// </summary>
        public string CajaCompesacionName { get; set; }

        public string CajaCode { get; set; }
    }
}
 