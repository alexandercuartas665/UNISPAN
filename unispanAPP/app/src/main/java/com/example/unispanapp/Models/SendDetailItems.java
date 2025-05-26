package com.example.unispanapp.Models;

public class SendDetailItems {

    public  Integer code;
    public Integer itemId;
    public Integer consecutive;
    public Integer count;
    public String url_photo1;
    public String observations;

    public Integer getCode() {
        return code;
    }

    public void setCode(Integer code) {
        this.code = code;
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

    public String getObservations() {
        return observations;
    }

    public void setObservations(String observations) {
        this.observations = observations;
    }
}
