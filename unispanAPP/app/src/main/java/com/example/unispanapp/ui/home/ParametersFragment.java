package com.example.unispanapp.ui.home;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.MainActivity;
import com.example.unispanapp.Models.Paramters_Globals;
import com.example.unispanapp.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.navigation.NavigationView;

import java.util.zip.Inflater;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link ParametersFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class ParametersFragment extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";
    public TextView tbxURL_Service;
    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    DbManager db;
    Button btnSaveParameter;

    FloatingActionButton fab;

    public ParametersFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment ParametersFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static ParametersFragment newInstance(String param1, String param2) {
        ParametersFragment fragment = new ParametersFragment();
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
        View root = inflater.inflate(R.layout.fragment_parameters, container, false);

        db = new DbManager(root.getContext());

        btnSaveParameter = (Button)root.findViewById(R.id.btnSaveParamteros);

        tbxURL_Service = (TextView)root.findViewById(R.id.tbxURL_Service);

        String url = "";


        url = db.DataParameter("URL_PARAMETER");

        tbxURL_Service.setText(url);

        btnSaveParameter.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                db.UpdateParameter("URL_PARAMETER",tbxURL_Service.getText().toString());
                Toast.makeText(getContext(),"Parametrod Modificado Correctamente", Toast.LENGTH_SHORT).show();
            }
        });


        return root;
    }
}