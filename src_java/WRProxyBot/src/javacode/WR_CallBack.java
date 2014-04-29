package javacode;

import java.util.Hashtable;



public class WR_CallBack {
	   private Hashtable<Integer,WR_Actions> callbackTable = new Hashtable<Integer,WR_Actions>();
	   private int messageID =0;
	   
	   
	   public int addCallBack(WR_Actions action)
	   {
		   Integer message = Integer.valueOf(messageID);
		   callbackTable.put(message, action);
	       messageID++;
	       return message.intValue();
	   }
	   
	   public WR_Actions getCallBack(Integer id)
	   {
		   WR_Actions action = callbackTable.get(id);
		   return action;
	   }
	   public void removeCallback(Integer id)
	   {
		   callbackTable.remove(id);
	   }
	   
	   public void acceptandRemove(Integer id,Boolean status)
	   {
		   WR_Actions action = callbackTable.get(id);
		   action.completionCallback(status);
		   callbackTable.remove(id);
	   }
	   
}
