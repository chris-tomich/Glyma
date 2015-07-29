<%@ Page language="C#"   Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Register tagprefix="MappingToolWebPart" namespace="SevenSigma.MappingTool.MappingToolWebPart" assembly="SevenSigma.MappingTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=2638f3aecc4ae62f" %>
<%@ Register tagprefix="RelatedContentPanelWebPart" namespace="VideoPlayerSPDeploy.RelatedContentPanelWebPart" assembly="VideoPlayerSPDeploy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c14f786c7d278a0d" %>
<asp:Content ContentPlaceholderID="PlaceHolderAdditionalPageHead" runat="server">
	<SharePointWebControls:CssRegistration ID="GlymaPageLayoutRCPCssRegistration" name="<% $SPUrl:~sitecollection/Style Library/Glyma/Common/glyma-minimal-relatedcontentpanels.css %>" runat="server" 
        after="<% $SPUrl:~sitecollection/$StyleLibraryDirectoryName$/Glyma/RelatedContentPanels/RelatedContentPanel.css %>" />
    <SharePointWebControls:CssRegistration ID="GlymaPageLayoutMTCssRegistration" name="<% $SPUrl:~sitecollection/Style Library/Glyma/Common/glyma-minimal-mappingtool.css %>" runat="server" 
        after="<% $SPUrl:~sitecollection/$StyleLibraryDirectoryName$/Glyma/MappingTool/GlymaMappingWebPart.css %>" />
    
    <script type="text/javascript">
        function RelatedContentPanelResizeCallback(width) {
            $("#left-webpart").css("right", (width + 5) + "px");
        }
        
        function RegisterResizeCallback() {
            //Register callback with the RelatedContentPanel webpart code to call whenever a resize event happens
            AddResizeCallback(RelatedContentPanelResizeCallback);
        }
        
        if (_spBodyOnLoadFunctionNames) {
           _spBodyOnLoadFunctionNames.push('RegisterResizeCallback');
        }
     </script>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
<WebPartPages:spproxywebpartmanager runat="server" id="spproxywebpartmanager"></WebPartPages:spproxywebpartmanager>
	<div id="glyma-body">
        <div id="left-webpart">
			<MappingToolWebPart:MappingToolWebPart runat="server" DomainUid="" NodeUid="" ChromeType="None" Description="The Glyma IBIS Mapping Web Part." ImportErrorMessage="$Resources:core,ImportErrorMessage;" Title="Glyma Mapping Web Part" __MarkupType="vsattributemarkup" __WebPartId="{7ee3ac55-d179-4418-a5a8-46b5568ddd2b}" WebPart="true" __designer:IsClosed="false" id="g_7ee3ac55_d179_4418_a5a8_46b5568ddd2b"></MappingToolWebPart:MappingToolWebPart>
		</div>
        <div id="right-webpart">
			<RelatedContentPanelWebPart:RelatedContentPanelWebPart runat="server" ChromeType="None" Description="The Glyma related content side panels." ImportErrorMessage="$Resources:core,ImportErrorMessage;" Title="Glyma Related Content Panels" __MarkupType="vsattributemarkup" __WebPartId="{0566f8ff-550b-4229-a018-ec5a3cdd4e7e}" WebPart="true" __designer:IsClosed="false" id="g_0566f8ff_550b_4229_a018_ec5a3cdd4e7e"></RelatedContentPanelWebPart:RelatedContentPanelWebPart>
		</div>
    </div>
</asp:Content>