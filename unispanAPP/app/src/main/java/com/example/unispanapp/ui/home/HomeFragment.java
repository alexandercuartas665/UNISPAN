package com.example.unispanapp.ui.home;

import android.app.DatePickerDialog;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TableLayout;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.navigation.Navigation;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.MainActivity;
import com.example.unispanapp.Models.Activitys;
import com.example.unispanapp.Models.CustomAdapter;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.R;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class HomeFragment extends Fragment {

    public static EditText dateNow;
    DatePickerDialog.OnDateSetListener setListener;
    private Button button;
    private ListView list;
    private ArrayAdapter<String>adapter;
    public  ArrayList<String> listProduction;
    List<Production> listPro;
    public static String DateNow;
    public static String Date;
      DbManager db;
   public static String date_n;
   public  Button btnNewMedicion;
    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

      db = new DbManager(getContext());

        View root = inflater.inflate(R.layout.fragment_home, container, false);

        list = (ListView)root.findViewById(R.id.list_view);

        dateNow = (EditText) root.findViewById(R.id.tbxDateNow);

        Calendar calendar = Calendar.getInstance();
        final int year = calendar.get(Calendar.YEAR);
        final int month = calendar.get(Calendar.MONTH);
        final int day = calendar.get(Calendar.DAY_OF_MONTH);
        if(date_n==null) {
            date_n = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(new Date());
            Date = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(new Date());

        }

        btnNewMedicion = (Button)root.findViewById(R.id.btnNewMedicion);

        btnNewMedicion.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Navigation.findNavController(getActivity(), R.id.nav_host_fragment).navigate(R.id.productionFragment);

            }
        });


      //  llenarListView();


        list.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                Production c = listPro.get(position);

                String Consecutivo = c.Consecutive.toString();

                Bundle bundle = new Bundle();
                bundle.putInt("consecutive", Integer.parseInt(Consecutivo));
                getParentFragmentManager().setFragmentResult("key",bundle);

                try{

                    Navigation.findNavController(getActivity(), R.id.nav_host_fragment).navigate(R.id.detail_production_fragment);


                }catch (Exception ex){
                    String Error =ex.getMessage();
                }


            }
        });

       dateNow.setOnClickListener(new View.OnClickListener() {
            @Override
           public void onClick(View v) {
               DatePickerDialog datePickerDialog = new DatePickerDialog(
                     getContext() , android.R.style.Theme_Holo_Light_Dialog_MinWidth,
                       setListener,year,month,day);
              datePickerDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
              datePickerDialog.show();
          }});

       setListener = new DatePickerDialog.OnDateSetListener() {
           @Override
           public void onDateSet(DatePicker view, int year, int month, int dayOfMonth) {
               month = month+1;
               String date = day + "/"+ month + "/" +year;
               // dateNow.setText(date);
           }
       };

       dateNow.setOnClickListener(new View.OnClickListener() {
           @Override
           public void onClick(View v) {
               DatePickerDialog datePickerDialog = new DatePickerDialog(
                       getContext(), new DatePickerDialog.OnDateSetListener() {
                   @Override
                   public void onDateSet(DatePicker view, int year, int month, int day) {
                       try {
                       month = month+1;
                        String date = day + "/" + month +"/"+year;

                       SimpleDateFormat format1 = new SimpleDateFormat("dd/MM/yyy");
                       Date dated = format1.parse(date);

                       date_n = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(dated);

                       //dateNow.setText(date_n);

                       Date = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(dated);


                           llenarListView();

                       } catch (ParseException e) {
                           e.printStackTrace();
                       }
                   }
               },year,month,day);
               datePickerDialog.show();
           }
       });



        return root;
    }

    public  void llenarListView() {

        dateNow.setText(date_n);
        listPro =  db.getListProducionts(Date);

        CustomAdapter adapter = new CustomAdapter(getContext(),listPro);

        list.setAdapter(adapter);
    }


    @Override
    public void onStart() {
        super.onStart();
        llenarListView();
    }
}