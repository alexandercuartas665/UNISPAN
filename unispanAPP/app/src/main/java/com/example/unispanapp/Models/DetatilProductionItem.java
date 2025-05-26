package com.example.unispanapp.Models;

import android.app.Activity;
import android.graphics.Bitmap;

import com.example.unispanapp.MainActivity;
import com.example.unispanapp.Util.UtilBitmap;

public class DetatilProductionItem {

    public  Integer code;
    public Integer itemId;


    public Integer consecutive;

    public Integer getCode() {
        return code;
    }

    public void setCode(Integer code) {
        this.code = code;
    }

    public Integer count;
    public String url_photo1;
    public String url_photo2;
    public String url_photo3;

    public String getBarcode() {
        return barcode;
    }

    public void setBarcode(String barcode) {
        this.barcode = barcode;
    }

    public String observations;
    public String descriptionItem;
    public String barcode;

    public String getDescriptionItem() {
        return descriptionItem;
    }

    public void setDescriptionItem(String descriptionItem) {
        this.descriptionItem = descriptionItem;
    }

    public Integer getItemId() {
        return itemId;
    }

    public void setItemId(Integer itemId) {
        this.itemId = itemId;
    }

    public Integer getConsecutive() {
        return consecutive;
    }

    public void setConsecutive(Integer consecutive) {
        this.consecutive = consecutive;
    }

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }

    public String getUrl_photo1() {
        return url_photo1;
    }

    public void setUrl_photo1(String url_photo1) {
        this.url_photo1 = url_photo1;
    }

    public String getUrl_photo2() {
        return url_photo2;
    }

    public void setUrl_photo2(String url_photo2) {
        this.url_photo2 = url_photo2;
    }

    public String getUrl_photo3() {
        return url_photo3;
    }

    public void setUrl_photo3(String url_photo3) {
        this.url_photo3 = url_photo3;
    }

    public String getObservations() {
        return observations;
    }

    public void setObservations(String observations) {
        this.observations = observations;
    }

    Bitmap MiniaturaPhoto1 =null;
    public Bitmap GetBitampPhoto1(Activity activity,String url_photo1)
    {
        if(MiniaturaPhoto1==null)
            MiniaturaPhoto1 = UtilBitmap.GetBitmapOfPath(activity,url_photo1);
        return MiniaturaPhoto1;
    }
}
