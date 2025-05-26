package com.example.unispanapp.Interfaces;

import android.app.DownloadManager;

import com.example.unispanapp.Models.Activitys;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Medidas;
import com.example.unispanapp.Models.Novedad;
import com.example.unispanapp.Models.Operators;
import com.example.unispanapp.Models.Paramters_Globals;
import com.example.unispanapp.Models.SendMediciones;
import com.example.unispanapp.Models.Users;

import java.util.List;

import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Headers;
import retrofit2.http.POST;
import retrofit2.http.Query;

public interface SincAPI {


    @GET("/api/Produccion/selectAllTypeActivitys")
    Call<List<Activitys>> getActivitys();

    @POST("api/Item/selectAllItems")
    Call<List<Items>>getItems(@Body RequestBody parameter);

    @POST("api/Tercero/selectAllTerceros")
    Call<List<Operators>>getOperators(@Body RequestBody parameter);

    @GET("/api/Parameter/SelectMedidas")
    Call<List<Medidas>> getMedidas();

    @POST("api/Produccion/RecibirMediciones")
   // Call<SendMediciones>RecibirMediciones();
   Call<SendMediciones>RecibirMediciones(@Body RequestBody parameter);

    @POST("api/Produccion/RecibirNovedades")
        // Call<SendMediciones>RecibirMediciones();
    Call<Novedad>RecibirNonovedades(@Body RequestBody parameter);

}
