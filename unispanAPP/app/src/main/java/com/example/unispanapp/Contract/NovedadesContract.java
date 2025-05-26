package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class NovedadesContract {

    private NovedadesContract(){}

    public static class novedad implements BaseColumns {
        public static  final String TABLE_NAME = "Novedades";
        public  static  final String COLUMN_NAME_NOVEDAD_ID = "NOVEDAD_ID";
        public  static final String COLUMN_NAME_OPERARIO = "OPERARIO_ID";
        public  static  final String COLUMN_NAME_FECHA_INI = "FECHA_INI";
        public  static  final String COLUMN_NAME_FECHA_FIN = "FECHA_FIN";
        public  static  final String COLUMN_NAME_CANTIDAD = "CANTIDAD";
        public  static final String  COLUMN_NAME_OBSERVACIONES = "OBSERVACIONES";

    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + novedad.TABLE_NAME + " (" +
                    novedad._ID + " INTEGER PRIMARY KEY," +
                    novedad.COLUMN_NAME_NOVEDAD_ID + " TEXT," +
                    novedad.COLUMN_NAME_OPERARIO + " TEXT," +
                    novedad.COLUMN_NAME_FECHA_INI + " TEXT," +
                    novedad.COLUMN_NAME_FECHA_FIN + " TEXT,"+
                    novedad.COLUMN_NAME_CANTIDAD + " REAL," +
                    novedad.COLUMN_NAME_OBSERVACIONES + " TEXT"+
                    ")";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ novedad.TABLE_NAME;
}
