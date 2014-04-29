package javacode;

import wm.WME;
import java.util.*;

import abl.runtime.PrimitiveAction;


public class WR_UndoWME extends WME {
    private boolean undo;
    
    public WR_UndoWME(boolean undo)
    {
    	this.undo = undo;
    }

    public WR_UndoWME() {}

 
   
    public synchronized boolean getUndo() { return undo; }


    
    public synchronized void setUndo(boolean undo) { this.undo = undo; }
}
    