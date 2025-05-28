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
using adesoft.adepos.webview.Bussines;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Util;
using Microsoft.AspNetCore.Authorization;

namespace adesoft.adepos.webview.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionGenericController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public TransactionGenericController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");

            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public TransactionGeneric Create(TransactionGeneric transactionGeneric)
        {
            ConsecutiveBussines consecut = new ConsecutiveBussines(_dbcontext);
            transactionGeneric = consecut.GetConsecutive(transactionGeneric);
            if (!transactionGeneric.TransactionIsOk)
                return transactionGeneric;

            if (transactionGeneric.TransOption == 6)//Orden de despacho desde archivo
            {
                TransactionGenericBussines transBuss = new TransactionGenericBussines(_configuration, _dbcontext);
                transactionGeneric = transBuss.CreateFromFile(transactionGeneric);
                //List<DTOInventary> inventars = JsonConvert.DeserializeObject<List<DTOInventary>>(result);
            }
            else if (transactionGeneric.TransOption == 5)//ORDEN DE TRABAJO
            {
                _dbcontext.TransactionGenerics.Add(transactionGeneric);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                transactionGeneric.TransactionIsOk = true;
                transactionGeneric.MessageResponse = "Orden de trabajo guardada correctamente.";
            }


            return transactionGeneric;
        }

        public TransactionGeneric ImportFile(TransactionGeneric transactionGeneric)
        {
            TransactionGenericBussines transBuss = new TransactionGenericBussines(_configuration, _dbcontext);
            transactionGeneric = transBuss.CreateOrUpdateFromFile(transactionGeneric);
            return transactionGeneric;
        }


        public TransactionGeneric Update(TransactionGeneric transactionGeneric)
        {

            if (transactionGeneric.TransOption == 0 || transactionGeneric.TransOption == 1)//facturacion
            {
                if (transactionGeneric.TransactionGenericId != 0)
                {
                }
            }
            else if (transactionGeneric.TransOption == 2)//Orden de trabajo
            {
                if (transactionGeneric.TransactionGenericId != 0)
                {
                    _dbcontext.Entry<TransactionGeneric>(transactionGeneric).State = EntityState.Modified;
                    foreach (DetailTransactionGeneric de in transactionGeneric.Details)
                    {
                        de.TransactionGenericId = transactionGeneric.TransactionGenericId;
                        if (de.DetailTransactionGenericId != 0)
                            _dbcontext.Entry<DetailTransactionGeneric>(de).State = EntityState.Modified;
                        else
                            _dbcontext.Entry<DetailTransactionGeneric>(de).State = EntityState.Added;
                    }

                    foreach (DetailTransactionGeneric de in transactionGeneric.DetailsRemove)
                    {
                        _dbcontext.Entry<DetailTransactionGeneric>(de).State = EntityState.Deleted;
                    }
                    transactionGeneric.TransactionIsOk = true;
                    transactionGeneric.MessageResponse = "Orden guardada correctamente.";
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }
            }
            else if (transactionGeneric.TransOption == 3)
            {
                decimal weight = 0;
                foreach (var detail in transactionGeneric.Details)
                {
                    weight += ((detail.Cant * detail.Item.Weight) / 1000);
                }

                transactionGeneric.Wight = weight;

                _dbcontext.Entry<TransactionGeneric>(transactionGeneric).State = EntityState.Modified;

                foreach (DetailTransactionGeneric de in transactionGeneric.Details.Where(x => x.ChanguedView).ToList())
                {
                    de.TransactionGenericId = transactionGeneric.TransactionGenericId;
                    if (de.DetailTransactionGenericId != 0)
                        _dbcontext.Entry<DetailTransactionGeneric>(de).State = EntityState.Modified;
                    else
                        _dbcontext.Entry<DetailTransactionGeneric>(de).State = EntityState.Added;
                }

                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return transactionGeneric;
        }


        public TransactionGeneric Delete(TransactionGeneric transactionGeneric)
        {
            if (transactionGeneric.TransOption == 1)//eliminar orden de despacho
            {//unispan
                if (transactionGeneric.TransactionGenericId != 0)
                {
                    List<MovementInventory> listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == transactionGeneric.WarehouseOriginId)
                        .AsTracking().ToList();
                    foreach (DetailTransactionGeneric de in transactionGeneric.Details)
                    {
                        MovementInventory mov = listmovements.Where(x => x.ItemId == de.ItemId).First();
                        //mov.CantMov = mov.CantMov - de.Cant;
                        //mov.CantNet = mov.CantNet + de.Cant;
                        mov.CantNow = mov.CantNow - de.Cant;
                        mov.CantNet = mov.CantNow;
                        if (mov.CantNow < 0)
                        {
                            mov.CantNow = 0;
                            mov.CantNet = 0;
                        }
                    }
                    _dbcontext.RemoveRange(transactionGeneric.Details);
                    _dbcontext.Entry<TransactionGeneric>(transactionGeneric).State = EntityState.Deleted;

                    transactionGeneric.TransactionIsOk = true;
                    transactionGeneric.MessageResponse = "Orden Despachada correctamente.";
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();

                }
                else
                {

                }
            }
            else if (transactionGeneric.TransOption == 2)
            {//eliminar orden de despacho .. pero sin descontar de inventario
                if (transactionGeneric.TransactionGenericId != 0)
                {
                    try
                    {
                        _dbcontext.RemoveRange(transactionGeneric.Details);
                        _dbcontext.Entry<TransactionGeneric>(transactionGeneric).State = EntityState.Deleted;
                        transactionGeneric.TransactionIsOk = true;
                        transactionGeneric.MessageResponse = "Orden Eliminada correctamente.";
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                }
            }
            return transactionGeneric;
        }

        public DetailTransactionGeneric Update(DetailTransactionGeneric detail)
        {
            if (detail.TransOption == 1)
            {

            }
            else if (detail.TransOption == 2)//actualizar cantidad de orden de despacho
            {
                _dbcontext.Entry<DetailTransactionGeneric>(detail).State = EntityState.Deleted;
                detail.TransactionIsOk = true;
                detail.MessageResponse = "Item Actualizado correctamente.";
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {
                new DetailTransactionGeneric();
            }
            return detail;
        }

        public List<DTOCardDistpatch> Update(List<DTOCardDistpatch> transactions, long option)
        {
            if (option == 1)
            {
                TransactionGenericBussines transBuss = new TransactionGenericBussines(_configuration, _dbcontext);
                transactions = transBuss.UpdateStockReserve(transactions);
            }
            else
            {

            }
            return transactions;
        }

        public TransactionGeneric SelectById(TransactionGeneric transactionGeneric)
        {
            TransactionGeneric find;
            if (transactionGeneric.TransOption == 0 || transactionGeneric.TransOption == 1)
            {
                find = _dbcontext.TransactionGenerics.Where(x => x.TransactionGenericId == transactionGeneric.TransactionGenericId).FirstOrDefault();
            }
            else if (transactionGeneric.TransOption == 4)
            {
                find = _dbcontext.TransactionGenerics.Where(x => x.DocumentExtern.ToUpper() == transactionGeneric.DocumentExtern.ToUpper()).FirstOrDefault();
            }
            else if (transactionGeneric.TransOption == 3)//Si es para el form de orden de trabajo
            {
                find = _dbcontext.TransactionGenerics.Where(x => x.TransactionGenericId == transactionGeneric.TransactionGenericId).FirstOrDefault();
                find.Details = _dbcontext.DetailTransactionGenerics.Include("Item").Where(x => x.TransactionGenericId == find.TransactionGenericId).ToList();
            }
            else
            {
                find = new TransactionGeneric();
            }
            return find;
        }
        [HttpGet("selectAll")]
        public List<TransactionGeneric> selectAll(TransactionGeneric TransactionGeneric)
        {
            if (TransactionGeneric.TransOption == 1 || TransactionGeneric.TransOption == 0)
            {
                return _dbcontext.TransactionGenerics.ToList();
            }
            else if (TransactionGeneric.TransOption == 2)
            {
                return _dbcontext.TransactionGenerics.Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 1).ToList();
            }
            else if (TransactionGeneric.TransOption == 3)
            {
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 6).ToList();
                foreach (TransactionGeneric tr in list)
                {
                    tr.AUxTercero = _dbcontext.Terceros.Where(x => x.TerceroId == tr.TerceroId).First();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 4)
            {
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 10).ToList();
                foreach (TransactionGeneric tr in list)
                {
                    tr.AUxTercero = _dbcontext.Terceros.Where(x => x.TerceroId == tr.TerceroId).First();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 5)
            {
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 1).ToList();
                foreach (TransactionGeneric tr in list)
                {
                    tr.AUxTercero = _dbcontext.Terceros.Where(x => x.TerceroId == tr.TerceroId).First();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 6)
            {//ORDEN DE TRABAJO -- FORMULARIO BUSQUEDA
                List<TransactionGeneric> list = new List<TransactionGeneric>();
                if (TransactionGeneric.DateReportInit != null && TransactionGeneric.DateReportEnd != null)
                {
                    if (TransactionGeneric.StateTransactionGenericId != 0)
                    {
                        list = _dbcontext.TransactionGenerics.Include("Details.Item").Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 9
                        && x.StateTransactionGenericId == TransactionGeneric.StateTransactionGenericId).ToList();
                    }
                    else
                    {
                        list = _dbcontext.TransactionGenerics.Include("Details.Item").Include(x => x.StateTransactionGeneric).Where(x => x.DateInit >= TransactionGeneric.DateInit && x.DateInit <= TransactionGeneric.DateEnd && x.TypeTransactionId == 9).ToList();
                    }
                }
                else if (TransactionGeneric.StateTransactionGenericId != 0)
                {
                    list = _dbcontext.TransactionGenerics.Include("Details.Item").Include(x => x.StateTransactionGeneric).Where(x => x.TypeTransactionId == 9 && x.StateTransactionGenericId == TransactionGeneric.StateTransactionGenericId).ToList();
                }

                foreach (TransactionGeneric tr in list)
                {
                    tr.AUxTercero = _dbcontext.Terceros.Where(x => x.TerceroId == tr.TerceroId).First();
                    tr.AUxTercero2 = _dbcontext.Terceros.Where(x => x.TerceroId == tr.AuxTerceroId).First();
                    tr.AlertXOrders = _dbcontext.AlertXOrders.Where(x => x.TransactionId == tr.TransactionGenericId).ToList();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 7)
            {//con typetransaction parametrizado
                List<Warehouse> listwarehouses = _dbcontext.Warehouses.ToList();
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include("Details.Item").Where(x => x.TypeTransactionId == TransactionGeneric.TypeTransactionId
                 && x.WarehouseOriginId == TransactionGeneric.WarehouseOriginId).ToList();
                foreach (TransactionGeneric tra in list)
                {
                    tra.Warehouse = listwarehouses.Where(x => x.WarehouseId == tra.WarehouseOriginId).First();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 8)
            {//solo el que tenga el item de la orden de fabricacion
                List<Warehouse> listwarehouses = _dbcontext.Warehouses.ToList();
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include("Details.Item").Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENTRABAJO
                 && x.WarehouseOriginId == TransactionGeneric.WarehouseOriginId
                 && x.Details.Where(x => x.ItemId == TransactionGeneric.AuxTerceroId).Count() > 0).ToList();
                foreach (TransactionGeneric tra in list)
                {
                    tra.Warehouse = listwarehouses.Where(x => x.WarehouseId == tra.WarehouseOriginId).First();
                    tra.Details = tra.Details.Where(x => x.ItemId == TransactionGeneric.AuxTerceroId).ToList();
                }
                return list;
            }
            else if (TransactionGeneric.TransOption == 9)
            { ///Orden de trabajo con items pendientes de fabricacion
                List<Warehouse> listwarehouses = _dbcontext.Warehouses.ToList();
                List<TransactionGeneric> list = _dbcontext.TransactionGenerics.Include("Details.Item").Where(x => x.TypeTransactionId == 11
                 && x.WarehouseOriginId == TransactionGeneric.WarehouseOriginId && x.Details.Where(x => x.InventarioPendiente > 0).Count() > 0).ToList();
                foreach (TransactionGeneric tra in list)
                {
                    tra.Warehouse = listwarehouses.Where(x => x.WarehouseId == tra.WarehouseOriginId).First();
                }
                return list;
            }
            else
            {
                return null;
            }
        }


        [HttpGet("selectAllReport")]
        public List<DTOTransactionReport> selectAllReport(string SDateInit, string SDateEnd, int TransOption)
        {
            List<DTOTransactionReport> datreturn = new List<DTOTransactionReport>();
            TransactionGenericBussines bussin = new TransactionGenericBussines(_dbcontext);
            return bussin.GenerateDataSetToReport(SDateInit, SDateEnd, TransOption);
        }

        [HttpPost("GetDataReportDispatch")]
        public List<DTOTransactionReport> GetDataReportDispatch(DTOTransactionReport data)
        {
            TransactionGenericBussines bussin = new TransactionGenericBussines(_dbcontext);
            return bussin.GenerateDataSetToReport(data);
        }


        public static List<DTOFiltersCompras> filterscompras = new List<DTOFiltersCompras>();
        [HttpPost("GetDataReport")]
        public List<DTOInventary> GetDataReport(string guidfilter)
        {
            DTOFiltersCompras dtodata = filterscompras.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            if (dtodata != null)
            {
                if (dtodata.TypeReportId == 1)
                {
                    filterscompras.Remove(dtodata);
                    return dtodata.Inventaries;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new List<DTOInventary>();
            }
        }

        public void AddFilterCompras(DTOFiltersCompras data)
        {
            filterscompras.Add(data);
        }
        public List<DetailTransactionGeneric> AgregarItems(List<DetailTransactionGeneric> list)
        {
            try
            {
                List<DetailTransactionGeneric> listclone = list.Clone<DetailTransactionGeneric>();
                list.ForEach(x => x.ChanguedView = false);

                if (listclone.Count() > 0)
                {
                    long transid = (long)listclone.First().TransactionGenericId;
                    long warehouseid = _dbcontext.TransactionGenerics.Where(x => x.TransactionGenericId == transid).First().WarehouseOriginId;
                    List<TransactionGeneric> listOrderFabri = _dbcontext.TransactionGenerics.Include("Details.Item").Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENTRABAJO
                                && x.WarehouseOriginId == warehouseid).ToList();

                    foreach (DetailTransactionGeneric det in listclone)
                    {
                        det.Item = _dbcontext.Items.Where(x => x.ItemId == det.ItemId).First();
                        det.Auxnum = listOrderFabri.Select(x => x.Details.Where(t => t.ItemId == det.ItemId).Sum(h => h.Cant)).Sum();
                    }
                }
                return listclone;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}