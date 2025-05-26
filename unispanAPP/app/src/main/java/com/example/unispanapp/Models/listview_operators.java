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

import com.example.unispanapp.Contract.OperatorContract;
import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogAddOperatorFragment;
import com.example.unispanapp.Dialogs.DialogFindOperatorsFragment;
import com.example.unispanapp.Dialogs.DialogNewNovedadFragment;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;
import com.example.unispanapp.ui.home.NewProductionFragment;

import java.util.List;

public class listview_operators extends BaseAdapter {
    Context context;
    List<Operators> list;
    Operators c;
    DbManager db;
    NewProductionFragment fragment;
    DialogNewNovedadFragment novedad;
    Integer acc;
    DialogFindOperatorsFragment operatorsFragment;
    public listview_operators(Context context, DialogFindOperatorsFragment operatorsFragment, List<Operators>list, NewProductionFragment fragment, DialogNewNovedadFragment novedad, Integer acc){
        this.context = context;
        this.list = list;
        this.acc= acc;
        this.operatorsFragment = operatorsFragment;
        if(fragment != null){
            this.fragment = fragment;
        }
        if(novedad != null){
            this.novedad = novedad;
        }

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

        try{
            TextView lblNameOperator;
            Button btnAddOperator;

            if(acc == 1){
                db = new DbManager(fragment.getContext());

            }else {
                db = new DbManager(novedad.getContext());

            }

            if (convertView == null)
                convertView = LayoutInflater.from(context).inflate(R.layout.listview_operators,null);

            c = list.get(position);
            lblNameOperator = convertView.findViewById(R.id.lblNameOperator);
            btnAddOperator = convertView.findViewById(R.id.btnAddOperator);

            lblNameOperator.setText(c.codeEnterprise + "-" + c.fullName);

            btnAddOperator.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    c = list.get(position);

                    if(acc == 1){
                        if(!fragment.codLis.contains(c.codeEnterprise)){

                            fragment.arrayList.add(c.codeEnterprise+"-"+c.fullName);
                            fragment.adapter.notifyDataSetChanged();

                            fragment.codLis.add(c.codeEnterprise);

                            Toast.makeText(fragment.getActivity().getApplicationContext(),"Operario Agregado", Toast.LENGTH_SHORT).show();

                        }else {
                            Toast.makeText(fragment.getActivity().getApplicationContext(),"Ya existe el Operario", Toast.LENGTH_SHORT).show();

                        }
                    }else {
                        novedad.lblNameOperario.setText(c.codeEnterprise+"-"+c.fullName);
                        novedad.codPerator =c.codeEnterprise;
                        operatorsFragment.getDialog().dismiss();
                    }


                }
            });

        }catch (Exception ex){
            String msg = ex.getMessage();
        }

        return convertView;

    }
}
