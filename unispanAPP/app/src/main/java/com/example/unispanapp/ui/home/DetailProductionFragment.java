package com.example.unispanapp.ui.home;

import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.MediaScannerConnection;
import android.net.Uri;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.FileProvider;
import androidx.fragment.app.DialogFragment;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentResultListener;
import androidx.navigation.Navigation;

import android.os.Environment;
import android.provider.MediaStore;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.load.model.ModelLoader;
import com.example.unispanapp.DB.DbManager;
import com.example.unispanapp.Dialogs.DialogAddOperatorFragment;
import com.example.unispanapp.Dialogs.DialogFindProductFragment;
import com.example.unispanapp.Dialogs.DialogViewPhoto;
import com.example.unispanapp.Dialogs.UpdateResulItemDialogFragment;
import com.example.unispanapp.Models.CustomAdapter;
import com.example.unispanapp.Models.CustomAdapterDetailProductionItem;
import com.example.unispanapp.Models.DetatilProductionItem;
import com.example.unispanapp.Models.Items;
import com.example.unispanapp.Models.Production;
import com.example.unispanapp.Models.items_listview_adapter;
import com.example.unispanapp.R;
import com.example.unispanapp.Util.UtilBase64;
import com.example.unispanapp.Util.UtilBitmap;
import com.example.unispanapp.Util.UtilDirectorys;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link DetailProductionFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class DetailProductionFragment extends Fragment {


    private String path;
    File fileImage;
    Bitmap bitmap;


    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    public static DbManager db;
    public static Integer Consecutive;
    public  TextView lblNameActivity;
   public  TextView lblNameOperator;
   public  TextView lblDate;
    Button btnAddProduct;
    ImageButton btnOpenCameraProduction;
    ImageView imgGlobal;
    List<DetatilProductionItem> listProDetail;
    public static ListView litViewItems;
    public static ArrayList<String> arrayList;
    public static ArrayAdapter<String>adapter;
    ImageButton bntEditOperator;


    DetatilProductionItem datasavefoto;

    public DetailProductionFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment DetailProductionFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static DetailProductionFragment newInstance(String param1, String param2) {
        DetailProductionFragment fragment = new DetailProductionFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, final ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root= inflater.inflate(R.layout.fragment_detail_production, container, false);
        db = new DbManager(getContext());



        litViewItems = (ListView)root.findViewById(R.id.litViewItems);

        imgGlobal = (ImageView)root.findViewById(R.id.imgGlobal);
        imgGlobal.setImageBitmap(null);
        lblNameActivity = (TextView)root.findViewById(R.id.lblNameActivity);
      lblNameOperator = (TextView)root.findViewById(R.id.lbloperators);
      lblDate = (TextView)root.findViewById(R.id.lblDate);
      btnAddProduct = (Button)root.findViewById(R.id.btnAddProduct);
     btnOpenCameraProduction = (ImageButton)root.findViewById(R.id.btnOpenCameraProduction);

        bntEditOperator = (ImageButton)root.findViewById(R.id.bntEditOperator);

     btnOpenCameraProduction.setOnClickListener(new View.OnClickListener() {
         @Override
         public void onClick(View v) {
             openCamera(null,2);
           /*  Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
             startActivityForResult(takePictureIntent, 2);*/
         }
     });



        getParentFragmentManager().setFragmentResultListener("key", this, new FragmentResultListener() {
            @Override
            public void onFragmentResult(@NonNull String requestKey, @NonNull Bundle result) {
                Integer consecutivo = result.getInt("consecutive");
                Consecutive = consecutivo;
                loadData(consecutivo);
            }
        });

        if(Consecutive != null && Consecutive !=0){
            loadData(Consecutive);
        }
 final DetailProductionFragment det=this;
        btnAddProduct.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

              /*  Bundle bundle = new Bundle();
                bundle.putInt("consecutive",Consecutive);
                getParentFragmentManager().setFragmentResult("key",bundle);

                Navigation.findNavController(getActivity(), R.id.nav_host_fragment).navigate(R.id.add_product_fragment);*/

                DialogFindProductFragment findProductFragment = new DialogFindProductFragment(det);
                findProductFragment.show(getActivity().getSupportFragmentManager(),"DialogFindProductFragment");

            }
        });

        imgGlobal.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Production data = db.getDataProducionts(Consecutive);

                DialogViewPhoto dialogViewPhoto = new DialogViewPhoto(data.photo,"0",Consecutive,det);
                dialogViewPhoto.show(getActivity().getSupportFragmentManager(),"dialogViewPhoto");
            }
        });


        bntEditOperator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                DialogAddOperatorFragment Do = new DialogAddOperatorFragment(Consecutive, det);
                Do.show(getActivity().getSupportFragmentManager(),"DialogAddOperatorFragment");
            }
        });




        return root;

    }



    @Override
    public void onDestroyView() {
       if(listProDetail!=null && listProDetail.size()==0){
            db.deleteOperator(Consecutive.toString());
            db.deleteProduction(Consecutive.toString());
        }
        super.onDestroyView();
    }

    public void loadData(Integer consecutive)  {

        try {
            Production data = db.getDataProducionts(consecutive);

            if(data.getPhoto() != null && !data.getPhoto().equals("")){
                Bitmap bitmap = UtilBitmap.GetBitmapOfPath(getActivity(),data.photo);
                imgGlobal.setImageBitmap(bitmap);
            }else{
                imgGlobal.setImageBitmap(null);
            }

            lblNameActivity.setText(data.NameActivity);
            lblNameOperator.setText(data.NameOperator);

            String date = data.DateProduction;

            SimpleDateFormat format1 = new SimpleDateFormat("yyy-MM-dd");
            Date dated = format1.parse(date);

            String date_n = new SimpleDateFormat("dd/MMMM/yyy", Locale.getDefault()).format(dated);

            lblDate.setText(date_n);

            listProDetail = db.getListDetailProducionItems(Consecutive);



            try {
//revisar ese context

                CustomAdapterDetailProductionItem adapter = new CustomAdapterDetailProductionItem(getContext(),listProDetail,this);

                litViewItems.setAdapter(adapter);


            }catch (Exception ex){
                Toast.makeText(getContext(),ex.getMessage(), Toast.LENGTH_SHORT).show();

            }

        }catch (Exception ex)
        {

        }
    }

    public void SaveItem(DetatilProductionItem item){

        Integer code = db.getCodeDetailProduction();

        DetatilProductionItem newItem = new DetatilProductionItem();
        newItem.code = code;
        newItem.count = item.count;
        newItem.consecutive = item.consecutive;
        newItem.itemId = item.itemId;
        newItem.observations = "";
        newItem.url_photo1 = "";
        newItem.url_photo2 = "";
        newItem.url_photo3 = "";

      boolean inser=   db.CreateDetailProduciontItem(newItem);

      if(inser){
          Toast.makeText(getActivity().getApplicationContext(),"Item Agregado", Toast.LENGTH_SHORT).show();

      }

        loadData(item.consecutive);



    }

    String currentPhotoPath;
    String imageName ;
    File FileimagePhoto ;
    private File createImageFile() throws IOException {
    /*    if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.KITKAT) {
            File[] files= getActivity().getExternalFilesDirs(null);
        }*/
        // Create an image file name
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
       String   imageFileName = "JPEG_" + timeStamp + "_";
        String pathsd=UtilDirectorys.getSDcardDirectoryPath();
        File storageDir = getActivity().getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        File image = File.createTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
        );

        imageName= image.getName();
        // Save a file: path for use with ACTION_VIEW intents
        currentPhotoPath = image.getAbsolutePath();
        return image;
    }

    Uri  photoURI;
    public void  openCamera (DetatilProductionItem c,int resultcode){
        try {
            datasavefoto = c;
            Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            FileimagePhoto =createImageFile();
             photoURI = FileProvider.getUriForFile(getContext(),
                    "com.example.unispanapp.provider",
                     FileimagePhoto);
            takePictureIntent.putExtra(MediaStore.EXTRA_OUTPUT, photoURI);
            startActivityForResult(takePictureIntent, resultcode);

        }catch (Exception ex){
                String msg = ex.getMessage();
        }

    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        Bitmap bitmap=null;
        // image = (Bitmap) data.getExtras().get("data");//miniatura
        try
        {
            getActivity().getContentResolver().notifyChange(photoURI, null);
            ContentResolver cr = getActivity().getContentResolver();
            Bitmap bitmapor = android.provider.MediaStore.Images.Media.getBitmap(cr, photoURI);
            FileimagePhoto.delete();
            imageName="C"+imageName;
            UtilBitmap.CompreessBitmap(getActivity(),bitmapor,imageName);
            bitmap=UtilBitmap.GetBitmapOfPath(getActivity(),imageName);
        }
        catch (Exception e)
        {
            //  Toast.makeText(this, "Failed to load", Toast.LENGTH_SHORT).show();
            //  Log.d(TAG, "Failed to load", e);
        }
        if(bitmap!=null) {
            if (requestCode == 1) {
                try {


                    DetatilProductionItem d = new DetatilProductionItem();
                    d.code = datasavefoto.code;
                    d.url_photo1 = imageName;
                    db.UpdatephotoProduciontItem(d);
                    loadData(datasavefoto.consecutive);

                    // image=bitmap;

                    //   imgGlobal.setImageBitmap(bitmap);
                } catch (Exception e) {
                    //  Toast.makeText(this, "Failed to load", Toast.LENGTH_SHORT).show();
                    //  Log.d(TAG, "Failed to load", e);
                }

            } else if (requestCode == 2) {

                Production dataP = new Production();
                dataP.photo = imageName;
                dataP.Consecutive = Consecutive;
                db.UpdateProductionPhoto(dataP);
                imgGlobal.setImageBitmap(bitmap);


            }
        }

    }

    //String currentPhotoPath;


}