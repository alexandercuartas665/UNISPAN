package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class ProductionContract {

    private  ProductionContract(){}

    public static class production implements BaseColumns {
        public static final String TABLE_NAME = "PRODUCTION";
        public static final String COLUMN_NAME_CONSECUTIVE = "CONSECUTIVE";
        public static final String COLUMN_NAME_TYPE_ACTIVITY = "TYPE_ACTIVITY";
        public static final String COLUMN_NAME_STATE = "STATE";
        public static final String COLUMN_NAME_DATE = "DATE";
        public static final String COLUMN_NAME_USER = "USER";
        public static final String COLUMN_NAME_PHOTO = "PHOTO";
        public static final String COLUMN_NAME_GUID = "GUID";
    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + production.TABLE_NAME + " (" +
                     production._ID + " INTEGER PRIMARY KEY," +
                    production.COLUMN_NAME_CONSECUTIVE+ " INTEGER,"+
                    production.COLUMN_NAME_TYPE_ACTIVITY + " INTEGER,"+
                    production.COLUMN_NAME_STATE + " TEXT,"+
                    production.COLUMN_NAME_DATE + " TEXT,"+
                    production.COLUMN_NAME_USER + " INTEGER,"+
                    production.COLUMN_NAME_PHOTO + " TEXT, " +
                    production.COLUMN_NAME_GUID + " TEXT " +
                    ")";




    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ production.TABLE_NAME;


}
