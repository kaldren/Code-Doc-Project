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
                    <table class="table">
                        <asp:Repeater ID="rptWikiMenu" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <a href="/Wiki.aspx?category=<%#Eval("Title") %>" data-category-id=""><%# Eval("Title") %></a><br />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>

                <div class="col-xs-12 col-lg-9">
                    <h2>Data</h2>
                    <asp:PlaceHolder ID="phQueryResults" Visible="false" runat="server">
                        <asp:Label Text="" CssClass="query-results" ID="lblResults" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phWikiEntries" runat="server">
                        <asp:ListView ID="lvWikies" ItemPlaceholderID="itemPlaceholder" runat="server">
                           <LayoutTemplate> 
                                <div class="table-wrapper-scroll-y">
                                    <table class="table table-bordered table-striped">
                                        <div id="itemPlaceholder" runat="server">
                                        </div>
                                    </table>
                                </div>
                                <div id="pagination" class="text-center">
                                     <asp:DataPager ID="ListingDataPager" runat="server" PageSize="10" PagedControlID="lvWikies" QueryStringField="page">
                                        <Fields>
                                            <asp:NextPreviousPagerField 
                                            FirstPageText="First"
                                            LastPageText="Last"
                                            NextPageText="Next"
                                            PreviousPageText="Back" />
                                        </Fields>
                                    </asp:DataPager>
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <p><a href="/Wiki.aspx?show=<%# Eval("WikiId") %>" target="_blank"><%# Eval("Title") %></a></p>
                                    </td>
                                </tr>
                            </ItemTemplate>

                        </asp:ListView>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phWikiEntry" runat="server">
                        <div class="col-xs-12">
                            <asp:Repeater ID="rptWikiEntry" runat="server">
                                <ItemTemplate>
                                    <h3><%# Eval("Title") %></h3>
                                    <p><%# Eval("Content") %></p>
                                    <a href="/Edit.aspx?id=<%# Eval("WikiId") %>" class="btn btn-md btn-success" target="_blank">Edit</a>
                                    <a href="javascript:void(0)" class="btn btn-md btn-danger" data-toggle="modal" data-target="#myModal"">Delete</a>

                                    <!-- Modal -->
                                    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
                                      <div class="modal-dialog" role="document">
                                        <div class="modal-content">
                                          <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <h4 class="modal-title" id="myModalLabel">Deleting Wiki</h4>
                                          </div>
                                          <div class="modal-body" id="modal-response">
                                            Are you sure you want to delete this wiki?
                                          </div>
                                          <div class="modal-footer">
                                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                            <button type="button" class="js-delete-wiki btn btn-danger" data-wiki-id="<%# Eval("WikiId") %>">Delete</button>
                                          </div>
                                        </div>
                                      </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>
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


        // AJAX Call to delete wiki
        $(".js-delete-wiki").click(function () {
            $.ajax({
                type: 'POST',
                url: 'Wiki.aspx/DeleteWiki',
                data: "{'id':" + JSON.stringify($(".js-delete-wiki").data("wiki-id")) + "}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function () {
                    // Notice that msg.d is used to retrieve the result object
                    $("#modal-response").html("<h2>Done.</h2>");

                    setTimeout(redirectToWiki, 2000);
                }
            });
        })


        function redirectToWiki() {
            window.location.replace("/Wiki.aspx")
        }

    </script>
    <!-- Latest compiled and minified JavaScript -->
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
</body>
</html>
