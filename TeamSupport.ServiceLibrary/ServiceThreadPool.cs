using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  public class ServiceThreadPool<T>: ServiceThread
    where T: ServiceThreadPoolProcess
  {
    public ServiceThreadPool(string serviceName)
    {
      RunHandlesStop = true;
      _serviceName = serviceName;
    }

    private static object _staticLock = new object();
    private T[] _threads;
    private string _serviceName;

    private static bool _isPoolStopped = false;
    public static bool IsPoolStopped
    {
      get
      {
        lock (_staticLock)
        {
          return _isPoolStopped;
        }

      }
      set
      {
        lock (_staticLock)
        {

          _isPoolStopped = value;
        }

      }
    }


    public override void Stop()
    {
      IsPoolStopped = true;
      base.Stop();
    }

    public override void Start()
    {
      IsPoolStopped = false;
      Service service = Services.GetService(LoginUser, _serviceName);
      service.RunCount = 0;
      service.RunTimeAvg = 0;
      service.RunTimeMax = 0;
      service.ErrorCount = 0;
      service.LastError = "";
      service.LastResult = "";
      service.Collection.Save();
      int maxThreads = Settings.ReadInt("Max Worker Processes", 1);
      _threads = new T[maxThreads];
      GetProcessObject().ReleaseAllLocks();
      base.Start();
    }

    public T GetProcessObject()
    {
      return (T)Activator.CreateInstance(typeof(T));
    }
        
    public override void Run()
    {
      int nextEmptySpot = -1;

      for (int i = 0; i < _threads.Length; i++)
			{
        if (_threads[i] != null)
        {
          if (IsStopped)
          {
            _threads[i].Stop();
            _threads[i] = null; 
          }
          else if (_threads[i].IsStopped)
          {
            _threads[i] = null;
          }
        }
        else
        {
          nextEmptySpot = i;
        }
			}

      if (nextEmptySpot < 0) return;

      T process = GetProcessObject();
      process.Start(nextEmptySpot);
      _threads[nextEmptySpot] = process;
    }
  }
}


