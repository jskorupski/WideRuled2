package javacode;


import abl.runtime.*;

public class WR_UndoSensor extends WR_Sensors
{
    public WR_UndoSensor() { super(); }

    private void sense()
    {
    	WR_UndoWME[] undos = proxy.getUndo();
	    //deleteAllOldWMEs();
	    addAllInteractions(undos);
    }

    // Adds new WMEs for the rotations of every seeable player.
    public void senseOneShot(Object[] args) { sense(); }

    private void addAllInteractions(WR_UndoWME[] undos)
    {
        for(int i = 0; i < undos.length; i++) {
	        BehavingEntity.getBehavingEntity().addWME(undos[i]);
        }
    }

    // Delete all old PlayerRotationWMEs
    private synchronized void deleteAllOldWMEs()
    {
	    Object[] wmeArray = BehavingEntity.getBehavingEntity().lookupWME("WR_UndoWME").toArray();
	    for(int i = 0; i < wmeArray.length; i++) {
	        BehavingEntity.getBehavingEntity().deleteWME((WR_UndoWME)wmeArray[i]);
        }
    }

    // Sense the rotation of all seeable players - called when a continuous condition referencing player rotations first
    // activates.
    public void initializeContinuous(Object[] args) { sense(); }

    // Sense the rotation of all seeable players - called when a continuous condition referencing player rotations first
    // activates.
    public void senseContinuous(Object[] args) { 
        sense(); 
	slowDownContinuousSensing();
    }
}
