/*
 * GameClient controls all the socket input and output 
 * 
 */

package javacode;


import java.net.*;
import java.io.*;
import java.util.*;


public class WR_GameClient
{
   

   protected Socket socket;
   protected int port = 5025;
   protected String hostname="localhost";

   private BufferedReader in;
   private PrintWriter out;
   private WR_Bot bot;
   private WR_Util log = new WR_Util(true,true);
   
   public WR_CallBack ck = new WR_CallBack();

   public WR_GameClient()
   {
   }
   
   //Sets the Bot class running
   public void setBot(WR_Bot in)
   {
      bot = in;
   }
   //Sets the address to connect to
   public void setAddress(String hostin, int portin)
   {
      hostname = hostin;
      port = portin;
   }
   
   //connects to the address specified
   //Starts up the receiver thread
   public void connect()
   {
	   Thread insocket;
	   int retryCount = 0;
	  boolean connected = false;
	  while(!connected)
	  {
	     try
	     {
	        	System.out.println("Connecting ...");
	            socket = new Socket(hostname,port+retryCount);
	        	
	            in = new BufferedReader(  new InputStreamReader( socket.getInputStream() ));
	            out = new PrintWriter(socket.getOutputStream());
	            insocket = new Thread(new receive());
	            
	            insocket.start();
	            connected = true;
	          
		  }
		  catch(Exception e)
		  {
			 System.out.println("Error: " + e.getMessage() + ", retrying ..");
			 try{
			   Thread.sleep(500); 
			 }
			 catch (Exception except) {}
			  
		     retryCount++;
			  
			 if(retryCount > 5)
			  {
				  if(socket != null) { 
					  try {
						  socket.close();
					  }
					  catch(Exception ex) {} 
		  		  }

				  System.out.println("Could not connect to server.");
				  System.exit(1);
			  }
			  
		  }
	
	   }
   }

   
      //Send message class that takes a variable number of parameters
      public void sendMessage(String msg, Map<String,String> params, WR_Actions action)
      {
         StringBuffer output = new StringBuffer(msg); 
         Set entry = params.entrySet();
         Iterator i = entry.iterator();

         while(i.hasNext())
         {
            Map.Entry ent = (Map.Entry) i.next();
            output.append(WR_Messages.linesplit+ent.getKey());
            output.append(WR_Messages.valuesplit+ent.getValue());
         }
         
         sendMessage(output.toString(),action);
      }
      
      public void abort()
      {
    	  out.flush();
    	  try{
    		  socket.close();
    	  } catch (Exception ex) {}
    	  
    	  System.exit(0);
      }
      //send message with a single parameter
      public synchronized void sendMessage(String msg, String name, String params, WR_Actions action)
      {
         sendMessage(msg+WR_Messages.linesplit+name+WR_Messages.valuesplit+params,action);
      }
      
      //the default sendMessage actually sends the data through the socket
      public synchronized void sendMessage(String msg, WR_Actions action)
      {
   
    	 
         int id;
         String wholemessage = msg;
    	 if(action != null)
         {
    		 id =ck.addCallBack(action);
    		 System.out.println("send message "+msg);
    		 wholemessage = msg+WR_Messages.linesplit+WR_Messages.messageid+WR_Messages.valuesplit+id;
    		
    		 out.println(wholemessage);
    
         }
    	 else
    	 {
    		 out.println(msg);
    	
    	 }
    	out.flush();
    
    	//log.outputLog("Sent: " +wholemessage);
      }
 
   //Receive class handles threaded receive from Torque
   public class receive implements Runnable
   {
      public receive()
      {
      }

     public void run()
     {
        String input; 
        Map<String,String> mp;
        while(socket.isConnected())
        {
 
        	try
        	{

        	   input = in.readLine();
        	   System.out.println("Received: " + input);
        	   
        	   //log.outputLog("Received: " + input);
        	   mp = parseLine(input);
        	   bot.receiveMessage(mp);
        	   
        	}
        	catch(IOException e)
        	{
        		System.out.println("Port closed");
        		System.exit(1);
        	}
        }
        
     }

     //Parses out the input and creates a Map with all the parameters
     protected Map<String,String> parseLine(String input)
     {
         String[] split;
         
         //MESSAGETYPE<linesplit>PARAMNAME<valuesplit>PARAMVALUE<linesplit>PARAMNAME<valuesplit>PARAMVALUE.....
         
        
         String message;
         Map<String,String>mp = new HashMap<String,String>();
         //System.out.println(input);
         split = input.split(WR_Messages.linesplit);
         message = split[0];

         mp.put(WR_Messages.type,message);
         //System.out.println("length "+split.length);
         
         for(int i = 1; i< split.length; i++)
         {
        	 String[] param = split[i].split(WR_Messages.valuesplit);
        	 if(param.length > 1)
        	 {
        		 //System.out.println(param[0]+ " "+param[1] + " " + i); 
        		 mp.put(param[0],param[1]);
        	 }
        	 else
        	 {
        		 mp.put(param[0], null);
        	 }
         }

         return mp;
     }
   }
}

