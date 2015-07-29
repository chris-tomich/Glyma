<?xml version="1.0" encoding="utf-8"?>
<feature xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="50e77009-7f47-407c-a868-18ece74a8fb2" activateOnDefault="false" description="Installs a glyma minimal master page, a full glyma master page and a page layout containing Glyma web parts." featureId="50e77009-7f47-407c-a868-18ece74a8fb2" imageUrl="Glyma.SharePoint.Common\GlymaFeatureLogo.png" imageAltText="Feature Logo" scope="Site" solutionId="00000000-0000-0000-0000-000000000000" title="Glyma Branding" version="" deploymentPath="$SharePoint.Feature.FileNameWithoutExtension$" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel">
  <activationDependencies>
    <customFeatureActivationDependency minimumVersion="" featureTitle="Glyma Mapping Tool" featureDescription="Installs the Glyma Mapping Web Part, Map Content Type and import Custom Action." featureId="48f7dfa5-ca3d-4abb-a038-9cc985860b4e" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
    <customFeatureActivationDependency minimumVersion="" featureTitle="Glyma Related Content Panels Web Part" featureDescription="This feature deploys the Glyma Related Content Panels Web Part for use with the Glyma Mapping Web Part." featureId="c45ffcd2-6f57-4023-976c-d51877ab6177" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
    <customFeatureActivationDependency minimumVersion="" featureTitle="SharePoint Server Publishing Infrastructure" featureDescription="Provides centralized libraries, content types, master pages and page layouts and enables page scheduling and other publishing functionality for a site collection." featureId="f6924d36-2fa8-4f0b-b16d-06b7250180fa" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
  </activationDependencies>
  <projectItems>
    <projectItemReference itemId="51dad4d3-60ca-4c3f-b395-cbe09d5f5b3d" />
  </projectItems>
</feature>