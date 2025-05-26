using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adesoft.adepos.Extensions;
using System.IO;
using adesoft.adepos.webview.Bussines;
using ExcelDataReader;
using adesoft.adepos.webview.Data.DTO;
using System.Globalization;
using adesoft.adepos.webview.Data.Model.Simex;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NominaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        public static List<DTOHREmployFilter> employFilters = new List<DTOHREmployFilter>();

        public NominaController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        private string ToString(IExcelDataReader reader, int column)
        {
            var value = reader.GetValue(column);
            if (value is null)
                return "";

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    break;
                case TypeCode.Int16:
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.Double:
                    return reader.GetDouble(column).ToString();
                case TypeCode.Decimal:
                    return reader.GetDecimal(column).ToString();
                case TypeCode.DateTime:
                    return reader.GetDateTime(column).ToString();
                case TypeCode.String:
                    return reader.GetString(column);
                default:
                    break;
            }

            return "";
        }

        public bool UploadNewness(Parameter parameter)
        {
            bool readHeader = false;

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    while (reader.Read()) //Each ROW
                    {
                        if (!readHeader)
                        {
                            readHeader = true;
                            continue;
                        }

                        CodeNovedad novedad = new CodeNovedad();

                        for (int column = 0; column < 5; column++)
                        {
                            switch (column)
                            {
                                case 0:
                                    {
                                        var value = this.ToString(reader, 0);
                                        if(string.IsNullOrEmpty(value))
                                        {
                                            return true;
                                        }

                                        novedad.Syncode = value;
                                        break;
                                    }

                                case 1:
                                    {
                                        var value = this.ToString(reader, 1);
                                        novedad.NovedadName = value;
                                        break;
                                    }

                                case 2:
                                    {
                                        var value = this.ToString(reader, 2);
                                        novedad.NovedadAbrev = value;
                                        break;
                                    }

                                case 3:
                                    {
                                        var value = this.ToString(reader, 3);
                                        novedad.TypeNovedadName = value;
                                        break;
                                    }

                                case 4:
                                    {
                                        var value = this.ToString(reader, 4);
                                        novedad.PlaneType = value;
                                        break;
                                    }

                                default:
                                    break;
                            }
                        }

                        var find = _dbcontext.CodeNovedads
                            .Where(n => n.Syncode == novedad.Syncode)
                            .FirstOrDefault();
                        if (find is null)
                        {
                            _dbcontext.CodeNovedads.Add(novedad);                            
                        }
                        else
                        {
                            find.NovedadName = novedad.NovedadName;
                            find.NovedadAbrev = novedad.NovedadAbrev;
                            find.TypeNovedadName = novedad.TypeNovedadName;
                            find.PlaneType = novedad.PlaneType;
                            _dbcontext.CodeNovedads.Update(find);
                        }

                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                } while (reader.NextResult()); //Move to NEXT SHEET                
            }

            return true;
        }

        /*public bool UploadEmployes(Parameter parameter)
        {
            bool readHeader = false;
            int row = 0, row1 = 0;

            try
            {
                Stream stream = new MemoryStream(parameter.FileBuffer);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read()) //Each ROW
                        {
                            row1++;
                            if (row1 == 140)
                                row1 = 140;

                            if (!readHeader)
                            {
                                if (row < 4)
                                {
                                    row++;
                                    continue;
                                }
                                readHeader = true;
                            }

                            Tercero tercero = new Tercero();

                            for (int column = 0; column < 145; column++)
                            {
                                switch (column)
                                {
                                    case 0:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (string.IsNullOrEmpty(value))
                                            {
                                                return true;
                                            }

                                            if (value.Equals("1113671668"))
                                                tercero.NumDocument = "1113671668";

                                            tercero.NumDocument = value;
                                            break;
                                        }

                                    case 1:
                                        {
                                            var value = this.ToString(reader, column);
                                            var names = value.Split(" ");
                                            switch (names.Length)
                                            {
                                                case 4:
                                                    tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                    tercero.FirstName = string.Format("{0} {1}", names[2], names[3]);
                                                    break;

                                                case 3:
                                                    tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                    tercero.FirstName = string.Format("{0}", names[2]);
                                                    break;

                                                case 2:
                                                    tercero.LastName = string.Format("{0}", names[0]);
                                                    tercero.FirstName = string.Format("{0}", names[1]);
                                                    break;

                                                default:
                                                    if (names.Length > 4)
                                                    {
                                                        tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                        for(int i = 2; i < names.Length; i++)
                                                        {
                                                            tercero.FirstName = string.IsNullOrEmpty(tercero.FirstName) ? names[i] : tercero.FirstName + " " + names[i];
                                                        }
                                                    }                                                    
                                                    break;
                                            }
                                            break;
                                        }

                                    case 3:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.PlaceExpedition = value;
                                            break;
                                        }

                                    case 4:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.Sexo = value;
                                            break;
                                        }

                                    case 5:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.Adress1 = value;
                                            break;
                                        }

                                    case 6:
                                        {                                            
                                            try
                                            {                                                
                                                if(!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    tercero.DateIn = reader.GetDateTime(column);
                                                }                                                
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    tercero.DateIn = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }
                                                break;
                                            }
                                        }

                                    case 10:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.City = value;
                                            break;
                                        }

                                    case 12:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.Phone1 = value;
                                            break;
                                        }

                                    case 14:
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    tercero.DateBirth = reader.GetDateTime(column);
                                                }
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    tercero.DateBirth = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }
                                                break;
                                            }
                                        }

                                    case 16:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.CityBirth = value;
                                            break;
                                        }

                                    case 17:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var company = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "EMPRESA")
                                                .FirstOrDefault();
                                                if (!(company is null))
                                                {
                                                    tercero.EnterpriseId = company.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 20:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var area = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "AREA")
                                                .FirstOrDefault();
                                                if (!(area is null))
                                                {
                                                    tercero.AreaId = area.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 22:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var sucursal = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "SUCURSAL")
                                                .FirstOrDefault();
                                                if (!(sucursal is null))
                                                {
                                                    tercero.SucursalId = sucursal.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 29:
                                        {
                                            try
                                            {
                                                if(!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    tercero.DateContractStart = reader.GetDateTime(column);
                                                }                                                                                                
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    tercero.DateContractStart = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }
                                                break;
                                            }
                                        }

                                    case 30:
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    tercero.DateContractEnd = reader.GetDateTime(column);
                                                }
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if (!string.IsNullOrEmpty(value))
                                                {
                                                    if (value.Contains("99/99/9999"))
                                                        tercero.DateContractEnd = DateTime.MaxValue;
                                                    else
                                                        tercero.DateContractEnd = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }
                                                break;
                                            }
                                        }

                                    case 31:
                                        {                                            
                                            try
                                            {
                                                if(!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    tercero.VacationUntil = reader.GetDateTime(column);
                                                }                                                
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if(!string.IsNullOrEmpty(value))
                                                {
                                                    tercero.VacationUntil = DateTime.ParseExact(value, "dd/MM/yyyy", null);                                                    
                                                }
                                                break;
                                            }
                                        }

                                    case 32:
                                        {
                                            var value = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                            tercero.DayPaysVacations = value;
                                            break;
                                        }

                                    case 36:
                                        {
                                            var value = this.ToString(reader, column);
                                            if(!string.IsNullOrEmpty(value))
                                            {
                                                var afc = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "AFC")
                                                .FirstOrDefault();
                                                if(!(afc is null))
                                                {
                                                    tercero.AfcId = afc.LocationGenericId;
                                                }
                                            }                                            
                                            break;
                                        }

                                    case 38:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.IsActive = !value.Equals("R");
                                            break;
                                        }

                                    case 39:
                                        {
                                            try
                                            {
                                                if(!(string.IsNullOrEmpty(ToString(reader, column))))
                                                {
                                                    tercero.DateRetirement = reader.GetDateTime(column);
                                                }                                                
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if(!string.IsNullOrEmpty(value))
                                                {
                                                    tercero.DateRetirement = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }                                                
                                                break;
                                            }
                                        }

                                    case 40:
                                        {
                                            if (!(string.IsNullOrEmpty(ToString(reader, column))))
                                            {                                                
                                                tercero.ReasonRetirement = ToString(reader, column);
                                            }
                                            break;
                                        }

                                    case 45:
                                        {
                                            var value = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                            tercero.Salary = value;
                                            break;
                                        }

                                    case 48:
                                        {
                                            var value = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                            tercero.LastSalary = value;
                                            break;
                                        }

                                    case 50:
                                        {
                                            var value = this.ToString(reader, column);
                                            if(!(string.IsNullOrEmpty(value)))
                                            {
                                                var eps = _dbcontext.LocationGenerics
                                                    .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "EPS")
                                                    .FirstOrDefault();
                                                if(!(eps is null))
                                                {
                                                    tercero.EpsId = eps.LocationGenericId;
                                                }
                                            }                                            
                                            break;
                                        }

                                    case 52:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!(string.IsNullOrEmpty(value)))
                                            {
                                                var afp = _dbcontext.LocationGenerics
                                                    .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "AFP")
                                                    .FirstOrDefault();
                                                if (!(afp is null))
                                                {
                                                    tercero.AfpId = afp.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 69:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!(string.IsNullOrEmpty(value)))
                                            {
                                                var cargo = _dbcontext.LocationGenerics
                                                    .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "CARGO")
                                                    .FirstOrDefault();
                                                if (!(cargo is null))
                                                {
                                                    tercero.CargoId = cargo.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 77:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!(string.IsNullOrEmpty(value)))
                                            {
                                                var caja = _dbcontext.LocationGenerics
                                                    .Where(l => l.SyncCode == value
                                                            && l.TypeLocation == "CAJA")
                                                    .FirstOrDefault();
                                                if (!(caja is null))
                                                {
                                                    tercero.CajaCompesacionId = caja.LocationGenericId;
                                                }
                                            }
                                            break;
                                        }

                                    case 87:
                                        {
                                            if (!(string.IsNullOrEmpty(ToString(reader, column))))
                                            {
                                                tercero.Email = ToString(reader, column);
                                            }
                                            break;
                                        }

                                    case 111:
                                        {
                                            try
                                            {
                                                if(!string.IsNullOrEmpty(ToString(reader, column)))
                                                {
                                                    var value = reader.GetDateTime(column);
                                                    tercero.DateExpedition = value;
                                                }                                                
                                                break;
                                            }
                                            catch (Exception)
                                            {
                                                var value = ToString(reader, column);
                                                if(!(string.IsNullOrEmpty(value)))
                                                {
                                                    tercero.DateExpedition = DateTime.ParseExact(value, "dd/MM/yyyy", null);
                                                }
                                                break;
                                            }
                                        }

                                    default:
                                        break;
                                }
                            }

                            var find = _dbcontext.Terceros
                                .Where(t => t.NumDocument == tercero.NumDocument)
                                .FirstOrDefault();
                            if (find is null)
                            {
                                tercero.TypePersonId = 1; // Persona Natural
                                tercero.TypeTerceroId = 3; // Empleado
                                _dbcontext.Terceros.Add(tercero);
                            }
                            else
                            {
                                find.NumDocument = tercero.NumDocument;
                                find.FirstName = tercero.FirstName;
                                find.LastName = tercero.LastName;
                                find.EnterpriseId = tercero.EnterpriseId;
                                find.AreaId = tercero.AreaId;
                                find.AreaIdHomologate = tercero.AreaIdHomologate;
                                find.CargoIdHomologate = tercero.CargoIdHomologate;
                                find.CargoId = tercero.CargoId;
                                find.AfcId = tercero.AfcId;
                                find.EpsId = tercero.EpsId;
                                find.AfpId = tercero.AfpId;
                                find.Salary = tercero.Salary;
                                find.LastSalary = tercero.LastSalary;
                                find.DateExpedition = tercero.DateExpedition;
                                find.PlaceExpedition = tercero.PlaceExpedition;
                                find.Adress1 = tercero.Adress1;
                                find.City = tercero.City;
                                find.Phone1 = tercero.Phone1;
                                find.IsActive = tercero.IsActive;
                                find.DateRetirement = tercero.DateRetirement;
                                find.ReasonRetirement = tercero.ReasonRetirement;
                                find.DateBirth = tercero.DateBirth;
                                find.DateContractEnd = tercero.DateContractEnd;
                                find.DateContractStart = tercero.DateContractStart;
                                find.DateIn = tercero.DateIn;
                                find.Sexo = tercero.Sexo;
                                find.VacationUntil = tercero.VacationUntil;
                                find.CajaCompesacionId = tercero.CajaCompesacionId;
                                find.Email = tercero.Email;

                                _dbcontext.Terceros.Update(find);
                            }

                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();
                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET                
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }*/

        public bool UploadEmployes(Parameter parameter)
        {
            bool readHeader = false;
            int row = 0;

            try
            {
                Stream stream = new MemoryStream(parameter.FileBuffer);
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream) //Each ROW
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');                        

                        if (!readHeader)
                        {
                            if (row < 4)
                            {
                                row++;
                                continue;
                            }
                            readHeader = true;
                        }

                        if (values.Length < 145)
                            throw new Exception("Ha ocurrido un error, estructura de archivo invalido.");

                        Tercero tercero = new Tercero();

                        for (int column = 0; column < values.Length; column++)
                        {
                            switch (column)
                            {
                                case 0:
                                    {
                                        var value = values[column];
                                        if (string.IsNullOrEmpty(value))
                                        {
                                            return true;
                                        }

                                        if (value.Equals("1113671668"))
                                            tercero.NumDocument = "1113671668";

                                        tercero.NumDocument = value;
                                        break;
                                    }

                                case 1:
                                    {
                                        var value = values[column];
                                        var names = value.Split(" ");
                                        switch (names.Length)
                                        {
                                            case 4:
                                                tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                tercero.FirstName = string.Format("{0} {1}", names[2], names[3]);
                                                break;

                                            case 3:
                                                tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                tercero.FirstName = string.Format("{0}", names[2]);
                                                break;

                                            case 2:
                                                tercero.LastName = string.Format("{0}", names[0]);
                                                tercero.FirstName = string.Format("{0}", names[1]);
                                                break;

                                            default:
                                                if (names.Length > 4)
                                                {
                                                    tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                    for (int i = 2; i < names.Length; i++)
                                                    {
                                                        tercero.FirstName = string.IsNullOrEmpty(tercero.FirstName) ? names[i] : tercero.FirstName + " " + names[i];
                                                    }
                                                }
                                                break;
                                        }
                                        break;
                                    }

                                case 3:
                                    {
                                        var value = values[column];
                                        tercero.PlaceExpedition = value;
                                        break;
                                    }

                                case 4:
                                    {
                                        var value = values[column];
                                        tercero.Sexo = value;
                                        break;
                                    }

                                case 5:
                                    {
                                        var value = values[column];
                                        tercero.Adress1 = value;
                                        break;
                                    }

                                case 6:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.DateIn = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                case 10:
                                    {
                                        var value = values[column];
                                        tercero.City = value;
                                        break;
                                    }

                                case 12:
                                    {
                                        var value = values[column];
                                        tercero.Phone1 = value;
                                        break;
                                    }

                                case 14:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.DateBirth = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                case 16:
                                    {
                                        var value = values[column];
                                        tercero.CityBirth = value;
                                        break;
                                    }

                                case 17:
                                    {
                                        var value = values[column];
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            var company = _dbcontext.LocationGenerics
                                            .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "EMPRESA")
                                            .FirstOrDefault();
                                            if (!(company is null))
                                            {
                                                tercero.EnterpriseId = company.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 20:
                                    {
                                        var value = values[column];
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            var area = _dbcontext.LocationGenerics
                                            .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "AREA")
                                            .FirstOrDefault();
                                            if (!(area is null))
                                            {
                                                tercero.AreaId = area.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 22:
                                    {
                                        var value = values[column];
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            var sucursal = _dbcontext.LocationGenerics
                                            .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "SUCURSAL")
                                            .FirstOrDefault();
                                            if (!(sucursal is null))
                                            {
                                                tercero.SucursalId = sucursal.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 29:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.DateContractStart = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                case 30:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]) && !values[column].Equals("99/99/9999"))
                                        {
                                            var value = values[column];
                                            tercero.DateContractEnd = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                case 31:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.VacationUntil = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                /*case 32:
                                    {
                                        var value = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                        tercero.DayPaysVacations = value;
                                        break;
                                    }*/

                                case 36:
                                    {
                                        var value = values[column];
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            var afc = _dbcontext.LocationGenerics
                                            .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "AFC")
                                            .FirstOrDefault();
                                            if (!(afc is null))
                                            {
                                                tercero.AfcId = afc.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 38:
                                    {
                                        var value = values[column];
                                        tercero.IsActive = value.Equals("A");
                                        break;
                                    }

                                case 39:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.DateRetirement = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                case 40:
                                    {
                                        if (!(string.IsNullOrEmpty(values[column])))
                                        {
                                            tercero.ReasonRetirement = values[column];
                                        }
                                        break;
                                    }

                                case 45:
                                    {
                                        var value = values[column];
                                        tercero.Salary = decimal.Parse(value.Replace('.', ','));
                                        break;
                                    }

                                case 48:
                                    {
                                        var value = values[column];
                                        tercero.LastSalary = decimal.Parse(value.Replace('.', ','));
                                        break;
                                    }

                                case 50:
                                    {
                                        var value = values[column];
                                        if (!(string.IsNullOrEmpty(value)))
                                        {
                                            var eps = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "EPS")
                                                .FirstOrDefault();
                                            if (!(eps is null))
                                            {
                                                tercero.EpsId = eps.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 52:
                                    {
                                        var value = values[column];
                                        if (!(string.IsNullOrEmpty(value)))
                                        {
                                            var afp = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "AFP")
                                                .FirstOrDefault();
                                            if (!(afp is null))
                                            {
                                                tercero.AfpId = afp.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 69:
                                    {
                                        var value = values[column];
                                        if (!(string.IsNullOrEmpty(value)))
                                        {
                                            var cargo = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "CARGO")
                                                .FirstOrDefault();
                                            if (!(cargo is null))
                                            {
                                                tercero.CargoId = cargo.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 77:
                                    {
                                        var value = values[column];
                                        if (!(string.IsNullOrEmpty(value)))
                                        {
                                            var caja = _dbcontext.LocationGenerics
                                                .Where(l => l.SyncCode == value
                                                        && l.TypeLocation == "CAJA")
                                                .FirstOrDefault();
                                            if (!(caja is null))
                                            {
                                                tercero.CajaCompesacionId = caja.LocationGenericId;
                                            }
                                        }
                                        break;
                                    }

                                case 87:
                                    {
                                        if (!(string.IsNullOrEmpty(values[column])))
                                        {
                                            tercero.Email = values[column];
                                        }
                                        break;
                                    }

                                case 111:
                                    {
                                        if (!string.IsNullOrEmpty(values[column]))
                                        {
                                            var value = values[column];
                                            tercero.DateExpedition = DateTime.ParseExact(value, "d/MM/yyyy", null);
                                        }
                                        break;
                                    }

                                default:
                                    break;
                            }
                        }

                        var find = _dbcontext.Terceros
                            .Where(t => t.NumDocument == tercero.NumDocument && t.DateContractStart == tercero.DateContractStart)
                            .FirstOrDefault();

                        if (find is null)
                        {
                            tercero.TypePersonId = 1; // Persona Natural
                            tercero.TypeTerceroId = 3; // Empleado
                            _dbcontext.Terceros.Add(tercero);
                        }
                        else
                        {
                            find.NumDocument = tercero.NumDocument;
                            find.FirstName = tercero.FirstName;
                            find.LastName = tercero.LastName;
                            find.EnterpriseId = tercero.EnterpriseId;
                            find.AreaId = tercero.AreaId;
                            find.CargoId = tercero.CargoId;
                            find.AfcId = tercero.AfcId;
                            find.EpsId = tercero.EpsId;
                            find.AfpId = tercero.AfpId;
                            find.Salary = tercero.Salary;
                            find.LastSalary = tercero.LastSalary;
                            find.DateExpedition = tercero.DateExpedition;
                            find.PlaceExpedition = tercero.PlaceExpedition;
                            find.Adress1 = tercero.Adress1;
                            find.City = tercero.City;
                            find.Phone1 = tercero.Phone1;
                            find.IsActive = tercero.IsActive;
                            find.DateRetirement = tercero.DateRetirement;
                            find.ReasonRetirement = tercero.ReasonRetirement;
                            find.DateBirth = tercero.DateBirth;
                            find.DateContractEnd = tercero.DateContractEnd;
                            find.DateContractStart = tercero.DateContractStart;
                            find.DateIn = tercero.DateIn;
                            find.Sexo = tercero.Sexo;
                            find.VacationUntil = tercero.VacationUntil;
                            find.CajaCompesacionId = tercero.CajaCompesacionId;
                            find.Email = tercero.Email;

                            _dbcontext.Terceros.Update(find);
                        }

                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                }

                this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                {
                    Module = "HR-CG1",
                    LastUpdateModule_At = DateTime.Now
                });
                this._dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public bool UploadExternalEmployes(Parameter parameter)
        {
            bool readHeader = false;            

            try
            {
                Stream stream = new MemoryStream(parameter.FileBuffer);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read()) //Each ROW
                        {
                            if (!readHeader)
                            {                                
                                readHeader = true;
                                continue;
                            }

                            Tercero tercero = new Tercero();

                            for (int column = 0; column < 21; column++)
                            {
                                switch (column)
                                {
                                    case 0:
                                        {
                                            var value = this.ToString(reader, column);

                                            var company = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "EMPRESA")
                                                .FirstOrDefault();

                                            if (!(company is null))
                                                tercero.EnterpriseId = company.LocationGenericId;

                                            break;
                                        }

                                    case 1:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (string.IsNullOrEmpty(value))
                                            {
                                                return true;
                                            }

                                            tercero.NumDocument = value;
                                            break;
                                        }

                                    case 2:
                                        {
                                            var value = this.ToString(reader, column);
                                            var names = value.Split(" ");
                                            switch (names.Length)
                                            {
                                                case 4:
                                                    tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                    tercero.FirstName = string.Format("{0} {1}", names[2], names[3]);
                                                    break;

                                                case 3:
                                                    tercero.LastName = string.Format("{0} {1}", names[0], names[1]);
                                                    tercero.FirstName = string.Format("{0}", names[2]);
                                                    break;

                                                case 2:
                                                    tercero.LastName = string.Format("{0}", names[0]);
                                                    tercero.FirstName = string.Format("{0}", names[1]);
                                                    break;

                                                default:
                                                    break;
                                            }
                                            break;
                                        }

                                    case 3:
                                        {
                                            if (!string.IsNullOrEmpty(ToString(reader, column)))
                                            {
                                                tercero.DateBirth = reader.GetDateTime(column);
                                            }
                                            break;
                                        }

                                    case 4:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.City = value;
                                            break;
                                        }

                                    case 5:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.Adress1 = value;
                                            break;
                                        }

                                    case 6:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.Phone1 = value;
                                            break;
                                        }

                                    case 7:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.IsActive = value.Equals("A");
                                            break;
                                        }

                                    case 8:
                                        {
                                            if (!string.IsNullOrEmpty(ToString(reader, column)))
                                            {
                                                tercero.DateIn = reader.GetDateTime(column);
                                                tercero.DateContractStart = reader.GetDateTime(column);
                                            }
                                            break;
                                        }

                                    case 9:
                                        {
                                            if (!(string.IsNullOrEmpty(ToString(reader, column))))
                                            {
                                                tercero.DateRetirement = reader.GetDateTime(column);
                                            }
                                            break;
                                        }

                                    case 10:
                                        {
                                            var value = this.ToString(reader, column);
                                            tercero.ReasonRetirement = value;
                                            break;
                                        }

                                    case 11:
                                        {
                                            var value = (decimal)reader.GetDouble(column);
                                            tercero.Salary = value;
                                            break;
                                        }

                                    case 12:
                                        {
                                            var value = (decimal)reader.GetDouble(column);
                                            tercero.LastSalary = value;
                                            break;
                                        }

                                    case 13:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var area = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "AREA")
                                                .FirstOrDefault();
                                                if (!(area is null))
                                                {
                                                    tercero.AreaId = area.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 14:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var cargo = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "CARGO")
                                                .FirstOrDefault();
                                                if (!(cargo is null))
                                                {
                                                    tercero.CargoId = cargo.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 15:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var afp = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "AFP")
                                                .FirstOrDefault();
                                                if (!(afp is null))
                                                {
                                                    tercero.AfpId = afp.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 16:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var eps = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "EPS")
                                                .FirstOrDefault();
                                                if (!(eps is null))
                                                {
                                                    tercero.EpsId = eps.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 17:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var caja = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "CAJA")
                                                .FirstOrDefault();
                                                if (!(caja is null))
                                                {
                                                    tercero.CajaCompesacionId = caja.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 18:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var afc = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "AFC")
                                                .FirstOrDefault();
                                                if (!(afc is null))
                                                {
                                                    tercero.AfcId = afc.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 19:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var centrohm = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "CENTROHM")
                                                .FirstOrDefault();
                                                if (!(centrohm is null))
                                                {
                                                    tercero.AreaIdHomologate = centrohm.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    case 20:
                                        {
                                            var value = this.ToString(reader, column);
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var cargohm = _dbcontext.LocationGenerics
                                                .Where(l => l.Description == value
                                                            && l.TypeLocation == "CARGOHM")
                                                .FirstOrDefault();
                                                if (!(cargohm is null))
                                                {
                                                    tercero.CargoIdHomologate = cargohm.LocationGenericId;
                                                }
                                            }

                                            break;
                                        }

                                    default:
                                        break;
                                }
                            }

                            var find = _dbcontext.Terceros
                            .Where(t => t.NumDocument == tercero.NumDocument && t.DateContractStart == tercero.DateContractStart)
                            .FirstOrDefault();

                            if (find is null)
                            {
                                tercero.TypePersonId = 1; // Persona Natural
                                tercero.TypeTerceroId = 3; // Empleado
                                _dbcontext.Terceros.Add(tercero);
                            }
                            else
                            {
                                find.NumDocument = tercero.NumDocument;
                                find.FirstName = tercero.FirstName;
                                find.LastName = tercero.LastName;
                                find.EnterpriseId = tercero.EnterpriseId;
                                find.AreaId = tercero.AreaId;
                                find.AreaIdHomologate = tercero.AreaIdHomologate;
                                find.CargoId = tercero.CargoId;
                                find.CargoIdHomologate = tercero.CargoIdHomologate;
                                find.AfcId = tercero.AfcId;
                                find.EpsId = tercero.EpsId;
                                find.AfpId = tercero.AfpId;
                                find.Salary = tercero.Salary;
                                find.LastSalary = tercero.LastSalary;
                                find.DateExpedition = tercero.DateExpedition;
                                find.PlaceExpedition = tercero.PlaceExpedition;
                                find.Adress1 = tercero.Adress1;
                                find.City = tercero.City;
                                find.Phone1 = tercero.Phone1;
                                find.IsActive = tercero.IsActive;
                                find.DateRetirement = tercero.DateRetirement;
                                find.ReasonRetirement = tercero.ReasonRetirement;
                                find.DateBirth = tercero.DateBirth;
                                find.DateContractEnd = tercero.DateContractEnd;
                                find.DateContractStart = tercero.DateContractStart;
                                find.DateIn = tercero.DateIn;
                                find.Sexo = tercero.Sexo;
                                find.VacationUntil = tercero.VacationUntil;
                                find.CajaCompesacionId = tercero.CajaCompesacionId;
                                find.Email = tercero.Email;

                                _dbcontext.Terceros.Update(find);
                            }

                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();
                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET                
                }

                this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                {
                    Module = "HR-External",
                    LastUpdateModule_At = DateTime.Now
                });
                this._dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public NominaProgramation Create(NominaProgramation nominaprogramation)
        {

            if (_dbcontext.NominaProgramations.Where(x => x.DayInit <= nominaprogramation.DayInit
            && x.DayEnd >= nominaprogramation.DayInit).Count() > 0)
            {
                nominaprogramation.MessageResponse = "Ya existe una programacion de liquidacion de nomina que se cruza con las fechas de esta liquidacion por favor revisar.";
                nominaprogramation.TransactionIsOk = false;
                return nominaprogramation;
            }
            else if (_dbcontext.NominaProgramations.Where(x => x.DayInit <= nominaprogramation.DayEnd
            && x.DayEnd >= nominaprogramation.DayEnd).Count() > 0)
            {
                nominaprogramation.TransactionIsOk = false;
                nominaprogramation.MessageResponse = "Ya existe una programacion de liquidacion de nomina que se cruza con las fechas de esta liquidacion por favor revisar.";
                return nominaprogramation;
            }
            else
            {
                _dbcontext.NominaProgramations.Add(nominaprogramation);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }

            return nominaprogramation;
        }

        public NominaProgramation Update(NominaProgramation nominaprogramation)
        {
            if (_dbcontext.NominaProgramations.Where(x => x.DayInit <= nominaprogramation.DayInit
              && x.DayEnd >= nominaprogramation.DayInit && x.NominaProgramationId != nominaprogramation.NominaProgramationId).Count() > 0)
            {
                nominaprogramation.MessageResponse = "Ya existe una programacion de liquidacion de nomina que se cruza con las fechas de esta liquidacion por favor revisar.";
                nominaprogramation.TransactionIsOk = false;
                return nominaprogramation;
            }
            else if (_dbcontext.NominaProgramations.Where(x => x.DayInit <= nominaprogramation.DayEnd
            && x.DayEnd >= nominaprogramation.DayEnd && x.NominaProgramationId != nominaprogramation.NominaProgramationId).Count() > 0)
            {
                nominaprogramation.TransactionIsOk = false;
                nominaprogramation.MessageResponse = "Ya existe una programacion de liquidacion de nomina que se cruza con las fechas de esta liquidacion por favor revisar.";
                return nominaprogramation;
            }
            NominaProgramation find = _dbcontext.NominaProgramations.Where(x => x.NominaProgramationId == nominaprogramation.NominaProgramationId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<NominaProgramation>(nominaprogramation).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }

            return nominaprogramation;
        }

        public NominaProgramation SelectById(NominaProgramation nominaprogramation)
        {
            NominaProgramation find = _dbcontext.NominaProgramations.Where(x => x.NominaProgramationId == nominaprogramation.NominaProgramationId).FirstOrDefault();

            return find;
        }

        public List<NominaProgramation> selectAll(NominaProgramation NominaProgramation)
        {
            return _dbcontext.NominaProgramations.ToList();
        }

        public NominaNovedad Create(NominaNovedad nominanovedad)
        {
            if (nominanovedad.TransOption == 1 || nominanovedad.TransOption == 0)
            {
                _dbcontext.NominaNovedads.Add(nominanovedad);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                CreateOrUpdateFile(nominanovedad);
                _dbcontext.Entry<NominaNovedad>(nominanovedad).State = EntityState.Modified;
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();                

                return nominanovedad;
            }
            else if (nominanovedad.TransOption == 2)
            {
                _dbcontext.NominaNovedads.Add(nominanovedad);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                return nominanovedad;
            }
            else
            {
                return nominanovedad;
            }
        }

        public NominaProgramation GeneratePlanos(NominaProgramation model)
        {
            PreliquidationBussines bussine = new PreliquidationBussines(_dbcontext, _configuration);
            return bussine.GeneratePlanos(model);
        }

        public void CreateOrUpdateFile(NominaNovedad nominanov)
        {
            if (nominanov.FileEntry != null)
            {
                string pathapp = Directory.GetCurrentDirectory();
                string directory = "/wwwroot/FilesApp/DocNovedades/" + nominanov.NominaNovedadId;
                nominanov.PathDocumentoAdjunto = directory + "/" + nominanov.NameFile;
                if (!Directory.Exists(pathapp + directory))
                {
                    Directory.CreateDirectory(pathapp + directory);
                }
                //var reader = new System.IO.StreamReader(nominanov.FileEntry.Data);
                //byte[] arraybytes;
                //Task<string> task = reader.ReadToEndAsync();
                //task.Wait();
                //string t = task.Result;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    Task task = reader.BaseStream.CopyToAsync(ms);
                //    task.wa();
                //    arraybytes = reader..ToArray();
                //}
                FileStream filestream = System.IO.File.Create(pathapp + nominanov.PathDocumentoAdjunto);
                filestream.Write(nominanov.FileBuffer, 0, nominanov.FileBuffer.Count());
                filestream.Close();
            }
        }

        public NominaNovedad Update(NominaNovedad nominanovedad)
        {
            if (nominanovedad.TransOption == 1 || nominanovedad.TransOption == 0)
            {
                NominaNovedad find = _dbcontext.NominaNovedads.Where(x => x.NominaNovedadId == nominanovedad.NominaNovedadId).FirstOrDefault();
                if (find != null)
                {
                    _dbcontext.Entry<NominaNovedad>(nominanovedad).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                    CreateOrUpdateFile(nominanovedad);
                    _dbcontext.Entry<NominaNovedad>(nominanovedad).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                    var tercero = _dbcontext.Terceros
                        .Where(x => x.TerceroId == nominanovedad.TerceroId)
                        .FirstOrDefault();

                    var operario = _dbcontext.Terceros
                        .Where(x => x.NumDocument == tercero.NumDocument
                        && x.TypeTerceroId == 5)
                        .FirstOrDefault();

                    if (!(operario is null) && (operario.TypeTerceroId.Equals(5)))
                    {
                        nominanovedad.NominaNovedadId = 0;
                        nominanovedad.NominaProgramationId = 0;
                        nominanovedad.HoursNovedad2 = (decimal)nominanovedad.HoursNovedad.TotalHours;
                        _dbcontext.NominaNovedads.Add(nominanovedad);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                }
                return nominanovedad;
            }
            else if (nominanovedad.TransOption == 2)
            {
                NominaNovedad find = _dbcontext.NominaNovedads.Where(x => x.NominaNovedadId == nominanovedad.NominaNovedadId).FirstOrDefault();
                if (find != null)
                {
                    _dbcontext.Entry<NominaNovedad>(nominanovedad).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }
                return nominanovedad;
            }
            else
            {
                return nominanovedad;
            }
        }

        public NominaNovedad SelectById(NominaNovedad nominanovedad)
        {
            NominaNovedad find = _dbcontext.NominaNovedads.Where(x => x.NominaNovedadId == nominanovedad.NominaNovedadId).FirstOrDefault();
            find.CodeNovedad = _dbcontext.CodeNovedads.Where(x => x.CodeNovedadId == find.CodeNovedadId).First();
            find.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == find.TerceroId).First();
            if (find.PathDocumentoAdjunto != null)
                find.NameFile = System.IO.Path.GetFileName(find.PathDocumentoAdjunto);
            return find;
        }

        public List<NominaNovedad> selectAll(NominaNovedad nominanovedad)
        {
            if (nominanovedad.TransOption == 0 || nominanovedad.TransOption == 1)
            {
                return _dbcontext.NominaNovedads.Where(x => x.StateNovedad != 3).ToList();
            }
            else if (nominanovedad.TransOption == 2)//por id de programacion de liquidacion
            {
                List<NominaNovedad> novedades = _dbcontext.NominaNovedads.Where(x => x.NominaProgramationId == nominanovedad.NominaProgramationId
                 && x.StateNovedad != 3).ToList();
                foreach (NominaNovedad nom in novedades)
                {
                    nom.CodeNovedad = _dbcontext.CodeNovedads.Where(x => x.CodeNovedadId == nom.CodeNovedadId).First();
                    nom.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == nom.TerceroId).First();
                }
                return novedades;
            }
            else if (nominanovedad.TransOption == 3)//por rango de fechas y para produccion
            {
                List<NominaNovedad> novedades = _dbcontext.NominaNovedads
                    .Where(x => ((x.DayInit >= nominanovedad.DayInit && x.DayInit <= nominanovedad.DayEnd)
                            || (x.DayEnd >= nominanovedad.DayInit && x.DayEnd <= nominanovedad.DayEnd))
                            && x.NominaProgramationId == 0).ToList(); 
                            //&& x.CodeNovedadId==nominanovedad.CodeNovedadId).ToList();
                foreach (NominaNovedad nom in novedades)
                {
                    //   nom.CodeNovedad = _dbcontext.CodeNovedads.Where(x => x.CodeNovedadId == nom.CodeNovedadId).First();
                    nom.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == nom.TerceroId).First();
                }
                return novedades;
            }
            else
            {
                return _dbcontext.NominaNovedads.ToList();
            }
        }

        public List<CodeNovedad> selectAll(CodeNovedad codeNovedad)
        {
            return _dbcontext.CodeNovedads.ToList();
        }

        public void AddEmployFilter(DTOHREmployFilter employFilter)
        {
            employFilters.Add(employFilter);
        }

        [HttpGet("GetEmployes")]
        public IActionResult GetEmployes(string guidfilter)
        {
            DTOHREmployFilter hrEmployFilter = employFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            var filter = new List<Tercero>();
            var terceros = _dbcontext.Terceros
                            .Select(t => t)
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (!hrEmployFilter.ReportName.Equals("Retirado") ? 
                                    (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate) : 
                                    (t.DateRetirement >= hrEmployFilter.FromDate && t.DateRetirement <= hrEmployFilter.ToDate))
                                && (hrEmployFilter.ReportName.Equals("Retirado") ? t.IsActive == false : t.IsActive == true))
                            .ToList();

            if (!hrEmployFilter.ReportName.Equals("Retirado"))
            {
                var retirados = _dbcontext.Terceros
                            .Select(t => t)
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate)
                                && (t.DateRetirement > hrEmployFilter.ToDate)
                                && (t.IsActive == false))
                            .ToList();

                if (retirados.Count != 0)
                {
                    terceros.AddRange(retirados);
                }
            }

            foreach (var enterpriceId in hrEmployFilter.EnterpriceIds)
            {
                var ef = terceros
                    .Where(t => t.EnterpriseId == enterpriceId)
                    .ToList();

                switch (hrEmployFilter.FilterId)
                {
                    case 1:
                        foreach (var areaId in hrEmployFilter.AreaIds)
                        {
                            var f = ef.Where(t => t.AreaId == areaId)
                                .ToList();
                            filter.AddRange(f);
                        }
                        break;

                    case 2:
                        foreach (var areaId in hrEmployFilter.AreaIdsHomologate)
                        {
                            var f = ef.Where(t => t.AreaIdHomologate == areaId)
                                .ToList();
                            filter.AddRange(f);
                        }
                        break;

                    case 3:
                        foreach (var cargoId in hrEmployFilter.CargoIdsHomologate)
                        {
                            var f = ef.Where(t => t.CargoIdHomologate == cargoId)
                                .ToList();
                            filter.AddRange(f);
                        }
                        break;

                    case 4:
                        foreach (var cargoId in hrEmployFilter.CargoIds)
                        {
                            var f = ef.Where(t => t.CargoId == cargoId)
                                .ToList();
                            filter.AddRange(f);
                        }
                        break;

                    default:
                        {                            
                            filter.AddRange(ef);
                            break;
                        }
                }                
            }            

            List<DTOHREmployReport> employes = new List<DTOHREmployReport>(); 
            foreach (var tercero in filter)
            {
                var AFP = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.AfpId
                                    && l.TypeLocation == "AFP")
                            .FirstOrDefault();

                var AFC = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.AfcId
                                    && l.TypeLocation == "AFC")
                            .FirstOrDefault();

                var EPS = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.EpsId
                                    && l.TypeLocation == "EPS")
                            .FirstOrDefault();

                var cargo = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.CargoId
                                    && l.TypeLocation == "CARGO")
                            .FirstOrDefault();

                var cargoHM = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.CargoIdHomologate
                                    && l.TypeLocation == "CARGOHM")
                            .FirstOrDefault();

                var costCenter = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.AreaId
                                    && l.TypeLocation == "AREA")
                            .FirstOrDefault();

                var costCenterHM = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.AreaIdHomologate
                                    && l.TypeLocation == "AREAHM")
                            .FirstOrDefault();                

                var enterprise = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == tercero.EnterpriseId
                                    && l.TypeLocation == "EMPRESA")
                            .FirstOrDefault();

                if (hrEmployFilter.ReportName.Equals("Planta"))
                {
                    if(costCenter is null)
                        continue;

                    if (!costCenter.Description.Equals("PLANTA"))
                        continue;
                }

                if (hrEmployFilter.ReportName.Equals("Gerencial"))
                {
                    if (costCenter is null)
                        continue;
                }

                employes.Add(new DTOHREmployReport
                {
                    IdentificationNum = tercero.NumDocument,
                    Name = string.Format("{1} {0}", tercero.FirstName, tercero.LastName),
                    Company = (enterprise is null) ? "" : enterprise.Description,
                    CostCenter = (costCenter is null) ? "" : costCenter.Description,
                    Position = (cargo is null) ? "" : cargo.Description,
                    CostCenterHomologate = (costCenterHM is null) ? "" : costCenterHM.Description,
                    PositionHomologate = (cargoHM is null) ? "" : cargoHM.Description,
                    SalaryLastYear = tercero.LastSalary,
                    SalaryCurrent = tercero.Salary,
                    InitDate = tercero.DateContractStart,
                    Old = (int)(DateTime.Today - tercero.DateContractStart).TotalDays,
                    EndDate = (DateTime)(tercero.DateContractEnd is null ? DateTime.MinValue : tercero.DateContractEnd),
                    EndDateStr = ((DateTime)(tercero.DateContractEnd is null ? DateTime.MinValue : tercero.DateContractEnd)) == DateTime.MaxValue ? "INDEFINIDO" :
                    ((DateTime)(tercero.DateContractEnd is null ? DateTime.MinValue : tercero.DateContractEnd)) == DateTime.MinValue ? "INDEFINIDO" :
                    ((DateTime)(tercero.DateContractEnd is null ? DateTime.MinValue : tercero.DateContractEnd)).ToString("dd/MM/yyyy"),
                    Observations = "",
                    Sex = tercero.Sexo,
                    EPS = (EPS is null) ? "" : EPS.Description,
                    AFP = (AFP is null) ? "" : AFP.Description,
                    AFC = (AFC is null) ? "" : AFC.Description,
                    Address = tercero.Adress1,
                    City = tercero.City,
                    MovilNumber = tercero.Phone1,
                    DateBirth = tercero.DateBirth,
                    PlaceBirth = tercero.CityBirth,
                    DateExpedition = tercero.DateExpedition,
                    PlaceExpedition = tercero.PlaceExpedition,
                    State = tercero.IsActive ? "A" : "R",
                    DateRetirement = (DateTime)(tercero.DateRetirement is null ? DateTime.MinValue : tercero.DateRetirement),
                    ReasonRetirement = tercero.ReasonRetirement,
                    HideSubtotal = hrEmployFilter.HideSubtotal
                }) ; 
            }

            return Ok(employes);
        }

        [HttpGet("GetChartData")]
        public List<DTOChartData> GetChartData(string guidfilter, string valueType)
        {
            DTOHREmployFilter hrEmployFilter = employFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            List<DTOChartData> chartData = new List<DTOChartData>();
            List<DTOChartData> chartData1 = new List<DTOChartData>();
            switch (valueType)
            {
                case "Salary":
                    {
                        var terceros = _dbcontext.Terceros                            
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate)
                                && (t.IsActive == true))                            
                            .ToList();

                        var retirados = _dbcontext.Terceros
                            .Select(t => t)
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate)
                                && (t.DateRetirement > hrEmployFilter.ToDate)
                                && (t.IsActive == false))
                            .ToList();

                        if (retirados.Count != 0)
                        {
                            terceros.AddRange(retirados);
                        }

                        foreach (var tercero in terceros)
                        {
                            var cd = chartData.Where(t => t.Id == tercero.AreaId).FirstOrDefault();
                            if(cd is null)
                            {
                                chartData.Add(new DTOChartData()
                                {
                                    Id = tercero.AreaId,
                                    Value = tercero.Salary
                                });
                            }
                            else
                            {
                                cd.Value += tercero.Salary;
                            }
                            
                        }
                    }
                    break;

                case "Count":
                    {
                        var terceros = _dbcontext.Terceros
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate)
                                && (t.IsActive == true))
                            .ToList();

                        var retirados = _dbcontext.Terceros
                            .Select(t => t)
                            .Where(t => (t.TypeTerceroId == 3 || t.TypeTerceroId == 5)
                                && (t.DateContractStart >= hrEmployFilter.FromDate && t.DateContractStart <= hrEmployFilter.ToDate)
                                && (t.DateRetirement > hrEmployFilter.ToDate)
                                && (t.IsActive == false))
                            .ToList();

                        if (retirados.Count != 0)
                        {
                            terceros.AddRange(retirados);
                        }

                        foreach (var tercero in terceros)
                        {
                           
                            var cd = chartData.Where(t => t.Id == tercero.AreaId).FirstOrDefault();
                            if (cd is null)
                            {
                                chartData.Add(new DTOChartData()
                                {
                                    Id = tercero.AreaId,
                                    Value = 1
                                });
                            }
                            else
                            {
                                cd.Value += 1;
                            }

                        }
                    }
                    break;

                default:
                    break;
            }

            decimal total = 0;

            foreach (var data in chartData)
            {
                var costCenter = _dbcontext.LocationGenerics
                            .Where(l => l.LocationGenericId == data.Id
                                    && l.TypeLocation == "AREA")
                            .FirstOrDefault();
                if (!(costCenter is null) && !string.IsNullOrEmpty(costCenter.ChartDescription))
                {
                    var data1 = chartData1.Where(dc => dc.Description == costCenter.ChartDescription).FirstOrDefault();
                    if (data1 is null)
                    {
                        data.Description = costCenter.ChartDescription;
                        chartData1.Add(data);
                    }
                    else 
                    {
                        data1.Value += data.Value;
                    }

                    total += data.Value;
                }
                else if (!(costCenter is null) && string.IsNullOrEmpty(costCenter.ChartDescription))
                {
                    var data1 = chartData1.Where(dc => dc.Description == costCenter.Description).FirstOrDefault();
                    if (data1 is null)
                    {
                        data.Description = costCenter.Description;
                        chartData1.Add(data);
                    }
                    else
                    {
                        data1.Value += data.Value;
                    }

                    total += data.Value;
                }                
            }

            foreach (var data in chartData1)
            {
                var value = (100 * data.Value) / total;
                data.Description = string.Format("{1} - {0}", data.Description, (value / 100).ToString("P1"));
            }

            return chartData1;
        }

        public List<LocationGeneric> GetLocations(string typeLocation)
        {
            var locations = _dbcontext.LocationGenerics
                .Where(l => l.TypeLocation == typeLocation)
                .OrderBy(l => l.Description)
                .ToList();

            return locations;
        }
    }
}