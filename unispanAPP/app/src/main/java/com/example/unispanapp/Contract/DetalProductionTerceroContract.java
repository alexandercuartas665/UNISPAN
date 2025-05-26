package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

import java.security.PublicKey;

public class DetalProductionTerceroContract {

    private DetalProductionTerceroContract(){}

    public static class DetalProductionTercero implements BaseColumns {
        public static  final String TABLE_NAME = "DetailProductionTercero";
        public  static  final String COLUMN_NAME_DATE_PRODUCTION_ID = "ProductionId";
        public  static final  String COLUMN_NAME_COD_ENTERPRISE = "codeEnterprise";

    }

    public   static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + DetalProductionTercero.TABLE_NAME + " (" +
                    DetalProductionTercero.COLUMN_NAME_DATE_PRODUCTION_ID + " INTEGER,"+
                    DetalProductionTercero.COLUMN_NAME_COD_ENTERPRISE + " TEXT" +
                    ")";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ DetalProductionTercero.TABLE_NAME;


}
