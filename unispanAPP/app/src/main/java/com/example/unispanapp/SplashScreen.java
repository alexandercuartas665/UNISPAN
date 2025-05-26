package com.example.unispanapp;

import androidx.appcompat.app.AppCompatActivity;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.StrictMode;
import android.view.View;
import android.view.WindowManager;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.Contract.UserContract;
import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Interfaces.UserAPI;
import com.example.unispanapp.Models.Paramters_Globals;
import com.example.unispanapp.Models.Users;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class SplashScreen extends AppCompatActivity {

    private final int WRITE_REQUEST_CODE = 1;
    List<Users> ListUser;
    DbManager db;
    public Button intentar;
    public  TextView lblMsg;
    TextView tbxURL_Service;
    RelativeLayout relaUrl;
    Button btnSaveUrl;
    Paramters_Globals parameter;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,WindowManager.LayoutParams.FLAG_FULLSCREEN);
        setContentView(R.layout.activity_splash_screen);

        //  String[] permissions = {Manifest.permission.WRITE_EXTERNAL_STORAGE,Manifest.permission.CAMERA,Manifest.permission.INTERNET
        //     ,Manifest.permission.ACCESS_NETWORK_STATE,Manifest.permission.WRITE_EXTERNAL_STORAGE};

        String[] permissions = {Manifest.permission.CAMERA, Manifest.permission.WRITE_EXTERNAL_STORAGE, Manifest.permission.READ_EXTERNAL_STORAGE};
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            requestPermissions(permissions, WRITE_REQUEST_CODE);
        }

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);
    try
    {
        db = new DbManager(getApplicationContext());
    }catch (Exception ex)
    {
        Toast.makeText(SplashScreen.this, ex.getMessage(), Toast.LENGTH_SHORT);
    }
        relaUrl = (RelativeLayout)findViewById(R.id.relaUrl);
        tbxURL_Service = (TextView)findViewById(R.id.tbxURL_Service);
        btnSaveUrl = (Button)findViewById(R.id.btnSaveUrl);

        Animation animation1 = AnimationUtils.loadAnimation(this,R.anim.desplazamiento_arriba);

        ImageView logo = findViewById(R.id.logo);

        logo.setAnimation(animation1);

        intentar  = (Button)findViewById(R.id.btnIntentar);
        intentar.setVisibility(View.INVISIBLE);

        boolean existeParameter = db.ExistParameter("URL_PARAMETER");


        if(!existeParameter){

        db.CreateParameter("URL_PARAMETER","http://192.168.1.9:8010/");
          //  db.CreateParameter("URL_PARAMETER","http://192.168.1.44:10333/");

        }

        IniciarSincronisacion();

        intentar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                intentar.setVisibility(View.INVISIBLE);
                IniciarSincronisacion();
            }
        });

        btnSaveUrl.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String newURL = tbxURL_Service.getText().toString();

                db.UpdateParameter("URL_PARAMETER",newURL);

                IniciarSincronisacion();
            }
        });



    }

    private void IniciarSincronisacion(){

        btnSaveUrl.setVisibility(View.INVISIBLE);
        relaUrl.setVisibility(View.INVISIBLE);

        ConnectivityManager connectivityManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();

        lblMsg  = (TextView)findViewById(R.id.lblMsg);

        lblMsg.setText("Sincronizando Usuarios");

        if (networkInfo != null && networkInfo.isConnected()) {
            intentar.setVisibility(View.INVISIBLE);

            //Intent i = new Intent(getApplicationContext(),Login.class);
            //startActivity(i);

           sincronizarUser();

        } else {
            // No hay conexión a Internet en este momento
            ValidarUsuariosLocal();
        }

    }

    void ValidarUsuariosLocal()
    {
        Boolean existUSer = db.listUsers();

        if(existUSer == false){
           /* try{

            }catch (Exception ex){
                lblMsg.setText(ex.getMessage());
            }
            lblMsg.setText("El dispositivo no esta conectado a internet y es imposible realializar la sincronozación de Usuarios");
*/
            intentar.setVisibility(View.VISIBLE);
            btnSaveUrl.setVisibility(View.VISIBLE);
            relaUrl.setVisibility(View.VISIBLE);
            String url = "";
            url = db.DataParameter("URL_PARAMETER");
            tbxURL_Service.setText(url);
            lblMsg.setText("No conecto con el Servicio de Sincronización Verifique la ruta o Comuniquese con el Administrador");


        }else {
            new Handler().postDelayed(new Runnable() {
                @Override
                public void run() {
                    Intent i = new Intent(getApplicationContext(),Login.class);
                    startActivity(i);
                }
            }, 4000);
        }
    }

    private void sincronizarUser ()
    {

        String url = "";

        url = db.DataParameter("URL_PARAMETER");

        Retrofit retrofit = new Retrofit.Builder().baseUrl(url)
                .addConverterFactory(GsonConverterFactory.create()).build();
        UserAPI userAPI = retrofit.create(UserAPI.class);
        Call<List<Users>> call = userAPI.getUsers();
        call.enqueue(new Callback<List<Users>>() {
            @Override
            public void onResponse(Call<List<Users>> call, Response<List<Users>> response) {
                try
                {

                    if (response.isSuccessful())
                    {

                        ListUser = response.body();

                        for(Users user : response.body()) {

                            Number id = user.getUserAppId();

                            Boolean ExitU = db.existeUser(user.getUserAppId());

                            if(ExitU == false){
                                db.CreateUser(Integer.parseInt(user.getUserAppId().toString()),user.getUsername(),user.getPassworNotCry());
                            }else {

                                db.UpdateUser(Integer.parseInt(user.getUserAppId().toString()),user.getUsername(),user.getPassworNotCry());

                            }

                        }

                        new Handler().postDelayed(new Runnable() {
                            @Override
                            public void run() {
                                Intent i = new Intent(getApplicationContext(),Login.class);
                                startActivity(i);
                            }
                        }, 4000);

                    }

                } catch (Exception ex) {

                    Toast.makeText(SplashScreen.this, ex.getMessage(), Toast.LENGTH_SHORT);
                }
            }

            @Override
            public void onFailure(Call<List<Users>> call, Throwable t) {
                ValidarUsuariosLocal();
            }
        });


    }
}