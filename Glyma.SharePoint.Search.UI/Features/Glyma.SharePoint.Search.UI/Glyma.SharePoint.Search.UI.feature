<?xml version="1.0" encoding="utf-8"?>
<feature xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="9216f77d-a011-4731-a9be-13720fc21345" activateOnDefault="false" description="Installs the components required to display Glyma search results.  These components include files deployed to the Style Library (CSS, JavaScript and XSL files) and a custom control that injects a reference to the CSS file into pages using the SharePoint AdditionalPageHead delegate control." featureId="9216f77d-a011-4731-a9be-13720fc21345" imageUrl="$SharePoint.Project.FileNameWithoutExtension$\GlymaFeatureLogo.png" scope="Site" solutionId="00000000-0000-0000-0000-000000000000" title="Glyma Search Pre-requisites" version="" deploymentPath="$SharePoint.Feature.FileNameWithoutExtension$" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel">
  <projectItems>
    <projectItemReference itemId="88f44c75-71be-4131-8786-8c60199a552d" />
    <projectItemReference itemId="2f84f7cc-41a3-4e8e-a294-189400f8a65c" />
  </projectItems>
</feature>