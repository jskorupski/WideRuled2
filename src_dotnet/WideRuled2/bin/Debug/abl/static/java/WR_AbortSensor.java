package javacode;


import abl.runtime.*;

public class WR_AbortSensor extends WR_Sensors
{
    public WR_AbortSensor() { super(); }

    private void sense()
    {
    	WR_AbortWME[] aborts = proxy.getAbort();
	    //deleteAllOldWMEs();
	    addAllInteractions(aborts);
    }

    // Adds new WMEs for the rotations of every seeable player.
    public void senseOneShot(Object[] args) { sense(); }

    private void addAllInteractions(WR_AbortWME[] aborts)
    {
        for(int i = 0; i < aborts.length; i++) {
	        BehavingEntity.getBehavingEntity().addWME(aborts[i]);
        }
    }

    // Delete all old PlayerRotationWMEs
    private synchronized void deleteAllOldWMEs()
    {
	    Object[] wmeArray = BehavingEntity.getBehavingEntity().lookupWME("WR_AbortWME").toArray();
	    for(int i = 0; i < wmeArray.length; i++) {
	        BehavingEntity.getBehavingEntity().deleteWME((WR_InteractionWME)wmeArray[i]);
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
