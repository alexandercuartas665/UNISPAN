package com.example.unispanapp.DB;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.example.unispanapp.Contract.DetalProductionTerceroContract;
import com.example.unispanapp.Contract.DetatilProductionItemContract;
import com.example.unispanapp.Contract.ItemsContract;
import com.example.unispanapp.Contract.MeasurementsContract;
import com.example.unispanapp.Contract.NovedadesContract;
import com.example.unispanapp.Contract.OperatorContract;
import com.example.unispanapp.Contract.ParametersContract;
import com.example.unispanapp.Contract.ProductionContract;
import com.example.unispanapp.Contract.TypeActivityContract;
import com.example.unispanapp.Models.Activitys;
import com.example.unispanapp.Models.Category;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Medidas;
import com.example.unispanapp.Models.Medidas2;
import com.example.unispanapp.Models.Medidas3;
import com.example.unispanapp.Models.Novedad;
import com.example.unispanapp.Models.Operators;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.Contract.UserContract;
import com.example.unispanapp.Models.ProductionDetailTerceros;
import com.example.unispanapp.Util.UtilDirectorys;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class DbManager extends SQLiteOpenHelper {



    ArrayList<Production> productionList;

    public static final int DATABASE_VERSION = 3;
    public static final String DATABASE_NAME = "DBUnispan.db";

    public DbManager(Context context)
    {
        super(context,DATABASE_NAME,null,DATABASE_VERSION); //entornos de produccion , pero esta no se deja ver
          //  super(context, pathsd + "/"+ DATABASE_NAME, null, DATABASE_VERSION);
   //    super(new DatabaseContext(context), DATABASE_NAME, null, DATABASE_VERSION);
    }





    @Override
    public void onCreate(SQLiteDatabase db) {
        db.execSQL(UserContract.SQL_CREATE_ENTRIES);
        db.execSQL(ParametersContract.SQL_CREATE_ENTRIES);
        db.execSQL(TypeActivityContract.SQL_CREATE_ENTRIES);
        db.execSQL(ItemsContract.SQL_CREATE_ENTRIES);
        db.execSQL(OperatorContract.SQL_CREATE_ENTRIES);
        db.execSQL(ProductionContract.SQL_CREATE_ENTRIES);
        db.execSQL(DetalProductionTerceroContract.SQL_CREATE_ENTRIES);
        db.execSQL(DetatilProductionItemContract.SQL_CREATE_ENTRIES);
        db.execSQL(MeasurementsContract.SQL_CREATE_ENTRIES);
        db.execSQL(NovedadesContract.SQL_CREATE_ENTRIES);


    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        db.execSQL(UserContract.SQL_DELETE_ENTRIES);
        db.execSQL(ParametersContract.SQL_DELETE_ENTRIES);
        db.execSQL(TypeActivityContract.SQL_DELETE_ENTRIES);
        db.execSQL(ItemsContract.SQL_DELETE_ENTRIES);
        db.execSQL(OperatorContract.SQL_DELETE_ENTRIES);
        db.execSQL(ProductionContract.SQL_DELETE_ENTRIES);
        db.execSQL(DetalProductionTerceroContract.SQL_DELETE_ENTRIES);
        db.execSQL(DetatilProductionItemContract.SQL_DELETE_ENTRIES);
        db.execSQL(MeasurementsContract.SQL_DELETE_ENTRIES);
        db.execSQL(NovedadesContract.SQL_DELETE_ENTRIES);
        onCreate(db);
    }

    public void onDowngrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        onUpgrade(db, oldVersion, newVersion);
    }

    public boolean CreateUser(Integer ID, String User, String Password)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(UserContract.User.COLUMN_NAME_USER_APP_ID, ID);
            values.put(UserContract.User.COLUMN_NAME_USERNAME, User);
            values.put(UserContract.User.COLUMN_NAME_PASSWORD,Password);

            long newRowID =db.insert(UserContract.User.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean CreateParameter( String ParameterName, String ParameterValue)
    {

        try
        {

            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(ParametersContract.Parameters.COLUMN_NAME_PARAMETER_NAME, ParameterName);
            values.put(ParametersContract.Parameters.COLUMN_NAME_VALUES, ParameterValue);

            long newRowID =db.insert(ParametersContract.Parameters.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean UpdateUser(Integer ID, String User, String Password){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put("username",User); //These Fields should be your String values of actual column names
            cv.put("Password", Password);

            db.update(UserContract.User.TABLE_NAME, cv, "userAppId = ?", new String[]{ID.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public boolean UpdateParameter(String ParameterName, String ParameterValues){

        try{

            SQLiteDatabase db = getWritableDatabase();

            String  query = ("UPDATE Parameters SET values = "+"'"+ParameterValues+"'"+" WHERE userAppId ="+ParameterName);

            ContentValues cv = new ContentValues();
            cv.put("paremeterValue",ParameterValues); //These Fields should be your String values of actual column names

            db.update(ParametersContract.Parameters.TABLE_NAME, cv, "parameterName = ?", new String[]{ParameterName.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public Boolean Login (String User, String Password){

        try{

            SQLiteDatabase db = getReadableDatabase();


            Cursor cursor = db.rawQuery("select * from USER where username = ? and PASSWORD = ?", new String[]{User, Password});

            if(cursor.getCount() > 0) return true;
            else  return  false;

        }catch (Exception ex){
            return  false;
        }

    }

    public Boolean listUsers(){
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("select * from USER ", new String[]{});

        if(cursor.getCount() > 0) return  true;
        else return false;
    }

    public Boolean ExistParameter(String ParameterName){
        try{
            SQLiteDatabase db = getReadableDatabase();
            Cursor cursor = db.rawQuery("select * from Parameters  WHERE parameterName = ?", new String[]{ParameterName});

            if(cursor.getCount() > 0) return  true;
            else return false;
        }catch (Exception Ex){
            return  false;
        }

    }

    public String DataParameter(String ParameterName){
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("select * from Parameters  WHERE parameterName = ?", new String[]{ParameterName});

        String URL = "";
        if(cursor.getCount() > 0){

            cursor.moveToFirst();
            while (cursor.isAfterLast() == false)
            {

             URL= cursor.getString(cursor.getColumnIndex("paremeterValue"));

                cursor.moveToNext();
            }


        }
        return URL;

    }

    public Boolean existeUser(Number Id_User)
    {

        SQLiteDatabase db = getReadableDatabase();



        String  query = ("SELECT * FROM " + UserContract.User.TABLE_NAME + " WHERE " + UserContract.User.COLUMN_NAME_USER_APP_ID + " = " + Id_User + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public Boolean existeActivity(Number idActivity)
    {

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + TypeActivityContract.TypeActivity.TABLE_NAME + " WHERE " + TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID + " = " + idActivity + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public boolean CreateActivity(Integer ID, String Name, Integer categoryID, String CategoryName)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID, ID);
            values.put(TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_NAME, Name);
            values.put(TypeActivityContract.TypeActivity.COLUMN_NAME_CATEGORY_ID, categoryID);
            values.put(TypeActivityContract.TypeActivity.COLUMN_NAME_CATEGORY_NAME, CategoryName);

            long newRowID =db.insert(TypeActivityContract.TypeActivity.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean UpdateActivity(Integer ID, String Name){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put("name", Name);

            db.update(TypeActivityContract.TypeActivity.TABLE_NAME, cv, "typeActivityId = ?", new String[]{ID.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public  boolean DeleteActivitys()
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(TypeActivityContract.TypeActivity.TABLE_NAME, null , null) > 0;

    }

    public Boolean existeItem(Integer idItem)
    {

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + ItemsContract.item.TABLE_NAME + " WHERE " + ItemsContract.item.COLUMN_NAME_ITEM_ID + " = " + idItem + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public boolean CreateItem(Items dataitem)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(ItemsContract.item.COLUMN_NAME_ITEM_ID, dataitem.itemId);
            values.put(ItemsContract.item.COLUMN_NAME_DESCRIPTION, dataitem.description);
            values.put(ItemsContract.item.COLUMN_NAME_BARCODE, dataitem.barcode);
            values.put(ItemsContract.item.COLUMN_NAME_REFERENCIA, dataitem.referencia);
            values.put(ItemsContract.item.COLUMN_NAME_CATEGORY_ID,  dataitem.categoryId);
            values.put(ItemsContract.item.COLUMN_NAME_CATEGORY_MEDICION_ID,dataitem.categoryMedicionId);
            values.put(ItemsContract.item.COLUMN_NAME_BARCODE_NAME, dataitem.barcodeAndName);
            values.put(ItemsContract.item.COLUMN_NAME_WEIGHT, dataitem.weight);
            values.put(ItemsContract.item.COLUMN_NAME_AREA, dataitem.area);

            long newRowID =db.insert(ItemsContract.item.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean UpdateItem(Items dataitem){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put("description", dataitem.description);
            cv.put("barcode", dataitem.barcode);
            cv.put("referencia", dataitem.referencia);
            cv.put("categoryId", dataitem.categoryId);
            cv.put("categoryMedicionId", dataitem.categoryMedicionId);
            cv.put("weight", dataitem.weight);
            cv.put("area", dataitem.area);
            cv.put("barcodeAndName", dataitem.barcodeAndName);

            db.update(ItemsContract.item.TABLE_NAME, cv, "itemId = ?", new String[]{dataitem.itemId.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public Boolean existeOperator(Integer idOperator)
    {

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + OperatorContract.Operator.TABLE_NAME + " WHERE " + OperatorContract.Operator.COLUMN_NAME_TERCERO_ID + " = " + idOperator + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public boolean CreateOperator(Operators dataOperator)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(OperatorContract.Operator.COLUMN_NAME_TERCERO_ID, dataOperator.terceroId);
            values.put(OperatorContract.Operator.COLUMN_NAME_FULLNAME, dataOperator.fullName);
            values.put(OperatorContract.Operator.COLUMN_NAME_NUM_DOCUMENT, dataOperator.numDocument);
            values.put(OperatorContract.Operator.COLUMN_NAME_CODE_ENTERPRISE, dataOperator.codeEnterprise);


            long newRowID =db.insert(OperatorContract.Operator.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean UpdateOperator(Operators dataOperator){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put(OperatorContract.Operator.COLUMN_NAME_FULLNAME, dataOperator.fullName);
            cv.put(OperatorContract.Operator.COLUMN_NAME_NUM_DOCUMENT, dataOperator.numDocument);
            cv.put(OperatorContract.Operator.COLUMN_NAME_CODE_ENTERPRISE, dataOperator.codeEnterprise);


            db.update(OperatorContract.Operator.TABLE_NAME, cv, "terceroId = ?", new String[]{dataOperator.terceroId.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public  boolean DeleteOperators()
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(OperatorContract.Operator.TABLE_NAME, null , null) > 0;

    }

    public List<Activitys>getListActivityes(Integer categortID){

       List<Activitys> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        db.beginTransaction();

        try {

            String query = "SELECT * FROM "+ TypeActivityContract.TypeActivity.TABLE_NAME  +  " WHERE categoryId = ? ORDER BY name ";
            Cursor cursor =db.rawQuery(query, new String[]{categortID.toString()},null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Activitys  data = new Activitys();
                    data.typeActivityId = cursor.getInt(cursor.getColumnIndex("typeActivityId"));
                    data.name = cursor.getString(cursor.getColumnIndex("name"));
                    data.categoryId = cursor.getInt(cursor.getColumnIndex("categoryId"));
                    data.categoryName = cursor.getString(cursor.getColumnIndex("categoryName"));

                    list.add(data);

                }
                db.setTransactionSuccessful();

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }

        finally {
            db.endTransaction();
            db.close();
        }

        return list;


    }

    public boolean ExisteCodOperator (String cod){

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + OperatorContract.Operator.TABLE_NAME + " WHERE " + OperatorContract.Operator.COLUMN_NAME_CODE_ENTERPRISE + " = " + cod + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public String getFullNameOperator(String cod){
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("select * from "+OperatorContract.Operator.TABLE_NAME+"  WHERE "+ OperatorContract.Operator.COLUMN_NAME_CODE_ENTERPRISE + "= ?", new String[]{cod});

        String FulName = "";
        if(cursor.getCount() > 0){

            cursor.moveToFirst();
            while (cursor.isAfterLast() == false)
            {

                FulName= cursor.getString(cursor.getColumnIndex(OperatorContract.Operator.COLUMN_NAME_FULLNAME));

                cursor.moveToNext();
            }


        }
        return FulName;

    }

    public Integer getUserIdByUserName(String userNAme){
        SQLiteDatabase db = getReadableDatabase();
        Cursor cursor = db.rawQuery("select * from "+UserContract.User.TABLE_NAME+"  WHERE "+ UserContract.User.COLUMN_NAME_USERNAME + "= ?", new String[]{userNAme});

        Integer User_id =0 ;
        if(cursor.getCount() > 0){

            cursor.moveToFirst();
            while (cursor.isAfterLast() == false)
            {

                User_id= cursor.getInt(cursor.getColumnIndex(UserContract.User.COLUMN_NAME_USER_APP_ID));

                cursor.moveToNext();
            }


        }
        return User_id;

    }

    public Integer getConsecutiveProduction(){

        try {


            SQLiteDatabase db = getReadableDatabase();
            Cursor cursor = db.rawQuery("select * from "+ ProductionContract.production.TABLE_NAME+" Order by "+ ProductionContract.production.COLUMN_NAME_CONSECUTIVE +" DESC LIMIT 1 ", new String[]{});

            Integer consecutive =0 ;
            if(cursor.getCount() > 0){

                cursor.moveToFirst();
                while (cursor.isAfterLast() == false)
                {

                    consecutive= cursor.getInt(cursor.getColumnIndex(ProductionContract.production.COLUMN_NAME_CONSECUTIVE)) +1;

                    cursor.moveToNext();
                }


            }else {
                consecutive ++;
            }
            return consecutive;

        }catch (Exception ex){
            return  0;
        }



    }

    public boolean CreateProductionDetailTercero(ProductionDetailTerceros dataDetail)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_DATE_PRODUCTION_ID , dataDetail.ProductionId);
            values.put(DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_COD_ENTERPRISE,dataDetail.CodeEnterprise);


            long newRowID =db.insert(DetalProductionTerceroContract.DetalProductionTercero.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public boolean ExistRelProductionOperator (String cod, String Consecutive){

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + DetalProductionTerceroContract.DetalProductionTercero.TABLE_NAME + " WHERE " + DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_COD_ENTERPRISE + " = " + cod +
                " AND " + DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_DATE_PRODUCTION_ID +"="+Consecutive );

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public List<Production>getListProducionts(String Date){

        List<Production> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT P.CONSECUTIVE, TY.name FROM "+ ProductionContract.production.TABLE_NAME + " AS P INNER JOIN " +
                    TypeActivityContract.TypeActivity.TABLE_NAME +" AS TY ON P."+
                    ProductionContract.production.COLUMN_NAME_TYPE_ACTIVITY + " = TY."+
                    TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID +
                    " WHERE DATE = ?";

            Cursor cursor =db.rawQuery(query, new String[]{Date}, null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){
                    Integer consecutite = cursor.getInt(cursor.getColumnIndex("P.CONSECUTIVE"));

                    List<DetatilProductionItem> Listde = getListDetailProducionItems(consecutite);
                    if(Listde.size()>0) {
                        Production data = new Production();
                        data.NameActivity = cursor.getString(cursor.getColumnIndex("TY.name"));
                        data.Consecutive = cursor.getInt(cursor.getColumnIndex("P.CONSECUTIVE"));
                        List<ProductionDetailTerceros> List = getListDetailProductionTerceros(consecutite);
                        data.NameOperator = "";
                        for (ProductionDetailTerceros detail : List) {
                            data.NameOperator += detail.CodeEnterprise + "-" + detail.NameOperator + ", ";
                        }
                        list.add(data);
                    }
                }

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }



        return list;


    }

    public List<Production>getListAllProductions(){

        List<Production> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT CONSECUTIVE, TYPE_ACTIVITY, DATE, USER, PHOTO, GUID FROM "+ ProductionContract.production.TABLE_NAME;

            Cursor cursor =db.rawQuery(query, null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Production  data = new Production();
                    data.Consecutive = cursor.getInt(cursor.getColumnIndex("CONSECUTIVE"));
                    data.TypeActivity = cursor.getInt(cursor.getColumnIndex("TYPE_ACTIVITY"));
                    data.DateProduction = cursor.getString(cursor.getColumnIndex("DATE"));
                    data.UserAppId = cursor.getInt(cursor.getColumnIndex("USER"));
                    data.photo = cursor.getString(cursor.getColumnIndex("PHOTO"));
                    data.GUID = cursor.getString(cursor.getColumnIndex("GUID"));

                    list.add(data);
                }

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }



        return list;


    }

    public List<ProductionDetailTerceros>getListDetailProductionTerceros(Integer Consecutive){

        List<ProductionDetailTerceros> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        db.beginTransaction();

        try {

            String query = "SELECT D.codeEnterprise, O.fullName  FROM "+ DetalProductionTerceroContract.DetalProductionTercero.TABLE_NAME + " AS D INNER JOIN " +
                    OperatorContract.Operator.TABLE_NAME +" AS O ON D."+
                    DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_COD_ENTERPRISE + " = O."+
                    OperatorContract.Operator.COLUMN_NAME_CODE_ENTERPRISE + " WHERE D.ProductionId = " + Consecutive ;
            Cursor cursor =db.rawQuery(query , null);
            if(cursor.getCount()>0){


                while (cursor.moveToNext()){

                    ProductionDetailTerceros  data = new ProductionDetailTerceros();
                    data.NameOperator = cursor.getString(cursor.getColumnIndex("O.fullName"));
                    data.CodeEnterprise = cursor.getString(cursor.getColumnIndex("D.codeEnterprise"));

                    list.add(data);

                }
                db.setTransactionSuccessful();

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }

        finally {
            db.endTransaction();
            db.close();
        }

        return list;


    }

    public boolean CreateProduction(Production data)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(ProductionContract.production.COLUMN_NAME_CONSECUTIVE, data.Consecutive);
            values.put(ProductionContract.production.COLUMN_NAME_TYPE_ACTIVITY, data.TypeActivity);
            values.put(ProductionContract.production.COLUMN_NAME_STATE,data.StateId);
            values.put(ProductionContract.production.COLUMN_NAME_DATE,data.DateProduction);
            values.put(ProductionContract.production.COLUMN_NAME_USER,data.UserAppId);
            values.put(ProductionContract.production.COLUMN_NAME_PHOTO, data.photo);
            values.put(ProductionContract.production.COLUMN_NAME_GUID, UUID.randomUUID().toString());

            long newRowID =db.insert(ProductionContract.production.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public Production getDataProducionts(Integer consecutive){



        try {

            Production data = new Production();
            SQLiteDatabase db = getReadableDatabase();

            String query = "SELECT P.CONSECUTIVE, P.DATE, P.PHOTO, TY.name FROM "+ ProductionContract.production.TABLE_NAME + " AS P INNER JOIN " +
                    TypeActivityContract.TypeActivity.TABLE_NAME +" AS TY ON P."+
                    ProductionContract.production.COLUMN_NAME_TYPE_ACTIVITY + " = TY."+
                    TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID +
                    " WHERE P.CONSECUTIVE =" + consecutive;
                    ;

            Cursor cursor =db.rawQuery(query, null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    data.NameActivity = cursor.getString(cursor.getColumnIndex("TY.name"));
                    data.DateProduction = cursor.getString(cursor.getColumnIndex("P.DATE"));
                    data.photo = cursor.getString(cursor.getColumnIndex("P.PHOTO"));

                    Integer consecutite = cursor.getInt(cursor.getColumnIndex("P.CONSECUTIVE"));
                    List<ProductionDetailTerceros> List = getListDetailProductionTerceros(consecutite);
                    data.NameOperator = "";

                    for(ProductionDetailTerceros detail : List){
                        data.NameOperator += detail.CodeEnterprise+"-"+ detail.NameOperator+", ";
                    }

                }

            }

            return data;

        }catch (Exception ex){
            ex.printStackTrace();
            return new Production();
        }


    }

    public Production getDataProduciontCategory(Integer consecutive){

        Production data = new Production();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT P.CONSECUTIVE, TY.name, TY.categoryId FROM "+ ProductionContract.production.TABLE_NAME + " AS P INNER JOIN " +
                    TypeActivityContract.TypeActivity.TABLE_NAME +" AS TY ON P."+
                    ProductionContract.production.COLUMN_NAME_TYPE_ACTIVITY + " = TY."+
                    TypeActivityContract.TypeActivity.COLUMN_NAME_TYPE_ACTIVITY_ID +
                    " WHERE P.CONSECUTIVE =" + consecutive;
            ;

            Cursor cursor =db.rawQuery(query, null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    data.NameActivity = cursor.getString(cursor.getColumnIndex("TY.name"));
                    data.Consecutive = cursor.getInt(cursor.getColumnIndex("P.CONSECUTIVE"));
                    data.categoryId = cursor.getInt(cursor.getColumnIndex("TY.categoryId"));


                }

            }

        }catch (Exception ex){
            ex.printStackTrace();
        }
        return data;

    }


    public List<Items>getListImtes(String parametro1, String parametro2, Integer category, String Alto, String Ancho1, String Ancho2){

        List<Items> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT * FROM "+ItemsContract.item.TABLE_NAME +
                    " WHERE categoryMedicionId = ? AND description LIKE ? AND description LIKE ? AND description LIKE ? AND description LIKE ? AND description LIKE ?";

            Cursor cursor =db.rawQuery(query, new String[] {category.toString(),"%"+parametro1+"%","%"+parametro2+"%","%"+Alto+"%","%"+Ancho1+"%","%"+Ancho2+"%"}, null);

            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Items  data = new Items();
                    data.itemId = cursor.getInt(cursor.getColumnIndex("itemId"));
                    data.description = cursor.getString(cursor.getColumnIndex("description"));
                    data.barcodeAndName = cursor.getString(cursor.getColumnIndex("barcodeAndName"));
                    data.barcode =  cursor.getString(cursor.getColumnIndex("barcode"));

                    list.add(data);

                }

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }



        return list;


    }

    public boolean CreateDetailProduciontItem(DetatilProductionItem data)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_CODE, data.code);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_ITEM_ID, data.itemId);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_CONSECUTIVE_PRODUCTION, data.consecutive);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_COUNT, data.count);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_URL_PHOTO1, data.url_photo1);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_URL_PHOTO2, data.url_photo2);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_URL_PHOTO3, data.url_photo3);
            values.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_OSERVAIONS, data.observations);

            long newRowID =db.insert(DetatilProductionItemContract.DetailProductionItem.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public List<DetatilProductionItem>getListDetailProducionItems(Integer Consecutive){

        List<DetatilProductionItem> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT DI.code, DI.consecutive,  DI.itemId, DI.count, I.description, DI.url_photo1, DI.observations FROM "+ DetatilProductionItemContract.DetailProductionItem.TABLE_NAME +" AS DI" +
                    " INNER JOIN " + ItemsContract.item.TABLE_NAME + " AS I"+
                    " ON DI." + DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_ITEM_ID +
                    " =  I." + ItemsContract.item.COLUMN_NAME_ITEM_ID +
                    " WHERE DI.consecutive = ?  ORDER BY DI.code DESC ";

            Cursor cursor =db.rawQuery(query, new String[] {Consecutive.toString()}, null);

            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    DetatilProductionItem data = new DetatilProductionItem();

                    data.code = cursor.getInt(cursor.getColumnIndex("DI.code"));
                    data.url_photo1 = cursor.getString(cursor.getColumnIndex("DI.url_photo1"));
                    data.itemId = cursor.getInt(cursor.getColumnIndex("DI.itemId"));
                    data.consecutive = cursor.getInt(cursor.getColumnIndex("DI.consecutive"));
                    data.descriptionItem = cursor.getString(cursor.getColumnIndex("I.description"));
                    data.count = cursor.getInt(cursor.getColumnIndex("DI.count"));
                    data.observations = cursor.getString(cursor.getColumnIndex("DI.observations"));

                    list.add(data);
                }
            }

        }catch (Exception ex){
            ex.printStackTrace();
        }

        return list;
    }

    public DetatilProductionItem DataProductionItem(Integer code){
        SQLiteDatabase db = getReadableDatabase();

        String query = "SELECT DI.code, DI.itemId, DI.count, DI.observations, I.description, I.barcode FROM "+ DetatilProductionItemContract.DetailProductionItem.TABLE_NAME +" AS DI" +
                " INNER JOIN " + ItemsContract.item.TABLE_NAME + " AS I"+
                " ON DI." + DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_ITEM_ID +
                " =  I." + ItemsContract.item.COLUMN_NAME_ITEM_ID +
                " WHERE DI.code = ? ";

        Cursor cursor =db.rawQuery(query, new String[]{code.toString()}, null);
        DetatilProductionItem data = new DetatilProductionItem();
        String URL = "";
        if(cursor.getCount() > 0){

            cursor.moveToFirst();
            while (cursor.isAfterLast() == false)
            {

                data.code =  cursor.getInt(cursor.getColumnIndex("DI.code"));
                data.itemId = cursor.getInt(cursor.getColumnIndex("DI.itemId"));
                data.barcode = cursor.getString(cursor.getColumnIndex("I.barcode"));
                data.descriptionItem = cursor.getString(cursor.getColumnIndex("I.description"));
                data.count = cursor.getInt(cursor.getColumnIndex("DI.count"));
                data.observations = cursor.getString(cursor.getColumnIndex("DI.observations"));

                cursor.moveToNext();
            }
        }
        return data;

    }

    public boolean UpdateDetailProduciontItem(DetatilProductionItem data){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_COUNT, data.count);
            cv.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_OSERVAIONS, data.observations);

            db.update(DetatilProductionItemContract.DetailProductionItem.TABLE_NAME, cv, "code = ? ", new String[]{data.code.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public boolean UpdatephotoProduciontItem(DetatilProductionItem data){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_URL_PHOTO1, data.url_photo1);

            db.update(DetatilProductionItemContract.DetailProductionItem.TABLE_NAME, cv, "code = ? ", new String[]{data.code.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }


    public List<Category>getListCategorys(){

        List<Category> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        db.beginTransaction();

        try {

            String query = "SELECT DISTINCT categoryId, categoryName FROM "+ TypeActivityContract.TypeActivity.TABLE_NAME+
                    " ORDER BY categoryName ";
            Cursor cursor =db.rawQuery(query,null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Category  data = new Category();
                    data.categoryId = cursor.getInt(cursor.getColumnIndex("categoryId"));
                    data.categoryName = cursor.getString(cursor.getColumnIndex("categoryName"));

                    list.add(data);

                }
                db.setTransactionSuccessful();

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }

        finally {
            db.endTransaction();
            db.close();
        }

        return list;


    }

    public Integer getCodeDetailProduction(){

        try {


            SQLiteDatabase db = getReadableDatabase();
            Cursor cursor = db.rawQuery("select * from "+ DetatilProductionItemContract.DetailProductionItem.TABLE_NAME+" Order by "+ DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_CODE +" DESC LIMIT 1 ", new String[]{});

            Integer consecutive =0 ;
            if(cursor.getCount() > 0){

                cursor.moveToFirst();
                while (cursor.isAfterLast() == false)
                {

                    consecutive= cursor.getInt(cursor.getColumnIndex(DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_NAME_CODE)) +1;

                    cursor.moveToNext();
                }


            }else {
                consecutive ++;
            }
            return consecutive;

        }catch (Exception ex){
            return  0;
        }



    }

    public boolean UpdateProductionPhoto(Production data){

        try{

            SQLiteDatabase db = getWritableDatabase();

            ContentValues cv = new ContentValues();
            cv.put(ProductionContract.production.COLUMN_NAME_CONSECUTIVE, data.Consecutive);
            cv.put(ProductionContract.production.COLUMN_NAME_PHOTO, data.photo);

            db.update(ProductionContract.production.TABLE_NAME, cv, "CONSECUTIVE = ? ", new String[]{data.Consecutive.toString()});

            return  true;

        }catch (Exception ex){
            return  false;
        }

    }

    public boolean CreateMedicion(Medidas data)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(MeasurementsContract.medida.COLUMN_NAME_VALOR, data.valor);
            values.put(MeasurementsContract.medida.COLUMN_NAME_TIPO_MEDIDA, data.tipoMedida);

            long newRowID =db.insert(MeasurementsContract.medida.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    public Boolean existeMedida(String medida)
    {

        SQLiteDatabase db = getReadableDatabase();

        String  query = ("SELECT * FROM " + MeasurementsContract.medida.TABLE_NAME + " WHERE " + MeasurementsContract.medida.COLUMN_NAME_VALOR + " = " + medida + "");

        Cursor cursor = db.rawQuery(query, null );

        if(cursor.getCount() > 0) return true;
        else  return  false;

    }

    public List<Medidas>getListaMedidad(String medida){

        List<Medidas> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        try {

            String query = "SELECT tipoMedida, valor  FROM "+ MeasurementsContract.medida.TABLE_NAME +" AS DI" +
                    " WHERE DI.tipoMedida = ?";

            Cursor cursor =db.rawQuery(query, new String[] {medida.toString()}, null);

            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Medidas data = new Medidas();

                    data.tipoMedida = cursor.getString(cursor.getColumnIndex("tipoMedida"));
                    data.valor = cursor.getString(cursor.getColumnIndex("valor"));

                    list.add(data);
                }
            }

        }catch (Exception ex){
            ex.printStackTrace();
        }

        return list;
    }

    public  boolean DeleteMedidas()
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(MeasurementsContract.medida.TABLE_NAME, null , null) > 0;

    }

    //---deletes a particular title---
    public boolean deleteOperator(String cod)
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(DetalProductionTerceroContract.DetalProductionTercero.TABLE_NAME, DetalProductionTerceroContract.DetalProductionTercero.COLUMN_NAME_DATE_PRODUCTION_ID + "=" + cod , null) > 0;
    }


    public boolean deleteItems(String consecutive)
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(DetatilProductionItemContract.DetailProductionItem.TABLE_NAME, DetatilProductionItemContract.DetailProductionItem.COLUMN_NAME_CONSECUTIVE_PRODUCTION+ "=" + consecutive , null) > 0;
    }


    public boolean deleteProduction(String consecutive)
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(ProductionContract.production.TABLE_NAME, ProductionContract.production.COLUMN_NAME_CONSECUTIVE+ "=" + consecutive , null) > 0;
    }

    public List<Operators>getlistOperators(){

        List<Operators> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        db.beginTransaction();

        try {

            String query = "SELECT codeEnterprise,  fullName FROM "+ OperatorContract.Operator.TABLE_NAME+
                    " ORDER BY fullName ";
            Cursor cursor =db.rawQuery(query,null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Operators  data = new Operators();
                    data.codeEnterprise = cursor.getString(cursor.getColumnIndex("codeEnterprise"));
                    data.fullName = cursor.getString(cursor.getColumnIndex("fullName"));

                    list.add(data);

                }
                db.setTransactionSuccessful();

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }

        finally {
            db.endTransaction();
            db.close();
        }

        return list;


    }

    public List<Novedad>getListNovedades(){

        List<Novedad> list = new ArrayList<>();
        SQLiteDatabase db = getReadableDatabase();

        db.beginTransaction();

        try {

            String query = "SELECT * FROM "+ NovedadesContract.novedad.TABLE_NAME +" AS N"+
                    " INNER JOIN " + OperatorContract.Operator.TABLE_NAME + " AS OP ON N.OPERARIO_ID = OP.codeEnterprise" ;
            Cursor cursor =db.rawQuery(query, null,null);
            if(cursor.getCount()>0){

                while (cursor.moveToNext()){

                    Novedad  data = new Novedad();
                    data.CANTIDAD = cursor.getFloat(cursor.getColumnIndex("N.CANTIDAD"));
                    data.FECHA_FIN = cursor.getString(cursor.getColumnIndex("N.FECHA_FIN"));
                    data.FECHA_INI = cursor.getString(cursor.getColumnIndex("N.FECHA_INI"));
                    data.NOVEDAD_ID = cursor.getString(cursor.getColumnIndex("N.NOVEDAD_ID"));
                    data.OBSERVACIONES = cursor.getString(cursor.getColumnIndex("N.OBSERVACIONES"));
                    data.OPERARIO_ID = cursor.getString(cursor.getColumnIndex("N.OPERARIO_ID"));
                    data.NameOperators = cursor.getString(cursor.getColumnIndex("OP.fullName"));

                    list.add(data);

                }
                db.setTransactionSuccessful();

            }



        }catch (Exception ex){

            ex.printStackTrace();

        }

        finally {
            db.endTransaction();
            db.close();
        }

        return list;


    }

    public boolean CreateNovedad(Novedad data)
    {

        try
        {
            SQLiteDatabase db = getWritableDatabase();

            ContentValues values = new ContentValues();
            values.put(NovedadesContract.novedad.COLUMN_NAME_CANTIDAD, data.CANTIDAD);
            values.put(NovedadesContract.novedad.COLUMN_NAME_FECHA_FIN, data.FECHA_FIN);
            values.put(NovedadesContract.novedad.COLUMN_NAME_FECHA_INI, data.FECHA_INI);
            values.put(NovedadesContract.novedad.COLUMN_NAME_NOVEDAD_ID, UUID.randomUUID().toString() );
            values.put(NovedadesContract.novedad.COLUMN_NAME_OBSERVACIONES, data.OBSERVACIONES);
            values.put(NovedadesContract.novedad.COLUMN_NAME_OPERARIO, data.OPERARIO_ID);


            long newRowID =db.insert(NovedadesContract.novedad.TABLE_NAME, null,values);

            return  true;

        }catch (Exception ex){
            return  false;
        }
    }

    //---deletes a particular title---
    public boolean deleteNovedad(String cod)
    {
        SQLiteDatabase db = getWritableDatabase();

        return db.delete(NovedadesContract.novedad.TABLE_NAME, NovedadesContract.novedad.COLUMN_NAME_NOVEDAD_ID + "=" + "'" + cod + "'" , null) > 0;
    }


}
