package com.example.unispanapp.Models;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import androidx.recyclerview.widget.RecyclerView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogFindProductFragment;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.List;

class WrapperItemSearch
{
    public View baseView;
    private Button button;
    private TextView textproduc;
    EditText txtcant;
    public Items item;
    public WrapperItemSearch(View base,Items item)
    {
        this.baseView = base;
        this.item=item;
    }
    public Button getButton()
    {
        if (button == null)
        {
            button = (Button) baseView.findViewById(R.id.bntAddItem);
        }
        return (button);
    }
    public TextView getTextProduct()
    {
        if (textproduc == null)
        {
            textproduc = (TextView) baseView.findViewById(R.id.lblItemName);
        }
        return (textproduc);
    }
    public EditText getTxtCant()
    {
        if (txtcant == null)
        {
            txtcant = (EditText) baseView.findViewById(R.id.tbxCount);
        }
        return (txtcant);
    }
}

public class items_listview_adapter extends BaseAdapter {

    Context context;
    List<Items> list;
    DetailProductionFragment fragment;

    public items_listview_adapter(Context context, List<Items> list,DetailProductionFragment fragment) {
        this.context = context;
        this.list = list;
        this.fragment=fragment;
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
        Items c = list.get(position);
        WrapperItemSearch wrapper=null;
        if (convertView == null) {
            convertView = LayoutInflater.from(context).inflate(R.layout.items_listview_adapter, null);
            wrapper=new WrapperItemSearch(convertView,c);
            convertView.setTag(wrapper);
        }
        else
        {
            wrapper  = (WrapperItemSearch) convertView.getTag();
        }
        wrapper.item=c;
        wrapper.getTextProduct().setText(c.description);
        wrapper.getTxtCant().setText("");
        wrapper.getButton().setTag(wrapper);
        wrapper.getButton().setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                WrapperItemSearch  wrapper2  = (WrapperItemSearch)v.getTag();
                EditText tbxCount2 = wrapper2.getTxtCant();
                try{
                    Integer.parseInt( tbxCount2.getText().toString());
                }catch (Exception ex){
                    return  ;
                }
                Items c2 = wrapper2.item;
                DetatilProductionItem newItem = new DetatilProductionItem();
                newItem.count = Integer.parseInt(tbxCount2.getText().toString());
                newItem.consecutive = fragment.Consecutive;
                newItem.itemId = Integer.parseInt(c2.itemId.toString());
                newItem.observations = "";
                newItem.url_photo1 = "";
                newItem.url_photo2 = "";
                newItem.url_photo3 = "";
                fragment.SaveItem(newItem);
                tbxCount2.setText("");
            }
        });

        return convertView;

    }



}


