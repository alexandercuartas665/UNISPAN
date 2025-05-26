package com.example.unispanapp.ui.home;

import android.os.Bundle;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import androidx.navigation.Navigation;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogFindOperatorsFragment;
import com.example.unispanapp.Dialogs.DialogFindProductFragment;
import com.example.unispanapp.MainActivity;
import com.example.unispanapp.Models.Activitys;
import com.example.unispanapp.Models.Category;
import com.example.unispanapp.Models.Operators;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.Models.ProductionDetailTerceros;
import com.example.unispanapp.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.util.ArrayList;
import java.util.List;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link NewProductionFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class NewProductionFragment extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    EditText tbxDateNow;
    DbManager db;
    Button btnAddOperator;
    EditText tbxCodOperario;
    Button btnSeveProduction;
    Spinner ddlActivityList;
    Spinner ddlCategorys;
    ImageButton btnOpenFind;

    Integer Consecutivo = 0;
    public static   ArrayList<String>arrayList;
    public static ArrayAdapter<String>adapter;
    public static ListView list;
    public static List<String>codLis;

    public NewProductionFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment NewProductionFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static NewProductionFragment newInstance(String param1, String param2) {
        NewProductionFragment fragment = new NewProductionFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
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
        final View root = inflater.inflate(R.layout.fragment_new_production, container, false);

        db = new DbManager(root.getContext());

        final HomeFragment home = new HomeFragment();

        String date = home.date_n;

        tbxDateNow = (EditText)root.findViewById(R.id.tbxDateNow);
        tbxDateNow.setText(date);
        btnSeveProduction = (Button)root.findViewById(R.id.btnSeveProduction);
        tbxCodOperario = (EditText)root.findViewById(R.id.tbxCodOperario);
        btnAddOperator = (Button)root.findViewById(R.id.btnAddOperator);
        list = (ListView)root.findViewById(R.id.list_view);
        arrayList = new ArrayList<String>();
        btnOpenFind = (ImageButton)root.findViewById(R.id.btnOpenFind);

        codLis = new ArrayList<>();

        adapter = new ArrayAdapter<String>(getContext(), R.layout.support_simple_spinner_dropdown_item,arrayList);
        list.setAdapter(adapter);
        final  NewProductionFragment nf = this;
        btnOpenFind.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogFindOperatorsFragment findProductFragment = new DialogFindOperatorsFragment(nf,1,null);
                findProductFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");
            }
        });

        btnAddOperator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(!tbxCodOperario.getText().toString().isEmpty()){

                String cod = tbxCodOperario.getText().toString();
                    tbxCodOperario.setText("");
                    boolean exist = db.ExisteCodOperator(cod);

                    if(exist){

                        if(!codLis.contains(cod)){

                            String fullName = db.getFullNameOperator(cod);
                            arrayList.add(cod+"-"+fullName);
                            adapter.notifyDataSetChanged();


                            codLis.add(cod);
                        }else {
                            Toast.makeText(getActivity().getApplicationContext(),"Ya existe el Operario", Toast.LENGTH_SHORT).show();

                        }

                    }else {
                        Toast.makeText(getActivity().getApplicationContext(),"El Código el Operario no Existe", Toast.LENGTH_SHORT).show();

                    }

                }else {
                    Toast.makeText(getActivity().getApplicationContext(),"El Codigo del Operario está vacío", Toast.LENGTH_SHORT).show();

                }

            }
        });

        ddlCategorys = (Spinner)root.findViewById(R.id.ddlCategorys);
        ddlActivityList  = (Spinner)root.findViewById(R.id.ddlActivityList);

        llenarCategorys();

        ddlCategorys.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {

                Category category =(Category)  ddlCategorys.getSelectedItem();

                llenarActivits(category.categoryId);

            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        btnSeveProduction.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Category category =(Category)  ddlCategorys.getSelectedItem();

                Activitys activit =(Activitys)  ddlActivityList.getSelectedItem();

                if(category.categoryId !=-1){

                    if(activit.typeActivityId != -1){


                        if(codLis.size() != 0){

                            Consecutivo = db.getConsecutiveProduction();
                            Integer user = getActivity().getIntent().getExtras().getInt("user_id");

                            Production data = new Production();
                            data.Consecutive = Consecutivo;
                            data.UserAppId = user;
                            data.DateProduction = home.Date.toString();
                            data.TypeActivity = activit.typeActivityId;
                            data.StateId = "0";
                            data.photo = "";

                            db.CreateProduction(data);

                            for (String operator : codLis) {

                                ProductionDetailTerceros detail = new ProductionDetailTerceros();
                                detail.ProductionId = Consecutivo;
                                detail.CodeEnterprise = operator.trim();

                                db.CreateProductionDetailTercero(detail);

                            }

                            Bundle bundle = new Bundle();
                            bundle.putInt("consecutive",Consecutivo);
                            getParentFragmentManager().setFragmentResult("key",bundle);

                            try{

                                Navigation.findNavController(getActivity(), R.id.nav_host_fragment).navigate(R.id.detail_production_fragment);


                            }catch (Exception ex){
                                String Error =ex.getMessage();
                            }


                        }else {
                            Toast.makeText(getActivity().getApplicationContext(),"Debe Agregar Operarios", Toast.LENGTH_SHORT).show();

                        }

                    }else {
                        Toast.makeText(getActivity().getApplicationContext(),"Seleccione Actividad", Toast.LENGTH_SHORT).show();

                    }


                }else {
                    Toast.makeText(getActivity().getApplicationContext(),"Seleccione Categoría", Toast.LENGTH_SHORT).show();

                }

            }
        });



        return  root;

    }

    public void llenarCategorys(){
        Category firstItemCa = new Category();
        firstItemCa.categoryId = -1;
        firstItemCa.categoryName = "Seleccione Categoria";
        List<Category> listCategorys = new ArrayList<Category>();
        listCategorys.add(firstItemCa);

        for(Category c : db.getListCategorys()){
            listCategorys.add(c);
        }

        ArrayAdapter<Category> adapterCategory = new ArrayAdapter<Category>(getContext(), R.layout.spinner_layout, R.id.txt,listCategorys);
        ddlCategorys.setAdapter(adapterCategory);
    }

    public void llenarActivits(Integer categoryId){

        List<Activitys> listActivitys = new ArrayList<Activitys>();
        Activitys firstItemAct = new Activitys();
        firstItemAct.typeActivityId = -1;
        firstItemAct.name = "Seleccione Actividad";
        listActivitys.add(firstItemAct);

        for (Activitys a :  db.getListActivityes(categoryId)){
            listActivitys.add(a);
        }

        ArrayAdapter<Activitys> adapter = new ArrayAdapter<Activitys>(getContext(), R.layout.spinner_layout, R.id.txt,listActivitys);
        ddlActivityList.setAdapter(adapter);
    }

    @Override
    public void onStart() {
        super.onStart();
        if(codLis!=null){
            codLis.clear();
        }
    }
}