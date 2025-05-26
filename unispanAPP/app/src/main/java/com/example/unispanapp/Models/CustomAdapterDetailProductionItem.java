package com.example.unispanapp.Models;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Base64;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.FragmentManager;

import com.example.unispanapp.Dialogs.DialogViewPhoto;
import com.example.unispanapp.Dialogs.UpdateResulItemDialogFragment;
import com.example.unispanapp.MainActivity;
import com.example.unispanapp.R;
import com.example.unispanapp.Util.UtilBitmap;
import com.example.unispanapp.ui.home.DetailProductionFragment;

import java.util.List;

class WrapperItemsMedicion
{
    public View baseView;
    private Button button;

    private ImageView imageview;
    private TextView lblNameItem;
    private  TextView lblCount;
    public Items item;

    public WrapperItemsMedicion(View base,Items item)
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
        if (lblNameItem == null)
        {
            lblNameItem = (TextView) baseView.findViewById(R.id.lblItemName);
        }
        return (lblNameItem);
    }
    public TextView getTxtCant()
    {
        if (lblCount == null)
        {
            lblCount = (TextView) baseView.findViewById(R.id.lblcantidadAdapter);
        }
        return (lblCount);
    }
}


public class CustomAdapterDetailProductionItem extends BaseAdapter {

    Context context;
    List<DetatilProductionItem> list;
    DetailProductionFragment fragment;
    public CustomAdapterDetailProductionItem(Context context, List<DetatilProductionItem> list,DetailProductionFragment fragment) {
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

        TextView lblNameItem;
        TextView lblCount;
        ImageView img;

        final DetatilProductionItem c = list.get(position);
        if (convertView == null)
            convertView = LayoutInflater.from(context).inflate(R.layout.listview_detail_production_item,null);

        lblNameItem = convertView.findViewById(R.id.lblNameItem);
        lblCount = convertView.findViewById(R.id.lblcantidadAdapter);
        img = convertView.findViewById(R.id.imvfoto);


        if(c.url_photo1!=null && !c.url_photo1.equals("")) {
            img.setImageBitmap(c.GetBitampPhoto1((MainActivity)context,c.url_photo1));
        }else{
            img.setImageBitmap(null);
        }
        img.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogViewPhoto dialogViewPhoto = new DialogViewPhoto(c.url_photo1,c.code.toString(),c.consecutive,fragment);
                dialogViewPhoto.show(((MainActivity)context).getSupportFragmentManager(),"dialogViewPhoto");
            }
        });

        lblNameItem.setText(c.descriptionItem);

        lblCount.setText(c.count.toString());

        final ImageButton btnonCamera = convertView.findViewById(R.id.btnOpenCamera);

        btnonCamera.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                fragment.openCamera(c,1);
            }
        });


        lblNameItem.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                DetatilProductionItem c = list.get(position);

                Bundle args = new Bundle();
                args.putString("code", c.code.toString());
                args.putString("consecutive", c.consecutive.toString());
                DialogFragment newFragment = new UpdateResulItemDialogFragment(fragment);
                newFragment.setArguments(args);

                newFragment.show(((MainActivity)context).getSupportFragmentManager(), "UpdateResulItemDialogFragment");
            }
        });

        return convertView;
    }



}
