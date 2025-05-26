package com.example.unispanapp.Models;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import com.example.unispanapp.R;

import java.util.List;

public class CustomAdapter extends BaseAdapter {

    Context context;
    List<Production> list;

    public CustomAdapter(Context context, List<Production> list) {
        this.context = context;
        this.list = list;
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

        TextView lblNameActivity;
        TextView lblNameOperators;

        Production c = list.get(position);
        if (convertView == null)
            convertView = LayoutInflater.from(context).inflate(R.layout.listview_list_production,null);

        lblNameActivity = convertView.findViewById(R.id.lblNameActivity);
        lblNameOperators = convertView.findViewById(R.id.lblNameOperator);

        lblNameActivity.setText(c.NameActivity);

        lblNameOperators.setText(c.NameOperator);

        return convertView;
    }
}
