package com.example.unispanapp.Models;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.TextView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogAddOperatorFragment;
import com.example.unispanapp.Novedades.NovedadesFragment;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.List;

public class listview_novedades  extends BaseAdapter {

    Context context;
    List<Novedad> list;
    DetailProductionFragment fragment;
    List<Novedad> newList;
    Novedad c;
    NovedadesFragment novedadesFragment;
    DbManager db;
    public listview_novedades(Context context, List<Novedad> list, NovedadesFragment novedadesFragment) {
        this.context = context;
        this.list = list;
        this.novedadesFragment = novedadesFragment;
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
    public View getView(int position, View convertView, ViewGroup parent) {
        TextView lblNameOperator;
        TextView fecha;
        TextView cantidad;
        if (convertView == null)
            convertView = LayoutInflater.from(context).inflate(R.layout.listview_novedades,null);

        db = new DbManager(novedadesFragment.getContext());

        c = list.get(position);
        newList = list;

        lblNameOperator = convertView.findViewById(R.id.lblNameOperator);
        fecha = convertView.findViewById(R.id.fecha);
        cantidad = convertView.findViewById(R.id.cantidad);

        lblNameOperator.setText(c.OPERARIO_ID + "-" + c.NameOperators);
        fecha.setText(c.FECHA_INI+" / "+c.FECHA_FIN);
        cantidad.setText(c.CANTIDAD.toString());

        c = list.get(position);
        newList = list;



        return convertView;
    }
}
