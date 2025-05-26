using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Util;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DAO
{
    public class DaoSqlBiable
    {
        SqlConnection conn;
        public DaoSqlBiable(string ConnectionBiable)
        {
            conn = new SqlConnection(ConnectionBiable);
            conn.Open();
        }


        public List<DTOTercero> SelectAllProveedores()
        {
            List<DTOTercero> Listtercer = new List<DTOTercero>();
            string sql = "SELECT [DESCRIPCION] , [NIT] ,[CODIGO] FROM [dbo].[TERCEROS]  WHERE [IND_PRO]=1 AND NIT!=''";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOTercero ter = new DTOTercero();
                if (!reader.IsDBNull(0))
                    ter.FirstName = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    ter.NumDocument = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    ter.CodeEnterprise = reader.GetString(2);
                Listtercer.Add(ter);
            }

            reader.Close();
            conn.Close();
            return Listtercer;
        }


        public List<DTOYear> SelectAnosCompras()
        {
            List<DTOYear> years = new List<DTOYear>();
            string sql = "SELECT DISTINCT SUBSTRING([LAPSO_DOC],1,4) A FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    DTOYear yea = new DTOYear();
                    yea.IdYear = long.Parse(reader.GetString(0));
                    yea.Name = reader.GetString(0);
                    years.Add(yea);
                }
            }
            if (years.Count > 0)
            {
                years = years.OrderBy(x => x.IdYear).ToList();
            }
            return years;
        }


        public List<DTOYear> SelectAnosMovInventario(int TipoMovId)
        {
            List<DTOYear> years = new List<DTOYear>();
            string sql = "SELECT DISTINCT SUBSTRING([LAPSO_DOC],1,4) A FROM [dbo].[MOVIMIENTO_INVENTARIO] MVI where IND_ES = " + TipoMovId;
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    DTOYear yea = new DTOYear();
                    yea.IdYear = long.Parse(reader.GetString(0));
                    yea.Name = reader.GetString(0);
                    years.Add(yea);
                }
            }
            if (years.Count > 0)
            {
                years = years.OrderBy(x => x.IdYear).ToList();
            }
            return years;
        }


        public List<DTOComprasReport> SelectAllMovInventario(DTOFiltersCompras filter)
        {
            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);

            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            SqlCommand cmdDate = new SqlCommand("SET DATEFORMAT YMD;", conn);
            cmdDate.ExecuteNonQuery();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            //string sql = "SELECT  MVI.DOC_INV_TIPO,MVI.DOCUMENTO_INV, ITE.ID_ITEM,ITE.DESCRIPCION,[LAPSO_DOC]"
            //      + ",SUBSTRING([LAPSO_DOC],1,4) ANO,SUBSTRING([LAPSO_DOC],5,6) MES, FECHA_FC,CANTIDAD_1,COSTO_UNI,COSTOT,MVI.DETALLE_DOC,ID_CO_MOV"
            //      + " FROM [biable01].[dbo].[MOVIMIENTO_INVENTARIO] MVI "
            //      + "  INNER  JOIN[dbo].[ITEMS] ITE ON MVI.ID_ITEM = ITE.ID_ITEM  "
            //      + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
            //      + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
            //      " AND [ID_CO_MOV] IN (" + sede + ") AND [IND_ES]=" + filter.TypeMovementId;
            string sql = "SELECT  MVI.DOC_INV_TIPO,MVI.DOCUMENTO_INV, ITE.ID_ITEM,ITE.DESCRIPCION,[LAPSO_DOC]"
                  + ",SUBSTRING([LAPSO_DOC],1,4) ANO,SUBSTRING([LAPSO_DOC],5,6) MES, FECHA_FC,CANTIDAD_1,COSTO_UNI,COSTOT,MVI.DETALLE_DOC,ID_CO_MOV,ITE.UNIMED_COM"
                  + " FROM [biable01].[dbo].[MOVIMIENTO_INVENTARIO] MVI "
                  + "  INNER JOIN [dbo].[ITEMS] ITE ON MVI.ID_ITEM = ITE.ID_ITEM  "
                  + " WHERE (CONVERT(DATE,FECHA_FC) BETWEEN @DATEINIT AND @DATEEND) " +
                  " AND [ID_CO_MOV] IN (" + sede + ") AND [IND_ES]=" + filter.TypeMovementId;

            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }

            string rangdate = "DESDE " + filter.DateInit.Value.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
            rangdate += " HASTA " + filter.DateEnd.Value.ToString("dd/MMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOComprasReport ter = new DTOComprasReport();
                ter.AuxField = rangdate;
                if (!reader.IsDBNull(0))
                    ter.TipoDoc = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    ter.DocumentoOC = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    ter.IdItem = reader.GetString(2);
                if (!reader.IsDBNull(3))
                    ter.NombreItem = reader.GetString(3);
                if (!reader.IsDBNull(5))
                    ter.Ano = long.Parse(reader.GetString(5));
                if (!reader.IsDBNull(6))
                    ter.NumMes = long.Parse(reader.GetString(6));
                if (!reader.IsDBNull(7))
                {
                    string fechadoc = (reader.GetString(7));
                    ter.FechaDcto = DateTime.ParseExact(fechadoc, "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                }
                if (!reader.IsDBNull(8))
                    ter.VarCant = Math.Abs((reader.GetDecimal(8)));
                if (!reader.IsDBNull(9))
                    ter.valorunit = Math.Abs((reader.GetDecimal(9)));
                if (!reader.IsDBNull(10))
                    ter.VarTotal = Math.Abs((reader.GetDecimal(10)));
                if (!reader.IsDBNull(11))
                    ter.DetalleDoc = (reader.GetString(11));
                if (!reader.IsDBNull(12))
                {
                    ter.IdCo = (reader.GetString(12));
                    if (ter.IdCo == "001")
                    {
                        ter.Sede = "PALMIRA";
                    }
                    else if (ter.IdCo == "002")
                    {
                        ter.Sede = "BOGOTA";
                    }
                }
                ter.NombreMes = DTOViewRptCompra.GetMonths().Where(x => x.IdMonth == ter.NumMes).First().Name;

                if (!reader.IsDBNull(13))
                    ter.IdUnidad = (reader.GetString(13));

                Listcompra.Add(ter);
            }

            reader.Close();
            conn.Close();
            return Listcompra;
        }

        public List<DTOComprasReport> SelectAllMovInventarioMensual(DTOFiltersCompras filter)
        {
            List<DTOMonth> listdtomonth = DTOViewRptCompra.GetMonths();
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

            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);


            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = string.Empty;

            sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],5,6) MES,"
               + " SUM(CANTIDAD_1),AVG(COSTO_UNI),SUM(COSTOT),ITE.UNIMED_COM"
               + " FROM [biable01].[dbo].[MOVIMIENTO_INVENTARIO] MVI "
               + " INNER JOIN [dbo].[ITEMS] ITE ON MVI.ID_ITEM = ITE.ID_ITEM "
               + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
               + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
               " AND ID_CO_MOV IN (" + sede + ") AND [IND_ES]=" + filter.TypeMovementId;

            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }

            sql += " GROUP BY ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],5,6),ITE.UNIMED_COM";




            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOComprasReport ter = new DTOComprasReport();
                ter.AuxField = yearfilter;
                if (!reader.IsDBNull(0))
                    ter.IdItem = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    ter.NombreItem = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    ter.NumMes = long.Parse(reader.GetString(2));
                if (!reader.IsDBNull(3))
                    ter.VarCant = Math.Abs((reader.GetDecimal(3)));
                if (!reader.IsDBNull(4))
                    ter.valorunit = Math.Abs((reader.GetDecimal(4)));
                if (!reader.IsDBNull(5))
                    ter.VarTotal = Math.Abs((reader.GetDecimal(5)));
                if (!reader.IsDBNull(6))
                    ter.IdUnidad = (reader.GetString(6));

                ter.NombreMes = listdtomonth.Where(x => x.IdMonth == ter.NumMes).First().Name;
                Listcompra.Add(ter);
            }
            reader.Close();

            //adiciono los valores en 0
            List<DTOComprasReport> Listprovis = new List<DTOComprasReport>();
            Listprovis.AddRange(Listcompra);
            foreach (DTOComprasReport dti in Listprovis.ToList())
            {
                foreach (long d in filter.multipleValuesMonth.OrderBy(x => x).ToList())
                {
                    DTOComprasReport res = Listcompra.Where(x => x.NumMes == d && x.IdItem == dti.IdItem).FirstOrDefault();
                    if (res == null)
                    {
                        DTOComprasReport clon = (DTOComprasReport)dti.Clone();
                        clon.VarCant = 0; clon.valorunit = 0; clon.VarTotal = 0;
                        clon.NombreMes = listdtomonth.Where(x => x.IdMonth == d).First().Name;
                        clon.NumMes = d; Listcompra.Add(clon);
                    }
                }
            }
            conn.Close();
            return Listcompra;
        }



        public List<DTOComprasReport> SelectAllCompras(DTOFiltersCompras filter)
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


            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);



            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            filter.Proveedores = filter.Proveedores.Select(t => t.Trim()).ToList();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION,[LAPSO_DOC],SUBSTRING([LAPSO_DOC],1,4) ANO, "
                  + " SUBSTRING([LAPSO_DOC],5,6) MES , CANTIDAD ,TOT_BRUTO ,MVOC.ID_TERC , TERC.DESCRIPCION FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC "
                  + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM "
                  + "  INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                  + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                  + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                  " AND ID_CO IN (" + sede + ") ";

            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOComprasReport ter = new DTOComprasReport();
                ter.AuxField = yearfilter;
                if (!reader.IsDBNull(0))
                    ter.IdItem = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    ter.NombreItem = reader.GetString(1);
                if (!reader.IsDBNull(3))
                    ter.Ano = long.Parse(reader.GetString(3));
                if (!reader.IsDBNull(4))
                    ter.NumMes = long.Parse(reader.GetString(4));
                if (!reader.IsDBNull(5))
                    ter.CantMes = (reader.GetDecimal(5));
                if (!reader.IsDBNull(6))
                    ter.CostMes = (reader.GetDecimal(6));
                if (!reader.IsDBNull(7))
                {
                    ter.TerceroId = (reader.GetString(7)).Trim();
                    ter.NombreProveedor = (reader.GetString(8)).Trim();
                    if (!filter.Proveedores.Contains(ter.TerceroId))
                    {
                        continue;
                    }
                }

                ter.NombreMes = DTOViewRptCompra.GetMonths().Where(x => x.IdMonth == ter.NumMes).First().Name;

                Listcompra.Add(ter);
            }

            reader.Close();
            conn.Close();
            return Listcompra;
        }


        public List<DTOComprasReport> SelectAllComprasXProveedor(DTOFiltersCompras filter)
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

            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);


            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            filter.Proveedores = filter.Proveedores.Select(t => t.Trim()).ToList();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION,[LAPSO_DOC],SUBSTRING([LAPSO_DOC],1,4) ANO, "
                + " SUBSTRING([LAPSO_DOC],5,6) MES , CANTIDAD ,TOT_BRUTO ,MVOC.ID_TERC , TERC.DESCRIPCION FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC "
                + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM "
                + "  INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                " AND ID_CO IN (" + sede + ") ";


            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOComprasReport ter = new DTOComprasReport();
                ter.AuxField = yearfilter;
                if (!reader.IsDBNull(0))
                    ter.IdItem = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    ter.NombreItem = reader.GetString(1);
                if (!reader.IsDBNull(3))
                    ter.Ano = long.Parse(reader.GetString(3));
                if (!reader.IsDBNull(4))
                    ter.NumMes = long.Parse(reader.GetString(4));
                if (!reader.IsDBNull(5))
                    ter.CantMes = (reader.GetDecimal(5));
                if (!reader.IsDBNull(6))
                    ter.CostMes = (reader.GetDecimal(6));
                if (!reader.IsDBNull(7))
                {
                    ter.TerceroId = (reader.GetString(7)).Trim();
                    ter.NombreProveedor = (reader.GetString(8)).Trim();
                    if (!filter.Proveedores.Contains(ter.TerceroId))
                    {
                        continue;
                    }
                }

                ter.NombreMes = DTOViewRptCompra.GetMonths().Where(x => x.IdMonth == ter.NumMes).First().Name;

                Listcompra.Add(ter);
            }

            reader.Close();
            conn.Close();
            return Listcompra;
        }

        public List<DTOComprasReport> SelectAllComprasAnual(DTOFiltersCompras filter)
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


            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);



            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            filter.Proveedores = filter.Proveedores.Select(t => t.Trim()).ToList();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = string.Empty;
            if (filter.AddDynamicField)
            {
                sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],1,4) ANO , "
                   + " SUM(CANTIDAD) CANT ,SUM(TOT_BRUTO) TOT ,MVOC.ID_TERC , TERC.DESCRIPCION FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC"
                   + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM  "
                   + " INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                   + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                   + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                   " AND ID_CO IN (" + sede + ") ";
            }
            else
            {

                sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION, SUBSTRING([LAPSO_DOC],1,4) ANO , "
                 + " SUM(CANTIDAD) CANT ,SUM(TOT_BRUTO) TOT   FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC"
                 + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM  "
                 + " INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                 + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                 + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                 " AND ID_CO IN (" + sede + ") ";
            }

            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            if (filter.AddDynamicField)
            {
                sql += " GROUP BY ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],1,4),MVOC.ID_TERC,TERC.DESCRIPCION";
            }
            else
            {
                sql += " GROUP BY ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],1,4)";
            }

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (filter.AddDynamicField)
                {
                    DTOComprasReport ter = new DTOComprasReport();
                    ter.AuxField = yearfilter;
                    if (!reader.IsDBNull(0))
                        ter.IdItem = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        ter.NombreItem = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        ter.Ano = long.Parse(reader.GetString(2));
                    if (!reader.IsDBNull(3))
                        ter.CantMes = (reader.GetDecimal(3));
                    if (!reader.IsDBNull(4))
                        ter.CostMes = (reader.GetDecimal(4));
                    if (!reader.IsDBNull(5))
                    {
                        ter.TerceroId = (reader.GetString(5)).Trim();
                        ter.NombreProveedor = (reader.GetString(6)).Trim();
                        if (!filter.Proveedores.Contains(ter.TerceroId))
                        {
                            continue;
                        }
                    }
                    ter.valorunit = ter.CostMes / ter.CantMes;
                    Listcompra.Add(ter);
                }
                else
                {
                    DTOComprasReport ter = new DTOComprasReport();
                    ter.AuxField = yearfilter;
                    if (!reader.IsDBNull(0))
                        ter.IdItem = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        ter.NombreItem = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        ter.Ano = long.Parse(reader.GetString(2));
                    if (!reader.IsDBNull(3))
                        ter.CantMes = (reader.GetDecimal(3));
                    if (!reader.IsDBNull(4))
                        ter.CostMes = (reader.GetDecimal(4));

                    ter.valorunit = ter.CostMes / ter.CantMes;

                    Listcompra.Add(ter);
                }


                //  ter.NombreMes = DTOViewRptCompra.GetMonths().Where(x => x.IdMonth == ter.NumMes).First().Name;

            }

            //adiciono los valores en 0
            List<DTOComprasReport> Listprovis = new List<DTOComprasReport>();
            Listprovis.AddRange(Listcompra);
            foreach (DTOComprasReport dti in Listprovis.ToList())
            {
                foreach (long d in filter.multipleValuesYear.OrderBy(x => x).ToList())
                {
                    if (!filter.AddDynamicField)
                    {
                        DTOComprasReport res = Listcompra.Where(x => x.Ano == d && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (res == null)
                        {
                            DTOComprasReport clon = (DTOComprasReport)dti.Clone();
                            clon.CantMes = 0; clon.valorunit = 0; clon.CostMes = 0;
                            clon.Ano = d; Listcompra.Add(clon);
                        }
                    }
                    else
                    {
                        DTOComprasReport res = Listcompra.Where(x => x.Ano == d && x.TerceroId == dti.TerceroId && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (res == null)
                        {
                            DTOComprasReport clon = (DTOComprasReport)dti.Clone();
                            clon.CantMes = 0; clon.valorunit = 0; clon.CostMes = 0;
                            clon.Ano = d; Listcompra.Add(clon);
                        }
                    }
                }
            }

            reader.Close();
            conn.Close();
            long minyear = filter.multipleValuesYear.Min();
            foreach (long d in filter.multipleValuesYear.OrderBy(x => x).ToList())
            {
                foreach (DTOComprasReport dti in Listcompra.Where(x => x.Ano == d).ToList())
                {
                    //si es igual a 0 oculte en el reporte porque es hidden
                    dti.visibleyear = filter.multipleValuesYear.Where(x => x == (d - 1)).Count() == 0
                          || !filter.AddVariaciones;

                    if (!filter.AddDynamicField)
                    {
                        //List<DTOComprasReport> dte = Listcompra.Where(x => x.Ano == (d - 1) && x.IdItem == dti.IdItem).ToList();
                        //if (Listcompra.Where(x => x.Ano == (d - 1) && x.IdItem == dti.IdItem).Count() > 1)
                        //{


                        //}
                        DTOComprasReport lastyear = Listcompra.Where(x => x.Ano == (d - 1) && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (lastyear != null)
                        {
                            dti.VarCant = dti.CantMes - lastyear.CantMes;
                            dti.VarTotal = dti.CostMes - lastyear.CostMes;
                            if (lastyear.CostMes != 0)
                                dti.VarTotal2 = ((dti.CostMes / lastyear.CostMes) - 1) * 100;
                            //decimal totalyear = Listcompra.Where(x => x.Ano == d).Sum(x => x.CantMes);
                            //dti.VarTotal2 = dti.VarTotal / totalyear;
                        }
                        else if (d != minyear)
                        {
                            dti.VarCant = dti.CantMes;
                            dti.VarTotal = dti.CostMes;
                            dti.VarTotal2 = 0;
                        }
                    }
                    else
                    {
                        //if (Listcompra.Where(x => x.Ano == (d - 1) && x.TerceroId == dti.TerceroId && x.IdItem == dti.IdItem).Count() > 1)
                        //{

                        //}
                        DTOComprasReport lastyear = Listcompra.Where(x => x.Ano == (d - 1) && x.TerceroId == dti.TerceroId && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (lastyear != null)
                        {
                            dti.VarCant = dti.CantMes - lastyear.CantMes;
                            dti.VarTotal = dti.CostMes - lastyear.CostMes;
                            if (lastyear.CostMes != 0)
                                dti.VarTotal2 = ((dti.CostMes / lastyear.CostMes) - 1) * 100;
                            //decimal totalyear = Listcompra.Where(x => x.Ano == d).Sum(x => x.CantMes);
                            //dti.VarTotal2 = dti.VarTotal / totalyear;
                        }
                        else if (d != minyear)
                        {
                            dti.VarCant = dti.CantMes;
                            dti.VarTotal = dti.CostMes;
                            dti.VarTotal2 = 0;
                        }
                    }
                }
            }
            //    decimal cantt = Listcompra.Where(x => x.Ano == 2019).Sum(x => x.VarCant);
            return Listcompra;
        }
        public List<DTOComprasReport> SelectAllComprasXProveedorAnual(DTOFiltersCompras filter)
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

            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);


            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            filter.Proveedores = filter.Proveedores.Select(t => t.Trim()).ToList();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = string.Empty;
            if (filter.AddDynamicField)
            {
                sql = "SELECT ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],1,4) ANO , "
                   + " SUM(CANTIDAD) CANT ,SUM(TOT_BRUTO) TOT ,MVOC.ID_TERC , TERC.DESCRIPCION FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC"
                   + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM  "
                   + " INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                   + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                   + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                   " AND ID_CO IN (" + sede + ") ";
            }
            else
            {

                sql = "SELECT SUBSTRING([LAPSO_DOC],1,4) ANO , "
                 + " SUM(CANTIDAD) CANT ,SUM(TOT_BRUTO) TOT ,MVOC.ID_TERC , TERC.DESCRIPCION FROM [dbo].[MOVIMIENTO_OCOMPRA] MVOC"
                 + " INNER JOIN[dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM  "
                 + " INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO  "
                 + " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
                 + " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
                 " AND ID_CO IN (" + sede + ") ";
            }

            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            if (filter.AddDynamicField)
            {
                sql += " GROUP BY ITE.ID_ITEM,ITE.DESCRIPCION,SUBSTRING([LAPSO_DOC],1,4),MVOC.ID_TERC,TERC.DESCRIPCION";
            }
            else
            {
                sql += " GROUP BY SUBSTRING([LAPSO_DOC],1,4),MVOC.ID_TERC,TERC.DESCRIPCION";
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (filter.AddDynamicField)
                {
                    DTOComprasReport ter = new DTOComprasReport();
                    ter.AuxField = yearfilter;
                    if (!reader.IsDBNull(0))
                        ter.IdItem = reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        ter.NombreItem = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        ter.Ano = long.Parse(reader.GetString(2));
                    if (!reader.IsDBNull(3))
                        ter.CantMes = (reader.GetDecimal(3));
                    if (!reader.IsDBNull(4))
                        ter.CostMes = (reader.GetDecimal(4));
                    if (!reader.IsDBNull(5))
                    {
                        ter.TerceroId = (reader.GetString(5)).Trim();
                        ter.NombreProveedor = (reader.GetString(6)).Trim();
                        if (!filter.Proveedores.Contains(ter.TerceroId))
                        {
                            continue;
                        }
                    }
                    ter.valorunit = ter.CostMes / ter.CantMes;
                    Listcompra.Add(ter);
                }
                else
                {
                    DTOComprasReport ter = new DTOComprasReport();
                    ter.AuxField = yearfilter;
                    if (!reader.IsDBNull(0))
                        ter.Ano = long.Parse(reader.GetString(0));
                    if (!reader.IsDBNull(1))
                        ter.CantMes = (reader.GetDecimal(1));
                    if (!reader.IsDBNull(2))
                        ter.CostMes = (reader.GetDecimal(2));
                    if (!reader.IsDBNull(3))
                    {
                        ter.TerceroId = (reader.GetString(3)).Trim();
                        ter.NombreProveedor = (reader.GetString(4)).Trim();
                        if (!filter.Proveedores.Contains(ter.TerceroId))
                        {
                            continue;
                        }
                    }
                    ter.valorunit = ter.CostMes / ter.CantMes;
                    Listcompra.Add(ter);
                }
                // ter.NombreMes = DTOViewRptCompra.GetMonths().Where(x => x.IdMonth == ter.NumMes).First().Name;

            }
            reader.Close();
            conn.Close();
            //adiciono los valores en 0
            List<DTOComprasReport> Listprovis = new List<DTOComprasReport>();
            Listprovis.AddRange(Listcompra);
            foreach (DTOComprasReport dti in Listcompra.ToList())
            {
                foreach (long d in filter.multipleValuesYear.OrderBy(x => x).ToList())
                {
                    if (!filter.AddDynamicField)
                    {
                        DTOComprasReport res = Listcompra.Where(x => x.Ano == d && x.TerceroId == dti.TerceroId).FirstOrDefault();
                        if (res == null)
                        {
                            DTOComprasReport clon = (DTOComprasReport)dti.Clone();
                            clon.CantMes = 0; clon.valorunit = 0; clon.CostMes = 0;
                            clon.Ano = d; Listcompra.Add(clon);
                        }
                    }
                    else
                    {
                        DTOComprasReport res = Listcompra.Where(x => x.Ano == d && x.TerceroId == dti.TerceroId && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (res == null)
                        {
                            DTOComprasReport clon = (DTOComprasReport)dti.Clone();
                            clon.CantMes = 0; clon.valorunit = 0; clon.CostMes = 0;
                            clon.Ano = d; Listcompra.Add(clon);
                        }
                    }
                }
            }

            long minyear = filter.multipleValuesYear.Min();
            foreach (long d in filter.multipleValuesYear.OrderBy(x => x).ToList())
            {
                foreach (DTOComprasReport dti in Listcompra.Where(x => x.Ano == d).ToList())
                {
                    //si es igual a 0 oculte en el reporte porque es hidden
                    dti.visibleyear = filter.multipleValuesYear.Where(x => x == (d - 1)).Count() == 0
                         || !filter.AddVariaciones;
                    if (!filter.AddDynamicField)
                    {
                        DTOComprasReport lastyear = Listcompra.Where(x => x.Ano == (d - 1) && x.TerceroId == dti.TerceroId).FirstOrDefault();
                        if (lastyear != null)
                        {
                            dti.VarCant = dti.CantMes - lastyear.CantMes;
                            dti.VarTotal = dti.CostMes - lastyear.CostMes;
                            if (lastyear.CostMes != 0)
                                dti.VarTotal2 = ((dti.CostMes / lastyear.CostMes) - 1) * 100;
                            //decimal totalyear = Listcompra.Where(x => x.Ano == d).Sum(x => x.CantMes);
                            //dti.VarTotal2 = dti.VarTotal / totalyear;
                        }
                        else if (d != minyear)
                        {
                            dti.VarCant = dti.CantMes;
                            dti.VarTotal = dti.CostMes;
                            dti.VarTotal2 = 0;

                        }
                    }
                    else
                    {
                        DTOComprasReport lastyear = Listcompra.Where(x => x.Ano == (d - 1) && x.TerceroId == dti.TerceroId && x.IdItem == dti.IdItem).FirstOrDefault();
                        if (lastyear != null)
                        {
                            dti.VarCant = dti.CantMes - lastyear.CantMes;
                            dti.VarTotal = dti.CostMes - lastyear.CostMes;
                            if (lastyear.CostMes != 0)
                                dti.VarTotal2 = ((dti.CostMes / lastyear.CostMes) - 1) * 100;
                            //decimal totalyear = Listcompra.Where(x => x.Ano == d).Sum(x => x.CantMes);
                            //dti.VarTotal2 = dti.VarTotal / totalyear;
                        }
                        else if (d != minyear)
                        {
                            dti.VarCant = lastyear.CantMes;
                            dti.VarTotal = lastyear.CostMes;
                            dti.VarTotal2 = 0;
                        }
                    }
                }
            }

            return Listcompra;
        }

        public List<DTOComprasReport> SelectAllComprasDetallado(DTOFiltersCompras filter)
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

            bool otroscos = false;
            string local = string.Empty;
            foreach (string d in filter.COs.ToList())
            {
                if (d == "-1")
                {
                    otroscos = true;
                }
                else
                {
                    local += "'" + d.ToString() + "',";
                }
            }
            if (!string.IsNullOrEmpty(local))
                local = local.Substring(0, local.Length - 1);


            string sede = string.Empty;
            foreach (string d in filter.Sedes.ToList())
            {
                sede += "'" + d.ToString() + "',";
            }
            sede = sede.Substring(0, sede.Length - 1);

            filter.Proveedores = filter.Proveedores.Select(t => t.Trim()).ToList();

            SqlCommand cmdDate = new SqlCommand("SET DATEFORMAT YMD;", conn);
            cmdDate.ExecuteNonQuery();

            List<DTOComprasReport> Listcompra = new List<DTOComprasReport>();
            string sql = "SELECT  FECHA_DCTO,DOCUMENTO_OC,ITE.ID_ITEM,ITE.DESCRIPCION,[LAPSO_DOC],SUBSTRING([LAPSO_DOC],1,4) ANO,"
                + "SUBSTRING([LAPSO_DOC],5,6) MES,CANTIDAD,TOT_BRUTO,MVOC.ID_TERC,TERC.DESCRIPCION,MVOC.PARA_USAR,MVOC.ID_LOCAL,MVOC.ID_UNIDAD,"
                + "MVOC.CANTIDAD_ENT,MVOC.TOT_VENTA,MVOC.ESTADO_NOM FROM "
                + "[dbo].[MOVIMIENTO_OCOMPRA] MVOC INNER JOIN [dbo].[ITEMS] ITE ON MVOC.ID_ITEM = ITE.ID_ITEM "
                + "INNER JOIN [dbo].[TERCEROS] TERC ON MVOC.ID_TERC = TERC.CODIGO "
                + "WHERE ID_CO IN (" + sede + ") AND (CONVERT(DATE,FECHA_DCTO) BETWEEN @DATEINIT AND @DATEEND) ";
            //+ " where SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") AND "
            //+ " SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") " +
            //" ";
            if (!string.IsNullOrEmpty(local))
            {
                if (!otroscos)
                    sql += "AND ( ID_LOCAL IN (" + local + "))";
                else
                    sql += "AND ( ID_LOCAL IN (" + local + ") OR ID_LOCAL NOT IN ('00101','00199','00198'))";
            }
            else
            {
                if (otroscos)
                    sql += "AND (ID_LOCAL NOT IN ('00101','00199','00198'))";
            }

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@DATEINIT", filter.DateInit));
            cmd.Parameters.Add(new SqlParameter("@DATEEND", filter.DateEnd));
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOComprasReport ter = new DTOComprasReport();
                if (!reader.IsDBNull(0))
                {
                    ter.FechaDctoLbl = reader.GetString(0);
                    ter.FechaDcto = DateTime.ParseExact(ter.FechaDctoLbl, "yyyyMMdd", null);
                }
                if (!reader.IsDBNull(1))
                    ter.DocumentoOC = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    ter.IdItem = reader.GetString(2);
                if (!reader.IsDBNull(3))
                    ter.NombreItem = reader.GetString(3);

                if (!reader.IsDBNull(7))
                    ter.CantMes = (reader.GetDecimal(7));
                if (!reader.IsDBNull(8))
                    ter.CostMes = (reader.GetDecimal(8));
                if (!reader.IsDBNull(7))
                {
                    ter.TerceroId = (reader.GetString(9)).Trim();
                    ter.NombreProveedor = (reader.GetString(10)).Trim();
                    if (!filter.Proveedores.Contains(ter.TerceroId))
                    {
                        continue;
                    }
                }
                if (!reader.IsDBNull(11))
                    ter.ParaUsar = (reader.GetString(11));
                if (!reader.IsDBNull(12))
                    ter.IdLocal = (reader.GetString(12));
                if (!reader.IsDBNull(13))
                    ter.IdUnidad = (reader.GetString(13));
                if (!reader.IsDBNull(14))
                    ter.CantEntrada = (reader.GetDecimal(14));
                if (!reader.IsDBNull(15))
                    ter.TotalVenta = (reader.GetDecimal(15));
                if (!reader.IsDBNull(16))
                    ter.EstadoNom = (reader.GetString(16));

                Listcompra.Add(ter);
            }

            reader.Close();
            conn.Close();
            return Listcompra;
        }


        public List<DTOParamContable> BuildDataParameter(long yearfilter, AdeposDBContext dbcontext)
        {


            List<DTOParamContable> listdolar = new List<DTOParamContable>();
            Parameter paramt = dbcontext.Parameters.Where(x => x.NameIdentify == "ParametrosMensuales").FirstOrDefault();
            if (paramt.Value2 != null)
            {
                listdolar = JsonConvert.DeserializeObject<List<DTOParamContable>>(paramt.Value2);
                listdolar = listdolar.Where(x => x.year <= yearfilter).ToList();
                foreach (DTOParamContable mont in listdolar)
                {
                    mont.ImptRenta = mont.ImptRenta / 100;
                    mont.Iva = mont.Iva / 100;
                    if (mont.month == 1)
                    {
                        DTOParamContable paramdic = listdolar.Where(x => x.year == (mont.year - 1) && x.month == 12).FirstOrDefault();
                        if (paramdic != null)
                        {
                            mont.cierremesanterior = paramdic.ValueDolar;
                        }
                        else
                        {
                            mont.cierremesanterior = 0;
                        }
                    }
                    else
                    {
                        DTOParamContable paramdic = listdolar.Where(x => x.year == mont.year && x.month == (mont.month - 1)).FirstOrDefault();
                        if (paramdic != null)
                        {
                            mont.cierremesanterior = paramdic.ValueDolar;
                        }
                        else
                        {
                            mont.cierremesanterior = 0;
                        }
                    }

                    if (mont.cierremesanterior != 0)
                    {
                        mont.devaluacionmes = mont.ValueDolar / mont.cierremesanterior - 1;
                    }
                    else
                    {
                        mont.devaluacionmes = 0;
                    }

                }
            }
            return listdolar;
        }

        public List<DTOReportBalanceMonth> SelectDTOReportBalanceMonth(List<DetailReportDynamic> listdeta, DTOFiltersCompras filter, AdeposDBContext dbcontext)
        {
            EventLog evento;
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";

            //  evento.WriteEntry("generando reporte : ", EventLogEntryType.Information);
            string mesfilter = string.Empty;
            foreach (long d in filter.multipleValuesMonth.ToList())
            {
                mesfilter += d.ToString() + ",";
            }
            mesfilter = mesfilter.Substring(0, mesfilter.Length - 1);

            List<DTOReportBalanceMonth> Listyears = ListMonthContable(filter);
            List<DTOReportBalanceMonth> Listdat = new List<DTOReportBalanceMonth>();
            List<DTOParamContable> listdolar = new List<DTOParamContable>();

            long lastyearmonth = Listyears.Max(x => x.OrderMonth);

            try
            {
                listdolar = BuildDataParameter(filter.yearfilter, dbcontext);
            }
            catch (Exception ex)
            {
                evento.WriteEntry("error parametros : " + ex.ToString(), EventLogEntryType.Error);
            }
            try
            {
                if (listdolar.Count == 0)
                {
                    return Listdat;
                }
                //se toma el valor del dolar del ultimo mes
                decimal lastvaluedolar = listdolar.Where(x => x.Yearmonth == lastyearmonth).First().ValueDolar;

                foreach (DetailReportDynamic det in listdeta.OrderBy(x => x.PositionNum))
                {
                    if (det.Type == "ITEM")
                    {
                        //if (det.Description == "SUELDOS Y SALARIOS")
                        //{

                        //}
                        if (!string.IsNullOrEmpty(det.Accounts))
                        {
                            string initaccoun = det.ArrayAccounts[0].Substring(0, 1);
                            det.Accounts = det.Accounts.Replace("PORTERCERO", "");
                            try
                            {
                                if (initaccoun == "1" || initaccoun == "2" || initaccoun == "3")
                                {//CUENTAS DE BALANCE SE SUMAN LOS SALDOS DEL ANO ANTERIOR

                                    string sql = "SELECT SUM([SALDOS_FINAL_L2]) , [LAPSO_DOC] FROM [biable01].[dbo].[CGRESUMEN_CUENTA_TERC] where ID_CUENTA IN(" + det.Accounts + ") " +
                                    " AND SUBSTRING([LAPSO_DOC],1,4) = " + filter.yearfilter + " AND [ID_CO] IN (" + det.Centros + ") ";
                                    if (!string.IsNullOrEmpty(det.NitTercero))
                                    {
                                        sql += " AND ID_TERC = '" + det.NitTercero.Trim() + "' ";
                                    }
                                    sql += " AND SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") ";
                                    sql += "GROUP BY [LAPSO_DOC] order by [LAPSO_DOC]";

                                    SqlCommand cmd = new SqlCommand(sql, conn);
                                    cmd.CommandType = CommandType.Text;
                                    SqlDataReader reader = cmd.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                            m.Description = det.Description;
                                            m.Accounts = det.Accounts;
                                            m.Month = reader.GetString(1);
                                            m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                            m.PositionNum = det.PositionNum;
                                            m.OrderMonth = long.Parse(m.Month);
                                            m.OrderNum = det.OrderNum;
                                            m.Value = reader.GetDecimal(0);
                                            if (initaccoun == "2" || initaccoun == "3")
                                            {
                                                m.Value = Math.Abs(m.Value.Value);
                                            }
                                            //if (filter.Moneda == "DOLAR")
                                            //{
                                            decimal valuedolarm = lastvaluedolar; //listdolar.Where(x => x.Yearmonth == m.OrderMonth).First().ValueDolar;
                                            m.ValueDolar = m.Value / valuedolarm;
                                            //}
                                            m.Type = det.Type;
                                            Listdat.Add(m);
                                        }
                                        reader.Close();
                                    }
                                    else
                                    {
                                        var months = Listyears.Select(x => new { x.OrderMonth, x.Month }).ToList();
                                        foreach (var t in months)
                                        {
                                            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                            m.Description = det.Description;
                                            m.Accounts = det.Accounts;
                                            m.PositionNum = det.PositionNum;
                                            m.Month = t.Month;
                                            m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                            m.OrderMonth = t.OrderMonth;
                                            m.OrderNum = det.OrderNum;
                                            m.Type = det.Type;
                                            Listdat.Add(m);
                                        }
                                    }
                                }
                                else
                                {//RESTO DE CUENTAS SE SUMA MES A MES
                                    det.Accounts = det.Accounts.Replace("DB", "");
                                    string sql = "SELECT (SUM(SALDOS_DEB_MES_L2)-SUM(SALDOS_CRE_MES_L2)) SAL, [LAPSO_DOC] FROM [biable01].[dbo].[CGRESUMEN_CUENTA_TERC] "
                                      + " where ID_CUENTA IN (" + det.Accounts + ") AND SUBSTRING([LAPSO_DOC],1,4) = " + filter.yearfilter + " AND [ID_CO] IN (" + det.Centros + ") ";
                                    if (!string.IsNullOrEmpty(det.NitTercero))
                                    {
                                        sql += " AND ID_TERC = '" + det.NitTercero.Trim() + "' ";
                                    }
                                    sql += " AND SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") ";
                                    sql += " GROUP BY [LAPSO_DOC] order by [LAPSO_DOC]";
                                    SqlCommand cmd = new SqlCommand(sql, conn);
                                    cmd.CommandType = CommandType.Text;
                                    SqlDataReader reader = cmd.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                            m.Description = det.Description;
                                            m.Accounts = det.Accounts;
                                            m.Month = reader.GetString(1);
                                            m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                            m.PositionNum = det.PositionNum;
                                            m.OrderMonth = long.Parse(m.Month);
                                            m.OrderNum = det.OrderNum;
                                            m.Value = reader.GetDecimal(0);
                                            //if (filter.Moneda == "DOLAR")
                                            //{
                                            decimal valuedolarm = lastvaluedolar; //listdolar.Where(x => x.Yearmonth == m.OrderMonth).First().ValueDolar;
                                            m.ValueDolar = m.Value / valuedolarm;
                                            //}
                                            m.Type = det.Type;
                                            Listdat.Add(m);
                                        }
                                        reader.Close();
                                    }
                                    else
                                    {
                                        var months = Listyears.Select(x => new { x.OrderMonth, x.Month }).ToList();
                                        foreach (var t in months)
                                        {
                                            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                            m.Description = det.Description;
                                            m.Accounts = det.Accounts;
                                            m.PositionNum = det.PositionNum;
                                            m.Month = t.Month;
                                            m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                            m.OrderMonth = t.OrderMonth;
                                            m.OrderNum = det.OrderNum;
                                            m.Type = det.Type;
                                            Listdat.Add(m);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                evento.WriteEntry("error consulta : " + ex.ToString(), EventLogEntryType.Error);
                            }
                        }
                        else
                        {//si cuenta esta vacia
                            try
                            {
                                var months = Listyears.Select(x => new { x.OrderMonth, x.Month }).ToList();
                                foreach (var t in months)
                                {
                                    DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                    m.Description = det.Description;
                                    m.Accounts = det.Accounts;
                                    m.PositionNum = det.PositionNum;
                                    m.Month = t.Month;
                                    m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                    m.OrderMonth = t.OrderMonth;
                                    m.OrderNum = det.OrderNum;
                                    m.Type = det.Type;
                                    Listdat.Add(m);
                                }
                            }
                            catch (Exception ex)
                            {
                                evento.WriteEntry("error1 : " + ex.ToString(), EventLogEntryType.Error);
                            }
                        }
                    }
                    else if (det.Type == "TOTAL" && det.Accounts != null
                        && det.Description != "UTILIDAD (PERDIDA) DEL EJERCICIO")
                    {

                        string[] numstosum = null;
                        //if (det.Accounts.Contains(","))
                        //{
                        //    //1,4,5
                        //    numstosum = det.Accounts.Split(",");
                        //}
                        string typeform = "";
                        if (det.Accounts.Contains(":"))
                        {
                            typeform = "RANGO";
                            string[] auxarr = det.Accounts.Split(":");
                            int fin = int.Parse(auxarr[1].Replace("F", ""));
                            int inicio = int.Parse(auxarr[0].Replace("F", ""));
                            numstosum = new string[(fin - inicio) + 1];
                            int posv = 0;
                            for (int i = inicio; i <= fin; i++)
                            {
                                numstosum[posv++] = "F" + i.ToString();
                            }
                        }
                        else
                        {
                            typeform = "FORMULA";
                            if (det.Accounts.Contains("*") || det.Accounts.Contains("/"))
                            {

                            }
                            numstosum = det.Accounts.Split('+', '-', '*');
                        }
                        List<long> listnum = new List<long>();
                        foreach (string d in numstosum)
                        {
                            if (!string.IsNullOrEmpty(d.Trim()) && d.Contains("F"))
                                listnum.Add(int.Parse(d.Replace("F", "")));
                        }
                        var months = Listyears.Select(x => new { x.OrderMonth, x.Month }).ToList();
                        foreach (var t in months)
                        {
                            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                            m.Description = det.Description;
                            m.Accounts = det.Accounts;
                            m.PositionNum = det.PositionNum;
                            m.Month = t.Month;
                            m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                            m.OrderMonth = t.OrderMonth;
                            m.OrderNum = det.OrderNum;
                            m.Type = det.Type;
                            if (typeform == "RANGO")
                            {
                                m.Value = Listdat.Where(x => listnum.Contains(x.OrderNum)
                                && x.OrderMonth == t.OrderMonth && x.Value != null).Sum(x => x.Value);
                                m.ValueDolar = Listdat.Where(x => listnum.Contains(x.OrderNum)
                           && x.OrderMonth == t.OrderMonth && x.ValueDolar != null).Sum(x => x.ValueDolar);
                            }
                            else
                            {//FORMULA
                                string formulapeso = det.Accounts;
                                string formuladolar = det.Accounts;
                                List<DTOReportBalanceMonth> listres = Listdat.Where(x => listnum.Contains(x.OrderNum)
                               && x.OrderMonth == t.OrderMonth).ToList();
                                if (listres.Count > 0)
                                {
                                    foreach (DTOReportBalanceMonth dat in listres)
                                    {
                                        formulapeso = formulapeso.Replace("F" + dat.OrderNum, "(" + (dat.Value == null ? "0" : dat.Value.ToString()) + ")");
                                        formuladolar = formuladolar.Replace("F" + dat.OrderNum, "(" + (dat.ValueDolar == null ? "0" : dat.ValueDolar.ToString()) + ")");
                                    }

                                    //System.Globalization.NumberFormatInfo MyNFI = System.Globalization.NumberFormatInfo.CurrentInfo;
                                    //if (MyNFI.NumberDecimalSeparator.Equals("."))
                                    //{
                                    //    formulapeso = formulapeso.Replace(",", ".");
                                    //    formuladolar = formuladolar.Replace(",", ".");
                                    //}
                                    //else if (MyNFI.NumberDecimalSeparator.Equals(","))
                                    //{
                                    formulapeso = formulapeso.Replace(",", ".");//El .compute evalua correctamente solo con el .
                                    formuladolar = formuladolar.Replace(",", ".");//El .compute evalua correctamente solo con el .
                                    //}
                                    try
                                    {
                                        // evento.WriteEntry("formula : " + formulapeso, EventLogEntryType.Information);
                                        DataTable dt = new DataTable();
                                        var vp = dt.Compute(formulapeso, "");
                                        var vd = dt.Compute(formuladolar, "");
                                        // evento.WriteEntry("result : " + vp.ToString(), EventLogEntryType.Information);
                                        m.Value = decimal.Parse(vp.ToString());
                                        m.ValueDolar = decimal.Parse(vd.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        evento.WriteEntry("error2 : " + ex.ToString(), EventLogEntryType.Error);
                                    }

                                }
                            }
                            Listdat.Add(m);
                        }


                    }
                    else if (det.Type == "GRUPO" || det.Accounts == null)
                    {
                        try
                        {
                            var months = Listyears.Select(x => new { x.OrderMonth, x.Month }).ToList();
                            foreach (var t in months)
                            {
                                DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                                m.Description = det.Description;
                                m.Accounts = det.Accounts;
                                m.PositionNum = det.PositionNum;
                                m.Month = t.Month;
                                m.MonthDateRecord = DateTime.ParseExact(m.Month + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                                m.OrderMonth = t.OrderMonth;
                                m.OrderNum = det.OrderNum;
                                m.Type = det.Type;
                                Listdat.Add(m);
                            }
                        }
                        catch (Exception ex)
                        {
                            evento.WriteEntry("error3 : " + ex.ToString(), EventLogEntryType.Error);
                        }
                    }
                }
                conn.Close();
                if (Listdat.Count > 0)
                {
                    DTOReportBalanceMonth dtrpt = Listdat.First();
                    dtrpt.ParamsContable = listdolar;
                }
                return Listdat;
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error generando reporte : " + ex.ToString(), EventLogEntryType.Error);
                return new List<DTOReportBalanceMonth>();
            }
        }

        public List<DTOYear> ListYearsContable()
        {
            List<DTOYear> listm = new List<DTOYear>();
            string sql = "SELECT DISTINCT SUBSTRING([LAPSO_DOC],1,4) FROM [biable01].[dbo].[CGRESUMEN_CUENTA_TERC]";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DTOYear a = new DTOYear();
                a.Name = reader.GetString(0);
                a.IdYear = long.Parse(a.Name);
                listm.Add(a);
            }
            return listm;
        }

        public List<DTOReportBalanceMonth> ListMonthContable(DTOFiltersCompras filter)
        {
            List<DTOReportBalanceMonth> listm = new List<DTOReportBalanceMonth>();
            //string sql = "SELECT DISTINCT [LAPSO_DOC] FROM [biable01].[dbo].[CGRESUMEN_CUENTA_TERC] where SUBSTRING([LAPSO_DOC],1,4) = " + filter.yearfilter;
            //SqlCommand cmd = new SqlCommand(sql, conn);
            //cmd.CommandType = CommandType.Text;
            //SqlDataReader reader = cmd.ExecuteReader();
            foreach (long mon in filter.multipleValuesMonth)
            {
                DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                m.Month = filter.yearfilter + "" + mon.ToString().PadLeft(2, '0');
                m.OrderMonth = long.Parse(m.Month);
                listm.Add(m);
            }
            return listm;
        }



        /// <summary>
        /// SALDOS DE INGRESOS POR ARRIENDO
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<DTOReportBalanceMonth> GetIncomeRental(DTOFiltersCompras filter, AdeposDBContext context)
        {
            List<DTOReportBalanceMonth> listm = new List<DTOReportBalanceMonth>();
            if (filter.TypeReportId == 2 || filter.TypeReportId == 3)
            {//mensual
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

                ///revisar aqui
                List<SnapshotBiableValueMonth> Listsnapsho = context.SnapshotBiableValueMonths.Where(x => filter.multipleValuesMonth.Contains(x.MonthId) &&
                 filter.multipleValuesYear.ToList().Contains(x.YearId)).ToList();
                if (Listsnapsho.Count > 0)
                {
                    foreach (SnapshotBiableValueMonth snapsh in Listsnapsho)
                    {
                        DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                        m.Monthyear = snapsh.MonthYear;
                        m.MonthDateRecord = DateTime.ParseExact(m.Monthyear + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                        m.IdMonth = snapsh.MonthId;
                        m.IdYear = snapsh.YearId;
                        m.Value = snapsh.ValueBalanceMonth;
                        listm.Add(m);
                    }
                }
                else
                {
                    Listsnapsho = new List<SnapshotBiableValueMonth>();
                }
                string sql = "SELECT (SUM(SALDOS_CRE_MES_L2)-SUM(SALDOS_DEB_MES_L2)) SAL, [LAPSO_DOC] FROM [dbo].[CGRESUMEN_CUENTA_TERC]  " +
                              "  where ID_CUENTA IN (415520,415525,41750515) AND SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") ";
                sql += " AND SUBSTRING([LAPSO_DOC],5,6) IN (" + mesfilter + ") ";
                sql += "GROUP BY [LAPSO_DOC]";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DTOReportBalanceMonth m = new DTOReportBalanceMonth();
                    m.Monthyear = reader.GetString(1);
                    if (Listsnapsho.Where(x => x.MonthYear == m.Monthyear).Count() == 0)
                    {
                        m.MonthDateRecord = DateTime.ParseExact(m.Monthyear + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                        m.IdMonth = int.Parse(m.Monthyear.Substring(4, 2));
                        m.IdYear = int.Parse(m.Monthyear.Substring(0, 4));
                        m.Value = reader.GetDecimal(0);
                        listm.Add(m);
                    }
                    else
                    {

                    }
                }
                reader.Close();
            }
            //else if (filter.TypeReportId == 3)
            //{//anual
            //    string yearfilter = string.Empty;
            //    foreach (long d in filter.multipleValuesYear.ToList())
            //    {
            //        yearfilter += d.ToString() + ",";
            //    }
            //    if (!string.IsNullOrEmpty(yearfilter))
            //        yearfilter = yearfilter.Substring(0, yearfilter.Length - 1);

            //    List<SnapshotBiableValueMonth> Listsnapsho = context.SnapshotBiableValueMonths.Where(x => filter.multipleValuesYear.ToList().Contains(x.YearId)).ToList();
            //    if (Listsnapsho.Count > 0)
            //    {
            //        foreach (List<SnapshotBiableValueMonth> snapshs in Listsnapsho.GroupBy(x => x.YearId).Select(x => x.ToList()).ToList())
            //        {
            //            DTOReportBalanceMonth m = new DTOReportBalanceMonth();
            //            m.IdYear = snapshs.First().YearId;
            //            m.Value = snapshs.Sum(x => x.ValueBalanceMonth);
            //            listm.Add(m);
            //        }
            //    }
            //    else
            //    {
            //        Listsnapsho = new List<SnapshotBiableValueMonth>();
            //    }


            //    string sql = "SELECT (SUM(SALDOS_CRE_MES_L2)-SUM(SALDOS_DEB_MES_L2)) SAL, SUBSTRING([LAPSO_DOC],1,4) FROM [dbo].[CGRESUMEN_CUENTA_TERC]  " +
            //                  "  where ID_CUENTA IN (415520,415525,41750515) AND SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") ";
            //    sql += "GROUP BY SUBSTRING([LAPSO_DOC],1,4)";

            //    SqlCommand cmd = new SqlCommand(sql, conn);
            //    cmd.CommandType = CommandType.Text;
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        DTOReportBalanceMonth m = new DTOReportBalanceMonth();
            //        m.IdYear = int.Parse(reader.GetString(1));
            //        if (Listsnapsho.Where(x => x.YearId == m.IdYear).Count() == 0)
            //        {
            //            m.Value = reader.GetDecimal(0);
            //            listm.Add(m);
            //        }
            //        else
            //        {

            //        }
            //    }
            //    reader.Close();
            //}
            return listm;
        }

        /// <summary>
        /// SALDOS DE INGRESOS POR ARRIENDO PARA ALMACENAR DATOS HISTORICOS
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<SnapshotBiableValueMonth> GetIncomeRentalToHistoric(DTOFiltersCompras filter)
        {
            List<SnapshotBiableValueMonth> listm = new List<SnapshotBiableValueMonth>();

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

            string sql = "SELECT (SUM(SALDOS_CRE_MES_L2)-SUM(SALDOS_DEB_MES_L2)) SAL, [LAPSO_DOC] FROM [dbo].[CGRESUMEN_CUENTA_TERC]  " +
                          "  where ID_CUENTA IN (415520,415525,41750515) AND SUBSTRING([LAPSO_DOC],1,4) IN (" + yearfilter + ") ";
            sql += " AND SUBSTRING([LAPSO_DOC],5,6) IN (1,2,3,4,5,6,7,8,9,10,11,12) ";
            sql += "GROUP BY [LAPSO_DOC]";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SnapshotBiableValueMonth m = new SnapshotBiableValueMonth();
                m.MonthYear = reader.GetString(1);
                m.MonthId = int.Parse(m.MonthYear.Substring(4, 2));
                m.YearId = int.Parse(m.MonthYear.Substring(0, 4));
                m.ValueBalanceMonth = reader.GetDecimal(0);
                listm.Add(m);
            }
            reader.Close();

            return listm;
        }

    }
}
