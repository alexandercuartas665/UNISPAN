package com.example.unispanapp.Models;

public class Items {

   public Integer itemId;
   public String description;
   public String barcode;
   public String referencia;
    public Integer categoryId;
   public  String barcodeAndName;
   public Integer categoryMedicionId;
   public double weight;
    public double area;


    public String getReferencia() {
        return referencia;
    }

    public void setReferencia(String referencia) {
        this.referencia = referencia;
    }

    public String getBarcodeAndName() {
        return barcodeAndName;
    }

    public void setBarcodeAndName(String barcodeAndName) {
        this.barcodeAndName = barcodeAndName;
    }

    public Integer getCategoryMedicionId() {
        return categoryMedicionId;
    }

    public void setCategoryMedicionId(Integer categoryMedicionId) {
        this.categoryMedicionId = categoryMedicionId;
    }

    public double getWeight() {
        return weight;
    }

    public void setWeight(double weight) {
        this.weight = weight;
    }

    public double getArea() {
        return area;
    }

    public void setArea(double area) {
        this.area = area;
    }


    public Integer getItemId() {;

        return itemId;
    }

    public void setItemId(Integer itemId) {
        this.itemId = itemId;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getBarcode() {
        return barcode;
    }

    public void setBarcode(String barcode) {
        this.barcode = barcode;
    }

    public Integer getCategoryId() {
        return categoryId;
    }

    public void setCategoryId(Integer categoryId) {
        this.categoryId = categoryId;
    }



}
