package javacode;

import java.util.Hashtable;
import abl.runtime.PrimitiveAction;

public class WR_AbortAction extends WR_Actions {

    public void execute(Object[] args) {
    // Called every decision cycle. The default implementation does
    // nothing. Can be overridden on concrete action classes if the
    // specific sensory-motor system needs some processing to be
    // performed every decision cycle.

    
       
        System.out.println("aborting:" + Thread.currentThread());
        
        WR_Bot.getBot().sendAbort(this);
        this.completionStatus = PrimitiveAction.SUCCESS;
     }

     public void abort() { /* System.out.println("ABORT - sendGlobalChatMessage"); */ } // a SendGlobalChatMessage cannot be aborted
}
