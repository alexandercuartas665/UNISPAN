package com.example.unispanapp;

import android.app.FragmentTransaction;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.Menu;
import android.widget.TextView;

import com.example.unispanapp.ui.home.DetailProductionFragment;
import com.example.unispanapp.ui.home.NewProductionFragment;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.snackbar.Snackbar;
import com.google.android.material.navigation.NavigationView;

import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.navigation.NavController;
import androidx.navigation.Navigation;
import androidx.navigation.fragment.NavHostFragment;
import androidx.navigation.ui.AppBarConfiguration;
import androidx.navigation.ui.NavigationUI;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

public class MainActivity extends AppCompatActivity {

    private AppBarConfiguration mAppBarConfiguration;

    TextView userName;
    FloatingActionButton fab;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        fab = (FloatingActionButton)findViewById(R.id.fab);

        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Navigation.findNavController(MainActivity.this, R.id.nav_host_fragment).navigate(R.id.productionFragment);
            }
        });
        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        NavigationView navigationView = findViewById(R.id.nav_view);

        navigationView.getMenu().findItem(R.id.logout).setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
            @Override
            public boolean onMenuItemClick(MenuItem item) {
                logout();

                return true;
            }
        });

        // Passing each menu ID as a set of Ids because each
        // menu should be considered as top level destinations.
        mAppBarConfiguration = new AppBarConfiguration.Builder(
                R.id.nav_home,
                R.id.parameter,
                R.id.administracion)
                .setDrawerLayout(drawer)
                .build();

        View header = ((NavigationView)findViewById(R.id.nav_view)).getHeaderView(0);
        TextView userSession =(TextView)header.findViewById(R.id.lblUserName);

        String user = getIntent().getExtras().getString("usuario");

        userSession.setText(user);

        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment);
        NavigationUI.setupActionBarWithNavController(this, navController, mAppBarConfiguration);
        NavigationUI.setupWithNavController(navigationView, navController);


    }

    @Override
    public void onBackPressed() {
        Navigation.findNavController(this,R.id.nav_host_fragment).navigate(R.id.nav_home);
    }


    private void logout() {

        Intent i = new Intent(getApplicationContext(),Login.class);
        i.putExtra("usuario", "");
        i.putExtra("user_id", "");
        startActivity(i);

    }

    @Override
    public boolean onSupportNavigateUp() {
        NavController navController = Navigation.findNavController(this, R.id.nav_host_fragment);
        return NavigationUI.navigateUp(navController, mAppBarConfiguration)
                || super.onSupportNavigateUp();
    }

    public FloatingActionButton getFloatingActionButton() { return fab; }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        if(item.getItemId()==16908332) {
            Navigation.findNavController(this, R.id.nav_host_fragment);
            Fragment navHostFragt = getSupportFragmentManager().findFragmentById(R.id.nav_host_fragment);
            Fragment current = navHostFragt.getChildFragmentManager().getFragments().get(0);
            if(current.getClass() == DetailProductionFragment.class)
            {
                Navigation.findNavController(this,R.id.nav_host_fragment).navigate(R.id.nav_home);
                return  true;
            }
        }
              /*  switch (item.getItemId()) {
            case android.R.id.home:
                Toast.makeText(getApplicationContext(),"Back button clicked", Toast.LENGTH_SHORT).show();
                break;
        }*/
       return super.onOptionsItemSelected(item);
       // return true;
    }


}