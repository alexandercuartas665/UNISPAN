using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DAO
{
    public class DaoQuantify
    {
        SqlConnection conn;
        public DaoQuantify(string ConnectionQuantify)
        {
            conn = new SqlConnection(ConnectionQuantify);
            conn.Open();

        }

        public List<DTOInventary> SelectInventary()
        {
            List<DTOInventary> Listinventarys = new List<DTOInventary>();
            string sql = "SELECT SUM(ISNULL([QForRent],0)) CantStock , ReceiveRequireInspection, pro.Description , pro.PartNumber , pro.Weight "
                + " FROM [ASIRentalManager].[dbo].[StockingLocation] SL "
                + " inner join [dbo].[StockedProduct]  SP on  SL.StockingLocationID=SP.StockingLocationID inner join "
                + " Product pro on pro.ProductID=SP.BaseProductID GROUP BY  ReceiveRequireInspection, pro.Description  , pro.PartNumber , pro.Weight";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOInventary dat = new DTOInventary();
                if (!reader.IsDBNull(0))
                    dat.CantInv = (decimal)reader.GetDouble(0);
                if (!reader.IsDBNull(1))
                    dat.Warehouseid = reader.GetBoolean(1) ? 1 : 0;//1 Es palmira  = 0 es bogota
                if (!reader.IsDBNull(2))
                {
                    dat.ItemName = reader.GetString(2);
                    dat.ItemName = dat.ItemName.Replace("\"", "'");
                }
                if (!reader.IsDBNull(3))
                {
                    dat.Barcode = reader.GetString(3);
                }
                if (!reader.IsDBNull(4))
                {
                    dat.Weight = (decimal)reader.GetDouble(4);
                }
                Listinventarys.Add(dat);
            }

            reader.Close();
            conn.Close();
            return Listinventarys;
        }


        /// <summary>
        /// Foto de inventario bodega arriendo y stock disponible detallado por item
        /// </summary>
        /// <returns></returns>
        public List<SnapshotInventoryQuantify> SelectInventarySnapshot()
        {//BODEGA ALQUILER SIEMPRE ESTA EN LA 0 --BOGOTA

            List<SnapshotInventoryQuantify> Listinventarys = new List<SnapshotInventoryQuantify>();

            // string sql = "SELECT SUM(ISNULL([QForRent],0)) CantStock , ReceiveRequireInspection, pro.Description , pro.PartNumber "
            //+ " FROM [ASIRentalManager].[dbo].[StockingLocation] SL "
            //+ " inner join [dbo].[StockedProduct]  SP on  SL.StockingLocationID=SP.StockingLocationID inner join "
            //+ " Product pro on pro.ProductID=SP.BaseProductID GROUP BY  ReceiveRequireInspection, pro.Description  , pro.PartNumber";

            string sql = "SELECT SUM(ISNULL([QForRent],0)) 'CANT-DISPONIBLE' ,SUM(ISNULL([QOnRent],0)) 'CANT-ALQUILER' " +
                " , ROUND((SUM(ISNULL([QForRent],0)*pro.Weight)/1000),2) 'TON-DISPONIBLE' , ROUND((SUM(ISNULL([QOnRent],0)*pro.Weight)/1000),2) 'TON-ALQUILER' " +
                " ,  ReceiveRequireInspection, pro.Description , pro.PartNumber,HideZeroInvoiceItems "
                + " FROM [ASIRentalManager].[dbo].[StockingLocation] SL "
                + " inner join [dbo].[StockedProduct]  SP on  SL.StockingLocationID=SP.StockingLocationID inner join "
                + " Product pro on pro.ProductID=SP.BaseProductID GROUP BY  ReceiveRequireInspection, pro.Description,pro.PartNumber,HideZeroInvoiceItems";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SnapshotInventoryQuantify dat = new SnapshotInventoryQuantify();
                if (!reader.IsDBNull(0))
                    dat.CantInven = (decimal)reader.GetDouble(0);
                if (!reader.IsDBNull(1))
                    dat.CantRent = (decimal)reader.GetDouble(1);
                if (!reader.IsDBNull(2))
                    dat.CantInvenTon = (decimal)reader.GetDouble(2);
                if (!reader.IsDBNull(3))
                    dat.CantRentTon = (decimal)reader.GetDouble(3);
                if (!reader.IsDBNull(4))
                    dat.ReceiveRequireInspection = reader.GetBoolean(4);//1 Es palmira  = 0 es bogota
                //if (!reader.IsDBNull(5))
                //{
                //    dat.ItemName = reader.GetString(5);
                //    dat.ItemName = dat.ItemName.Replace("\"", "'");
                //}
                if (!reader.IsDBNull(6))
                {
                    dat.SyncodeItem = reader.GetString(6);
                }
                if (!reader.IsDBNull(7))
                    dat.HideZeroInvoiceItems = reader.GetBoolean(7);//1 es bogota , 0 palmira sospecho

                dat.YearInve = DateTime.Now.Year;
                dat.MonthInve = DateTime.Now.Month;
                dat.DateInventary = DateTime.Now.Date;

                Listinventarys.Add(dat);
            }

            reader.Close();
            conn.Close();
            return Listinventarys;
        }

        /// <summary>
        /// Foto de inventario bodega arriendo y stock disponible general
        /// </summary>
        /// <returns></returns>
        public List<SnapshotInventoryQuantify> SelectInventarySnapshotNotItem()
        {//BODEGA ALQUILER SIEMPRE ESTA EN LA 0 --BOGOTA

            List<SnapshotInventoryQuantify> Listinventarys = new List<SnapshotInventoryQuantify>();

            // string sql = "SELECT SUM(ISNULL([QForRent],0)) CantStock , ReceiveRequireInspection, pro.Description , pro.PartNumber "
            //+ " FROM [ASIRentalManager].[dbo].[StockingLocation] SL "
            //+ " inner join [dbo].[StockedProduct]  SP on  SL.StockingLocationID=SP.StockingLocationID inner join "
            //+ " Product pro on pro.ProductID=SP.BaseProductID GROUP BY  ReceiveRequireInspection, pro.Description  , pro.PartNumber";

            string sql = "SELECT SUM(ISNULL([QForRent],0)) 'CANT-DISPONIBLE' ,SUM(ISNULL([QOnRent],0)) 'CANT-ALQUILER' " +
                " , ROUND((SUM(ISNULL([QForRent],0)*pro.Weight)/1000),2) 'TON-DISPONIBLE' , ROUND((SUM(ISNULL([QOnRent],0)*pro.Weight)/1000),2) 'TON-ALQUILER' " +
                " ,  ReceiveRequireInspection,HideZeroInvoiceItems "
                + " FROM [ASIRentalManager].[dbo].[StockingLocation] SL "
                + " inner join [dbo].[StockedProduct]  SP on  SL.StockingLocationID=SP.StockingLocationID inner join "
                + " Product pro on pro.ProductID=SP.BaseProductID GROUP BY  ReceiveRequireInspection,HideZeroInvoiceItems";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SnapshotInventoryQuantify dat = new SnapshotInventoryQuantify();
                if (!reader.IsDBNull(0))
                    dat.CantInven = (decimal)reader.GetDouble(0);
                if (!reader.IsDBNull(1))
                    dat.CantRent = (decimal)reader.GetDouble(1);
                if (!reader.IsDBNull(2))
                    dat.CantInvenTon = (decimal)reader.GetDouble(2);
                if (!reader.IsDBNull(3))
                    dat.CantRentTon = (decimal)reader.GetDouble(3);
                if (!reader.IsDBNull(4))
                    dat.ReceiveRequireInspection = reader.GetBoolean(4);//1 Es palmira  = 0 es bogota
                //if (!reader.IsDBNull(5))
                //{
                //    dat.ItemName = reader.GetString(5);
                //    dat.ItemName = dat.ItemName.Replace("\"", "'");
                //}
                //if (!reader.IsDBNull(6))
                //{
                //    dat.SyncodeItem = reader.GetString(6);
                //}
                if (!reader.IsDBNull(5))
                    dat.HideZeroInvoiceItems = reader.GetBoolean(5);//1 es bogota , 0 palmira sospecho

                dat.YearInve = DateTime.Now.Year;
                dat.MonthInve = DateTime.Now.Month;
                dat.DateInventary = DateTime.Now.Date;

                Listinventarys.Add(dat);
            }

            reader.Close();
            conn.Close();
            return Listinventarys;
        }

        //Movimiento de equipos sin item
        #region MovimientoEquipos

        public List<DTORptMovEquipo> SelectMovEquiposSegundaOpcion(DTOFiltersCompras filter)
        {
            string yearfilter = string.Empty;
            foreach (long d in filter.multipleValuesYear.ToList())
            {
                yearfilter += d.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(yearfilter))
                yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);
            string mesfilter = string.Empty;
            foreach (long d in filter.multipleValuesMonth.ToList())
            {
                mesfilter += d.ToString() + ",";
            }
            mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);


            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();

            string sqlREM = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, RentStartDate) MES , DATEPART(YEAR, RentStartDate) ANO "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                + " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ") AND " +
                " AND DATEPART(MONTH, RentStartDate) IN (" + mesfilter + ") AND  ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 "
                + "  GROUP BY  DATEPART(MONTH, RentStartDate), DATEPART(YEAR, RentStartDate) order by  DATEPART(YEAR, RentStartDate), DATEPART(MONTH, RentStartDate) ";

            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalPeso = reader.GetDecimal(0);
                mv.Mes = reader.GetInt32(1);
                mv.Ano = reader.GetInt32(2);
                mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                mv.TipoDocument = "DESPACHOS";
                movs.Add(mv);
            }
            reader.Close();

            string sqlDEV = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                  + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1 "
                + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            cmddev.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmddev.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalPeso = readerdev.GetDecimal(0);
                mv.Mes = readerdev.GetInt32(1);
                mv.Ano = readerdev.GetInt32(2);
                mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                mv.TipoDocument = "DEVOLUCIONES";
                movs.Add(mv);
            }
            readerdev.Close();


            string sqlREP = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
             " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
             + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
             " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
               + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1 "
             + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            cmdrep.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmdrep.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalPeso = readerrep.GetDecimal(0);
                mv.Mes = readerrep.GetInt32(1);
                mv.Ano = readerrep.GetInt32(2);
                mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                mv.TipoDocument = "CIERRE";
                movs.Add(mv);
            }
            readerrep.Close();

            foreach (string TipoDocument in movs.Select(x => x.TipoDocument).Distinct())
            {
                foreach (int ano in movs.Select(x => x.Ano).Distinct())
                {
                    foreach (DTOMonth d in listmonths.OrderBy(x => x.IdMonth).ToList())
                    {
                        DTORptMovEquipo res = movs.Where(x => x.Mes == d.IdMonth && x.Ano == ano
                         && x.TipoDocument == TipoDocument).FirstOrDefault();
                        if (res == null)
                        {
                            DTORptMovEquipo newmov = new DTORptMovEquipo(); ;
                            newmov.TotalPeso = 0;
                            newmov.Mes = (int)d.IdMonth;
                            newmov.Ano = ano;
                            newmov.ObjMes = d;
                            newmov.TipoDocument = TipoDocument;
                            newmov.EtiquetaMesAno = newmov.ObjMes.NameAbrev + "-" + newmov.Ano;
                            movs.Add(newmov);

                        }
                    }
                }
            }

            return movs;

        }

        /// <summary>
        /// Mensual
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTORptMovEquipo> SelectMovEquipos(DTOFiltersCompras filter)
        {
            string yearfilter = string.Empty;
            foreach (long d in filter.multipleValuesYear.ToList())
            {
                yearfilter += d.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(yearfilter))
                yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);
            string mesfilter = string.Empty;
            foreach (long d in filter.multipleValuesMonth.ToList())
            {
                mesfilter += d.ToString() + ",";
            }
            mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);


            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, RentStartDate) MES , DATEPART(YEAR, RentStartDate) ANO "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                + " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH, RentStartDate) IN (" + mesfilter + ") AND  ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 "
                + "  GROUP BY  DATEPART(MONTH, RentStartDate), DATEPART(YEAR, RentStartDate) order by  DATEPART(YEAR, RentStartDate), DATEPART(MONTH, RentStartDate) ";

            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDespacho = (decimal)reader.GetDouble(0);
                mv.Mes = reader.GetInt32(1);
                mv.Ano = reader.GetInt32(2);
                mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                mv.TitleFilterReport = yearfilter;
                movs.Add(mv);
            }
            reader.Close();
            //DEVOLUCIONES
            string sqlDEV = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                  + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1 "
                + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDevolucion = (decimal)readerdev.GetDouble(0);
                mv.Mes = readerdev.GetInt32(1);
                mv.Ano = readerdev.GetInt32(2);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Mes == mv.Mes && x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {
                    mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
             " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
             + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
             " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
               + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1 "
             + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalCierre = (decimal)readerrep.GetDouble(0);
                mv.Mes = readerrep.GetInt32(1);
                mv.Ano = readerrep.GetInt32(2);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Mes == mv.Mes && x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerrep.Close();

            //foreach (int ano in movs.Select(x => x.Ano).Distinct().ToList())
            //{
            //    foreach (DTOMonth d in listmonths.OrderBy(x => x.IdMonth).ToList())
            //    {
            //        DTORptMovEquipo res = movs.Where(x => x.Mes == d.IdMonth && x.Ano == ano).FirstOrDefault();
            //        if (res == null)
            //        {
            //            DTORptMovEquipo newmov = new DTORptMovEquipo(); ;
            //            newmov.Mes = (int)d.IdMonth;
            //            newmov.Ano = ano;
            //            newmov.ObjMes = d;
            //            newmov.TitleFilterReport = yearfilter;
            //            newmov.EtiquetaMesAno = newmov.ObjMes.NameAbrev + "-" + newmov.Ano;
            //            movs.Add(newmov);

            //        }
            //    }
            //}
            return movs;

        }

        /// <summary>
        /// DIARIO
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTORptMovEquipo> SelectMovEquiposDiario(DTOFiltersCompras filter)
        {
            //string yearfilter = string.Empty;
            //foreach (long d in filter.multipleValuesYear.ToList())
            //{
            //    yearfilter += d.ToString() + ",";
            //}
            //if (!string.IsNullOrEmpty(yearfilter))
            //    yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);
            //string mesfilter = string.Empty;
            //foreach (long d in filter.multipleValuesMonth.ToList())
            //{
            //    mesfilter += d.ToString() + ",";
            //}
            //mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);
            string filtertilte = "DESDE " + filter.DateInit.Value.ToString("MMM dd, yyyy") + " HASTA " + filter.DateEnd.Value.ToString("MMM dd, yyyy");

            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  ,  CONVERT(DATE, RentStartDate) DIA  "
                + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  "
                + " where CONVERT(DATE, RentStartDate) BETWEEN CONVERT(DATE,@DATEINIT) " +
                " AND CONVERT(DATE,@DATEEND) AND  ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 "
                + "  GROUP BY    CONVERT(DATE, RentStartDate) order by    CONVERT(DATE, RentStartDate) ";

            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDespacho = (decimal)reader.GetDouble(0);
                mv.DateMov = reader.GetDateTime(1);
                //mv.Ano = reader.GetInt32(2);
                mv.EtiquetaMesAno = mv.DateMov.ToString("MMM dd, yyyy");
                mv.TitleFilterReport = filtertilte;
                movs.Add(mv);
            }
            reader.Close();

            //DEVOLUCIONES
            string sqlDEV = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  ,  CONVERT(DATE, ReturnRentStopDate) DIA  "
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  "
               + " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT) " +
               " AND CONVERT(DATE,@DATEEND) AND OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1  "
               + "  GROUP BY    CONVERT(DATE, ReturnRentStopDate) order by    CONVERT(DATE, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            cmddev.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmddev.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDevolucion = (decimal)readerdev.GetDouble(0);
                mv.DateMov = readerdev.GetDateTime(1);
                mv.EtiquetaMesAno = mv.DateMov.ToString("MMM dd, yyyy");
                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerdev.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {
                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  ,  CONVERT(DATE, ReturnRentStopDate) DIA  "
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  "
               + " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT) " +
               " AND CONVERT(DATE,@DATEEND) and  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1  "
               + "  GROUP BY    CONVERT(DATE, ReturnRentStopDate) order by CONVERT(DATE, ReturnRentStopDate) ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            cmdrep.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmdrep.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalCierre = (decimal)readerrep.GetDouble(0);
                mv.DateMov = readerrep.GetDateTime(1);
                mv.EtiquetaMesAno = mv.DateMov.ToString("MMM dd, yyyy");
                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerrep.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerrep.Close();
            DateTime dateacum = filter.DateInit.Value;
            while (dateacum <= filter.DateEnd.Value)
            {
                DTORptMovEquipo res = movs.Where(x => x.DateMov == dateacum).FirstOrDefault();
                if (res == null)
                {
                    DTORptMovEquipo newmov = new DTORptMovEquipo(); ;
                    newmov.DateMov = dateacum;
                    newmov.TitleFilterReport = filtertilte;
                    newmov.EtiquetaMesAno = newmov.DateMov.ToString("MMM dd, yyyy");
                    //  newmov.EtiquetaMesAno = newmov.ObjMes.NameAbrev + "-" + newmov.Ano;
                    movs.Add(newmov);
                }
                dateacum = dateacum.AddDays(1);
            }

            return movs;

        }


        public List<DTORptMovEquipo> SelectMovEquiposAnual(DTOFiltersCompras filter)
        {
            string yearfilter = string.Empty;
            foreach (long d in filter.multipleValuesYear.ToList())
            {
                yearfilter += d.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(yearfilter))
                yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);
            string mesfilter = string.Empty;
            foreach (long d in filter.multipleValuesMonth.ToList())
            {
                mesfilter += d.ToString() + ",";
            }
            mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);


            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, RentStartDate) MES , DATEPART(YEAR, RentStartDate) ANO "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                + " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH, RentStartDate) IN (" + mesfilter + ") AND  ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 "
                + "  GROUP BY  DATEPART(MONTH, RentStartDate), DATEPART(YEAR, RentStartDate) order by  DATEPART(YEAR, RentStartDate), DATEPART(MONTH, RentStartDate) ";

            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDespacho = (decimal)reader.GetDouble(0);
                mv.Mes = reader.GetInt32(1);
                mv.Ano = reader.GetInt32(2);
                mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                mv.EtiquetaMesAno = mv.Ano.ToString();
                mv.TitleFilterReport = yearfilter;
                movs.Add(mv);
            }
            reader.Close();
            //DEVOLUCIONES
            string sqlDEV = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                  + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1 "
                + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDevolucion = (decimal)readerdev.GetDouble(0);
                mv.Mes = readerdev.GetInt32(1);
                mv.Ano = readerdev.GetInt32(2);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Mes == mv.Mes && x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {
                    mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    mv.EtiquetaMesAno = mv.Ano.ToString();
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
             " DATEPART(MONTH, ReturnRentStopDate) MES ,  DATEPART(YEAR, ReturnRentStopDate) ANO  "
             + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
             " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
               + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND DATEPART(MONTH,ReturnRentStopDate)  IN (" + mesfilter + ") and  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1 "
             + "  GROUP BY DATEPART(MONTH, ReturnRentStopDate), DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate)  , DATEPART(MONTH, ReturnRentStopDate) ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalCierre = (decimal)readerrep.GetDouble(0);
                mv.Mes = readerrep.GetInt32(1);
                mv.Ano = readerrep.GetInt32(2);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Mes == mv.Mes && x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    mv.EtiquetaMesAno = mv.Ano.ToString();
                    movs.Add(mv);
                }
            }
            readerrep.Close();

            return movs;

        }


        /// <summary>
        /// QUEDO EN DESHUSO
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTORptMovEquipo> SelectMovEquiposAnual2(DTOFiltersCompras filter)
        {
            string yearfilter = string.Empty;
            foreach (long d in filter.multipleValuesYear.ToList())
            {
                yearfilter += d.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(yearfilter))
                yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);
            //string mesfilter = string.Empty;
            //foreach (long d in filter.multipleValuesMonth.ToList())
            //{
            //    mesfilter += d.ToString() + ",";
            //}
            //mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);


            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                " DATEPART(YEAR, RentStartDate) ANO "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                + " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ")  " +
                " AND  ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 "
                + "  GROUP BY DATEPART(YEAR, RentStartDate) order by  DATEPART(YEAR, RentStartDate) ";

            //List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDespacho = (decimal)reader.GetDouble(0);
                mv.Ano = reader.GetInt32(1);
                mv.EtiquetaMesAno = mv.Ano.ToString();
                mv.TitleFilterReport = yearfilter;
                movs.Add(mv);
            }
            reader.Close();
            //DEVOLUCIONES
            string sqlDEV = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
                "  DATEPART(YEAR, ReturnRentStopDate) ANO  "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
                " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
                  + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1 "
                + "  GROUP BY DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalDevolucion = (decimal)readerdev.GetDouble(0);
                mv.Ano = readerdev.GetInt32(1);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {
                    mv.EtiquetaMesAno = mv.Ano.ToString();
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT ROUND((SUM(Prod.Weight*ShiPro.SentQuantity)/1000),2) TON  , " +
             "  DATEPART(YEAR, ReturnRentStopDate) ANO  "
             + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro " +
             " ON Shi.ShipmentID=ShiPro.ShipmentID  INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID "
               + " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ")  " +
                " AND OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1 "
             + "  GROUP BY DATEPART(YEAR, ReturnRentStopDate) order by DATEPART(YEAR, ReturnRentStopDate) ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.TotalCierre = (decimal)readerrep.GetDouble(0);
                mv.Ano = readerrep.GetInt32(1);
                mv.TitleFilterReport = yearfilter;
                DTORptMovEquipo mvlis = movs.Where(x => x.Ano == mv.Ano).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    mv.EtiquetaMesAno = mv.Ano.ToString();
                    movs.Add(mv);
                }
            }
            readerrep.Close();

            foreach (long d in filter.multipleValuesYear)
            {
                DTORptMovEquipo res = movs.Where(x => x.Ano == d).FirstOrDefault();
                if (res == null)
                {
                    DTORptMovEquipo newmov = new DTORptMovEquipo();
                    newmov.Ano = (int)d;
                    newmov.TitleFilterReport = yearfilter;
                    newmov.EtiquetaMesAno = newmov.Ano.ToString();
                    movs.Add(newmov);
                }
            }
            return movs;

        }

        #endregion

        public List<DTOYear> ListYearsMovAlquilerEquipos()
        {
            List<DTOYear> listm = new List<DTOYear>();
            string sql = "  SELECT DISTINCT DATEPART(YEAR, RentStartDate) ANO FROM [dbo].[Shipment] Shi where RentStartDate is not null";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOYear a = new DTOYear();
                a.Name = reader.GetInt32(0).ToString();
                a.IdYear = long.Parse(a.Name);
                listm.Add(a);
            }
            return listm.OrderByDescending(x => x.IdYear).ToList();
        }



        /// <summary>
        /// Este metodo retorno los movimientos de equipos agrupados por item .. no por fechas
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTORptMovEquipo> SelectMovEquiposDetalladoXItem(DTOFiltersCompras filter)
        {
            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            string yearfilter = string.Empty;
            string mesfilter = string.Empty;
            string mesfilterName = string.Empty;
            if (filter.TypeReportId == 5 || filter.TypeReportId == 6)
            {
                foreach (long d in filter.multipleValuesYear.ToList())
                {
                    yearfilter += d.ToString() + ",";
                }
                if (!string.IsNullOrEmpty(yearfilter))
                    yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);

                foreach (long d in filter.multipleValuesMonth.ToList())
                {
                    mesfilterName += listmonths.Where(x => x.IdMonth == d).First().NameAbrev + ",";
                    mesfilter += d.ToString() + ",";
                }
                mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);
                mesfilterName = mesfilterName.Substring(0, mesfilterName.Length - 1);
            }
            string filtertilte = string.Empty;

            if (filter.TypeReportId == 4)//diario
                filtertilte = "DESDE " + filter.DateInit.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co")) + " HASTA " + filter.DateEnd.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co"));
            if (filter.TypeReportId == 5)//mensual
                filtertilte = "AÑOS (" + yearfilter + ") MESES (" + mesfilterName + ") ";
            if (filter.TypeReportId == 6)//anual
                filtertilte = "AÑOS (" + yearfilter + ")";

            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT Prod.PartNumber, Prod.Description , SUM(ISNULL(ShiPro.SentQuantity,0)) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ISNULL(ShiPro.SentQuantity,0))/1000),2) TON "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID   ";
            if (filter.TypeReportId == 4)
                sqlREM += " where CONVERT(DATE, RentStartDate) BETWEEN CONVERT(DATE,@DATEINIT) AND CONVERT(DATE,@DATEEND)  AND ";
            if (filter.TypeReportId == 5)
                sqlREM += " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ") AND DATEPART(MONTH, RentStartDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 6)
                sqlREM += " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ") AND  ";

            sqlREM += " ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13  "
                + "   GROUP BY  Prod.PartNumber, Prod.Description ";

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                if (!reader.IsDBNull(0))
                    mv.CodigoItem = reader.GetString(0);
                else
                    mv.CodigoItem = "";
                if (!reader.IsDBNull(1))
                    mv.DescripcionItem = reader.GetString(1).Replace("\"", "'");
                else
                    mv.DescripcionItem = "";
                if (!reader.IsDBNull(2))
                    mv.CantDespacho = (decimal)reader.GetDouble(2);
                else
                    mv.CantDespacho = 0;
                if (!reader.IsDBNull(3))
                    mv.TotalDespacho = (decimal)reader.GetDouble(3);
                else
                    mv.TotalDespacho = 0;

                mv.TitleFilterReport = filtertilte;
                movs.Add(mv);
            }
            reader.Close();

            //DEVOLUCIONES
            string sqlDEV = "SELECT Prod.PartNumber, Prod.Description , SUM(ISNULL(ShiPro.SentQuantity,0)) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ISNULL(ShiPro.SentQuantity,0))/1000),2) TON   "
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  ";

            if (filter.TypeReportId == 4)
                sqlDEV += " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT)  AND CONVERT(DATE,@DATEEND) AND ";
            if (filter.TypeReportId == 5)
                sqlDEV += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND DATEPART(MONTH, ReturnRentStopDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 6)
                sqlDEV += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND  ";

            sqlDEV += " OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1  "
               + "  GROUP BY  Prod.PartNumber, Prod.Description  ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            cmddev.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmddev.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.CodigoItem = readerdev.GetString(0);
                mv.DescripcionItem = readerdev.GetString(1);
                mv.CantDevolucion = (decimal)readerdev.GetDouble(2);

                mv.TotalDevolucion = (decimal)readerdev.GetDouble(3);

                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerdev.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov && x.CodigoItem == mv.CodigoItem).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.CantDevolucion = mv.CantDevolucion;
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {
                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT  Prod.PartNumber, Prod.Description , SUM(ShiPro.SentQuantity) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ShiPro.SentQuantity)/1000),2) TON "
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  ";

            if (filter.TypeReportId == 4)
                sqlREP += " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT)  AND CONVERT(DATE,@DATEEND) AND ";
            if (filter.TypeReportId == 5)
                sqlREP += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND DATEPART(MONTH, ReturnRentStopDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 6)
                sqlREP += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND  ";

            sqlREP += "  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1  "
               + "  GROUP BY  Prod.PartNumber, Prod.Description ";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            cmdrep.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmdrep.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.CodigoItem = readerrep.GetString(0);
                mv.DescripcionItem = readerrep.GetString(1);
                mv.CantCierre = (decimal)readerrep.GetDouble(2);

                mv.TotalCierre = (decimal)readerrep.GetDouble(3);
                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerrep.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov && x.CodigoItem == mv.CodigoItem).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.CantCierre = mv.CantCierre;
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerrep.Close();



            return movs;

        }

        public List<DTORptMovEquipo> SelectMovEquiposConItemDetalladoPorFecha(DTOFiltersCompras filter)
        {
            List<DTOMonth> listmonths = DTOViewRptCompra.GetMonths();

            string yearfilter = string.Empty;
            string mesfilter = string.Empty;
            string mesfilterName = string.Empty;
            if (filter.TypeReportId == 8 || filter.TypeReportId == 9)
            {
                foreach (long d in filter.multipleValuesYear.ToList())
                {
                    yearfilter += d.ToString() + ",";
                }
                if (!string.IsNullOrEmpty(yearfilter))
                    yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);

                foreach (long d in filter.multipleValuesMonth.ToList())
                {
                    mesfilterName += listmonths.Where(x => x.IdMonth == d).First().NameAbrev + ",";
                    mesfilter += d.ToString() + ",";
                }
                mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);
                mesfilterName = mesfilterName.Substring(0, mesfilterName.Length - 1);
            }
            string filtertilte = string.Empty;

            if (filter.TypeReportId == 7)//diario
                filtertilte = "DESDE " + filter.DateInit.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co")) + " HASTA " + filter.DateEnd.Value.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co"));
            if (filter.TypeReportId == 8)//mensual
                filtertilte = "AÑOS (" + yearfilter + ") MESES (" + mesfilterName + ") ";
            if (filter.TypeReportId == 9)//anual
                filtertilte = "AÑOS (" + yearfilter + ")";

            List<DTORptMovEquipo> movs = new List<DTORptMovEquipo>();
            //DESPACHO
            string sqlREM = "SELECT Prod.PartNumber, Prod.Description , SUM(ISNULL(ShiPro.SentQuantity,0)) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ISNULL(ShiPro.SentQuantity,0))/1000),2) TON , CONVERT(DATE, RentStartDate) DIA  "
                + " FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID   ";
            if (filter.TypeReportId == 7)
                sqlREM += " where CONVERT(DATE, RentStartDate) BETWEEN CONVERT(DATE,@DATEINIT) AND CONVERT(DATE,@DATEEND)  AND ";
            if (filter.TypeReportId == 8)
                sqlREM += " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ") AND DATEPART(MONTH, RentStartDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 9)
                sqlREM += " where DATEPART(YEAR, RentStartDate) IN (" + yearfilter + ") AND  ";

            sqlREM += " ShipmentType=0  and IsBillable=1 and ShipmentStatus!=13 and Prod.PartNumber='" + filter.Item + "'"
                + "   GROUP BY  Prod.PartNumber, Prod.Description , CONVERT(DATE, RentStartDate) ";

            SqlCommand cmd = new SqlCommand(sqlREM, conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                if (!reader.IsDBNull(0))
                    mv.CodigoItem = reader.GetString(0);
                else
                    mv.CodigoItem = "";
                if (!reader.IsDBNull(1))
                    mv.DescripcionItem = reader.GetString(1).Replace("\"", "'");
                else
                    mv.DescripcionItem = "";
                if (!reader.IsDBNull(2))
                    mv.CantDespacho = (decimal)reader.GetDouble(2);
                else
                    mv.CantDespacho = 0;
                if (!reader.IsDBNull(3))
                    mv.TotalDespacho = (decimal)reader.GetDouble(3);
                else
                    mv.TotalDespacho = 0;
                mv.DateMov = reader.GetDateTime(4);
                mv.TitleFilterReport = filtertilte;

                movs.Add(mv);
            }
            reader.Close();

            //DEVOLUCIONES
            string sqlDEV = "SELECT Prod.PartNumber, Prod.Description , SUM(ISNULL(ShiPro.SentQuantity,0)) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ISNULL(ShiPro.SentQuantity,0))/1000),2) TON  , CONVERT(DATE, ReturnRentStopDate)  "
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  ";

            if (filter.TypeReportId == 7)
                sqlDEV += " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT)  AND CONVERT(DATE,@DATEEND) AND ";
            if (filter.TypeReportId == 8)
                sqlDEV += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND DATEPART(MONTH, ReturnRentStopDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 9)
                sqlDEV += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND  ";

            sqlDEV += " OutOfServiceLostQuantity IS NULL and ShipmentStatus!=13 AND ShipmentType=1 and Prod.PartNumber='" + filter.Item + "'"
               + "  GROUP BY  Prod.PartNumber, Prod.Description  , CONVERT(DATE, ReturnRentStopDate) ";

            SqlCommand cmddev = new SqlCommand(sqlDEV, conn);
            cmddev.CommandType = CommandType.Text;
            cmddev.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmddev.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerdev = cmddev.ExecuteReader();
            while (readerdev.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.CodigoItem = readerdev.GetString(0);
                mv.DescripcionItem = readerdev.GetString(1);
                mv.CantDevolucion = (decimal)readerdev.GetDouble(2);

                mv.TotalDevolucion = (decimal)readerdev.GetDouble(3);
                mv.DateMov = readerdev.GetDateTime(4);
                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerdev.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov && x.CodigoItem == mv.CodigoItem).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.CantDevolucion = mv.CantDevolucion;
                    mvlis.TotalDevolucion = mv.TotalDevolucion;
                }
                else
                {

                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerdev.Close();

            //devolucion reposicion
            string sqlREP = "SELECT  Prod.PartNumber, Prod.Description , SUM(ShiPro.SentQuantity) Cant , ROUND((SUM(ISNULL(Prod.Weight,0)*ShiPro.SentQuantity)/1000),2) TON  , CONVERT(DATE, ReturnRentStopDate)"
               + "FROM [dbo].[Shipment] Shi INNER JOIN  [dbo].[ShipmentProduct] ShiPro   " +
               " ON Shi.ShipmentID=ShiPro.ShipmentID INNER JOIN [dbo].[Product] Prod ON Prod.ProductID=ShiPro.BaseProductID  ";

            if (filter.TypeReportId == 7)
                sqlREP += " where CONVERT(DATE, ReturnRentStopDate) BETWEEN CONVERT(DATE,@DATEINIT)  AND CONVERT(DATE,@DATEEND) AND ";
            if (filter.TypeReportId == 8)
                sqlREP += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND DATEPART(MONTH, ReturnRentStopDate) IN (" + mesfilter + ") AND ";
            if (filter.TypeReportId == 9)
                sqlREP += " where DATEPART(YEAR, ReturnRentStopDate) IN (" + yearfilter + ") AND  ";

            sqlREP += "  OutOfServiceLostQuantity IS  NOT NULL and ShipmentStatus!=13 AND ShipmentType=1 and Prod.PartNumber='" + filter.Item + "'"
               + "  GROUP BY  Prod.PartNumber, Prod.Description , CONVERT(DATE, ReturnRentStopDate)";

            SqlCommand cmdrep = new SqlCommand(sqlREP, conn);
            cmdrep.CommandType = CommandType.Text;
            cmdrep.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmdrep.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            SqlDataReader readerrep = cmdrep.ExecuteReader();
            while (readerrep.Read())
            {
                DTORptMovEquipo mv = new DTORptMovEquipo();
                mv.CodigoItem = readerrep.GetString(0);
                mv.DescripcionItem = readerrep.GetString(1);
                mv.CantCierre = (decimal)readerrep.GetDouble(2);

                mv.TotalCierre = (decimal)readerrep.GetDouble(3);
                mv.DateMov = readerrep.GetDateTime(4);
                mv.TitleFilterReport = filtertilte;
                //mv.Ano = readerrep.GetInt32(2);
                DTORptMovEquipo mvlis = movs.Where(x => x.DateMov == mv.DateMov && x.CodigoItem == mv.CodigoItem).FirstOrDefault();
                if (mvlis != null)
                {
                    mvlis.CantCierre = mv.CantCierre;
                    mvlis.TotalCierre = mv.TotalCierre;
                }
                else
                {
                    //mv.ObjMes = listmonths.Where(x => x.IdMonth == mv.Mes).First();
                    //mv.EtiquetaMesAno = mv.ObjMes.NameAbrev + "-" + mv.Ano;
                    movs.Add(mv);
                }
            }
            readerrep.Close();

            if (filter.TypeReportId == 7)
            {
                DateTime dateacum = filter.DateInit.Value;
                while (dateacum <= filter.DateEnd.Value)
                {
                    DTORptMovEquipo res = movs.Where(x => x.DateMov == dateacum).FirstOrDefault();
                    if (res == null)
                    {
                        DTORptMovEquipo newmov = new DTORptMovEquipo(); ;
                        newmov.DateMov = dateacum;
                        newmov.TitleFilterReport = filtertilte;
                        newmov.EtiquetaMesAno = newmov.DateMov.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co"));
                        //  newmov.EtiquetaMesAno = newmov.ObjMes.NameAbrev + "-" + newmov.Ano;
                        movs.Add(newmov);
                    }
                    dateacum = dateacum.AddDays(1);
                }
            }

            foreach (DTORptMovEquipo mvo in movs)
            {
                if (filter.TypeReportId == 7)
                {
                    mvo.EtiquetaMesAno = mvo.DateMov.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co")).ToUpper();
                }
                else if (filter.TypeReportId == 8)//MES
                {
                    mvo.EtiquetaMesAno = listmonths.Where(x => x.IdMonth == mvo.DateMov.Month).First().NameAbrev + "-" + mvo.DateMov.Year;
                }
                else if (filter.TypeReportId == 9)
                {
                    mvo.EtiquetaMesAno = mvo.DateMov.Year.ToString();
                }
            }

            return movs;
        }
    }
}
