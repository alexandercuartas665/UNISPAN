using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public static class DTOExtension
    {
        public static DTOOrderPallet ToModel(this OrderPallet orderPallet)
        {
            return new DTOOrderPallet()
            {
                OrderPalletId = orderPallet.Id,
                OrderId = orderPallet.OrderId,
                PalletNo = orderPallet.PalletNo,
                QRData = orderPallet.QRData,
                Status = orderPallet.Status,
                ZoneProduct = orderPallet.ZoneProduct?.ToModel(),
                ZoneProductId = orderPallet.ZoneProductId,
                CreatedOn = orderPallet.CreatedOn
            };
        }

        public static DTOOrderProduct ToModel(this OrderProduct orderProduct)
        {
            return new DTOOrderProduct()
            {
                OrderProductId = orderProduct.Id,
                Description = orderProduct.Description,
                ItemId = orderProduct.ItemId,
                Qty = orderProduct.Qty,
                QtyPending = orderProduct.QtyPending,
                Reference = orderProduct.Reference,
                Weight = orderProduct.Weight,
                OrderId = orderProduct.OrderId,
                ZoneProduct = orderProduct.ZoneProduct?.ToModel()
            };
        }

        public static DTOOrderPalletProduct ToModel(this OrderPalletProduct orderPalletProduct)
        {
            return new DTOOrderPalletProduct()
            {
                OrderPalletProductId = orderPalletProduct.Id,
                OrderPallet = orderPalletProduct.OrderPallet.ToModel(),
                //OrderProduct = orderPalletProduct.OrderProduct.ToModel()                
            };
        }

        public static DTOZoneProduct ToModel(this ZoneProduct zoneProduct)
        {
            return new DTOZoneProduct()
            {
                ZoneProductId = zoneProduct.Id,
                Name = zoneProduct.Name,
                AltName = zoneProduct.AltName,
            };
        }
    }
}
