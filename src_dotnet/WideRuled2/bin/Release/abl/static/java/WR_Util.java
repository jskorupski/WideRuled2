package javacode;



import java.io.*;

public class WR_Util {
	PrintStream file;
	boolean errorEnabled, log;
	public WR_Util(boolean log ,boolean err)
	{
	  	errorEnabled = err;
		this.log = log;
		if(log)
		{
			try
			{
				file = new PrintStream(new FileOutputStream("WideRuled2_StoryGen_log.txt"));
			}
			catch(Exception e) {}
			
			
		}
	}
	
	public void outputLog(String log)
	{
		if(file != null)
		{
			file.println(log);
			file.flush();
		
		}
	}
	public static void outputError(String error)
	{
		System.err.println(error);
	}
	
	

}
