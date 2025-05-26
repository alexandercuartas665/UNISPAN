package com.example.unispanapp.Dialogs;

import android.os.Bundle;

import androidx.fragment.app.DialogFragment;
import androidx.fragment.app.Fragment;

import android.text.InputType;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.R;
import com.example.unispanapp.ui.home.DetailProductionFragment;

public class UpdateResulItemDialogFragment extends DialogFragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    DbManager db;

    TextView lblCodProduct;
    TextView lblNameProducto;
    EditText tbxCantidad;
    EditText tbxObservaciones;
    Button btnSaveProduct;
    ImageButton btnCloseDialog;


    String code;
    String consecutive;
    DetailProductionFragment detail;
    public UpdateResulItemDialogFragment(DetailProductionFragment d) {
        // Required empty public constructor
        this.detail = d;
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
        View root = inflater.inflate(R.layout.fragment_update_resul_item_dialog, container, false);

        try {


            db = new DbManager(getContext());

            lblCodProduct = (TextView)root.findViewById(R.id.lblCodProduct);
            lblNameProducto = (TextView)root.findViewById(R.id.lblNameProducto);
            tbxCantidad = (EditText)root.findViewById(R.id.tbxCantidad);
            tbxObservaciones = (EditText)root.findViewById(R.id.tbxObservaciones);
            btnSaveProduct = (Button)root.findViewById(R.id.btnSaveProduct);

            btnCloseDialog =(ImageButton)root.findViewById(R.id.btnCloseDialog);

            btnCloseDialog.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    getDialog().dismiss();
                }
            });

            Bundle mArgs = getArguments();
             code = mArgs.getString("code");
            consecutive =  mArgs.getString("consecutive");

          final DetatilProductionItem data = db.DataProductionItem(Integer.parseInt(code));

          lblCodProduct.setText(data.barcode);
          lblNameProducto.setText(data.descriptionItem);
          String count = data.count.toString();
          tbxCantidad.setInputType(InputType.TYPE_TEXT_FLAG_CAP_SENTENCES);
          tbxCantidad.setText(count);

          btnSaveProduct.setOnClickListener(new View.OnClickListener() {
              @Override
              public void onClick(View v) {

                  Integer user = getActivity().getIntent().getExtras().getInt("user_id");

                  if(!tbxObservaciones.getText().toString().isEmpty()){

                      DetatilProductionItem dataUp = new DetatilProductionItem();
                      dataUp.code = Integer.parseInt(code);
                      dataUp.count = Integer.parseInt(tbxCantidad.getText().toString());

                      String obs = user.toString() +";"+ tbxCantidad.getText().toString()+";"+ tbxObservaciones.getText().toString();

                      dataUp.observations = data.observations + "|"+obs;

                     boolean up = db.UpdateDetailProduciontItem(dataUp);

                     if(up){

                         detail.loadData(Integer.parseInt(consecutive));
                         getDialog().dismiss();
                     }

                  }else {

                      Toast.makeText(getActivity().getApplicationContext(),"El campo observaciones está vacio", Toast.LENGTH_SHORT).show();

                  }
              }
          });

        }catch (Exception ex){
            String msg = ex.getMessage();
        }

        return root;
    }
}