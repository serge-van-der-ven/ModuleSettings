<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="XEC.DNN.ModuleSettingsModule.View" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<asp:Label runat="server" ID="lblModuleInitializedMessage"/>

<asp:Panel runat="server" ID="pnlSettings" CssClass="dnnForm">
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingCssClass" runat="server" /> 
        <asp:Label runat="server" ID="lblSettingCssClassValue"/>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingInitialize" runat="server" /> 
        <asp:CheckBox runat="server" ID="chkSettingInitialize" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingMaximumRetries" runat="server" /> 
        <asp:Label runat="server" ID="lblSettingMaximumRetriesValue" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingStatus" runat="server" /> 
        <asp:Label runat="server" ID="lblSettingStatusValue" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblSettingUserName" runat="server" /> 
        <asp:Label runat="server" ID="lblSettingUserNameValue" />
    </div>
</asp:Panel>