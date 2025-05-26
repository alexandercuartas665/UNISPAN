using adesoft.adepos.webview.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class MovementInventoryBussines
    {

        public AdeposDBContext _dbcontext { get; set; }
        public MovementInventoryBussines(AdeposDBContext context)
        {
            this._dbcontext = context;
        }
        /// <summary>
        /// desde archivo plano unispan
        /// </summary>
        /// <param name="transactionGeneric"></param>
        public void NuevaEntradaInventarioOfPlainText(TransactionGeneric transactionGeneric)
        {
            List<long> itemids = new List<long>();//esta lista la creee x si en su hoja de inventario se repide el producto efecto del conteo

            if (transactionGeneric != null && transactionGeneric.Details.Count > 0)
            {
                var listmovements = _dbcontext.MovementInventorys.Where(x => x.WarehouseId == transactionGeneric.WarehouseOriginId);
                foreach (MovementInventory m in listmovements)
                {
                    m.CantMov = 0; m.CantNet = 0; m.CantNow = 0;
                    _dbcontext.Entry(m).State = EntityState.Modified;
                }
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                List<MovementInventory> movments = _dbcontext.MovementInventorys.ToList();

                foreach (DetailTransactionGeneric de in transactionGeneric.Details)
                {
                    Item item = _dbcontext.Items.Where(x => x.Barcode.ToUpper() == de.Item.Barcode.ToUpper()).FirstOrDefault();
                    if (item == null)
                    {
                        item = de.Item;
                        _dbcontext.Entry(item).State = EntityState.Added;
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    }
                    else if (item.Weight != de.Item.Weight)
                    {
                        item.Weight = de.Item.Weight;
                        _dbcontext.Entry(item).State = EntityState.Modified;
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    }

                    if (item.HasIventory)
                    {
                        MovementInventory mov = movments.Where(x => x.ItemId == item.ItemId && x.WarehouseId == transactionGeneric.WarehouseOriginId && x.LastRecord).FirstOrDefault();
                        if (itemids.Where(x => x == de.Item.ItemId).Count() > 0)
                        {
                            mov.CantNow += de.Cant;
                            mov.CantNet += de.Cant;
                            if (mov.CantNow < 0)
                            {
                                mov.CantNow = 0;
                                mov.CantNet = 0;
                            }
                        }
                        else
                        {
                            if (mov != null)
                            {
                                // mov.LastRecord = false;
                                //   newmov.ItemId = item.ItemId; 
                                mov.TransactionId = transactionGeneric.TransactionGenericId;
                                mov.TypeTransactionId = transactionGeneric.TypeTransactionId;
                                mov.CantNow = de.Cant;
                                mov.CantNet = de.Cant;//siempre es el ultimo stock
                                                      // newmov.CantNet = newmov.CantNow + newmov.CantMov;
                                if (mov.CantNow < 0)
                                {
                                    mov.CantNow = 0;
                                    mov.CantNet = 0;
                                }
                                mov.DateRecord = DateTime.Now;
                                _dbcontext.Entry(mov).State = EntityState.Modified;
                            }
                            else
                            {
                                MovementInventory newmov = new MovementInventory();
                                newmov.LastRecord = true;
                                newmov.WarehouseId = transactionGeneric.WarehouseOriginId;
                                newmov.ItemId = item.ItemId; newmov.TransactionId = transactionGeneric.TransactionGenericId;
                                newmov.TypeTransactionId = transactionGeneric.TypeTransactionId;
                                newmov.CantNow = de.Cant;
                                newmov.CantNet = de.Cant;//siempre es el ultimo stock
                                if (newmov.CantNow < 0)
                                {
                                    newmov.CantNow = 0;
                                    newmov.CantNet = 0;
                                }
                                newmov.DateRecord = DateTime.Now;
                                _dbcontext.MovementInventorys.Add(newmov);

                                foreach (Warehouse ware in _dbcontext.Warehouses.Where(x => x.WarehouseId != transactionGeneric.WarehouseOriginId
                                && x.WarehouseId != 1).ToList())
                                {
                                    MovementInventory mov2 = movments.Where(x => x.ItemId == item.ItemId && x.WarehouseId == ware.WarehouseId && x.LastRecord).FirstOrDefault();
                                    if (mov2 == null)
                                    {
                                        MovementInventory newmov2 = new MovementInventory();
                                        newmov2.LastRecord = true;
                                        newmov2.WarehouseId = ware.WarehouseId;
                                        newmov2.ItemId = item.ItemId; newmov.TransactionId = transactionGeneric.TransactionGenericId;
                                        newmov2.TypeTransactionId = transactionGeneric.TypeTransactionId;
                                        newmov2.CantMov = 0;
                                        newmov2.CantNow = 0;
                                        newmov2.CantNet = 0;//siempre es el ultimo stock
                                                            //newmov.CantNet = newmov.CantNow + (newmov.CantMov);
                                        newmov2.DateRecord = DateTime.Now;
                                        _dbcontext.MovementInventorys.Add(newmov2);
                                    }
                                }
                            }

                        }
                        itemids.Add(item.ItemId);
                    }
                }
            }
        }

        public void NuevaEntradaInventarioXTransaction(TransactionGeneric transactionGeneric)
        {
            foreach (DetailTransactionGeneric de in transactionGeneric.Details)
            {
                if (de.HasIventory)
                {
                    decimal auxRest = de.Cant;
                    foreach (InventoryXTransaction invxtr in _dbcontext.InventoryXTransactions.Where(x => x.ItemId == de.ItemId && x.CantNet > 0 && x.WarehouseId == transactionGeneric.WarehouseOriginId).OrderByDescending(x => x.DateTransaction).ToList())
                    {
                        if (auxRest > invxtr.CantNet)
                        {
                            auxRest = auxRest - invxtr.CantNet;
                            invxtr.CantOut = invxtr.CantEntry;
                            invxtr.CantNet = 0;
                            //de.PriceCost = invxtr.Costo;
                            _dbcontext.Entry(invxtr).State = EntityState.Modified;

                        }
                        else
                        {
                            invxtr.CantOut = auxRest;
                            invxtr.CantNet = invxtr.CantNet - auxRest;
                            //de.PriceCost = invxtr.Costo;
                            auxRest = 0;
                            _dbcontext.Entry(invxtr).State = EntityState.Modified;

                            break;
                        }

                    }
                    MovementInventory mov = _dbcontext.MovementInventorys.Where(x => x.ItemId == de.ItemId && x.WarehouseId == transactionGeneric.WarehouseOriginId).OrderBy(x => x.DateRecord).LastOrDefault();
                    MovementInventory newmov = new MovementInventory();
                    newmov.LastRecord = true;
                    newmov.WarehouseId = transactionGeneric.WarehouseOriginId;
                    if (mov != null)
                    {
                        mov.LastRecord = false;
                        newmov.ItemId = de.ItemId; newmov.TransactionId = transactionGeneric.TransactionGenericId;
                        newmov.TypeTransactionId = transactionGeneric.TypeTransactionId;
                        newmov.CantMov = -de.Cant;//esto puede ser parametrizado
                        newmov.CantNow = mov.CantNet;
                        newmov.CantNet = newmov.CantNow + newmov.CantMov;
                        newmov.DateRecord = DateTime.Now;
                        //_dbcontext.Entry(mov).State = EntityState.Modified;
                    }
                    else
                    {
                        newmov.ItemId = de.ItemId; newmov.TransactionId = transactionGeneric.TransactionGenericId;
                        newmov.TypeTransactionId = transactionGeneric.TypeTransactionId;
                        newmov.CantMov = -de.Cant;//esto puede ser parametrizado
                        newmov.CantNow = 0;
                        newmov.CantNet = newmov.CantNow + (newmov.CantMov);
                        newmov.DateRecord = DateTime.Now;
                    }
                    _dbcontext.MovementInventorys.Add(newmov);

                }
            }
        }



    }
}
