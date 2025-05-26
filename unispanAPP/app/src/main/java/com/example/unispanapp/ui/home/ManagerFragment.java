package com.example.unispanapp.ui.home;

import android.content.Context;
import android.graphics.Bitmap;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Interfaces.SincAPI;
import com.example.unispanapp.Models.Activitys;
import com.example.unispanapp.Models.Converters;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Medidas;
import com.example.unispanapp.Models.Novedad;
import com.example.unispanapp.Models.Operators;
import com.example.unispanapp.Models.Paramters_Globals;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.Models.ProductionDetailTerceros;
import com.example.unispanapp.Models.SendDetailItems;
import com.example.unispanapp.Models.SendMediciones;
import com.example.unispanapp.R;
import com.example.unispanapp.Util.UtilBase64;
import com.example.unispanapp.Util.UtilBitmap;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link ManagerFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class ManagerFragment extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    ConnectivityManager connectivityManager;
    NetworkInfo networkInfo;
    Button btnSincMaestros;
    Button btnSincRedimiento;
    String URL;
    DbManager db;
    List<Activitys> ListActivitys;
    List<Medidas> ListMedidas;
    public int createMedidas = 0;
    public int updeteMedidas = 0;
    public int createActivity = 0;
    public int updeteActivity = 0;
    public int createItems = 0;
    public int updeteItems = 0;
    public  int createOperator = 0;
    public int upateOperator =0;
    public int pageIndex =0;
    TextView lblSincActivitis;
    TextView lblItemsSinc;
    TextView lblOperatorSinc , lblSyncProduction, lblSyncNovedades;
    LinearLayout progress;
    LinearLayout linearREsult;


    public ManagerFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment ManagerFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static ManagerFragment newInstance(String param1, String param2) {
        ManagerFragment fragment = new ManagerFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            mParam1 = getArguments().getString(ARG_PARAM1);
            mParam2 = getArguments().getString(ARG_PARAM2);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_manager, container, false);

        db = new DbManager(getActivity().getApplicationContext());

        linearREsult = (LinearLayout)root.findViewById(R.id.linearREsult);
        linearREsult.setVisibility(View.INVISIBLE);

        progress = (LinearLayout)root.findViewById(R.id.progress);
        progress.setVisibility(View.INVISIBLE);

        connectivityManager = (ConnectivityManager) getActivity().getSystemService(Context.CONNECTIVITY_SERVICE);
        networkInfo = connectivityManager.getActiveNetworkInfo();

        btnSincMaestros = (Button)root.findViewById(R.id.btnSincMaestros);
        btnSincRedimiento = (Button)root.findViewById(R.id.btnSincRedimiento);

        lblSincActivitis = (TextView)root.findViewById(R.id.lblSincActivits);
        lblItemsSinc = (TextView)root.findViewById(R.id.lblItemSinc);
        lblOperatorSinc =(TextView)root.findViewById(R.id.lblOperatorSinc);
        lblSyncProduction =(TextView)root.findViewById(R.id.lblSyncProduction);
        lblSyncNovedades = (TextView)root.findViewById(R.id.lblSyncNovedades);

        URL = db.DataParameter("URL_PARAMETER");

        btnSincMaestros.setEnabled(true);
        btnSincMaestros.setEnabled(true);

        btnSincMaestros.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                lblSyncProduction.setText("");
                lblOperatorSinc.setText("");
                lblItemsSinc.setText("");
                lblSincActivitis.setText("");

                if (networkInfo != null && networkInfo.isConnected()) {
                    progress.setVisibility(View.VISIBLE);
                    btnSincMaestros.setEnabled(false);
                    btnSincMaestros.setEnabled(false);
                    linearREsult.setVisibility(View.VISIBLE);
                    sincMedidas();
                    sincActivitys();
                }else {
                    Toast.makeText(getActivity().getApplicationContext(),"El dispositivo no esta conectado a internet y es imposible realizar la sincronización", Toast.LENGTH_SHORT).show();
                }

            }
        });

        btnSincRedimiento.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                SendMediciones();
                SendNovedades();
            }
        });

        return  root;

    }

    public void sincActivitys(){

        try {

            createActivity = 0;
            updeteActivity = 0;

                Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                        .addConverterFactory(GsonConverterFactory.create()).build();
                SincAPI sincAPI = retrofit.create(SincAPI.class);

                Call<List<Activitys>> call = sincAPI.getActivitys();
                call.enqueue(new Callback<List<Activitys>>() {
                    @Override
                    public void onResponse(Call<List<Activitys>> call, Response<List<Activitys>> response) {

                        if (response.isSuccessful())
                        {
                            db.DeleteActivitys();

                            ListActivitys = response.body();

                            for(Activitys activity : response.body()) {

                                Number id = activity.getTypeActivityId();

                                boolean exist = db.existeActivity(id);

                                if(!exist){

                                    db.CreateActivity(id.intValue(),activity.name.toString(),activity.categoryId,activity.categoryName);
                                    createActivity ++;

                                }
                               /* else {

                                    db.UpdateActivity(id.intValue(),activity.name.toString());

                                    updeteActivity ++;
                                }*/
                            }

                            lblSincActivitis.setText("Acividades Sincronizadas = " + createActivity);


                        }
                        sincOperarios();

                    }

                    @Override
                    public void onFailure(Call<List<Activitys>> call, Throwable t) {

                        Toast.makeText(getActivity().getApplicationContext(),"No conecto con el Servicio de Sincronización Verifique la ruta o Comuniquese con el Administrador", Toast.LENGTH_SHORT).show();

                    }
                });




        }catch (Exception ex)
        {
            String error = ex.getMessage();
        }

    }

    public void sincItems(){

        try {

            createItems = 0;
            updeteItems = 0;

            pageIndex = 0;

            OkHttpClient okHttpClient = new OkHttpClient.Builder()
                    .connectTimeout(1, TimeUnit.MINUTES)
                    .readTimeout(1, TimeUnit.MINUTES)
                    .writeTimeout(1, TimeUnit.MINUTES)
                    .build();


            Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                    .client(okHttpClient)
                    .addConverterFactory(GsonConverterFactory.create()).build();
            SincAPI sincAPI = retrofit.create(SincAPI.class);

                Paramters_Globals items = new Paramters_Globals(1,3000,pageIndex);

                Call<List<Items>> call = sincAPI.getItems(getParams(items));
                call.enqueue(new Callback<List<Items>>() {
                    @Override
                    public void onResponse(Call<List<Items>> call, Response<List<Items>> response) {

                        try{

                            if (response.isSuccessful())
                            {
                                   List<Items> itemsRes=response.body();
                                  for(Items item : itemsRes) {

                                      Integer id = item.getItemId();


                                          boolean exist = db.existeItem(id);

                                          Items dataItem = new Items();
                                          dataItem.itemId = id;
                                          dataItem.categoryMedicionId = item.getCategoryMedicionId();
                                          dataItem.area = item.getArea();
                                          dataItem.barcode = item.getBarcode();
                                          dataItem.barcodeAndName = item.getBarcodeAndName();
                                          dataItem.categoryId = item.getCategoryId();
                                          dataItem.referencia = item.getReferencia();
                                          dataItem.weight = item.getWeight();
                                          dataItem.description = item.getDescription();

                                          if(!exist){

                                              db.CreateItem(dataItem);
                                              createItems ++;

                                          }else {

                                              db.UpdateItem(dataItem);

                                              createItems ++;
                                          }

                                      lblItemsSinc.setText("Items Sincronizados = " + createItems);

                                  }
                                  progress.setVisibility(View.INVISIBLE);
                                  btnSincMaestros.setEnabled(true);

                            }else {
                                lblItemsSinc.setText("Lista de Items Vacia");
                            }


                        }catch (Exception ex){
                            lblItemsSinc.setText(ex.getMessage());

                        }

                    }

                    @Override
                    public void onFailure(Call<List<Items>> call, Throwable t) {

                        Toast.makeText(getActivity().getApplicationContext(),"No conecto con el Servicio de Sincronización Verifique la ruta o Comuniquese con el Administrador", Toast.LENGTH_SHORT).show();

                    }
                });


        }catch (Exception ex)
        {
            String error = ex.getMessage();
        }

    }

    public void sincOperarios(){

        createOperator = 0;
        upateOperator = 0;

        pageIndex = 0;

        Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                .addConverterFactory(GsonConverterFactory.create()).build();
        SincAPI sincAPI = retrofit.create(SincAPI.class);
        Paramters_Globals items = new Paramters_Globals(5);

        Call<List<Operators>> call = sincAPI.getOperators(getParams(items));
        call.enqueue(new Callback<List<Operators>>() {
            @Override
            public void onResponse(Call<List<Operators>> call, Response<List<Operators>> response) {
                if (response.isSuccessful()) {

                    db.DeleteOperators();

                    for (Operators operator : response.body()) {

                        Integer id = operator.getTerceroId();

                        boolean exist = db.existeOperator(id);

                        Operators dataOperator = new Operators();
                        dataOperator.terceroId = id;
                        dataOperator.fullName = operator.getFullName();
                        dataOperator.numDocument = operator.numDocument;
                        dataOperator.codeEnterprise = operator.codeEnterprise;

                        if(exist == false){

                            db.CreateOperator(dataOperator);
                            createOperator ++;

                        }
                        /*else {
                            db.UpdateOperator(dataOperator);
                            upateOperator ++;
                        }*/
                        lblOperatorSinc.setText("Operarios Sincronizados = " + createOperator);

                    }

                    sincItems();

                }
            }

            @Override
            public void onFailure(Call<List<Operators>> call, Throwable t) {
                Toast.makeText(getActivity().getApplicationContext(),"No conecto con el Servicio de Sincronización Verifique la ruta o Comuniquese con el Administrador", Toast.LENGTH_SHORT).show();

            }
        });

    }

    public void sincMedidas(){

        try
        {

            createMedidas = 0;
            updeteMedidas = 0;

            Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                    .addConverterFactory(GsonConverterFactory.create()).build();
            SincAPI sincAPI = retrofit.create(SincAPI.class);

            Call<List<Medidas>> call = sincAPI.getMedidas();
            call.enqueue(new Callback<List<Medidas>>() {
                @Override
                public void onResponse(Call<List<Medidas>> call, Response<List<Medidas>> response) {

                    if (response.isSuccessful())
                    {
                        db.DeleteMedidas();
                        ListMedidas = response.body();

                        for(Medidas activity : response.body()) {


                                Medidas m = new Medidas();
                                m.tipoMedida = activity.getTipoMedicion();
                                m.valor = activity.getValor();

                                db.CreateMedicion(m);
                                createMedidas ++;


                        }

                        //lblSincActivitis.setText("Acividades Creadas = " + createActivity+", Actividades Modificadas = "+updeteActivity);


                    }
                 //   sincOperarios();

                }

                @Override
                public void onFailure(Call<List<Medidas>> call, Throwable t) {

                    Toast.makeText(getActivity().getApplicationContext(),"No conecto con el Servicio de Sincronización Verifique la ruta o Comuniquese con el Administrador", Toast.LENGTH_SHORT).show();

                }
            });




        }catch (Exception ex)
        {
            String error = ex.getMessage();
        }

    }

    public RequestBody getParams(Object requestEntity){
        String body = Converters.fromObjectToString(requestEntity);
        return RequestBody.create(MediaType.parse("application/json"),body);
    }

    public void SendMediciones(){
//************************RETROFIT
        lblSyncProduction.setText("");
        lblOperatorSinc.setText("");
        lblItemsSinc.setText("");
        lblSincActivitis.setText("");

        if (networkInfo != null && networkInfo.isConnected()) {
            linearREsult.setVisibility(View.VISIBLE);
            progress.setVisibility(View.VISIBLE);
            progress.setVisibility(View.VISIBLE);
            progress.refreshDrawableState();
            progress.setVisibility(View.VISIBLE);

            Handler  handler=new Handler();
            Runnable r=new Runnable() {
                public void run() {

            Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                    .addConverterFactory(GsonConverterFactory.create()).build();
            SincAPI sincAPI = retrofit.create(SincAPI.class);
//******************************
            List<Production> listProducionts = db.getListAllProductions();
            int countmedicionSend = 0, countmedicionFallo = 0;


            //delay de 1 segundo

                    //what ever you do here will be done after 3 seconds delay.

            //*********************
            for (Production p : listProducionts) {

                SendMediciones mediciones = new SendMediciones();
                mediciones.Consecutive = p.Consecutive;
                mediciones.DateProduction = p.DateProduction;
                if (p.getPhoto() != null && !p.getPhoto().equals("")) {
                    try {
                        Bitmap bitm = UtilBitmap.GetBitmapOfPath(getActivity(), p.photo);
                        String base64image = UtilBase64.BitmapToBase64(bitm);
                        mediciones.photo = base64image;
                    } catch (Exception ex) {

                    }
                }
                //  mediciones.photo = p.photo;
                mediciones.TypeActivity = p.TypeActivity;
                mediciones.UserAppId = p.UserAppId;
                mediciones.GUID = p.GUID;
                mediciones.detailItems = new ArrayList<>();
                List<ProductionDetailTerceros> detTerceros = db.getListDetailProductionTerceros(p.Consecutive);
                mediciones.detailTerceros = detTerceros;

                List<DetatilProductionItem> detailProductionFragments = db.getListDetailProducionItems(p.Consecutive);
                for (DetatilProductionItem detail : detailProductionFragments) {

                    SendDetailItems d = new SendDetailItems();
                    d.code = detail.code;
                    d.consecutive = detail.consecutive;
                    d.count = detail.count;
                    d.itemId = detail.itemId;
                    d.observations = detail.observations;
                    if (detail.url_photo1 != null && !detail.url_photo1.equals("")) {
                        try {
                            Bitmap bitm = UtilBitmap.GetBitmapOfPath(getActivity(), detail.url_photo1);
                            String base64image = UtilBase64.BitmapToBase64(bitm);
                            d.url_photo1 = base64image;

                        } catch (Exception ex) {

                        }
                    }
                    mediciones.detailItems.add(d);
                }

                //Paramters_Globals items = new Paramters_Globals(5);
                try {

                    Call<SendMediciones> call = sincAPI.RecibirMediciones(getParams(mediciones));
                    // SendMediciones medici = call.execute().body();
                    Response<SendMediciones> newPostResponse = call.execute();
                    int statusCode = newPostResponse.code();
                    if (statusCode == 200) {
                        SendMediciones resp = newPostResponse.body();
                        if (resp.responseTransaction.equals("OK")) {
                            db.deleteOperator(p.Consecutive.toString());
                            db.deleteItems(p.Consecutive.toString());
                            db.deleteProduction(p.Consecutive.toString());
                            for (DetatilProductionItem detail : detailProductionFragments) {
                                if (detail.url_photo1 != null && !detail.url_photo1.equals("")) {
                                    UtilBitmap.DeleteFile(detail.url_photo1);
                                }
                            }
                            if (p.getPhoto() != null && !p.getPhoto().equals("")) {
                                UtilBitmap.DeleteFile(p.photo);
                            }
                            countmedicionSend++;
                        }
                    } else {
                        countmedicionFallo++;
                    }
                } catch (Exception e) {
                    countmedicionFallo++;
                    e.printStackTrace();
                }
            }
            lblSyncProduction.setText("Mediciones enviadas = " + countmedicionSend + " , Pendientes = " + (listProducionts.size() - countmedicionSend));
            progress.setVisibility(View.INVISIBLE);

                }};
          //  r.run();
            handler.postDelayed(r, 1000);
        }else{
            Toast.makeText(getActivity().getApplicationContext(),"El dispositivo no esta conectado a la red y es imposible realizar la sincronozación", Toast.LENGTH_SHORT).show();
        }
    }

    public void SendNovedades() {

        lblSyncProduction.setText("");
        lblOperatorSinc.setText("");
        lblItemsSinc.setText("");
        lblSincActivitis.setText("");

        if (networkInfo != null && networkInfo.isConnected()) {

            linearREsult.setVisibility(View.VISIBLE);
            progress.setVisibility(View.VISIBLE);
            progress.setVisibility(View.VISIBLE);
            progress.refreshDrawableState();
            progress.setVisibility(View.VISIBLE);

            Handler  handler=new Handler();
            Runnable r=new Runnable() {
                public void run() {

                    Retrofit retrofit = new Retrofit.Builder().baseUrl(URL)
                            .addConverterFactory(GsonConverterFactory.create()).build();
                    SincAPI sincAPI = retrofit.create(SincAPI.class);
//******************************
                    List<Novedad> listNovedades = db.getListNovedades();
                    int countNovedadend = 0, countNovedadFallo = 0;


                    //delay de 1 segundo

                    //what ever you do here will be done after 3 seconds delay.

                    //*********************
                    for (Novedad p : listNovedades) {

                        Novedad novedad = new Novedad();
                       novedad.FECHA_FIN = p.FECHA_FIN;
                       novedad.OPERARIO_ID = p.OPERARIO_ID;
                       novedad.OBSERVACIONES = p.OBSERVACIONES;
                       novedad.FECHA_INI = p.FECHA_INI;
                       novedad.CANTIDAD = p.CANTIDAD;
                       novedad.NOVEDAD_ID = p.NOVEDAD_ID;

                        try {

                            Call<Novedad> call = sincAPI.RecibirNonovedades(getParams(novedad));
                            // SendMediciones medici = call.execute().body();
                            Response<Novedad> newPostResponse = call.execute();
                            int statusCode = newPostResponse.code();
                            if (statusCode == 200) {
                                Novedad resp = newPostResponse.body();
                                if (resp.responsetransaction.equals("OK")) {

                                    db.deleteNovedad(p.NOVEDAD_ID);

                                    countNovedadend++;
                                }
                            } else {
                                countNovedadFallo++;
                            }
                        } catch (Exception e) {
                            countNovedadFallo++;
                            e.printStackTrace();
                        }
                    }
                    lblSyncNovedades.setText("Novedades enviadas = " + countNovedadend + " , Pendientes = " + (listNovedades.size() - countNovedadend));
                    progress.setVisibility(View.INVISIBLE);

                }};
            //  r.run();
            handler.postDelayed(r, 1000);

        }else {
            Toast.makeText(getActivity().getApplicationContext(),"El dispositivo no esta conectado a la red y es imposible realizar la sincronozación", Toast.LENGTH_SHORT).show();

        }

    }

}