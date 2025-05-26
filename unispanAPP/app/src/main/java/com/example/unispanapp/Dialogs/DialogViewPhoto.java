package com.example.unispanapp.Dialogs;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;

import android.util.Base64;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.R;
import com.example.unispanapp.Util.UtilBitmap;
import com.example.unispanapp.ui.home.DetailProductionFragment;

public class DialogViewPhoto extends DialogFragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    String PHOTHO;
    ImageView imgView;
    ImageButton btnDelPhoto;
    ImageButton btnCloseDialog;


    DetailProductionFragment fragment;
    String code;
    Integer Consecutive;
    DbManager db;

    public DialogViewPhoto(String photo, String code, Integer Concecutive, DetailProductionFragment fragment ) {
       this.PHOTHO = photo;
       this.code = code;
       this.fragment = fragment;
       this.Consecutive = Concecutive;

    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, final ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
      View root =   inflater.inflate(R.layout.fragment_dialog_view_photo, container, false);

      try{

          db = new DbManager(getContext());

          btnDelPhoto = (ImageButton)root.findViewById(R.id.btnDelPhoto);

          btnCloseDialog =(ImageButton)root.findViewById(R.id.btnCloseDialog);

          btnCloseDialog.setOnClickListener(new View.OnClickListener() {
              @Override
              public void onClick(View v) {
                  getDialog().dismiss();
              }
          });

          //byte[] decodedString = Base64.decode(PHOTHO, Base64.DEFAULT);
         // Bitmap decodedByte = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);
      //   Bitmap resizedBitmap = Bitmap.createScaledBitmap(decodedByte, 500 /*Ancho*/, 500 /*Alto*/, false /* filter*/);
          Bitmap decodedByte = UtilBitmap.GetBitmapOfPath(getActivity(),PHOTHO);
          imgView = (ImageView)root.findViewById(R.id.imgView);
          imgView.setImageBitmap(decodedByte);

          btnDelPhoto.setOnClickListener(new View.OnClickListener() {
              @Override
              public void onClick(View v) {

                  if(!code.equals("0")){
                      DetatilProductionItem d = new DetatilProductionItem();
                      d.code = Integer.parseInt(code);
                      d.url_photo1 = "";

                      db.UpdatephotoProduciontItem(d);
                  }else {
                      Production dataP = new Production();
                      dataP.photo ="";
                      dataP.Consecutive = Consecutive;

                      db.UpdateProductionPhoto(dataP);
                  }

                  fragment.loadData(Consecutive);

                  getDialog().dismiss();
              }
          });


      }catch (Exception ex){
          String msg = ex.getMessage();
      }

        return root;
    }
}