using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model.PL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet("GetBackOrders")]
        public IActionResult GetBackOrders([FromQuery] OrderType orderType)
        {
            try
            {
                var orders = _plantService.GetBackOrders(orderType, OrderStatus.Draft);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetPickingOrders")]
        public IActionResult GetPickingOrders([FromQuery] OrderType orderType)
        {
            try
            {
                var orders = _plantService.GetPickingOrders(orderType, OrderStatus.Draft);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetPendingOrders")]
        public IActionResult GetPendingOrders([FromQuery] OrderType orderType)
        {
            try
            {
                var orders = _plantService.GetPendingOrders(orderType);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("SyncPallets")]
        public IActionResult SyncPallets([FromBody] List<DTOOrder> orders)
        {
            try
            {
                _plantService.SyncPallets(orders);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetOrderReport")]
        public IActionResult GetOrderReport(string guidfilter)
        {
            try
            {
                var order = _plantService.GetOrderReport(guidfilter);
                return Ok(order);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetOrderPalletReport")]
        public IActionResult GetOrderPalletReport(string guidfilter)
        {
            try
            {
                var orderPallet = _plantService.GetOrderPalletReport(guidfilter);
                return Ok(orderPallet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetZoneProducts")]
        public IActionResult GetZoneProducts()
        {
            try
            {
                var zoneProducts = _plantService.GetZoneProducts();
                return Ok(zoneProducts);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetPickingStatus")]
        public IActionResult GetPickingStatus(string guidfilter)
        {
            try
            {
                var order = this._plantService.GetPickingStatus(guidfilter);
                return Ok(order);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
