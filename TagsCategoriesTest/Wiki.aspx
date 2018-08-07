<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Wiki.aspx.cs" Inherits="TagsCategoriesTest.Wiki" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~\App_Themes\Styles\Main.css" rel="stylesheet" />
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
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
                            <a href="/Wiki.aspx?category=<%#Eval("Title") %>" data-category-id=""><%# Eval("Title") %></a><br />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="col-xs-12 col-lg-9">
                    <h2>Data</h2>
                    <asp:Repeater ID="rptWikiContent" runat="server">
                        <ItemTemplate>
                            <p><a href="/Wiki.aspx?show=<%# Eval("WikiId") %>" target="_blank"><%# Eval("Title") %></a></p>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="row">
                <div class="col-xs-12 col-lg-9 col-lg-offset-3">
                    <asp:Repeater ID="rptWikiEntry" runat="server">
                        <ItemTemplate>
                            <h3><%# Eval("Title") %></h3>
                            <p><%# Eval("Content") %></p>
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
