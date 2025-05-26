package com.example.unispanapp.Dialogs;

import android.app.AlertDialog;
import android.app.DatePickerDialog;
import android.content.DialogInterface;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.Novedad;
import com.example.unispanapp.Novedades.NovedadesFragment;
import com.example.unispanapp.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

public class DialogNewNovedadFragment extends DialogFragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters

    DatePickerDialog.OnDateSetListener setListener;


    private String mParam1;
    private String mParam2;
    Integer acc;
    ImageButton btnCloseDialog;
    Button btnDelete;
    Button btnSaveNovedad;
    ImageButton btnListOperator;
    public static TextView lblNameOperario;
    public String codPerator;
    EditText tbxDateIni;
    EditText tbxDateFin;
    EditText tbxCantidad;
    EditText tbxObservaciones;
    public static String date_i;
    public static String date_f;
    public static String Date_i,Date_f;
    NovedadesFragment fragmentNov;
    DbManager db;
    public DialogNewNovedadFragment(Integer acc, NovedadesFragment fragmentNov) {
        this.acc = acc;
        this.fragmentNov=fragmentNov;
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
        View root = inflater.inflate(R.layout.fragment_dialog_new_novedad, container, false);

        db = new DbManager(getContext());

        Calendar calendar = Calendar.getInstance();
        final int year = calendar.get(Calendar.YEAR);
        final int month = calendar.get(Calendar.MONTH);
        final int day = calendar.get(Calendar.DAY_OF_MONTH);

        if(date_i==null) {
            date_i = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(new Date());
            Date_i = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(new Date());

        }
        if(date_f==null) {
            date_f = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(new Date());
            Date_f = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(new Date());

        }

        btnCloseDialog = (ImageButton)root.findViewById(R.id.btnCloseDialog);
        btnDelete = (Button)root.findViewById(R.id.btnDelete);
        btnSaveNovedad = (Button)root.findViewById(R.id.btnSaveNovedad);
        btnListOperator = (ImageButton)root.findViewById(R.id.btnListOperator);
        tbxDateIni = (EditText)root.findViewById(R.id.tbxDateIni);
        tbxDateFin = (EditText)root.findViewById(R.id.tbxDateFin);
        lblNameOperario = (TextView)root.findViewById(R.id.lblNameOperario);
        tbxCantidad = (EditText)root.findViewById(R.id.tbxCantidad);
        tbxObservaciones = (EditText)root.findViewById(R.id.tbxObservaciones);

        tbxDateIni.setText(date_i);
        tbxDateFin.setText(date_f);

        tbxDateIni.setOnClickListener(new View.OnClickListener() {
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

        tbxDateIni.setOnClickListener(new View.OnClickListener() {
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

                            Date_i = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(dated);

                            date_i = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(dated);

                            tbxDateIni.setText(date_i);

                        } catch (ParseException e) {
                            e.printStackTrace();
                        }
                    }
                },year,month,day);
                datePickerDialog.show();
            }
        });


        tbxDateFin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DatePickerDialog datePickerDialog = new DatePickerDialog(
                        getContext() , android.R.style.Theme_Holo_Light_Dialog_MinWidth,
                        setListener,year,month,day);
                datePickerDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
                datePickerDialog.show();
            }});



        tbxDateFin.setOnClickListener(new View.OnClickListener() {
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
                            Date_f = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(dated);

                            date_f = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(dated);

                            tbxDateFin.setText(date_f);

                        } catch (ParseException e) {
                            e.printStackTrace();
                        }
                    }
                },year,month,day);
                datePickerDialog.show();
            }
        });


        if(acc == 1){
            btnDelete.setVisibility(View.INVISIBLE);
            btnSaveNovedad.setVisibility(View.VISIBLE);
            btnListOperator.setVisibility(View.VISIBLE);

            lblNameOperario.setText("");
            tbxDateIni.setText("");
            tbxDateFin.setText("");
            tbxCantidad.setText("");
            tbxObservaciones.setText("");

        }else {

            Novedad data = fragmentNov.dataNovedad;

            btnListOperator.setVisibility(View.INVISIBLE);

            lblNameOperario.setText(data.OPERARIO_ID + "-" + data.NameOperators);
            tbxDateIni.setText(data.FECHA_INI);
            tbxDateFin.setText(data.FECHA_FIN);
            tbxCantidad.setText(data.CANTIDAD.toString());
            tbxObservaciones.setText(data.OBSERVACIONES);

            btnDelete.setVisibility(View.VISIBLE);
            btnSaveNovedad.setVisibility(View.INVISIBLE);
            btnListOperator.setVisibility(View.INVISIBLE);

        }

        btnDelete.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                boolean delete = db.deleteNovedad(fragmentNov.dataNovedad.NOVEDAD_ID);

                if(delete){


                Toast.makeText(getActivity().getApplicationContext(),"Novedad Eliminada", Toast.LENGTH_SHORT).show();
                fragmentNov.RefreshAdapter();
                getDialog().dismiss();

                }

            }
        });

        final DialogNewNovedadFragment f = this;
        btnListOperator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogFindOperatorsFragment findProductFragment = new DialogFindOperatorsFragment(null,2,f);
                findProductFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");
            }
        });

        btnCloseDialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                getDialog().dismiss();
            }
        });

        btnSaveNovedad.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                SaveNovedad();
            }
        });

        return root;
    }

    public void SaveNovedad(){
        if(codPerator!=null) {
            Novedad data = new Novedad();
            data.CANTIDAD = Float.parseFloat(tbxCantidad.getText().toString());
            data.FECHA_FIN = Date_f;
            data.FECHA_INI = Date_i;
            data.OBSERVACIONES = tbxObservaciones.getText().toString();
            data.OPERARIO_ID = codPerator;

            if (db.CreateNovedad(data)) {
                fragmentNov.RefreshAdapter();
                getDialog().dismiss();

            }
        }else{
            Toast.makeText(getActivity().getApplicationContext(),"El operario es obligatorio", Toast.LENGTH_SHORT).show();

        }

    }
}