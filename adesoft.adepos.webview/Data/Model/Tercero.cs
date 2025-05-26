using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Tercero : BaseEntity
    {

        public Tercero()
        {
            Salary = 0;
            LastSalary = 0;
            ListNovedades = new List<NominaNovedad>();
        }

        [Key]
        public long TerceroId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ComercialName { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + (string.IsNullOrEmpty(LastName) ? "" : LastName);
            }
        }
        [NotMapped]
        public string FullNameCode
        {
            get
            {
                return (string.IsNullOrEmpty(CodeEnterprise) ? "" : CodeEnterprise) + " " + FirstName + " " + (string.IsNullOrEmpty(LastName) ? "" : LastName);
            }
        }

        [NotMapped]
        public string FullNameAbrevCode
        {
            get
            {
                return (string.IsNullOrEmpty(CodeEnterprise) ? "" : CodeEnterprise) + " " + (string.IsNullOrEmpty(LastName) ? FirstName : LastName);
            }
        }
        public string NumDocument { get; set; }

        public string Adress1 { get; set; }
        public string Adress2 { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }
        public long TypeTerceroId { get; set; }

        public TypeTercero TypeTercero { get; set; }

        public int TypePersonId { get; set; }
        public TypePerson TypePerson { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public string Photo1 { get; set; }


        public string Photo2 { get; set; }
        [NotMapped]
        public string PhotoBase64 { get; set; }
        [NotMapped]
        public string Photo1Base64 { get; set; }
        [NotMapped]
        public string Photo2Base64 { get; set; }

        #region ContactoFamiliar

        public string ContactNameFamily1 { get; set; }

        public string ContactPhoneFamily1 { get; set; }

        public string ContactAdressFamily1 { get; set; }


        public string ContactParentescoFamily1 { get; set; }

        [NotMapped]
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
        public long EnterpriseId { get; set; }

        /// <summary>
        /// localizaciones genericas , la tabla es LocationGeneric
        /// </summary>
        public long AreaId { get; set; }
        /// <summary>
        /// localizaciones genericas , la tabla es LocationGeneric
        /// </summary>
        public long SucursalId { get; set; }

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


        public string CodeEnterprise { get; set; }
        /// <summary>
        /// Dias pagados vacaciones
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
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
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Entidad promotora de salud, la tabla es LocationGeneric
        /// </summary>
        public long EpsId { get; set; }

        /// <summary>
        /// Administradora de fondos pensionales, la tabla es LocationGeneric
        /// </summary>
        public long AfpId { get; set; }

        /// <summary>
        /// Asegurador riesgos laborales , la tabla es LocationGeneric
        /// </summary>
        public long ArlId { get; set; }

        /// <summary>
        /// Cargo , la tabla es LocationGeneric
        /// </summary>
        public long CargoId { get; set; }
        /// <summary>
        /// Caja de compesacion ,LocationGeneric
        /// </summary>
        public long CajaCompesacionId { get; set; }


        /// <summary>
        /// Fecha de inicio de contrato label
        /// </summary>
        [NotMapped]
        public string LabelDateContractStart
        {
            get
            {
                return DateContractStart.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }
        /// <summary>
        /// Fecha de fin de contrato label
        /// </summary>
        [NotMapped]
        public string LabelDateContractEnd
        {
            get
            {
                if (DateContractEnd == null)
                {
                    return "INDEFINIDO";
                }
                else
                {
                    return DateContractEnd.Value.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
                }
            }
        }
        [NotMapped]
        public string LabelVacationUntil
        {
            get
            {
                return VacationUntil.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }
        [NotMapped]
        public string LabelDateRetirement
        {
            get
            {
                if (DateRetirement != null)
                {
                    return DateRetirement.Value.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public decimal AuxCant { get; set; }

        [NotMapped]
        public List<NominaNovedad> ListNovedades { get; set; }

        /*Campos nuevos*/        

        public long AfcId { get; set; }

        public long AreaIdHomologate { get; set; }

        public long CargoIdHomologate { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal LastSalary { get; set; }

        public string PlaceExpedition { get; set; }

        public DateTime DateExpedition { get; set; }

        public string City { get; set; }

    }
}
