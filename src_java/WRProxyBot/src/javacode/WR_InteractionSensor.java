package javacode;


import abl.runtime.*;

public class WR_InteractionSensor extends WR_Sensors
{
    public WR_InteractionSensor() { super(); }

    private void sense()
    {
    	WR_InteractionWME[] interactions = proxy.getAllInteractions();
	    //deleteAllOldWMEs();
	    addAllInteractions(interactions);
    }

    // Adds new WMEs for the rotations of every seeable player.
    public void senseOneShot(Object[] args) { sense(); }

    private void addAllInteractions(WR_InteractionWME[] interactions)
    {
        for(int i = 0; i < interactions.length; i++) {
	        BehavingEntity.getBehavingEntity().addWME(interactions[i]);
        }
    }

    // Delete all old PlayerRotationWMEs
    private synchronized void deleteAllOldWMEs()
    {
	    Object[] wmeArray = BehavingEntity.getBehavingEntity().lookupWME("WR_InteractionWME").toArray();
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
