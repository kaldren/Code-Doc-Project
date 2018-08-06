<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Wiki.aspx.cs" Inherits="TagsCategoriesTest.Wiki" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~\App_Themes\Styles\Main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-xs-12 col-lg-4">
                </div>

                <div class="col-xs-12 col-lg-8">
                    <asp:Repeater ID="rptWikiMenu" runat="server">
                        <ItemTemplate>
                            <button class="accordion" onclick="return false;"><%# Eval("Title") %></button>
                            <div class="panel">
                                <p><%# Eval("Content") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </form>
    <script>
var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
  acc[i].addEventListener("click", function() {
    this.classList.toggle("active");
    var panel = this.nextElementSibling;
    if (panel.style.maxHeight){
      panel.style.maxHeight = null;
    } else {
      panel.style.maxHeight = panel.scrollHeight + "px";
    } 
  });
}
    </script>
</body>
</html>
