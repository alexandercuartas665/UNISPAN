package com.example.unispanapp;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.example.unispanapp.DB.DbManager;

public class Login extends AppCompatActivity {

    EditText UserName;
    EditText PassWord;
    Button btnLogin;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);



        final DbManager db = new DbManager(getApplicationContext());


        UserName = (EditText)findViewById(R.id.username_input);
        PassWord = (EditText)findViewById(R.id.pass);
        btnLogin = (Button)  findViewById(R.id.btnLogin);

        btnLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Boolean loginOK = db.Login(UserName.getText().toString(),PassWord.getText().toString());

                if(loginOK == true)
                {
                    Toast.makeText(getApplicationContext(),"Bienvenido", Toast.LENGTH_SHORT).show();

                    Integer user_id = db.getUserIdByUserName(UserName.getText().toString());

                    Intent i = new Intent(getApplicationContext(),MainActivity.class);
                    i.putExtra("usuario", UserName.getText().toString());
                    i.putExtra("user_id", user_id);
                    startActivity(i);

                }else
                {

                    Toast.makeText(getApplicationContext(),"Usuario No Existe", Toast.LENGTH_SHORT).show();
                }





            }
        });


    }
}