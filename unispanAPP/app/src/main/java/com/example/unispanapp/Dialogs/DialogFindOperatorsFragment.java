package com.example.unispanapp.Dialogs;

import android.os.Bundle;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;

import com.example.unispanapp.Models.CustomAdapterDetailProductionItem;
import com.example.unispanapp.Models.listview_operators;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ListView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.Operators;
import com.example.unispanapp.Models.items_listview_adapter;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.NewProductionFragment;

import java.util.List;

public class DialogFindOperatorsFragment extends DialogFragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    DbManager db;
    ImageButton btnCloseDialog;
    NewProductionFragment fragment;
    ListView listView;
    List<Operators>listOperator;
    DialogNewNovedadFragment novedad;
    Integer acc;
    public DialogFindOperatorsFragment(NewProductionFragment fragment, Integer acc, DialogNewNovedadFragment novedad) {

        this.acc = acc;

        if(fragment != null){
            this.fragment = fragment;

        }

        if(novedad != null){
            this.novedad = novedad;
        }

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
        View root = inflater.inflate(R.layout.fragment_dialog_find_operators, container, false);

        db = new DbManager(getContext());

        btnCloseDialog = (ImageButton) root.findViewById(R.id.btnCloseDialog);
        listView = (ListView)root.findViewById(R.id.list_view) ;

        btnCloseDialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                getDialog().dismiss();
            }
        });

        listOperator = db.getlistOperators();
        listview_operators adapter;
        if(acc == 1){
            adapter  = new listview_operators(getContext(),this,listOperator,fragment, null, acc);

        }else {

             adapter = new listview_operators(getContext(),this,listOperator,null, novedad, acc);

        }


        listView.setAdapter(adapter);

        return root;
    }
}