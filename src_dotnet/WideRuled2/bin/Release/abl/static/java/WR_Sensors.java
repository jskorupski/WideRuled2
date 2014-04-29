package javacode;


import abl.runtime.Sensor;
import abl.runtime.RuntimeError;

/*
 *	The Abstract Superclass for all ABL sensors in Unreal Tournament.
 */
public abstract class WR_Sensors extends Sensor {
    protected WR_Bot proxy;

    public WR_Sensors() { proxy = WR_Bot.getBot(); }

    // Necessary because ABL currently only works in async decision cycle mode and the async cycle is throttled only
    // by sensing (no external throttle).
    protected void slowDownContinuousSensing() {
       try {
          Thread.sleep(10); // Slow down sense continuous since we're doing asynchronous sensing
       } catch (InterruptedException e) { throw new RuntimeError("Sleep interrupted during sensing"); }
    }


}

