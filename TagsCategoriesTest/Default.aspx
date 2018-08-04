<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TagsCategoriesTest.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~\App_Themes\Styles\Main.css" rel="stylesheet" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="row form-group">
                <asp:Label Text="Title" AssociatedControlID="txtTitle" runat="server" />
                <asp:TextBox ID="txtTitle" runat="server" /><br /><br />
                <asp:Label Text="Content" AssociatedControlID="txtContent" runat="server" />
                <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine"/>
                <div class="col-xs-12 col-md-6">
                    <p>Tags:</p>
                    <asp:ListBox ID="lbTags" runat="server" SelectionMode="Multiple">
                        <asp:ListItem Value="Cats" Text="Cats" />
                        <asp:ListItem Value="Dogs" Text="Dogs" />
                    </asp:ListBox>
                    <div>
                        <asp:TextBox ID="txtTags" runat="server" />
                    </div>
                </div>
                <div class="col-xs-12 col-md-6">
                    <p>Categories:</p>
                    <asp:ListBox ID="lbCategories" runat="server" SelectionMode="Multiple">
                        <asp:ListItem Value="Animals" Text="Animals" />
                        <asp:ListItem Value="Aliens" Text="Aliens" />
                    </asp:ListBox>
                    <div>
                        <asp:TextBox ID="txtCategories" runat="server" />
                    </div>
                </div>
            </div>
            <div class="panel-footer">
                <asp:Button Text="Send" ID="btnSend" runat="server" OnClick="btnSend_Click" />
            </div>
        </div>


        <asp:ScriptManager runat="server" />

        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:Button ID="btnPreview" Text="Preview" OnClick="btnPreview_Click" runat="server" />
                <asp:Literal ID="code" Text="" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
