package com.example.unispanapp.Util;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Environment;
import android.util.Base64;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.OutputStream;

public class UtilBitmap {

    public  static Bitmap GetBitmapOfPath(Activity activity, String filename){
        try {
            File storageDir = activity.getExternalFilesDir(Environment.DIRECTORY_PICTURES);
            String pathimg = storageDir.getAbsolutePath();
            Uri urif = Uri.fromFile(new File(pathimg + "/" + filename));
            Bitmap bitmap = android.provider.MediaStore.Images.Media.getBitmap(activity.getContentResolver(), urif);
            return bitmap;
        }catch (Exception ex){
            return null;
        }
    }
    ///comprime y guarda
    public static void  CompreessBitmap(Activity activity,Bitmap bitmap, String filename) throws FileNotFoundException {
        File storageDir = activity.getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        String pathimg = storageDir.getAbsolutePath();
        OutputStream imagefile = new FileOutputStream(pathimg + "/" + filename);
// Write 'bitmap' to file using JPEG and 80% quality hint for JPEG:
        bitmap.compress(Bitmap.CompressFormat.JPEG, 50, imagefile);
    }

    public static void DeleteFile(String path){
        File fdelete = new File(path);
        if (fdelete.exists()) {
            if (fdelete.delete()) {
                System.out.println("file Deleted :");
            } else {
                System.out.println("file not Deleted :");
            }
        }
    }

    public static Bitmap  CompreessBitmapMiniature(Activity activity,String urlphto)
    {   try {
        Bitmap bitmap=  GetBitmapOfPath(activity, urlphto);
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG,5, byteArrayOutputStream);
        byte[] byteArray = byteArrayOutputStream.toByteArray();
        String encoded = Base64.encodeToString(byteArray, Base64.DEFAULT);
        byte[] decodedString = Base64.decode(encoded, Base64.DEFAULT);
        Bitmap miniatura = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);
        return miniatura;
    }catch (Exception ex){
        return null;
    }
    }
}
