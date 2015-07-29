<?xml version="1.0" encoding="utf-8"?>
<feature xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="c45ffcd2-6f57-4023-976c-d51877ab6177" activateOnDefault="false" description="This feature deploys the Glyma Related Content Panels Web Part for use with the Glyma Mapping Web Part." featureId="c45ffcd2-6f57-4023-976c-d51877ab6177" imageUrl="$SharePoint.Project.FileNameWithoutExtension$\GlymaFeatureLogo.png" imageAltText="Feature Logo" scope="Site" solutionId="00000000-0000-0000-0000-000000000000" title="Glyma Related Content Panels Web Part" version="" deploymentPath="$SharePoint.Feature.FileNameWithoutExtension$" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel">
  <activationDependencies>
    <customFeatureActivationDependency minimumVersion="" featureTitle="Glyma Pre-requisites" featureDescription="Installs the Glyma pre-requisite files required for any Glyma deployment" featureId="39c258eb-de61-4e94-9f21-08e7c1fd96f9" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
    <customFeatureActivationDependency minimumVersion="" featureTitle="Glyma Mapping Tool" featureDescription="Installs the Glyma Mapping Web Part, Map Content Type and import Custom Action." featureId="48f7dfa5-ca3d-4abb-a038-9cc985860b4e" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
  </activationDependencies>
  <projectItems>
    <projectItemReference itemId="376c5da3-51b1-4f17-87fd-212efdb3c9df" />
    <projectItemReference itemId="673581fe-3882-4911-a47b-d8549e5b45e0" />
  </projectItems>
</feature>