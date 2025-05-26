package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class OperatorContract {

    private OperatorContract(){}

    public static class Operator implements BaseColumns {
        public static  final String TABLE_NAME = "Operators";
        public  static  final String COLUMN_NAME_TERCERO_ID = "terceroId";
        public  static final String COLUMN_NAME_FULLNAME = "fullName";
        public  static  final String COLUMN_NAME_NUM_DOCUMENT = "numDocument";
        public  static  final String COLUMN_NAME_CODE_ENTERPRISE = "codeEnterprise";
    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + Operator.TABLE_NAME + " (" +
                    Operator._ID + " INTEGER PRIMARY KEY," +
                    Operator.COLUMN_NAME_TERCERO_ID + " INTEGER,"+
                    Operator.COLUMN_NAME_FULLNAME + " TEXT,"+
                    Operator.COLUMN_NAME_NUM_DOCUMENT + " TEXT," +
                    Operator.COLUMN_NAME_CODE_ENTERPRISE + " TEXT)";
    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ Operator.TABLE_NAME;

}
