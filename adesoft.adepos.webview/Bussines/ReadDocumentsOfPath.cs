using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.DAO;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.Simex;
using adesoft.adepos.webview.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class ReadDocumentsOfPath
    {
        IConfiguration configuration_;
        ConnectionDB connection_;
        string pathFileInventory;
        string ConnectionQuantify = string.Empty, ConnectionBiable = String.Empty;
        string urlappaux;
        EventLog evento;

        public ReadDocumentsOfPath(IConfiguration configuration, ConnectionDB connection)
        {
            this.configuration_ = configuration;
            pathFileInventory = configuration.GetValue<string>("Parameters:pathFileReadInventory");
            ConnectionQuantify = configuration.GetValue<string>("ConnectionStrings:ConnectionQuantify");
            ConnectionBiable = configuration.GetValue<string>("ConnectionStrings:ConnectionBiable");
            urlappaux = configuration.GetValue<string>("UrlBaseReports");
            connection_ = connection;

            // comentado por rocampo
            /*if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");*/
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
        }

        public void UpdateCommercialData()
        {
            try
            {
                evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "1", EventLogEntryType.Error);
                var updateCommercialData = false;
                var dbContext = new AdeposDBContext(connection_.Connection);
                var lastUpdateModel = dbContext.SimexLastUpdateModuleLog.Where(l => l.Module.Equals("Commercial")).FirstOrDefault();

                evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "2", EventLogEntryType.Error);

                if (lastUpdateModel is null)
                    updateCommercialData = true;
                else
                {
                    var compareDate = lastUpdateModel.LastUpdateModule_At.AddMinutes(5);
                    if (compareDate <= DateTime.Now)
                        updateCommercialData = true;
                }

                evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "3", EventLogEntryType.Error);

                if (updateCommercialData)
                {
                    evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "4", EventLogEntryType.Error);
                    using (SqlConnection con = new SqlConnection(configuration_.GetConnectionString("UnispanReports")))
                    {
                        evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "5", EventLogEntryType.Error);
                        using (SqlCommand cmd = new SqlCommand("dbo.sp_UpdateCommercialData", con))
                        {
                            evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "6", EventLogEntryType.Error);
                            cmd.CommandType = CommandType.StoredProcedure;

                            con.Open();
                            evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "7", EventLogEntryType.Error);
                            cmd.ExecuteNonQuery();
                            evento.WriteEntry(configuration_.GetConnectionString("UnispanReports") + "8", EventLogEntryType.Error);
                            con.Close();
                        }
                    }
                }                
                
                if (!(lastUpdateModel is null))
                {
                    if(updateCommercialData)
                    {
                        lastUpdateModel.LastUpdateModule_At = DateTime.Now;
                        dbContext.Update(lastUpdateModel);
                    }
                }
                else
                {
                    dbContext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                    {
                        Module = "Commercial",
                        LastUpdateModule_At = DateTime.Now
                    });
                }                    
                
                dbContext.SaveChanges();
                dbContext.Dispose();
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Ha ocurrido un erro cuando se actualizaba los datos del modulo de comercial. Message error: " + ex.ToString(), EventLogEntryType.Error);
            }
        }

        public static List<Warehouse> warehouses { get; set; }
        public async Task ReadInventoryStockOfPath()
        {
            try
            {//unispan
                if (warehouses == null)
                {
                    AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                    warehouses = adepos.Warehouses.Where(x => x.WarehouseId != 1).ToList();
                    adepos.Dispose();
                }

                foreach (Warehouse ware in warehouses)
                {
                    //  string paramsjson = "jsonData={\"pathFile\":\"" + pathFileInventory + "\",\"bodega\":\"" + ware.Name + "\"}";
                    string paramsurl = "?pathFile=" + pathFileInventory + "&bodega=" + ware.Name;
                    string result = await HttpAPIClient.PostSendRequest("", urlappaux, "api/Util/ReadInventaryOfDocumentFormFile" + paramsurl);
                    List<DTOInventary> inventars = JsonConvert.DeserializeObject<List<DTOInventary>>(result);
                    TransactionGeneric transaction = new TransactionGeneric();
                    transaction.WarehouseOriginId = ware.WarehouseId;
                    if (inventars != null && inventars.Count > 0)
                    {
                        evento.WriteEntry("Sincronizacion de inventario por archivo iniciada. Sede: " + ware.Name, EventLogEntryType.Information);
                        GuardarInventario(ware, inventars);
                        evento.WriteEntry("Sincronizacion de inventario por archivo finalizada. Sede: " + ware.Name, EventLogEntryType.Information);
                        ActualizarUltimaHoraSincronizacion("ParametrosQuantify");
                    }
                }
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Sincronizacion de inventario por archivo genero error . " + ex.ToString(), EventLogEntryType.Error);
            }
        }
        static DTOParamInventaryQuanty paraminventar;
        static bool sincroniced = false;
        public async Task ReadInventoryStockOfQuantify(bool BotonDemand)
        {
            try
            {//unispan
                if (warehouses == null)
                {
                    AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                    warehouses = adepos.Warehouses.Where(x => x.WarehouseId != 1).ToList();
                    string parameter = adepos.Parameters.Where(x => x.NameIdentify == "ParametrosQuantify").First().Value2;
                    paraminventar = JsonConvert.DeserializeObject<DTOParamInventaryQuanty>(parameter);
                    paraminventar.ReadTimes();
                    adepos.DetachAll(); adepos.Dispose();
                }
                if (paraminventar.IsEnable == 1 || BotonDemand)
                {
                    TimeSpan timenow = DateTime.Now.TimeOfDay;

                    if ((paraminventar.ListTimes.Count > 0 && paraminventar.ListTimes.Where(x => timenow >= x && timenow < x.Add(new TimeSpan(0, 2, 0))).Count() > 0)
                        || BotonDemand)
                    {
                        if (!sincroniced || BotonDemand)
                        {
                            sincroniced = true;

                            evento.WriteEntry("Sincronizacion de inventario programada Iniciada. ", EventLogEntryType.Information);

                            List<DTOInventary> inventars = new DaoQuantify(ConnectionQuantify).SelectInventary();
                            foreach (Warehouse ware in warehouses)
                            {
                                GuardarInventario(ware, inventars.Where(x => x.Warehouseid == ware.QuantifyId).ToList());
                            }

                            evento.WriteEntry("Sincronizacion de inventario programada Finalizada. ", EventLogEntryType.Information);
                            ActualizarUltimaHoraSincronizacion("ParametrosQuantify");

                        }
                    }
                    else
                    {
                        sincroniced = false;
                    }

                }
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Sincronizacion de inventario programada genero error . " + ex.ToString(), EventLogEntryType.Error);
            }
        }
        static DTOParamInventaryQuanty paraminventarBodgArr;
        static bool sincronicedBodArriendo = false;
        //Inventario bodega arriendo
        public async Task ReadInventoryStockOfWarehouseRent()
        {
            try
            {
                if (paraminventarBodgArr == null)
                {
                    AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                    string parameter = adepos.Parameters.Where(x => x.NameIdentify == "ParamInsertWarehouseRent").First().Value2;
                    paraminventarBodgArr = JsonConvert.DeserializeObject<DTOParamInventaryQuanty>(parameter);
                    paraminventarBodgArr.ReadTimes();
                    adepos.DetachAll(); adepos.Dispose();
                }

                if (paraminventarBodgArr.IsEnable == 1)
                {
                    TimeSpan timenow = DateTime.Now.TimeOfDay;

                    if (true)//((paraminventarBodgArr.ListTimes.Count > 0 && paraminventarBodgArr.ListTimes.Where(x => timenow >= x && timenow < x.Add(new TimeSpan(0, 2, 0))).Count() > 0))
                    {
                        if (!sincronicedBodArriendo)
                        {
                            sincronicedBodArriendo = true;
                            ActualizarFotoInventarioDisponibleArriendo();

                        }
                    }
                    else
                    {
                        sincronicedBodArriendo = false;
                    }
                }
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error metodo ReadInventoryStockOfWarehouseRent. " + ex.ToString(), EventLogEntryType.Error);
            }

        }
        public void ActualizarFotoInventarioDisponibleArriendo()
        {
            try
            {
                //evento.WriteEntry("Sincronizacion foto de inventario bodega arriendo Iniciada. ", EventLogEntryType.Information);

                List<SnapshotInventoryQuantify> inventarysShot = new DaoQuantify(ConnectionQuantify).SelectInventarySnapshotNotItem();
                if (inventarysShot.Count > 0)
                {
                    AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                    List<SnapshotInventoryQuantify> snapshots = adepos.SnapshotInventoryQuantifys.Where(x => x.DateInventary == inventarysShot.First().DateInventary).ToList();
                    if (snapshots != null && snapshots.Count > 0)
                    {
                        adepos.SnapshotInventoryQuantifys.RemoveRange(snapshots);
                        adepos.SaveChanges();
                        adepos.DetachAll();
                    }
                    foreach (SnapshotInventoryQuantify inventor in inventarysShot)
                    {
                        adepos.Entry<SnapshotInventoryQuantify>(inventor).State = EntityState.Added;
                    }
                    adepos.SaveChanges();
                    adepos.DetachAll();
                    adepos.Dispose();
                    ActualizarUltimaHoraSincronizacion("ParamInsertWarehouseRent");
                }
            }
            catch (Exception ex)
            {
                //evento.WriteEntry("Sincronizacion foto de inventario bodega arriendo genero error . " + ex.ToString(), EventLogEntryType.Error);
            }
        }


        public void ActualizarUltimaHoraSincronizacion(string paramname)
        {
            AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
            Parameter paramet = adepos.Parameters.Where(x => x.NameIdentify == paramname).First();
            string parameter = paramet.Value2;
            DTOParamInventaryQuanty param = JsonConvert.DeserializeObject<DTOParamInventaryQuanty>(parameter);
            param.LastSync = DateTime.Now;
            paramet.Value2 = JsonConvert.SerializeObject(param);
            adepos.Entry<Parameter>(paramet).State = EntityState.Modified;
            adepos.SaveChanges();
            adepos.Dispose();

        }
        public void GuardarInventario(Warehouse ware, List<DTOInventary> inventars)
        {
            TransactionGeneric transaction = new TransactionGeneric();
            transaction.WarehouseOriginId = ware.WarehouseId;
            if (inventars != null && inventars.Count > 0)
            {
                AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                foreach (DTOInventary dtoin in inventars)
                {
                    DetailTransactionGeneric det = new DetailTransactionGeneric();
                    transaction.Details.Add(det);
                    det.Item = new Item()
                    {
                        Referencia = dtoin.Barcode,
                        Barcode = dtoin.Barcode,
                        Description = dtoin.ItemName,
                        HasIventory = true,
                        CategoryId = 2, //generica
                        UnitMeasurementId = 3, //generica
                        TypeTaxId = 2, // ninguna
                        Weight = dtoin.Weight
                    };
                    det.Cant = dtoin.CantInv;

                }
                MovementInventoryBussines mov = new MovementInventoryBussines(adepos);
                mov.NuevaEntradaInventarioOfPlainText(transaction);
                adepos.SaveChanges();
                adepos.DetachAll();
            }
        }
        public async Task ReadPersonsOfDocument()
        {
            try
            {//unispan

                //  string paramsjson = "jsonData={\"pathFile\":\"" + pathFileInventory + "\",\"bodega\":\"" + ware.Name + "\"}";
                string paramsurl = "?pathFile=" + pathFileInventory;
                string result = await HttpAPIClient.PostSendRequest("", urlappaux, "api/Util/ReadDocumentPersonOfFile" + paramsurl);
                List<DTOTercero> terceros = JsonConvert.DeserializeObject<List<DTOTercero>>(result);
                if (terceros != null && terceros.Count > 0)
                {
                    AdeposDBContext adeposcontext = new AdeposDBContext(connection_.Connection);

                    foreach (string tercerodoc in terceros.Select(x => x.NumDocument).Distinct().ToList())
                    {
                        DTOTercero dtoin = terceros.Where(x => x.NumDocument == tercerodoc).OrderByDescending(x => x.DateContractStart).First();//terceros
                        Tercero terc = adeposcontext.Terceros.Where(x => x.NumDocument == dtoin.NumDocument).FirstOrDefault();
                        if (terc == null)
                            terc = new Tercero();
                        terc.NumDocument = dtoin.NumDocument;
                        terc.FirstName = dtoin.FirstName;
                        terc.LastName = dtoin.LastName;
                        terc.Sexo = dtoin.Sexo;
                        terc.DateIn = dtoin.DateIn;
                        terc.DateBirth = dtoin.DateBirth;
                        terc.CityBirth = dtoin.CityBirth;
                        terc.NDC = dtoin.NDC;
                        terc.TypePersonId = 1;
                        terc.TypeTerceroId = 4;
                        terc.IsActive = dtoin.IsActive;
                        LocationGeneric locatempr = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.CodeEnterprise
                        && x.TypeLocation == TypesLocations.EMPRESA).FirstOrDefault();
                        if (locatempr != null)
                        {
                            terc.EnterpriseId = locatempr.LocationGenericId;
                        }
                        else
                        {
                            locatempr = new LocationGeneric();
                            locatempr.Description = dtoin.EnterpriseName;
                            locatempr.SyncCode = dtoin.CodeEnterprise;
                            locatempr.TypeLocation = TypesLocations.EMPRESA;

                            adeposcontext.Entry(locatempr).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.EnterpriseId = locatempr.LocationGenericId;
                        }

                        LocationGeneric locatarea = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.CodeArea
                      && x.TypeLocation == TypesLocations.AREA).FirstOrDefault();
                        if (locatarea != null)
                        {
                            terc.AreaId = locatarea.LocationGenericId;
                        }
                        else
                        {
                            locatarea = new LocationGeneric();
                            locatarea.Description = dtoin.AreaName;
                            locatarea.SyncCode = dtoin.CodeArea;
                            locatarea.TypeLocation = TypesLocations.AREA;

                            adeposcontext.Entry(locatarea).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.AreaId = locatarea.LocationGenericId;
                        }

                        LocationGeneric locatsucursal = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.CodeSucursal
                    && x.TypeLocation == TypesLocations.SUCURSAL).FirstOrDefault();
                        if (locatsucursal != null)
                        {
                            terc.SucursalId = locatsucursal.LocationGenericId;
                        }
                        else
                        {
                            locatsucursal = new LocationGeneric();
                            locatsucursal.Description = dtoin.SucursalName;
                            locatsucursal.SyncCode = dtoin.CodeSucursal;
                            locatsucursal.TypeLocation = TypesLocations.SUCURSAL;

                            adeposcontext.Entry(locatsucursal).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.SucursalId = locatsucursal.LocationGenericId;
                        }

                        terc.DateContractStart = dtoin.DateContractStart;
                        terc.DateContractEnd = dtoin.DateContractEnd;
                        terc.VacationUntil = dtoin.VacationUntil;
                        terc.DayPaysVacations = dtoin.DayPaysVacations;
                        terc.DateRetirement = dtoin.DateRetirement;
                        terc.ReasonRetirement = dtoin.ReasonRetirement;
                        terc.Salary = dtoin.Salary;


                        LocationGeneric locateps = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.EpsCode
               && x.TypeLocation == TypesLocations.EPS).FirstOrDefault();
                        if (locateps != null)
                        {
                            terc.EpsId = locateps.LocationGenericId;
                        }
                        else
                        {
                            locateps = new LocationGeneric();
                            locateps.Description = dtoin.EpsName;
                            locateps.SyncCode = dtoin.EpsCode;
                            locateps.TypeLocation = TypesLocations.EPS;

                            adeposcontext.Entry(locateps).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.EpsId = locateps.LocationGenericId;
                        }


                        LocationGeneric locatafp = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.AfpCode
            && x.TypeLocation == TypesLocations.AFP).FirstOrDefault();
                        if (locatafp != null)
                        {
                            terc.AfpId = locatafp.LocationGenericId;
                        }
                        else
                        {
                            locatafp = new LocationGeneric();
                            locatafp.Description = dtoin.AfpName;
                            locatafp.SyncCode = dtoin.AfpCode;
                            locatafp.TypeLocation = TypesLocations.AFP;

                            adeposcontext.Entry(locatafp).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.AfpId = locatafp.LocationGenericId;
                        }

                        LocationGeneric locatarl = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.ArlCode
            && x.TypeLocation == TypesLocations.ARL).FirstOrDefault();
                        if (locatarl != null)
                        {
                            terc.ArlId = locatarl.LocationGenericId;
                        }
                        else
                        {
                            locatarl = new LocationGeneric();
                            locatarl.Description = dtoin.ArlName;
                            locatarl.SyncCode = dtoin.ArlCode;
                            locatarl.TypeLocation = TypesLocations.ARL;

                            adeposcontext.Entry(locatarl).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.ArlId = locatarl.LocationGenericId;
                        }

                        LocationGeneric locatcargo = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.CargoCode
           && x.TypeLocation == TypesLocations.CARGO).FirstOrDefault();
                        if (locatcargo != null)
                        {
                            terc.CargoId = locatcargo.LocationGenericId;
                        }
                        else
                        {
                            locatcargo = new LocationGeneric();
                            locatcargo.Description = dtoin.CargoName;
                            locatcargo.SyncCode = dtoin.CargoCode;
                            locatcargo.TypeLocation = TypesLocations.CARGO;

                            adeposcontext.Entry(locatcargo).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.CargoId = locatcargo.LocationGenericId;
                        }


                        LocationGeneric locatcaja = adeposcontext.LocationGenerics.Where(x => x.SyncCode == dtoin.CajaCode
          && x.TypeLocation == TypesLocations.CAJA).FirstOrDefault();
                        if (locatcaja != null)
                        {
                            terc.CajaCompesacionId = locatcaja.LocationGenericId;
                        }
                        else
                        {
                            locatcaja = new LocationGeneric();
                            locatcaja.Description = dtoin.CajaCompesacionName;
                            locatcaja.SyncCode = dtoin.CajaCode;
                            locatcaja.TypeLocation = TypesLocations.CAJA;

                            adeposcontext.Entry(locatcaja).State = EntityState.Added;
                            adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                            terc.CajaCompesacionId = locatcaja.LocationGenericId;
                        }
                        if (terc.TerceroId == 0)
                            adeposcontext.Entry(terc).State = EntityState.Added;
                        else
                            adeposcontext.Entry(terc).State = EntityState.Modified;
                        adeposcontext.SaveChanges(); adeposcontext.DetachAll();
                    }

                }

            }
            catch (Exception ex)
            {

            }
        }



        bool SnapshotSynchronize = false;
        public static List<DateTime> ListDatetTimesSync { get; set; }
        /// <summary>
        /// SNAPSHOT INGRESOS BODEGA ARRIENDO
        /// </summary>
        /// <returns></returns>
        public async Task SnapshotInventoryWarehouse()
        {
            try
            {//unispan
                if (ListDatetTimesSync == null || ListDatetTimesSync.Count == 0)
                {
                    ListDatetTimesSync = new List<DateTime>();
                    AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                    string value2 = adepos.Parameters.Where(x => x.NameIdentify == "SnapshotWarehouseRent").First().Value2;
                    foreach (string hou in value2.Split(","))
                    {
                        string[] diames = hou.Split("-");//DIA-MES
                        DateTime time = new DateTime(DateTime.Now.Year, int.Parse(diames[1]), int.Parse(diames[0]));
                        ListDatetTimesSync.Add(time.Date);
                    }
                }


                DateTime timenow = DateTime.Now.Date;

                if ((ListDatetTimesSync.Count > 0 && ListDatetTimesSync.Where(x => timenow == x).Count() > 0)
                    )
                {
                    if (!SnapshotSynchronize)
                    { 
                        evento.WriteEntry("Snapshot de Ingresos por arriendo Iniciado. ", EventLogEntryType.Information);
                        SnapshotSynchronize = true;
                        //SNAPSHOT INGRESOS BODEGA ARRIENDO 
                        DTOFiltersCompras filter = new DTOFiltersCompras();
                       // filter.multipleValuesYear = new List<long>();
                        ((IList<long>)filter.multipleValuesYear).Add(DateTime.Now.AddYears(-1).Year);
                        List<SnapshotBiableValueMonth> snapshots = new DaoSqlBiable(ConnectionBiable).GetIncomeRentalToHistoric(filter);

                        AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                        List<SnapshotBiableValueMonth> snapshotsremov = adepos.SnapshotBiableValueMonths.Where(x => filter.multipleValuesYear.Contains(x.YearId)).ToList();
                        if (snapshotsremov != null && snapshotsremov.Count > 0)
                        {
                            adepos.SnapshotBiableValueMonths.RemoveRange(snapshotsremov);
                            adepos.SaveChanges();
                            adepos.DetachAll();
                        }
                        if (snapshots != null && snapshots.Count > 0)
                            foreach (SnapshotBiableValueMonth inventor in snapshots)
                            {
                                adepos.Entry<SnapshotBiableValueMonth>(inventor).State = EntityState.Added;
                            }
                        adepos.SaveChanges();
                        adepos.DetachAll();
                        adepos.Dispose();
                        evento.WriteEntry("Snapshot de Ingresos por arriendo Finalizado. ", EventLogEntryType.Information);
                    }
                }
                else
                {
                    SnapshotSynchronize = false;
                }


            }
            catch (Exception ex)
            {
                evento.WriteEntry("Snapshot de Ingresos por arriendo genero error . " + ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
