using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BaseChartPage : System.Web.UI.Page
{
    protected int DateRangeMin = -7;
    protected DateTime? StartDate;
    protected DateTime? EndDate;
    protected bool VariableDateRange = false;

    public string DateRangeLabel;

	public void Page_Load()
    {
        HttpRequest Request = HttpContext.Current.Request;

            if (Request.QueryString["trailing"] != null)
            {
                DateRangeMin = -1 * int.Parse(Request.QueryString["trailing"]);
            }

            if (Request.QueryString["variable"] == "1")
            {
                VariableDateRange = true;
            }
    }

    protected override void OnLoadComplete(EventArgs e)
    {

        if (!StartDate.HasValue && !EndDate.HasValue) //if we don't have explicit values we're using a range
        {
            if (DateRangeMin == -365)
            {
                DateRangeLabel = "Past Year";
            }
            else
            {
                DateRangeLabel = "Past " + (-1 * DateRangeMin).ToString() + " Days";
            }
        }
        else
        {
            DateRangeLabel = StartDate.Value.ToShortDateString() + "-" + EndDate.Value.ToShortDateString();
        }

        base.OnLoadComplete(e);
    }
}