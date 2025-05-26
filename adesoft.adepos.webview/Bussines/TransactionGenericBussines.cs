using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace adesoft.adepos.webview.Bussines
{
    public class TransactionGenericBussines
    {
        IConfiguration configuration_;
        public AdeposDBContext _dbcontext { get; set; }

        string urlappaux;
        public TransactionGenericBussines(AdeposDBContext context)
        {
            this._dbcontext = context;
        }
        public TransactionGenericBussines(IConfiguration configuration, AdeposDBContext context)
        {
            this._dbcontext = context;
            configuration_ = configuration;
            urlappaux = configuration.GetValue<string>("UrlBaseReports");
        }
        public TransactionGeneric CreateFromFile(TransactionGeneric transaction)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                DTOTransaction trans = new DTOTransaction();
                trans.TransactionGenericId = 10;
                trans.AuxTest = transaction.AuxTest;
                string jsonobj = JsonConvert.SerializeObject(trans);
                //urlappaux = "http://localhost:1495/"; ELIMINAR LINEA DE CODIGO
                Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonobj, urlappaux, "api/Util/ReadOrderOfFile", true);
                res.Wait();
                trans = JsonConvert.DeserializeObject<DTOTransaction>(res.Result);
                transaction.AuxTest = null;
                if (trans.TransactionIsOk)
                {
                    try
                    {
                       // evento.WriteEntry("Recibe respuesta : ", EventLogEntryType.Information);
                        TransactionGeneric newtrans = MapperReflection.ToModelEntity<DTOTransaction, TransactionGeneric>(trans);
                        newtrans.TransactionGenericId = 0;
                        newtrans.MessageResponse = trans.Message;
                        newtrans.WarehouseOriginId = transaction.WarehouseOriginId;
                        newtrans.TerceroId = 1;

                        newtrans.TypeTransactionId = transaction.TypeTransactionId;//ordenes 
                        newtrans.TransOption = transaction.TransOption;
                        newtrans.Consecutive = transaction.Consecutive;
                        newtrans.DocumentExtern = transaction.DocumentExtern;
                        newtrans.ConsecutiveChar = transaction.ConsecutiveChar;
                        newtrans.NameWork = transaction.NameWork;
                        newtrans.Note = transaction.Note;
                        newtrans.DateEnd = transaction.DateEnd;
                        newtrans.DatePayInit = trans.DateEnd;
                        newtrans.StateTransactionGenericId = 11;

                        newtrans.Works = transaction.Works;
                        newtrans.CustomerAccount = transaction.CustomerAccount;
                        newtrans.SalesPersonId = transaction.SalesPersonId;
                        newtrans.ModuleId = transaction.ModuleId;
                        newtrans.CityId = transaction.CityId;
                        newtrans.ReponsableTransId = transaction.ReponsableTransId;
                        newtrans.VehicleTypeId = transaction.VehicleTypeId;
                        newtrans.Wight = transaction.Wight;
                        newtrans.Scheduled = transaction.Scheduled;

                        decimal weight = 0;

                        List<DetailTransactionGeneric> details = new List<DetailTransactionGeneric>(); //= MapperReflection.ToModelEntityList<DTOTransactionDetail, DetailTransactionGeneric>(trans.Details);
                        List<Item> listitems = _dbcontext.Items.ToList();
                        foreach (DTOTransactionDetail de in trans.Details)
                        {                            
                            DetailTransactionGeneric d = new DetailTransactionGeneric();
                            d.Cant = de.Cant;
                            d.TransactionState = "C";
                            Item item = listitems.Where(x => x.Barcode == de.ItemBarcode).FirstOrDefault();
                            if (item != null)
                            {
                                weight += ((d.Cant * item.Weight) / 1000);

                                d.ItemId = item.ItemId;
                                details.Add(d);
                            }
                            else
                            {
                                newtrans.MessageResponse += "No se encontro el item codigo: " + de.ItemBarcode + " en el inventario y no se pudo importar de la Orden de despacho";
                                //errror
                            }

                        }

                        newtrans.Wight = weight;

                        newtrans.Details.AddRange(details);
                        _dbcontext.TransactionGenerics.Add(newtrans);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();

                        return newtrans;
                    }
                    catch (Exception ex)
                    {
                        evento.WriteEntry("ERROR GUARDANDO ORDEN : " + ex.ToString(), EventLogEntryType.Error);
                    }
                }
                else
                {
                    TransactionGeneric newtrans = MapperReflection.ToModelEntity<DTOTransaction, TransactionGeneric>(trans);
                    newtrans.MessageResponse = trans.Message;
                    newtrans.TransactionIsOk = false;
                    return newtrans;
                }

                return null;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("ERROR GUARDANDO ORDEN : " + ex.ToString(), EventLogEntryType.Error);
                return null;
            }
        }

        public TransactionGeneric CreateOrUpdateFromFile(TransactionGeneric transaction)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            using (var tts = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    DTOTransaction trans = new DTOTransaction();
                    trans.TransactionGenericId = 10;
                    trans.AuxTest = transaction.AuxTest;
                    string jsonobj = JsonConvert.SerializeObject(trans);
                    Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonobj, urlappaux, "api/Util/ReadOrderOfFile", true);
                    res.Wait();
                    trans = JsonConvert.DeserializeObject<DTOTransaction>(res.Result);
                    transaction.AuxTest = null;
                    if (trans.TransactionIsOk)
                    {
                        try
                        {
                            List<DetailTransactionGeneric> detailsNew = new List<DetailTransactionGeneric>();
                            List<Item> listitems = _dbcontext.Items.ToList();

                            List<DetailTransactionGeneric> detailsUpd = _dbcontext.DetailTransactionGenerics
                                .Include(d => d.Item)
                                .Where(d => d.TransactionGenericId.Equals(transaction.TransactionGenericId))
                                .ToList();

                            detailsUpd.ForEach(d => { d.TransactionState = "D"; });

                            foreach (DTOTransactionDetail de in trans.Details)
                            {
                                var detailTransactionGeneric = detailsUpd
                                    .Where(d => d.Item.Barcode.Equals(de.ItemBarcode))
                                    .FirstOrDefault();

                                if (!(detailTransactionGeneric is null))
                                {
                                    detailTransactionGeneric.Cant = de.Cant;
                                    detailTransactionGeneric.TransactionState = "U";
                                }
                                else
                                {
                                    Item item = listitems.Where(x => x.Barcode == de.ItemBarcode).FirstOrDefault();
                                    if (item != null)
                                    {
                                        detailsNew.Add(new DetailTransactionGeneric()
                                        {
                                            TransactionGenericId = transaction.TransactionGenericId,
                                            ItemId = item.ItemId,
                                            Cant = de.Cant,
                                            TransactionState = "C"
                                        });
                                    }
                                }
                            }

                            if (detailsNew.Count != 0)
                                _dbcontext.DetailTransactionGenerics.AddRange(detailsNew);

                            _dbcontext.DetailTransactionGenerics.UpdateRange(detailsUpd);

                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();

                            var detailsDel = detailsUpd.Where(d => d.TransactionState.Equals("D")).ToList();                            

                            List<DeletedDetailTransactionGeneric> deletedDetailTransactionGenerics = new List<DeletedDetailTransactionGeneric>();
                            detailsDel.ForEach(d =>
                            {
                                deletedDetailTransactionGenerics.Add(new DeletedDetailTransactionGeneric()
                                {
                                    Auxnum = d.Auxnum,
                                    Cant = d.Cant,
                                    TransactionState = d.TransactionState,
                                    ChanguedView = d.ChanguedView,
                                    Discount = d.Discount,
                                    HasIventory = d.HasIventory,
                                    InventarioPendiente = d.InventarioPendiente,
                                    IsRawMaterial = d.IsRawMaterial,
                                    ItemId = d.ItemId,
                                    MessageResponse = d.MessageResponse,
                                    MessageType = d.MessageType,
                                    DetailTransactionGenericId = d.DetailTransactionGenericId,
                                    NumOrder = d.NumOrder,
                                    ObservationAuditory = d.ObservationAuditory,
                                    OtherDiscount = d.OtherDiscount,
                                    pageIndex = d.pageIndex,
                                    pageSize = d.pageSize,
                                    PriceCost = d.PriceCost,
                                    PriceUnd = d.PriceUnd,
                                    SaldoInventaryAfter = d.SaldoInventaryAfter,
                                    SaldoInventaryBefore = d.SaldoInventaryBefore,
                                    Subtotal = d.Subtotal,
                                    Tax = d.Tax,
                                    Total = d.Total,
                                    TransactionGenericId = d.TransactionGenericId,
                                    TransactionIsOk = d.TransactionIsOk,
                                    TransOption = d.TransOption
                                });
                            });

                            if (deletedDetailTransactionGenerics.Count != 0)
                            {
                                _dbcontext.DeletedDetailTransactionGenerics.AddRange(deletedDetailTransactionGenerics);

                                _dbcontext.RemoveRange(detailsDel);

                                _dbcontext.SaveChanges();
                                _dbcontext.DetachAll();
                            }

                            List<DetailTransactionGeneric> details = _dbcontext.DetailTransactionGenerics
                                .Include(d => d.Item)
                                .Where(d => d.TransactionGenericId.Equals(transaction.TransactionGenericId))
                                .ToList();

                            decimal weight = 0;
                            foreach (var detail in details)
                            {
                                weight += ((detail.Cant * detail.Item.Weight) / 1000);                                
                            }

                            transaction.Wight = weight;

                            //_dbcontext.TransactionGenerics.Update(transaction);
                            
                            //_dbcontext.SaveChanges();
                            //_dbcontext.DetachAll();

                            tts.Commit();

                            return transaction;
                        }
                        catch (Exception ex)
                        {
                            evento.WriteEntry("ERROR GUARDANDO ORDEN : " + ex.ToString(), EventLogEntryType.Error);
                        }
                    }
                    else
                    {
                        tts.Rollback();

                        TransactionGeneric newtrans = MapperReflection.ToModelEntity<DTOTransaction, TransactionGeneric>(trans);
                        newtrans.MessageResponse = trans.Message;
                        newtrans.TransactionIsOk = false;
                        return newtrans;
                    }

                    tts.Rollback();

                    return null;
                }
                catch (Exception ex)
                {
                    tts.Rollback();
                    evento.WriteEntry("ERROR GUARDANDO ORDEN : " + ex.ToString(), EventLogEntryType.Error);
                    return null;
                }
            }
        }

        public List<DTOTransactionReport> GenerateDataSetToReport(string SDateInit, string SDateEnd, int TransOption)
        {
            List<DTOTransactionReport> datset = new List<DTOTransactionReport>();

            return datset;

        }

        /// <summary>
        /// Ordenes de despacho unispan
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<DTOTransactionReport> GenerateDataSetToReport(DTOTransactionReport param)
        {
            List<DTOTransactionReport> datset = new List<DTOTransactionReport>();

            if (param.TransOption == 1)//Despachos
            {
                var ListstConsult = _dbcontext.TransactionGenerics.Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENDESPACHO && (x.StateTransactionGenericId == 11 || x.StateTransactionGenericId == 12)
                 && x.WarehouseOriginId == param.Warehouseid).Include("Details.Item");
                if (param.DateInit != DateTime.MinValue && param.DateEnd != DateTime.MinValue)
                {
                    ListstConsult = ListstConsult.Where(x => x.DateInit.Date >= param.DateInit && x.DateInit.Date <= param.DateEnd);
                }
                else if (param.DateEnd != DateTime.MinValue)
                {
                    param.DateEnd = param.DateEnd.Date.Add(new TimeSpan(23, 59, 59));
                    ListstConsult = ListstConsult.Where(x => x.DateInit.Date <= param.DateEnd);
                }
                List<TransactionGeneric> ListResultTran = ListstConsult.ToList();
                BuildReserve(ListResultTran);//constuye la reserva

                List<long> itemstofilter = new List<long>();//para cuando es completa y solo pendientes o todos
                if (param.TransactionGenericId != 0 && param.ItemId == 0)
                {
                    if (!param.FilterOnlyPendient)
                    {
                        itemstofilter = ListResultTran.Where(x => x.TransactionGenericId == param.TransactionGenericId).Select(x => x.Details.Where(x => x.InventarioPendiente > 0).Select(t => t.ItemId)).First().Select(x => x).ToList();
                    }
                    else if (param.FilterOnlyPendient)
                    {
                        itemstofilter = ListResultTran.Where(x => x.TransactionGenericId == param.TransactionGenericId).Select(x => x.Details.Select(t => t.ItemId)).First().Select(x => x).ToList();
                    }

                }

                foreach (TransactionGeneric trareal in ListResultTran)
                {
                    foreach (DetailTransactionGeneric trdto in trareal.Details)
                    {
                        DTOTransactionReport dto = new DTOTransactionReport();
                        dto.Cant = trdto.Cant;
                        dto.StockActual = trdto.Auxnum;
                        dto.SaldoInventaryAfter = trdto.SaldoInventaryAfter;
                        dto.SaldoInventaryBefore = trdto.SaldoInventaryBefore;
                        dto.InventarioPendiente = trdto.InventarioPendiente;
                        dto.ItemDescription = trdto.Item.Description;
                        dto.ItemBarcode = trdto.Item.Barcode;
                        dto.TurnId = trareal.TurnId;
                        dto.DocumentExtern = trareal.DocumentExtern;
                        dto.Note = trareal.Note;
                        dto.NameWork = trareal.NameWork;
                        dto.DateInit = trareal.DateInit;
                        if (param.TransactionGenericId != 0)
                        {//si es detallado
                            if (param.ItemId != 0 && param.ItemId == trdto.ItemId)
                            {
                                datset.Add(dto);
                            }
                            else if (param.ItemId == 0)
                            {
                                if (itemstofilter.Contains(trdto.ItemId))
                                {
                                    datset.Add(dto);
                                }
                            }
                        }
                        else
                        {//global
                            datset.Add(dto);
                        }
                    }
                }
                return datset;
            }
            else if (param.TransOption == 2)//fabricacion
            {
                var ListstConsult = _dbcontext.TransactionGenerics.Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENTRABAJO
                 && x.WarehouseOriginId == param.Warehouseid).Include("Details.Item");
                if (param.DateInit != DateTime.MinValue && param.DateEnd != DateTime.MinValue)
                {
                    ListstConsult = ListstConsult.Where(x => x.DateInit.Date >= param.DateInit && x.DateInit.Date <= param.DateEnd);
                }
                List<TransactionGeneric> ListResultTran = ListstConsult.ToList();

                foreach (TransactionGeneric trareal in ListResultTran)
                {
                    foreach (DetailTransactionGeneric trdto in trareal.Details)
                    {
                        DTOTransactionReport dto = datset.Where(x => x.ItemId == trdto.ItemId).FirstOrDefault();
                        if (dto != null)
                        {
                            dto.Cant += trdto.Cant;
                            dto.Total += trdto.Total;
                            dto.InventarioPendiente += trdto.InventarioPendiente;
                            //datset.Add(dto);
                        }
                        else
                        {
                            dto = new DTOTransactionReport();
                            dto.Cant = trdto.Cant;
                            dto.ItemId = trdto.ItemId;
                            dto.Total = trdto.Total;
                            dto.InventarioPendiente = trdto.InventarioPendiente;
                            dto.ItemDescription = trdto.Item.Description;
                            dto.ItemBarcode = trdto.Item.Barcode;
                            dto.DocumentExtern = trareal.DocumentExtern;
                            dto.Note = trareal.Note;
                            dto.ConsecutiveChar = trareal.ConsecutiveChar;
                            datset.Add(dto);
                        }
                    }
                }
                return datset;
            }
            else
            {
                return datset;
            }
        }



        public List<TransactionGeneric> BuildReserve(List<TransactionGeneric> liststock)
        {
            try
            {
                long warehouseid = liststock.First().WarehouseOriginId;
                List<MovementInventory> listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == warehouseid).ToList();
                listmovements.ForEach(x => { x.CantNet = x.CantNow; x.CantMov = 0; });
                foreach (TransactionGeneric tran in liststock.OrderBy(x => x.TurnId).ToList())
                {
                    foreach (DetailTransactionGeneric det in tran.Details)
                    {
                        MovementInventory mov = listmovements.Where(x => x.ItemId == det.ItemId).FirstOrDefault();
                        if (mov != null)
                        {
                            det.SaldoInventaryBefore = mov.CantNet;
                            if (mov.CantNet >= det.Cant)
                            {
                                det.InventarioPendiente = 0;//no tiene pendiente
                            }
                            else if (mov.CantNet > 0)
                            {
                                det.InventarioPendiente = Math.Abs(mov.CantNet - det.Cant);
                            }
                            else
                            {
                                det.InventarioPendiente = det.Cant;
                            }
                            det.Auxnum = mov.CantNow;///DIFERENTE DEL OTRO para pasarle al reporte
                            mov.CantMov += det.Cant;
                            mov.CantNet = mov.CantNet - det.Cant;
                            det.SaldoInventaryAfter = mov.CantNet;
                        }
                    }

                    if (tran.Details.Where(x => x.InventarioPendiente > 0).Count() > 0)
                    {
                        tran.StateTransactionGenericId = 11;
                    }
                    else
                    {
                        tran.StateTransactionGenericId = 12;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return liststock;
        }

        /// <summary>
        /// Unispan
        /// </summary>
        /// <param name="liststock"></param>
        /// <returns></returns>
        static object object_ = new object();
        public List<DTOCardDistpatch> UpdateStockReserve(List<DTOCardDistpatch> liststock)
        {
            try
            {
                long warehouseid = liststock.First().TransactionDistpatch.WarehouseOriginId;
                //List<MovementInventory> listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == warehouseid).AsTracking.ToList();
                List<MovementInventory> listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == warehouseid).ToList();
                listmovements.ForEach(x => { x.CantNet = x.CantNow; x.CantMov = 0; });
                foreach (DTOCardDistpatch dtocard in liststock.OrderBy(x => x.TurnOrder).ToList())
                {
                    TransactionGeneric tran = dtocard.TransactionDistpatch;
                    tran.TurnId = dtocard.TurnOrder;
                    tran.DateInit = dtocard.DateDistpatch; tran.DateEnd = dtocard.DateDistpatch;

                    foreach (DetailTransactionGeneric det in tran.Details)
                    {
                        MovementInventory mov = listmovements.Where(x => x.ItemId == det.ItemId).FirstOrDefault();
                        if (mov != null)
                        {
                            det.SaldoInventaryBefore = mov.CantNet;
                            if (mov.CantNet >= det.Cant)
                            {
                                det.InventarioPendiente = 0;//no tiene pendiente
                            }
                            else if (mov.CantNet > 0)
                            {
                                det.InventarioPendiente = Math.Abs(mov.CantNet - det.Cant);
                            }
                            else
                            {
                                det.InventarioPendiente = det.Cant;
                            }
                            mov.CantMov += det.Cant;
                            mov.CantNet = mov.CantNet - det.Cant;
                            det.SaldoInventaryAfter = mov.CantNet;
                            //_dbcontext.Entry<DetailTransactionGeneric>(det).State = EntityState.Modified;
                        }
                    }

                    if (tran.Details.Where(x => x.InventarioPendiente > 0).Count() > 0)
                    {
                        tran.StateTransactionGenericId = 11;
                        //poner en estado con pendientes.
                    }
                    else
                    {
                        tran.StateTransactionGenericId = 12;
                    }
                    TransactionGeneric traclone = tran.GetClone<TransactionGeneric>();
                    traclone.Details = null;
                    _dbcontext.Entry<TransactionGeneric>(traclone).State = EntityState.Modified;
                }
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();//al final para q no desatache movimiento
            }
            catch (Exception ex)
            {

            }
            return liststock;
        }

    }
}
