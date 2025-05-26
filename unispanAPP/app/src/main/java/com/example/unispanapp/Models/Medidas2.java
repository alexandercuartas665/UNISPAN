package com.example.unispanapp.Models;

public class Medidas2 {

    public String valor;
    public String tipoMedida;

    public String getValor() {
        return valor;
    }

    public void setValor(String valor) {
        this.valor = valor;
    }

    public String getTipoMedicion() {
        return tipoMedida;
    }

    public void setTipoMedicion(String tipoMedicion) {
        tipoMedida = tipoMedicion;
    }

    @Override
    public String toString() {
        return valor.toString();
    }
}
