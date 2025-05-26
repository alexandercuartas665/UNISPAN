package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class ParametersContract {

    private ParametersContract(){}

    public static class Parameters implements BaseColumns {
        public static  final String TABLE_NAME = "Parameters";
        public  static  final String COLUMN_NAME_PARAMETER_NAME = "parameterName";
        public  static final String COLUMN_NAME_VALUES = "paremeterValue";
    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + Parameters.TABLE_NAME + " (" +
                    Parameters._ID + " INTEGER PRIMARY KEY," +
                    Parameters.COLUMN_NAME_PARAMETER_NAME + " TEXT,"+
                    Parameters.COLUMN_NAME_VALUES+  " TEXT)";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ Parameters.TABLE_NAME;

}
