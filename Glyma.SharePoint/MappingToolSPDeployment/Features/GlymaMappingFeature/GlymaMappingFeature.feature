<?xml version="1.0" encoding="utf-8"?>
<feature xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="48f7dfa5-ca3d-4abb-a038-9cc985860b4e" activateOnDefault="false" description="Installs the Glyma Mapping Web Part, Map Content Type and import Custom Action." featureId="48f7dfa5-ca3d-4abb-a038-9cc985860b4e" imageUrl="$SharePoint.Project.FileNameWithoutExtension$\GlymaFeatureLogo.png" imageAltText="Feature Logo" scope="Site" solutionId="00000000-0000-0000-0000-000000000000" title="Glyma Mapping Tool" version="" deploymentPath="$SharePoint.Feature.FileNameWithoutExtension$" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel">
  <activationDependencies>
    <customFeatureActivationDependency minimumVersion="" featureTitle="Glyma Pre-requisites" featureDescription="Installs the Glyma pre-requisite files required for any Glyma deployment" featureId="39c258eb-de61-4e94-9f21-08e7c1fd96f9" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
  </activationDependencies>
  <projectItems>
    <projectItemReference itemId="64d7c543-1eb9-460e-9f0d-64f5dec882ee" />
    <projectItemReference itemId="c151ef72-d98d-439e-8e14-91026d0ccedc" />
    <projectItemReference itemId="85ec31c5-1917-4413-8514-bee2d2f5ac66" />
  </projectItems>
</feature>