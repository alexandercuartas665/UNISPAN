package com.example.unispanapp.Models;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogAddOperatorFragment;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.List;

public class listview_operator_edit extends BaseAdapter {

    Context context;
    List<ProductionDetailTerceros> list;
    DetailProductionFragment fragment;
    List<ProductionDetailTerceros> newList;
    ProductionDetailTerceros c;
    DialogAddOperatorFragment operatorFragment;
    DbManager db;
    public listview_operator_edit(Context context, List<ProductionDetailTerceros> list, DetailProductionFragment fragment, DialogAddOperatorFragment operators) {
        this.context = context;
        this.list = list;
        this.fragment=fragment;
        this.operatorFragment = operators;
    }

    @Override
    public int getCount() {
        return list.size();
    }

    @Override
    public Object getItem(int position) {
        return position;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(final int position, View convertView, ViewGroup parent) {

        TextView lblNameOperator;
        ImageButton btnDelteOperator;

        db = new DbManager(fragment.getContext());
        if (convertView == null)
            convertView = LayoutInflater.from(context).inflate(R.layout.listview_operator_edit,null);

         c = list.get(position);
        newList = list;
        lblNameOperator = convertView.findViewById(R.id.lblNameOperator);
        btnDelteOperator = convertView.findViewById(R.id.btnDelteOperator);

        lblNameOperator.setText(c.CodeEnterprise + "-" + c.NameOperator);

        btnDelteOperator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                c = list.get(position);
                newList.remove(c);
                 Toast.makeText(fragment.getActivity().getApplicationContext(),"Operario Eliminado", Toast.LENGTH_SHORT).show();
                operatorFragment.adapter.notifyDataSetChanged();
            }
        });


        return convertView;
    }
}
