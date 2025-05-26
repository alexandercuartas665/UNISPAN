using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;

using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Bussines;
using Microsoft.AspNetCore.Authorization;
using adesoft.adepos.webview.Data.DTO;
using Newtonsoft.Json;
using adesoft.adepos.webview.Util;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {

        private AdeposDBContext _dbcontext;
        private readonly TokenAuthenticationStateProvider _tookenState;

        string urlappaux = string.Empty;

        public ItemController(IConfiguration configuration, TokenAuthenticationStateProvider tookenState, IHttpContextAccessor httpContextAccessor)
        {
            urlappaux = configuration.GetValue<string>("UrlBaseReports");
            _tookenState = tookenState;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            if (connectionDB != null)
                _dbcontext = new AdeposDBContext(connectionDB.Connection);
        }


        [HttpPost("UploadFileToInventary")]
        public IActionResult UploadFileToInventary(IFormFile file)
        {
            try
            {

                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public Item Create(Item item)
        {
            Item find = _dbcontext.Items.Where(x => x.Barcode == item.Barcode).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.Items.Add(item);
                _dbcontext.SaveChanges();
                item.ListItemKits.ToList().ForEach(x => x.ItemFatherId = item.ItemId);
                _dbcontext.ItemKits.AddRange(item.ListItemKits);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return item;
        }


        public Item Update(Item item)
        {
            Item find = _dbcontext.Items.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
            if (find != null)
            {

                // find.Name = item.Name;
                //    _dbcontext.Entry<Item>(item).State = EntityState.Modified;
                _dbcontext.Update(item);
                //_dbcontext.Entry(item).State = EntityState.Modified;
                //var updatedEntity = _dbcontext.Set<Item>(item).Update(item);
                List<ItemKit> itemstoRemo = _dbcontext.ItemKits.Where(x => x.ItemFatherId == item.ItemId).ToList();
                _dbcontext.ItemKits.RemoveRange(itemstoRemo);
                // _dbcontext.Entry(itemstoRemo).State = EntityState.Deleted;
                _dbcontext.ItemKits.AddRange(item.ListItemKits);
                // _dbcontext.Entry(item.ListItemKits).State = EntityState.Added;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                //      _dbcontext.Entry(item).State = EntityState.Detached;

            }
            else
            {

            }
            return item;
        }

        public Item SelectById(Item item)
        {
            Item find = new Item();
            if (item.TransOption == 0 || item.TransOption == 1)
            {
                find = _dbcontext.Items.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                if (find != null)
                {
                    find.ListItemKits = _dbcontext.ItemKits.Include(t => t.Item).Where(x => x.ItemFatherId == find.ItemId).ToList();
                    find.ListItemKits.ForEach(x => x.Item.Cant = x.Cant);
                }
            }
            else if (item.TransOption == 2)//por codigo
            {
                find = _dbcontext.Items.Include(x => x.TypeTax).Include(x => x.Category).Where(x => x.Barcode == item.Barcode).FirstOrDefault();
            }
            if (find == null)
            {
                find = new Item();
            }
            return find;
        }

        [HttpPost("selectAll")]
        public List<Item> selectAll(Item item)
        {
            try
            {
                if (item.TransOption == 1 || item.TransOption == 0 || item.TransOption == 2)
                {
                    ItemBussines ItBu = new ItemBussines(_dbcontext);
                    return ItBu.selectAll(item);
                }
                else if (item.TransOption == 3)
                {
                    List<Category> Categorys = _dbcontext.Categorys.ToList();
                    List<Item> items = _dbcontext.Items
                        .Include(i => i.ZoneProduct)
                        .ToList();
                    foreach (Item it in items)
                    {
                        it.CategoryMedicion = Categorys.Where(x => x.CategoryId == it.categoryMedicionId && x.CategoryId != 0).FirstOrDefault();
                    }
                    return items;
                }
                else if (item.TransOption == 4)
                {
                    List<Category> Categorys = _dbcontext.Categorys.ToList();
                    List<Item> items = _dbcontext.Items.Where(x => x.categoryMedicionId == item.categoryMedicionId && x.CategoryId != 0).ToList();
                    foreach (Item it in items)
                    {
                        it.CategoryMedicion = Categorys.Where(x => x.CategoryId == it.categoryMedicionId).FirstOrDefault();
                    }
                    return items;
                }
                else if (item.TransOption == 5)
                {
                    List<Category> Categorys = _dbcontext.Categorys.ToList();
                    List<Item> items = _dbcontext.Items.ToList();
                    foreach (Item it in items)
                    {
                        it.CategoryMedicion = Categorys.Where(x => x.CategoryId == it.categoryMedicionId).FirstOrDefault();
                    }
                    return items;
                }
                else
                {

                    return null;
                }

            }
            catch (Exception ex)
            {
                return new List<Item>();
            }
        }


        //[HttpGet("selectAllItems")]
        //public List<Item> selectAllItems(Item item, int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        ItemBussines ItBu = new ItemBussines(_dbcontext);
        //        return ItBu.selectAllItems(item, pageIndex,pageSize);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<Item>();
        //    }
        //}


        [HttpPost("selectAllItems")]
        public List<Item> selectAllItems(Item item)
        {
            try
            {
                if (item.TransOption == 1 || item.TransOption == 0)
                {
                    ItemBussines ItBu = new ItemBussines(_dbcontext);
                    return ItBu.selectAllItems(item, item.pageIndex, item.pageSize);
                }
                else
                {
                    return new List<Item>();
                }

            }
            catch (Exception ex)
            {
                return new List<Item>();
            }
        }

        public Item CreateItemsFromFile(string file)
        {
            try
            {
                DTOTransaction trans = new DTOTransaction();
                trans.TransactionGenericId = 11;
                trans.AuxTest = file;
                string jsonobj = JsonConvert.SerializeObject(trans);
                Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonobj, urlappaux, "api/Util/ReadOrderOfFile", true);
                res.Wait();
                trans = JsonConvert.DeserializeObject<DTOTransaction>(res.Result);
                trans.AuxTest = null;
                if (trans.TransactionIsOk)
                {
                    foreach (DTOInventary dto in trans.ListItems)
                    {
                        Item item = _dbcontext.Items.Where(x => x.Barcode.ToUpper().Trim() == dto.Barcode.ToUpper().Trim()).FirstOrDefault();
                        if (item != null)
                        {
                            item.Area = dto.Area;
                            item.Weight = dto.Weight;
                            if (dto.WarehouseName == "ACCESORIO")
                            {
                                item.categoryMedicionId = 4;
                            }
                            else if (dto.WarehouseName == "ENCOFRADO")
                            {
                                item.categoryMedicionId = 3;
                            }
                            else
                            {
                                item.categoryMedicionId = 1;
                            }

                            var zoneProduct = _dbcontext.ZoneProducts
                                .Where(x => x.Name == dto.ZoneProduct)
                                .FirstOrDefault();
                            if(zoneProduct != null)
                            {
                                item.ZoneProductId = zoneProduct.Id;
                            }

                            _dbcontext.Entry<Item>(item).State = EntityState.Modified;
                        }
                        else
                        {                            
                            item = new Item()
                            {
                                Referencia = dto.Barcode,
                                Barcode = dto.Barcode,
                                Description = dto.ItemName,
                                HasIventory = true,
                                CategoryId = 2, //generica
                                UnitMeasurementId = 3, //generica
                                TypeTaxId = 2 // ninguna
                            };

                            item.Area = dto.Area;
                            item.Weight = dto.Weight;
                            if (dto.WarehouseName == "ACCESORIO")
                            {
                                item.categoryMedicionId = 4;
                            }
                            else if (dto.WarehouseName == "ENCOFRADO")
                            {
                                item.categoryMedicionId = 3;
                            }
                            else
                            {
                                item.categoryMedicionId = 1;
                            }



                            _dbcontext.Items.Add(item);
                        }
                    }
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                    return new Item() { TransactionIsOk = true, MessageResponse = "Sincronizacion Exitosa" };
                }
                else
                {
                    return new Item() { TransactionIsOk = true, MessageResponse = trans.Message };
                }
            }
            catch (Exception ex)
            {
                return new Item() { TransactionIsOk = true, MessageResponse = "Sincronizacion Fallo" + ex.ToString() };
            }
        }


        public Item ReadEquivalence85(string file)
        {
            try
            {
                List<DTOInventary> trans = new List<DTOInventary>();
                //trans.TransactionGenericId = 11;
                //trans.AuxTest = file;

                string jsonobj = JsonConvert.SerializeObject(file);
                string paramsurl = "?para1=NA&para2=NA";
               // string jsonobj = "{\"FilBase64\": \"" + file + "\" }";
                Task<string> res = HttpAPIClient.PostSendRequestConfigureAwait(jsonobj, urlappaux, "api/Util/ReadOrderOfFile" + paramsurl, true);
                res.Wait();
                trans = JsonConvert.DeserializeObject<List<DTOInventary>>(res.Result);


                foreach (DTOInventary dto in trans)
                {
                    Item item = _dbcontext.Items.Where(x => x.Barcode.ToUpper().Trim() == dto.Barcode.ToUpper().Trim()).FirstOrDefault();
                    if (item != null && dto.Syncode != null)
                    {
                        item.Syncode = dto.Syncode.Trim();
                        item.PrecioSyncode = dto.PriceUnd;
                        _dbcontext.Entry<Item>(item).State = EntityState.Modified;
                    }
                }
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                return new Item() { TransactionIsOk = true, MessageResponse = "Sincronizacion Exitosa" };

            }
            catch (Exception ex)
            {
                return new Item() { TransactionIsOk = true, MessageResponse = "Sincronizacion Fallo" + ex.ToString() };
            }
        }

        //[HttpGet("GetTodo")]
        //public async Task<ActionResult<Item>> GetTodo(long id)
        //{
        //    return _dbcontext.Items.Include(x => x.Category).Include(x => x.TypeTax).ToList();
        //    if (todoItem == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TodoItems.Remove(todoItem);
        //    await _context.SaveChangesAsync();

        //    return todoItem;
        //}

        public List<UnitMeasurement> SelectAllUnitMeasurement(UnitMeasurement model)
        {
            return _dbcontext.UnitMeasurements.ToList();
        }
    }
}
