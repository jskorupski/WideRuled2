package javacode;
/*
 * Bot is the proxy between ABL and and the GameClient
 * It contains code to send action messages and receive messages
 */


import java.util.*;





public class WR_Bot
{
    private boolean init = false;
    private String name;
    
    private WR_GameClient client;
    private String bump = null;
    private String globalchat;
   
    
    
    protected Hashtable<String, Integer> interactionSensorTable = new Hashtable<String,Integer>();
    protected boolean undo;
    protected boolean abortGeneration;
    
    
    
	static private ThreadLocal me = new ThreadLocal();
	
	
	
	
	//Bot created
    public WR_Bot()
    {
    	me.set(this);
    	System.out.println("Bot Initialized");
    	bump = null;
    	System.out.println(me.get().toString());
    	undo = false;
    	abortGeneration = false;
    }
    
    public WR_Bot(WR_GameClient gc)
    {
    	client = gc;
    	gc.setBot(this);
    	me.set(this);
    	abortGeneration = false;
    	undo = false;
    	
    }
    
    //returns the the current bot in this thread
    public static WR_Bot getBot()
    {
    	System.out.println(me.get().toString());
    	return (WR_Bot) me.get();
    }
    
    //sets the GameClient here and the bot client in GameClient code
    public void init(WR_GameClient cl)
    {
    	client = cl;
    	cl.setBot(this);
    }
    
    //sets the name of the bot
    public void setName(String name)
    {
    	this.name = name;
    }
    
    
    //Decides what to do with each message
    public void receiveMessage(Map<String,String> mp)
    {
      String type = (String) mp.get(WR_Messages.type);

    //  if(type.equalsIgnoreCase(WR_Messages.info) && init == false)
     // {
    //	  init = true;
    //	  sendInit();
     // }
      
      //if(init)
      //{
    	//  //if the message is a callback
    	//  if(type.equals(WR_Messages.callback))
    	//  {
         //       callBack(mp);
    	 // }
    	 /// else
    	  //{
    		  wrMessages(mp);
    	 // }
      //}
     // else
     //{
    	//  Util.outputError("Init not received");
     // }
   }

    //runs the callback code to finish the handling of an action
    private void callBack(Map<String,String> mp)
    {
    	System.out.println(mp.toString());
    	System.out.flush();
    	//String action = mp.get(TorqueMessages.action);
    	String success = mp.get(WR_Messages.success);
    	String messageid = mp.get(WR_Messages.messageid);
    	Boolean status = Boolean.valueOf(success);
    	Integer id = Integer.valueOf(messageid);
    	client.ck.acceptandRemove(id, status);
    }
    
    //Creates a new table with locations of other users
    private void wrMessages(Map<String,String> mp)
    {
        String type = mp.get(WR_Messages.type);

        if (type.equals(WR_Messages.INTERACT))
        {
      	     // if (interactionSensorTable != null) 
      	    	//interactionSensorTable.clear();
      	    updateInteractionTable( mp );
        }
        else if (type.equals(WR_Messages.ABORT)) 
        {
        	abortGeneration = true;
        }
        else if (type.equals(WR_Messages.UNDO))
        {
            undo = true;
        }

    }
  
    //update received regularly from messages from torque
    //synchronized
   
    
    private void updateInteractionTable(Map<String,String> mp)
    {
    	
    	String id = mp.get(WR_Messages.id);
    	int interactionId = Integer.parseInt(id);
    	
    	
    	//double[] loc = parseTorqueVector(location);
    	//double[] rot = parseTorqueRotation(rotation);
    	//System.out.println("Update Self "+location+" " + name+ " " +id+" "+loc[0]+ " "+rot[0]);
    	interactionSensorTable.put("Interaction", new Integer(interactionId));	
    }
  
    
   
    
    public synchronized WR_InteractionWME[] getAllInteractions()
    {
    	Vector<WR_InteractionWME>tempvector = new Vector<WR_InteractionWME>();
    	
    	Set st = interactionSensorTable.entrySet();
    	Iterator i = st.iterator();

         while(i.hasNext())
         {         
            Map.Entry<String,Integer> ent = (Map.Entry<String,Integer>) i.next();
            Integer p = ent.getValue();
            tempvector.add(new WR_InteractionWME(p.intValue())); 
         }  
         
         interactionSensorTable.clear();
         return (WR_InteractionWME[]) tempvector.toArray(new WR_InteractionWME[0]);
    }
    
    public synchronized WR_AbortWME[] getAbort()
    {
    	Vector<WR_AbortWME>tempvector = new Vector<WR_AbortWME>();
    	
    	if(abortGeneration)
    	{
    		tempvector.add(new WR_AbortWME(abortGeneration));
    	}
          
        return (WR_AbortWME[]) tempvector.toArray(new WR_AbortWME[0]);
    }
    
    public synchronized WR_UndoWME[] getUndo()
    {
		Vector<WR_UndoWME>tempvector = new Vector<WR_UndoWME>();
    	
    	if(undo)
    	{
    		tempvector.add(new WR_UndoWME(undo));
    	}
        undo = false;
        
        return (WR_UndoWME[]) tempvector.toArray(new WR_UndoWME[0]);
    }
    
  
    
    
    //List of actions to send to Torque
    public synchronized void sendInit()
    {
    	Map<String,String> mp = new HashMap<String,String>();
    	mp.put(WR_Messages.name,name);
    	client.sendMessage(WR_Messages.init,mp,null);
    }
    

    public synchronized void sendAbort(WR_Actions action)
    {
    	client.sendMessage(WR_Messages.ABORT, action);
    	//Shut down program after sending final abort message
    	try {
    	Thread.sleep(300);
    	}catch (Exception e){}
    	client.abort();
    }

    public synchronized void sendPrint(String msg, WR_PrintAction send)
    {
    	
    	client.sendMessage(WR_Messages.PRINT, WR_Messages.text, msg,send);

    }
    
    public synchronized void sendPrintLine(String msg, WR_PrintAction send)
    {
    
    	client.sendMessage(WR_Messages.PRINTLINE, WR_Messages.text, msg,send);

    }
    
   
}