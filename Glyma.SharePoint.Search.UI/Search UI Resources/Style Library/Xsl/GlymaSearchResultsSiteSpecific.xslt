<?xml version="1.0" encoding="UTF-8"?>
 <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
 
   <!-- Define the site collection server relative URL without the trailing "/" character. -->
   <xsl:variable name="siteCollectionServerRelativeUrl" select="''" />
 
   <!-- Define the URL's of the pages used to open Glyma search results. The URL's can be server relative or absolute.  
   Multiple entries can be defined for different Glyma repositories. --> 
   <xsl:variable name="glymaViewerPagesFragment">
       <ViewerPage repositoryName="Glyma"><xsl:value-of select="$siteCollectionServerRelativeUrl" />/Pages/Glyma.aspx</ViewerPage>
   </xsl:variable>
   
   <xsl:variable name="glymaViewerPages" select="msxsl:node-set($glymaViewerPagesFragment)" /> 
  
 </xsl:stylesheet> 