package com.example.unispanapp.Contract;

import android.provider.BaseColumns;

public class ItemsContract {

    private ItemsContract(){}

    public static class item implements BaseColumns {
        public static  final String TABLE_NAME = "Items";
        public  static  final String COLUMN_NAME_ITEM_ID = "itemId";
        public  static final String COLUMN_NAME_DESCRIPTION = "description";
        public  static  final String COLUMN_NAME_BARCODE = "barcode";
        public  static  final String COLUMN_NAME_REFERENCIA = "referencia";
        public  static  final String COLUMN_NAME_CATEGORY_ID = "categoryId";
        public  static final String  COLUMN_NAME_BARCODE_NAME = "barcodeAndName";
        public  static final String  COLUMN_NAME_CATEGORY_MEDICION_ID = "categoryMedicionId";
        public  static final String  COLUMN_NAME_WEIGHT = "weight";
        public  static final String  COLUMN_NAME_AREA = "area";


    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + item.TABLE_NAME + " (" +
                    item._ID + " INTEGER PRIMARY KEY," +
                    item.COLUMN_NAME_ITEM_ID + " INTEGER,"+
                    item.COLUMN_NAME_DESCRIPTION+ " TEXT,"+
                    item.COLUMN_NAME_BARCODE + " TEXT," +
                    item.COLUMN_NAME_REFERENCIA + " TEXT," +
                    item.COLUMN_NAME_CATEGORY_ID + " INTEGER,"+
                    item.COLUMN_NAME_BARCODE_NAME + " TEXT,"+
                    item.COLUMN_NAME_CATEGORY_MEDICION_ID + " INTEGER,"+
                    item.COLUMN_NAME_WEIGHT + " REAL,"+
                    item.COLUMN_NAME_AREA + " REAL"+
                    ")";

    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+ item.TABLE_NAME;

}
