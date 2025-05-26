package com.example.unispanapp.Models;

public class Activitys {

    public Integer typeActivityId;
    public String name;
    public Integer categoryId;
    public String categoryName;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Integer getCategoryId() {
        return categoryId;
    }

    public void setCategoryId(Integer categoryId) {
        this.categoryId = categoryId;
    }

    public String getCategoryName() {
        return categoryName;
    }

    public void setCategoryName(String categoryName) {
        this.categoryName = categoryName;
    }

    public Number getTypeActivityId() {
        return typeActivityId;
    }

    public void setTypeActivityId(Integer typeActivityId) {
        this.typeActivityId = typeActivityId;
    }

    @Override
    public String toString() {
        return name;
    }
}
