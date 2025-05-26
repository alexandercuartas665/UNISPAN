package com.example.unispanapp.Models;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class Converters {

        public static String fromObjectToString(Object obj){

            GsonBuilder builder = new GsonBuilder();
            Gson gson = builder.create();
            return gson.toJson(obj);
        }

        public static Object fromStringJSONToObject(String json, Class type){
            Gson gson = new Gson();
            //json = json.replace("\"{", "{").replace("}\"", "}");
            json = String.format("%s%s%s", "{", json.substring(2, json.length() - 2), "}");
            return  gson.fromJson(json, type);
        }


}
