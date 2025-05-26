package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class MeasurementsContract {

    private MeasurementsContract(){}

    public static class medida implements BaseColumns {
        public static  final String TABLE_NAME = "Medidas";
        public  static  final String COLUMN_NAME_VALOR = "valor";
        public  static final String COLUMN_NAME_TIPO_MEDIDA = "tipoMedida";

    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + medida.TABLE_NAME + " (" +
                    medida._ID + " INTEGER PRIMARY KEY," +
                    medida.COLUMN_NAME_VALOR + " INTEGER,"+
                    medida.COLUMN_NAME_TIPO_MEDIDA+ " TEXT)";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ medida.TABLE_NAME;


}
