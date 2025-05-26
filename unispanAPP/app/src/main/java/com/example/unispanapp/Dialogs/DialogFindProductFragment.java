package com.example.unispanapp.Dialogs;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.DialogFragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TextView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.Category;
import com.example.unispanapp.Models.CustomAdapter;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Medidas;
import com.example.unispanapp.Models.Medidas2;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.Models.items_listview_adapter;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.AddProductFragment;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.ArrayList;
import java.util.List;


public class DialogFindProductFragment extends DialogFragment {

    Activity actividad;
    Button Buscar;
    ListView LvItem;
    DbManager db;
    List<Items> listItem;
    EditText parametro1;
    EditText parametro2;
    Spinner ddlMedida1;
    Spinner ddlMedida2;
    Spinner ddlMedida3;
    ImageButton btnCloseDialog;

    public static Integer Consecutive;

    private ArrayList<String> arrayList;

    private ArrayAdapter<String>adapter;

    DetailProductionFragment detail;


    public DialogFindProductFragment(DetailProductionFragment detail) {
        this.detail=detail;
    }


    @NonNull



    @Override
    public void onAttach(@NonNull Context context) {
        super.onAttach(context);


    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_dialog_find_product, container, false);

        db = new DbManager(root.getContext());

        parametro1 = (EditText)root.findViewById(R.id.tbxFiltro1);
        parametro2 = (EditText)root.findViewById(R.id.tbxFiltro2);
        Buscar = (Button)root.findViewById(R.id.btnAddProduct) ;

        ddlMedida1 = (Spinner)root.findViewById(R.id.ddlMedida1);
        ddlMedida2 = (Spinner)root.findViewById(R.id.ddlMedida2);
        ddlMedida3 = (Spinner)root.findViewById(R.id.ddlMedida3);

        btnCloseDialog =(ImageButton)root.findViewById(R.id.btnCloseDialog);

        btnCloseDialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                getDialog().dismiss();
            }
        });

        llenarMedidas();

        LvItem = (ListView)root.findViewById(R.id.lvItems);

        LvItem.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                AddProductFragment d = new AddProductFragment();

                Items c = listItem.get(position);

                String Codigo = c.barcode.toString();
                String DescriptionItem = c.description;

                d.IdITem = c.itemId.toString();
                d.lblCodProduct.setText(Codigo);
                d.lblNameProducto.setText(DescriptionItem);
                getDialog().dismiss();

            }
        });


        llenarListView();

        Buscar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                llenarListView();
            }
        });


        return root;
    }

    @Override
    public void onStart() {
        super.onStart();
        //CAMBIAR TAMANO DE FRAGMENT AL 90%
        int width = (int)(getResources().getDisplayMetrics().widthPixels*0.95);
        int height = (int)(getResources().getDisplayMetrics().heightPixels*0.30);
        getDialog().getWindow().setLayout(width, ViewGroup.LayoutParams.WRAP_CONTENT);
        //THIS WILL MAKE WIDTH 90% OF SCREEN
        //HEIGHT WILL BE WRAP_CONTENT
        //getDialog().getWindow().setLayout(width, height);
    }

    public void llenarMedidas(){
        Medidas medida = new Medidas();
        medida.valor = "NA";
        medida.tipoMedida= "";
        List<Medidas> ListMedida1 = new ArrayList<Medidas>();
        ListMedida1.add(medida);

        for(Medidas c : db.getListaMedidad("ALTO")){
            ListMedida1.add(c);
        }

        ArrayAdapter<Medidas> adapterMedida1 = new ArrayAdapter<Medidas>(getContext(), R.layout.spinner_layout, R.id.txt,ListMedida1);
       // adapterMedida1.setDropDownViewResource(R.layout.spinner_layout);
        ddlMedida1.setAdapter(adapterMedida1);

        Medidas medida2 = new Medidas();
        medida2.valor = "NA";
        medida2.tipoMedida= "";
        List<Medidas> ListMedida2 = new ArrayList<Medidas>();
        ListMedida2.add(medida2);

        for(Medidas c : db.getListaMedidad("ANCHO1")){
            ListMedida2.add(c);
        }

        ArrayAdapter<Medidas> adapterMedida2 = new ArrayAdapter<Medidas>(getContext(), R.layout.spinner_layout, R.id.txt,ListMedida2);
        ddlMedida2.setAdapter(adapterMedida2);

        Medidas medida3 = new Medidas();
        medida3.valor = "NA";
        medida3.tipoMedida= "";
        List<Medidas> ListMedida3 = new ArrayList<Medidas>();
        ListMedida3.add(medida3);

        for(Medidas c : db.getListaMedidad("ANCHO2")){
            ListMedida3.add(c);
        }

        ArrayAdapter<Medidas> adapterMedida3 = new ArrayAdapter<Medidas>(getContext(), R.layout.spinner_layout, R.id.txt,ListMedida3);
        ddlMedida3.setAdapter(adapterMedida3);
    }

    public void llenarListView(){
        try {

            DetailProductionFragment add = new DetailProductionFragment();
             Consecutive  = add.Consecutive;

            Production dataProduction = db.getDataProduciontCategory(Consecutive);

            if(dataProduction.categoryId != 3){
                ddlMedida1.setVisibility(View.INVISIBLE);
                ddlMedida2.setVisibility(View.INVISIBLE);
                ddlMedida3.setVisibility(View.INVISIBLE);
            }else {
                ddlMedida1.setVisibility(View.VISIBLE);
                ddlMedida2.setVisibility(View.VISIBLE);
                ddlMedida3.setVisibility(View.VISIBLE);
            }

            Medidas medida1 =(Medidas)   ddlMedida1.getSelectedItem();
            Medidas medida2 =(Medidas)  ddlMedida2.getSelectedItem();
            Medidas medida3 =(Medidas)  ddlMedida3.getSelectedItem();

            String alto = medida1.tipoMedida == "" ? "" :  medida1.valor;
            String ancho1 =medida2.tipoMedida == "" ? "" :  medida2.valor;
            String ancho2 = medida3.tipoMedida == "" ? "" :  medida3.valor;

            listItem = db.getListImtes(parametro1.getText().toString(),parametro2.getText().toString(),dataProduction.categoryId,alto,ancho1,ancho2);
            items_listview_adapter adapter = new items_listview_adapter(getContext(),listItem,detail);



            LvItem.setAdapter(adapter);
        }
        catch (Exception ex){
            String msg = ex.getMessage();
        }


    }




}