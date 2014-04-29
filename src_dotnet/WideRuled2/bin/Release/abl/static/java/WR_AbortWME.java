package javacode;


import wm.WME;



public class WR_AbortWME extends WME {
    private boolean abortStory;
    


    public WR_AbortWME(boolean abortStory)
    {
    	this.abortStory = abortStory;
    }

    public WR_AbortWME() {}

 
   
    public synchronized boolean getAbortStory() { return abortStory; }


    
    public synchronized void setAbortStory(boolean abortStory) { this.abortStory = abortStory; }
  

}

