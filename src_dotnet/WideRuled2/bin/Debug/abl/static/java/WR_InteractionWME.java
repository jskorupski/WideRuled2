package javacode;


import wm.WME;



public class WR_InteractionWME extends WME {
    private int id;
    


    public WR_InteractionWME(int id)
    {
    	this.id = id;
    }

    public WR_InteractionWME() {}

    // ### get accessors ###
   
    public synchronized int getId() { return id; }


    
    public synchronized void setId(int id) { this.id = id; }
  

}

