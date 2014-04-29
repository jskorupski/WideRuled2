package javacode;




import java.util.ArrayList;


import abl.runtime.PrimitiveAction;

public class WR_PrintAction extends WR_Actions {
	


    public void execute(Object[] args) {
    // Called every decision cycle. The default implementation does
    // nothing. Can be overridden on concrete action classes if the
    // specific sensory-motor system needs some processing to be
    // performed every decision cycle.

       String rawText = (String)args[0];
       String processedText = rawText;
  
       boolean printLine = false;
       boolean processText = false;
       if(args.length == 1)
       {
    	   WR_Bot.getBot().sendPrint(processedText, this);
       }
       else if(args.length == 2)
       {
    	   if(args[1] instanceof Boolean)
    	   {
    		   printLine = (boolean)((Boolean)args[1]);
    		   processText = false;
    	   }
    	   else
    	   {
    		   printLine = false;
    		   processText = true;
    	   }
       
       }
       else if (args.length == 3)
       {
    	   processText = true;
    	   printLine = (boolean)((Boolean)args[2]);
       }
       
       if(processText)
       {
    	   ArrayList vars = (ArrayList)args[1];
    	   
    	   String resultString = "";
    	   
    	   String[] matches = rawText.split("<[^<]+>");
    	   
    	   for(int i=0; i<matches.length;i++) {
    
    		   resultString += matches[i];
    		   if(i < vars.size())
    		   {
    			   resultString += vars.get(i);
    		   }
    		   
    	   }
    	   
    	
    	   processedText = resultString;
		  
 
       }
       this.completionStatus = PrimitiveAction.NOT_COMPLETE;
       
       if(printLine)
       {
    	   WR_Bot.getBot().sendPrintLine(processedText, this);
       }
       else
       {
    	   WR_Bot.getBot().sendPrint(processedText, this);
       }
       
       
       try
       {
		   //Count words
		   int numWords = processedText.split("[\\w]+").length;
    	   Thread.sleep(1000 + 1000*(numWords/9));
       } catch (Exception e) {}
        
       
       
       this.completionStatus = PrimitiveAction.SUCCESS;
        
  
     }

     public void abort() { } // cannot be aborted
}
