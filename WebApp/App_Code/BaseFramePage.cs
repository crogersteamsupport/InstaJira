
public class BaseFramePage: System.Web.UI.Page
{
  private bool _cachePage = true;

  public bool CachePage
  {
    get { return _cachePage; }
    set { _cachePage = value; }
  }

  public BaseFramePage()
  {
  }
  
  
  
  
  
  protected override void SavePageStateToPersistenceMedium(object viewState)
  {

  }

  protected override object LoadPageStateFromPersistenceMedium()
  {
        return null;
  }
}



