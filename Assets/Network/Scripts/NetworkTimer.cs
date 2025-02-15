﻿namespace Network
{
  public class NetworkTimer // basic timer class to keep track of server ticks
  {
    float timer;
    public float MinTimeBetweenTicks { get; }
    public int   CurrentTick         { get; private set; }
 
    public NetworkTimer(float serverTickRate)
    {
      MinTimeBetweenTicks = 1f / serverTickRate;
    }
  
    public void Update(float deltaTime)
    {
      timer += deltaTime;
    }

    public bool ShouldTick()
    {
      if (timer >= MinTimeBetweenTicks)
      {
        timer -= MinTimeBetweenTicks;
        CurrentTick++;
        return true;
      }
      return false;
    }
  }
  
}
