package com.example.unispanapp.Models;

import android.app.Application;

public class Paramters_Globals extends Application {

     Number TransOption;
      Number pageSize;
     Number pageIndex;

    public Paramters_Globals(Number transOption, Number pageSize, Number pageIndex) {
        this.TransOption =transOption;
        this.pageSize = pageSize;
        this.pageIndex = pageIndex;
    }

    public Paramters_Globals(Number transOption) {
        this.TransOption =transOption;


    }

}
