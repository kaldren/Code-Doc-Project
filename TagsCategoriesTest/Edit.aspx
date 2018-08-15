﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="TagsCategoriesTest.Edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" />
        <div>
            <p>Title:</p>
            <asp:TextBox ID="txtTitle" runat="server" /><br />
            <br />

            <p>Content:</p>
            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" />

            <asp:HiddenField ID="wikiId" runat="server" />
            <div>
                <asp:Button ID="btnSave" Text="Save" runat="server" OnClick="btnSave_Click" />
            </div>
            <div class="col-xs-12 col-md-6">
                <p>Tags:</p>
                <asp:ListBox ID="lbTags" runat="server" SelectionMode="Multiple"></asp:ListBox>
                <div>
                    <asp:TextBox ID="txtTags" runat="server" />
                </div>
            </div>
            <div class="col-xs-12 col-md-6">
                <p>Categories:</p>
                <asp:ListBox ID="lbCategories" runat="server" SelectionMode="Multiple"></asp:ListBox>
                <div>
                    <asp:TextBox ID="txtCategories" runat="server" />
                </div>
            </div>
        </div>

        <div>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnPreview" Text="Preview" runat="server" OnClick="btnPreview_Click" />
                    <asp:Literal ID="code" Text="" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
