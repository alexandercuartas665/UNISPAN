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
using adesoft.adepos.webview.Data.DTO;
using Microsoft.AspNetCore.Authorization;

namespace adesoft.adepos.webview.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    public class MovementInventoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public MovementInventoryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public MovementInventory Create(MovementInventory movement)
        {
            //MovementInventory find = _dbcontext.MovementInventorys.Where(x => x.Name == movement.Name).FirstOrDefault();
            //if (find == null)
            //{
            //    _dbcontext.MovementInventorys.Add(movement);
            //    _dbcontext.SaveChanges();
            //    _dbcontext.DetachAll();
            //}
            //else
            //{

            //}
            return movement;
        }


        public MovementInventory Update(MovementInventory movement)
        {
            MovementInventory find = _dbcontext.MovementInventorys.Where(x => x.MovementInventoryId == movement.MovementInventoryId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<MovementInventory>(movement).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return movement;
        }

        public MovementInventory SelectById(MovementInventory movement)
        {
            MovementInventory find = _dbcontext.MovementInventorys.Where(x => x.MovementInventoryId == movement.MovementInventoryId).FirstOrDefault();

            return find;
        }
        public List<MovementInventory> selectAll(MovementInventory MovementInventory)
        {
            return _dbcontext.MovementInventorys.ToList();
        }

        [HttpPost("selectAll")]
        public List<DTOInventary> selectAll(DTOInventary model)
        {
            List<DTOInventary> lissDto = new List<DTOInventary>();
            if (model.TransOption == 2)//unispan
            {
                List<Item> listaux;
                List<MovementInventory> listmovements;
                //ordenes
                List<DetailTransactionGeneric> DetailsTrans = new List<DetailTransactionGeneric>();
                //en fabricacion
                List<DetailTransactionGeneric> DetailsFabrica = new List<DetailTransactionGeneric>();
                List<long> listitemid;
                listaux = _dbcontext.Items.Where(x => x.HasIventory).ToList();
                listitemid = listaux.Select(x => x.ItemId).ToList();
                if (model.Warehouseid != 0)
                {
                    listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == model.Warehouseid && x.LastRecord).ToList();
                    List<List<DetailTransactionGeneric>> lists = _dbcontext.TransactionGenerics.Include(x => x.Details).Where(x => x.WarehouseOriginId == model.Warehouseid && x.TypeTransactionId == CodTypeTransaction.ORDENDESPACHO).Select(x => x.Details).ToList();
                    foreach (var v in lists)
                    {
                        DetailsTrans.AddRange(v);
                    }

                    List<List<DetailTransactionGeneric>> listsfabri = _dbcontext.TransactionGenerics.Include(x => x.Details).Where(x => x.WarehouseOriginId == model.Warehouseid && x.TypeTransactionId == CodTypeTransaction.ORDENTRABAJO).Select(x => x.Details).ToList();
                    foreach (var v in listsfabri)
                    {
                        DetailsFabrica.AddRange(v);
                    }
                }
                else
                {
                    listmovements = _dbcontext.MovementInventorys.Where(x => x.LastRecord).ToList();
                    List<List<DetailTransactionGeneric>> lists = _dbcontext.TransactionGenerics.Include(x => x.Details).Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENDESPACHO).Select(x => x.Details).ToList();
                    foreach (var v in lists)
                    {
                        DetailsTrans.AddRange(v);
                    }

                    List<List<DetailTransactionGeneric>> listsfabri = _dbcontext.TransactionGenerics.Include(x => x.Details).Where(x => x.TypeTransactionId == CodTypeTransaction.ORDENTRABAJO).Select(x => x.Details).ToList();
                    foreach (var v in listsfabri)
                    {
                        DetailsFabrica.AddRange(v);
                    }
                }
                foreach (Item item in listaux)
                {
                    MovementInventory mov = listmovements.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                    if (mov != null)
                    {
                        DTOInventary dto = new DTOInventary();
                        dto.Warehouseid = model.Warehouseid;
                        dto.Barcode = item.Barcode;
                        dto.CantInv = mov.CantNow;
                        dto.Cost = item.PriceCost;
                        dto.ItemId = item.ItemId;
                        //reservada y saldo
                        dto.CantReservada = DetailsTrans.Where(x => x.ItemId == mov.ItemId).Sum(x => x.Cant);
                        dto.Saldo = dto.CantInv - dto.CantReservada;
                        dto.CantFabricacion = DetailsFabrica.Where(x => x.ItemId == mov.ItemId).Sum(x =>  x.Cant);
                        dto.CantPendienteFabricacion = DetailsFabrica.Where(x => x.ItemId == mov.ItemId).Sum(x => x.InventarioPendiente);
                        dto.PriceUnd = item.PrecioDef;
                        dto.ItemName = item.Description;
                        lissDto.Add(dto);
                    }
                    else
                    {
                        DTOInventary dto = new DTOInventary();
                        dto.Warehouseid = model.Warehouseid;
                        dto.Barcode = item.Barcode;
                        dto.CantInv = 0;
                        dto.Cost = item.PriceCost;
                        dto.ItemId = item.ItemId;
                        //reservada y saldo
                     
                        dto.CantReservada = DetailsTrans.Where(x => x.ItemId == item.ItemId).Sum(x => x.Cant);
                        dto.Saldo = dto.CantInv - dto.CantReservada;
                        dto.CantFabricacion = DetailsFabrica.Where(x => x.ItemId == item.ItemId).Sum(x => x.Cant);

                        dto.CantPendienteFabricacion = DetailsFabrica.Where(x => x.ItemId == item.ItemId).Sum(x => x.InventarioPendiente);
                        
                        dto.PriceUnd = item.PrecioDef;
                        dto.ItemName = item.Description;
                        lissDto.Add(dto);
                    }

                    //   dto.ItemId = model.i
                }
            }//fin unispan
            return lissDto;
        }
    }
}