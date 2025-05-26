package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class TypeActivityContract {
    private TypeActivityContract(){}

    public static class TypeActivity implements BaseColumns {
        public static  final String TABLE_NAME = "TypeActivity";
        public  static  final String COLUMN_NAME_TYPE_ACTIVITY_ID = "typeActivityId";
        public  static final  String COLUMN_NAME_TYPE_ACTIVITY_NAME= "name";
        public  static final  String COLUMN_NAME_CATEGORY_ID= "categoryId";
        public  static final  String COLUMN_NAME_CATEGORY_NAME= "categoryName";

    }

    public   static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + TypeActivity.TABLE_NAME + " (" +
                    TypeActivity._ID + " INTEGER PRIMARY KEY," +
                    TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID + " INTEGER,"+
                    TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_NAME + " TEXT," +
                    TypeActivity.COLUMN_NAME_CATEGORY_ID + " INTEGER,"+
                    TypeActivity.COLUMN_NAME_CATEGORY_NAME + " TEXT"+
                    ")";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ TypeActivity.TABLE_NAME;
}
