package com.example.unispanapp.Models;

public class Production {

    public Integer Consecutive;
    public String DateProduction;
    public Integer TypeActivity;
    public String NameActivity;
    public String StateId;
    public String NameOperator;
    public String photo;
    public String GUID;

    public String getGUID() {
        return GUID;
    }

    public void setGUID(String GUID) {
        this.GUID = GUID;
    }

    public String getPhoto() {
        return photo;
    }

    public void setPhoto(String photo) {
        this.photo = photo;
    }

    public Integer getCategoryId() {
        return categoryId;
    }

    public void setCategoryId(Integer categoryId) {
        this.categoryId = categoryId;
    }

    public Integer categoryId;

    public String getNameOperator() {
        return NameOperator;
    }

    public void setNameOperator(String nameOperator) {
        NameOperator = nameOperator;
    }

    public Number getConsecutive() {
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

    public Number getTypeActivity() {
        return TypeActivity;
    }

    public void setTypeActivity(Integer typeActivity) {
        TypeActivity = typeActivity;
    }

    public String getNameActivity() {
        return NameActivity;
    }

    public void setNameActivity(String nameActivity) {
        NameActivity = nameActivity;
    }

    public String getStateId() {
        return StateId;
    }

    public void setStateId(String stateId) {
        StateId = stateId;
    }

    public Number getUserAppId() {
        return UserAppId;
    }

    public void setUserAppId(Integer userAppId) {
        UserAppId = userAppId;
    }

    public String getTerceros() {
        return Terceros;
    }

    public void setTerceros(String terceros) {
        Terceros = terceros;
    }

    public Integer UserAppId;
    public String Terceros;

}
