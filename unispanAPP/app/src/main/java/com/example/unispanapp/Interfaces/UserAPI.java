package com.example.unispanapp.Interfaces;

import com.example.unispanapp.Models.Users;

import java.util.List;

import retrofit2.Call;
import retrofit2.http.GET;

public interface UserAPI {
   @GET("api/UserApp/SelectUsersMobile")
   Call<List<Users>> getUsers();

}
