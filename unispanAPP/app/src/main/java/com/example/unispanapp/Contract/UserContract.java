package com.example.unispanapp.Contract;


import android.provider.BaseColumns;
public final class UserContract {

    private UserContract(){}

    public static class User implements BaseColumns{
        public static  final String TABLE_NAME = "USER";
        public  static  final String COLUMN_NAME_USER_APP_ID = "userAppId";
        public  static final String COLUMN_NAME_USERNAME = "username";
        public  static  final String COLUMN_NAME_PASSWORD = "PASSWORD";
    }

    public static  final String SQL_CREATE_ENTRIES=
            "CREATE TABLE " + User.TABLE_NAME + " (" +
                    User._ID + " INTEGER PRIMARY KEY," +
                    User.COLUMN_NAME_USER_APP_ID + " INTEGER,"+
                    User.COLUMN_NAME_USERNAME + " TEXT,"+
                    User.COLUMN_NAME_PASSWORD + " TEXT)";
    public static  final String SQL_DELETE_ENTRIES =
            "DROP TABLE IF EXISTS "+User.TABLE_NAME;

}
