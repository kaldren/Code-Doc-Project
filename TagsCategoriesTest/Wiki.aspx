<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Wiki.aspx.cs" Inherits="TagsCategoriesTest.Wiki" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~\App_Themes\Styles\Main.css" rel="stylesheet" />
    <script
  src="https://code.jquery.com/jquery-3.3.1.min.js"
  integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8="
  crossorigin="anonymous"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div class="container">
            <div class="row">
                <div class="col-xs-12 col-lg-3">
                    <h2>Menu</h2>
                    <asp:Repeater ID="rptWikiMenu" runat="server">
                        <ItemTemplate>
                            <%--<asp:Button Text='<%# Eval("Title") %>' runat="server" /><br /><br />--%>
                            <a href="/Wiki.aspx?category=<%#Eval("Title") %>" data-category-id=""><%# Eval("Title") %></a><br />
                            <%--                            <div class="panel">
                                <p><%# Eval("Title") %></p>
                            </div>--%>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="col-xs-12 col-lg-9">
                    <asp:Repeater ID="rptWikiContent" runat="server">
                        <ItemTemplate>
                            <p><%# Eval("Title") %></p>
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
            acc[i].addEventListener("click", function () {
                this.classList.toggle("active");
                var panel = this.nextElementSibling;
                if (panel.style.maxHeight) {
                    panel.style.maxHeight = null;
                } else {
                    panel.style.maxHeight = panel.scrollHeight + "px";
                }
            });
        }

    </script>
</body>
</html>
