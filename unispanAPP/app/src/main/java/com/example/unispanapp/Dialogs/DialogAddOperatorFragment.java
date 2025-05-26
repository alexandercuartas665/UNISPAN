package com.example.unispanapp.Dialogs;

import android.os.Bundle;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.CustomAdapterDetailProductionItem;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.ProductionDetailTerceros;
import com.example.unispanapp.Models.listview_operator_edit;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.ArrayList;
import java.util.List;

public class DialogAddOperatorFragment extends DialogFragment {

    DbManager db;
    Button btnAddOperator;
    Button btnSeveProduction;
    ImageButton btnCloseDialog;
    EditText tbxCodOperario;
    public ListView list;
    Integer Consecutive;
    DetailProductionFragment fragment;
    private ArrayList<String> arrayList;
    public listview_operator_edit adapter;

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    public DialogAddOperatorFragment(Integer Consecutive, DetailProductionFragment fragment) {
        this.Consecutive = Consecutive;
        this.fragment = fragment;
    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_dialog_add_operator, container, false);

        db = new DbManager(getContext());
        btnAddOperator = (Button)root.findViewById(R.id.btnAddOperator);
        tbxCodOperario = (EditText)root.findViewById(R.id.tbxCodOperario);
        btnSeveProduction = (Button)root.findViewById(R.id.btnSeveProduction) ;
        btnCloseDialog =(ImageButton)root.findViewById(R.id.btnCloseDialog);

        list = (ListView)root.findViewById(R.id.list_view);

        final List<ProductionDetailTerceros> listTerceros = db.getListDetailProductionTerceros(Consecutive);

       adapter = new listview_operator_edit(getContext(),listTerceros,fragment,this);

        list.setAdapter(adapter);

        btnCloseDialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                getDialog().dismiss();
            }
        });

        btnAddOperator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(!tbxCodOperario.getText().toString().isEmpty()){

                    String cod = tbxCodOperario.getText().toString();

                    boolean exist = db.ExisteCodOperator(cod);

                    if(exist){

                        Integer count = 0;

                        for (ProductionDetailTerceros i : listTerceros){
                            if(i.CodeEnterprise.equals(cod)){
                                count = count + 1;
                            }
                        }


                        if(count == 0){

                            ProductionDetailTerceros dataP = new ProductionDetailTerceros();
                            dataP.NameOperator =  db.getFullNameOperator(cod);
                            dataP.CodeEnterprise = cod;
                            dataP.ProductionId = Consecutive;
                            tbxCodOperario.setText("");
                            listTerceros.add(dataP);
                            adapter.notifyDataSetChanged();


                        }else {
                            Toast.makeText(getActivity().getApplicationContext(),"Ya existe el Operario", Toast.LENGTH_SHORT).show();
                            count=0;
                        }

                    }else {
                        Toast.makeText(getActivity().getApplicationContext(),"El Código el Operario no Existe", Toast.LENGTH_SHORT).show();

                    }

                }else {
                    Toast.makeText(getActivity().getApplicationContext(),"El Codigo del Operario está vacío", Toast.LENGTH_SHORT).show();

                }

            }
        });

        btnSeveProduction.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if(listTerceros.size() != 0){

                    boolean delete = db.deleteOperator(Consecutive.toString());


                    for (ProductionDetailTerceros terceros : listTerceros) {

                        ProductionDetailTerceros detail = new ProductionDetailTerceros();
                        detail.ProductionId = Consecutive;
                        detail.CodeEnterprise = terceros.CodeEnterprise;

                        boolean exi =db.ExistRelProductionOperator(terceros.CodeEnterprise, Consecutive.toString());

                        if(!exi){
                            db.CreateProductionDetailTercero(detail);

                        }
                        fragment.loadData(Consecutive);
                        getDialog().dismiss();

                    }
                }else {
                    Toast.makeText(getActivity().getApplicationContext(),"Debe Agregar Operarios", Toast.LENGTH_SHORT).show();

                }


            }
        });

        return  root;
    }
}