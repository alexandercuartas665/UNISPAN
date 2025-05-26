package com.example.unispanapp.Models;

import java.util.List;

public class SendMediciones {

    public Integer Consecutive;
    public String DateProduction;
    public Integer TypeActivity;
    public Integer UserAppId;
    public String photo;
    public String GUID;
    public String responseTransaction;

    public String getresponseTransaction() {
        return responseTransaction;
    }

    public void setresponseTransaction(String responseTransaction) {
        this.responseTransaction = responseTransaction;
    }

    public String getGUID() {
        return GUID;
    }

    public void setGUID(String GUID) {
        this.GUID = GUID;
    }

    public List<ProductionDetailTerceros> detailTerceros;

    public List<ProductionDetailTerceros> getDetailTerceros() {
        return detailTerceros;
    }

    public void setDetailTerceros(List<ProductionDetailTerceros> detailTerceros) {
        this.detailTerceros = detailTerceros;
    }

    public List<SendDetailItems> detailItems;



    public List<SendDetailItems> getDetailItems() {
        return detailItems;
    }

    public void setDetailItems(List<SendDetailItems> detailItems) {
        this.detailItems = detailItems;
    }



    public Integer getConsecutive() {
        return Consecutive;
    }

    public void setConsecutive(Integer consecutive) {
        Consecutive = consecutive;
    }

    public String getDateProduction() {
        return DateProduction;
    }

    public void setDateProduction(String dateProduction) {
        DateProduction = dateProduction;
    }

    public Integer getTypeActivity() {
        return TypeActivity;
    }

    public void setTypeActivity(Integer typeActivity) {
        TypeActivity = typeActivity;
    }

    public Integer getUserAppId() {
        return UserAppId;
    }

    public void setUserAppId(Integer userAppId) {
        UserAppId = userAppId;
    }

    public String getPhoto() {
        return photo;
    }

    public void setPhoto(String photo) {
        this.photo = photo;
    }


}

