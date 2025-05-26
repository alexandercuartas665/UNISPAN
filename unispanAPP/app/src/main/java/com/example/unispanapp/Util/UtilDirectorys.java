package com.example.unispanapp.Util;

import android.os.Build;
import android.os.Environment;
import android.util.Log;

import java.io.File;

public class UtilDirectorys {

    /**
     * Returns the path to internal storage ex:- /storage/emulated/0
     *
     * @return
     */
    public static String getInternalDirectoryPath() {
        return Environment.getExternalStorageDirectory().getAbsolutePath();
    }

    /**
     * Returns the SDcard storage path for samsung ex:- /storage/extSdCard
     *
     * @return
     */
    public static String getSDcardDirectoryPath() {//EXTERNAL_STORAGE
        return System.getenv("EXTERNAL_SDCARD_STORAGE");
    }

    public static String getExternalStoragePath() {

        String internalPath = Environment.getExternalStorageDirectory().getAbsolutePath();
        String[] paths = internalPath.split("/");
        String parentPath = "/";
        for (String s : paths) {
            if (s.trim().length() > 0) {
                parentPath = parentPath.concat(s);
                break;
            }
        }
        File parent = new File(parentPath);
        if (parent.exists()) {
            File[] files = parent.listFiles();
            for (File file : files) {
                String filePath = file.getAbsolutePath();
                Log.d("TAG", filePath);
                if (filePath.equals(internalPath)) {
                    continue;
                } else if (filePath.toLowerCase().contains("sdcard")) {
                    return filePath;
                } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
                    try {
                        if (Environment.isExternalStorageRemovable(file)) {
                            return filePath;
                        }
                    } catch (RuntimeException e) {
                        Log.e("TAG", "RuntimeException: " + e);
                    }
                }
            }

        }
        return null;
    }

}
