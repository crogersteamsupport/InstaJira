using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.ServiceLibrary
{
  public abstract class ServiceThreadPoolProcess: ServiceThread
  {
    protected int? _threadPosition = null;

    public ServiceThreadPoolProcess() { 
      IsLoop = false; 
    }

    public override string GetLogFileCategory()
    {
      return _threadPosition == null ? ServiceName : ServiceName + " - " + _threadPosition.ToString();
    }

    public void Start(int threadPosition)
    {
      _threadPosition = threadPosition;
      base.Start();
    }

    public abstract void ReleaseAllLocks();
  }
}
