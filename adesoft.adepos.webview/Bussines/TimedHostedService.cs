using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;


namespace adesoft.adepos.webview.Bussines
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private Timer _timer;
        readonly IConfiguration _configuration;
        //   string[] companysThread;
        List<ConnectionDB> listconn;
        bool WorkingSyncAll;
        public TimedHostedService(IConfiguration configuration)
        {
            _configuration = configuration;
            WorkingSyncAll = bool.Parse(_configuration["TimeHostedService:Syncronized"].ToString()); // rocampo quitar a la hora de entregar 17/12/2021
            //companysThread = _configuration.GetValue<string>("Parameters:CompanyExecuteTimer").Split(",");
            listconn = SecurityController.GetConnections();
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(25));

            return Task.CompletedTask;
        }

        
        static ConnectionDB connect;
        private async void DoWork(object state)
        {
            //if (companysThread.Length == 0)
            //    Dispose();
            if (!WorkingSyncAll)
            {
                WorkingSyncAll = true;
                //var tasks = companysThread.Select(async compan =>
                //{
                string compan = "UnisPanPro";
                if (connect == null)
                    connect = listconn.Where(x => x.Name.ToUpper() == compan.ToUpper()).FirstOrDefault();

                ReadDocumentsOfPath read = new ReadDocumentsOfPath(_configuration, connect);
                await read.ReadInventoryStockOfWarehouseRent();
                await read.ReadInventoryStockOfQuantify(false);
                await read.ReadInventoryStockOfPath();
                await read.ReadPersonsOfDocument();
                await read.SnapshotInventoryWarehouse();

                //read.UpdateCommercialData();

                ConnectorCRM connectCRM = new ConnectorCRM(_configuration, connect);
                await connectCRM.StartSyncCRM(false);



                //});

                // await Task.WhenAll(tasks);
                WorkingSyncAll = false;
                //foreach (DtoSessionUbication session in sessions)
                //{

            }

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            //  _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
