using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace adesoft.adepos.webview.Bussines
{
    public class ItemBussines
    {

        public AdeposDBContext context { get; set; }
        public ItemBussines(AdeposDBContext context)
        {
            this.context = context;
        }


        public List<Item> selectAll(Item mode)
        {
            if (mode.TransOption == 1 || mode.TransOption == 0)
            {
                return context.Items.Include(x => x.Category).ToList();
            }
            else if (mode.TransOption == 2)
            {
                List<ItemKit> itemsKit = context.ItemKits.Where(x => x.ItemFatherId == mode.ItemId).ToList();
                List<long> itemsKitId = itemsKit.Select(x => x.ItemId).ToList();
                List<Item> resp = context.Items.Where(x => itemsKitId.Contains(x.ItemId)).ToList();
                foreach (Item it in resp)
                {
                    it.Cant = itemsKit.Where(x => x.ItemId == it.ItemId).Sum(x => x.Cant);
                }
                return resp;
            }
            else
            {
                return new List<Item>();
            }
        }

        public List<Item> selectAllItems(Item mode, int pageIndex, int pageSize)
        {
            if (mode.TransOption == 1 || mode.TransOption == 0)//selecione los de un itemkit
            {
                return context.Items.Include(x => x.Category).OrderBy(x => x.ItemId).Skip(pageIndex).Take(pageSize).ToList();
            }
            else
            {
                return new List<Item>();
            }
        }
    }
}
