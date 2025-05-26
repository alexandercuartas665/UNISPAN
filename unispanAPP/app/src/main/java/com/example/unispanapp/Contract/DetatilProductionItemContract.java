package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class DetatilProductionItemContract {

    private DetatilProductionItemContract(){}

    public static class DetailProductionItem implements BaseColumns {
        public static  final String TABLE_NAME = "DetailProductionItem";
        public  static  final String COLUMN_NAME_NAME_CODE = "code";
        public  static  final String COLUMN_NAME_NAME_ITEM_ID = "itemId";
        public  static final  String COLUMN_NAME_CONSECUTIVE_PRODUCTION= "consecutive";
        public  static final  String COLUMN_NAME_COUNT= "count";
        public  static final  String COLUMN_NAME_URL_PHOTO1= "url_photo1";
        public  static final  String COLUMN_NAME_URL_PHOTO2= "url_photo2";
        public  static final  String COLUMN_NAME_URL_PHOTO3= "url_photo3";
        public  static final  String COLUMN_NAME_OSERVAIONS= "observations";


    }

    public   static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + DetailProductionItem.TABLE_NAME + " (" +
                    DetailProductionItem._ID + " INTEGER PRIMARY KEY," +
                    DetailProductionItem.COLUMN_NAME_NAME_CODE + " INTEGER," +
                    DetailProductionItem.COLUMN_NAME_NAME_ITEM_ID + " INTEGER," +
                    DetailProductionItem.COLUMN_NAME_CONSECUTIVE_PRODUCTION + " INTEGER,"+
                    DetailProductionItem.COLUMN_NAME_COUNT + " INTEGER," +
                    DetailProductionItem.COLUMN_NAME_URL_PHOTO1 + " TEXT," +
                    DetailProductionItem.COLUMN_NAME_URL_PHOTO2 + " TEXT," +
                    DetailProductionItem.COLUMN_NAME_URL_PHOTO3 + " TEXT," +
                    DetailProductionItem.COLUMN_NAME_OSERVAIONS + " TEXT )";


    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ DetailProductionItem.TABLE_NAME;

}
