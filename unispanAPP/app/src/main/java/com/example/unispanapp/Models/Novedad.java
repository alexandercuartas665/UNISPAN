package com.example.unispanapp.Models;

public class Novedad {

    public String NOVEDAD_ID;
    public String OPERARIO_ID;
    public String FECHA_INI;
    public String FECHA_FIN;
    public Float CANTIDAD;
    public  String OBSERVACIONES;
    public String NameOperators;

    public String getNameOperators() {
        return NameOperators;
    }

    public void setNameOperators(String nameOperators) {
        NameOperators = nameOperators;
    }

    public String getResponsetransaction() {
        return responsetransaction;
    }

    public void setResponsetransaction(String responsetransaction) {
        this.responsetransaction = responsetransaction;
    }

    public String responsetransaction;

    public String getNOVEDAD_ID() {
        return NOVEDAD_ID;
    }

    public void setNOVEDAD_ID(String NOVEDAD_ID) {
        this.NOVEDAD_ID = NOVEDAD_ID;
    }

    public String getOPERARIO_ID() {
        return OPERARIO_ID;
    }

    public void setOPERARIO_ID(String OPERARIO_ID) {
        this.OPERARIO_ID = OPERARIO_ID;
    }

    public String getFECHA_INI() {
        return FECHA_INI;
    }

    public void setFECHA_INI(String FECHA_INI) {
        this.FECHA_INI = FECHA_INI;
    }

    public String getFECHA_FIN() {
        return FECHA_FIN;
    }

    public void setFECHA_FIN(String FECHA_FIN) {
        this.FECHA_FIN = FECHA_FIN;
    }

    public Float getCANTIDAD() {
        return CANTIDAD;
    }

    public void setCANTIDAD(Float CANTIDAD) {
        this.CANTIDAD = CANTIDAD;
    }

    public String getOBSERVACIONES() {
        return OBSERVACIONES;
    }

    public void setOBSERVACIONES(String OBSERVACIONES) {
        this.OBSERVACIONES = OBSERVACIONES;
    }
}
