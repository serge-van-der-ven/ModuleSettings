<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="XEC.DNN.ModuleSettings.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="#" class="dnnSectionExpanded"><%=this.LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingCssClass" runat="server" /> 
        <asp:TextBox ID="txtCssClass" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblSettingInitialize" runat="server" />
        <asp:CheckBox ID="chkSettingInitialize" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblSettingMaximumRetries" runat="server" />
        <asp:TextBox ID="txtSettingMaximumRetries" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblSettingStatus" runat="server" />
        <asp:DropDownList ID="ddlSettingStatus" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label ID="lblSettingUserName" runat="server" />
        <asp:TextBox ID="txtSettingUserName" runat="server" />
    </div>
</fieldset>
