package com.example.unispanapp.Novedades;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;

import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogFindOperatorsFragment;
import com.example.unispanapp.Dialogs.DialogNewNovedadFragment;
import com.example.unispanapp.Models.Novedad;
import com.example.unispanapp.Models.listview_novedades;
import com.example.unispanapp.R;

import java.util.List;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link NovedadesFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class NovedadesFragment extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";
    public  Novedad dataNovedad;

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;
    Button btnNewNovedad;
    ListView listViewNovedades;
    List<Novedad> listNovedades;
    listview_novedades adapter ;
            DbManager db;

    public NovedadesFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment NovedadesFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static NovedadesFragment newInstance(String param1, String param2) {
        NovedadesFragment fragment = new NovedadesFragment();
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
        View root = inflater.inflate(R.layout.fragment_novedades, container, false);

        db= new DbManager(getContext());

        btnNewNovedad = (Button)root.findViewById(R.id.btnNewNovedad);
        listViewNovedades = (ListView)root.findViewById(R.id.listViewNovedades);

        listNovedades = db.getListNovedades();

        adapter = new listview_novedades(getContext(),listNovedades,this);
        listViewNovedades.setAdapter(adapter);

        final  NovedadesFragment novfra=this;

        listViewNovedades.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                  dataNovedad  = listNovedades.get(position);

                DialogNewNovedadFragment dialogNewNovedadFragment = new DialogNewNovedadFragment(2,novfra);
                dialogNewNovedadFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");
            }
        });


        btnNewNovedad.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogNewNovedadFragment dialogNewNovedadFragment = new DialogNewNovedadFragment(1,novfra);
                dialogNewNovedadFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");
            }
        });

        return  root;
    }

    public void RefreshAdapter(){
        List<Novedad>  listNov = db.getListNovedades();
        listNovedades.clear();
        listNovedades.addAll(listNov);
        adapter.notifyDataSetChanged();
    }
}