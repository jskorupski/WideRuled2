package javacode;

//package eis.abltorque;


//This is an example class needed to load and run the ABL bot 
//Uses Demo.Kork as an example
import abl.runtime.BehavingEntity;


public class WR_RunBot extends Thread {

	WR_GenAgent gen;
	
	static String hostname = "localhost";
	static int port = 5025;
	
	public WR_RunBot(String[] args)
	{
		if(args.length == 2)
		{
			hostname = args[0];
			port = Integer.parseInt(args[1]);
		}
		else 
		{
			hostname =  "localhost";
			port =  5025;
		}
		
	}
	//Each Bot is in its own thread
	public void run()
	{	
		try
		{
			//Pause thread for a few seconds, to give time
			//for Wide Ruled to start the server;
			 
			 gen = new WR_GenAgent();
			 WR_Bot proxy = new WR_Bot();
			 WR_GameClient client = new WR_GameClient();
			 proxy.init(client);
			 client.setAddress(hostname, port);
			 proxy.setName("WR_GenAgentProxy");
			 client.connect();
			 gen.startBehaving();
		}
		catch(Exception e) {
			
			System.out.println("Run Failed");
			System.out.println(e);
			e.printStackTrace();
			System.exit(1);
		}
		
	}
	
	
	/**
	 * @param args
	 */
	public static void main(String[] args) {
			Thread th = new Thread(new WR_RunBot(args));
			th.start();
	}

}
