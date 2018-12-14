<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QueryStatus.aspx.cs" Inherits="ServiceStatus"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" />
    <script src="/frontend/library/jquery-latest.min.js"></script>
    <script src="/frontend/library/bootstrap3.min.js"></script>

    <title></title>

    <script type="text/javascript" language="javascript">    
        $(document).ready(function () {
            $('.container').on('click', '.clearcache', function (e) {
                e.stopPropagation();
                e.preventDefault();
                var plan = $(this).closest('tr').data('plan');
                if (confirm('Are you sure you would like to clear the execution plan?'))
                {
                    PageMethods.ClearCache(plan, function (result) {
                        alert(result);
                    });
                }
            });
            $('.refresh').click(function (e) {
                e.stopPropagation();
                e.preventDefault();
                //$('.queries').html('');
                var btn = $(this).button('loading')
                PageMethods.GetRows(function (result) {
                    $('.queries').html(result);
                    btn.button('reset');
                });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
          <asp:ScriptManager ID="ScriptManagerMain"
            runat="server"
            EnablePageMethods="true" 
            ScriptMode="Release" 
            LoadScriptsBeforeUI="true">
    </asp:ScriptManager>
        <div class="container">
            <div class="page-header">
                <h2>Query Statuses</h2><button type="button" class="btn btn-primary refresh" data-loading-text="Loading...">Refresh</button>
            </div>
            <div>
                <table class="table table-hover table-striped queries">
                    <asp:Literal ID="litRows" runat="server"></asp:Literal>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
