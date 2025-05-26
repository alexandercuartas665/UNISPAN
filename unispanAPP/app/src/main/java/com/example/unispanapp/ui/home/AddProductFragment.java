package com.example.unispanapp.ui.home;

import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentResultListener;
import androidx.navigation.Navigation;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogFindProductFragment;
import com.example.unispanapp.Models.CustomAdapter;
import com.example.unispanapp.Models.CustomAdapterDetailProductionItem;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.R;

import java.util.ArrayList;
import java.util.List;


public class AddProductFragment extends Fragment {

   public static String IdITem;

  public static TextView lblCodProduct;
  public static TextView lblNameProducto;

  DbManager db;

    Button bntBuscar;

    Button btnSaveProduct;
    private ListView detailProdItems;
    private ArrayAdapter<String> adapter;
    public ArrayList<String> listProduction;
    List<DetatilProductionItem> listProDetail;
    EditText tbxCantidad;

    public static Integer Consecutive;

    public AddProductFragment() {
        // Required empty public constructor
    }



    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_add_product, container, false);

        db = new DbManager(getContext());

        final  DetailProductionFragment dpf = new DetailProductionFragment();

        Consecutive = dpf.Consecutive;

        lblNameProducto = (TextView)root.findViewById(R.id.lblNameProducto);
        lblCodProduct = (TextView)root.findViewById(R.id.lblCodProduct);

        bntBuscar = (Button)root.findViewById(R.id.btnAddProduct);

        btnSaveProduct = (Button)root.findViewById(R.id.btnSaveProduct);

        tbxCantidad = (EditText)root.findViewById(R.id.tbxCantidad);

        bntBuscar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                DialogFindProductFragment findProductFragment = new DialogFindProductFragment(null);

                findProductFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");
            }
        });

        btnSaveProduct.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                DetatilProductionItem newItem = new DetatilProductionItem();
                newItem.count = Integer.parseInt(tbxCantidad.getText().toString());
                newItem.consecutive = Consecutive;
                newItem.itemId = Integer.parseInt(IdITem);
                newItem.observations = "";
                newItem.url_photo1 = "";
                newItem.url_photo2 = "";
                newItem.url_photo3 = "";

                db.CreateDetailProduciontItem(newItem);


                Navigation.findNavController(getActivity(), R.id.nav_host_fragment).navigate(R.id.detail_production_fragment);



            }
        });


        return root;
    }

}