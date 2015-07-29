using System;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides details for a SharePoint column.
   /// </summary>
   public class ColumnDetails
   {
      private const string _textColumnXml = "<Field ID=\"{0}\" Type=\"Text\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _multiLineColumnXml = "<Field ID=\"{0}\" Type=\"Note\"  Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" UnlimitedLengthInDocumentLibrary=\"TRUE\" NumLines=\"6\" RichText=\"FALSE\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3/fields\"/>";
      private const string _htmlXmlTemplate = "<Field ID=\"{0}\" Type=\"HTML\" RichText=\"TRUE\" RichTextMode=\"FullHtml\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _dateColumnXml = "<Field ID=\"{0}\" Type=\"DateTime\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" Format=\"DateOnly\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3/fields\" />";
      private const string _hyperlinkColumnXml = "<Field ID=\"{0}\" Type=\"URL\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _personColumnXml = "<Field ID=\"{0}\" Type=\"User\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" List=\"UserInfo\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _booleanXmlTemplate = "<Field ID=\"{0}\" Type=\"Boolean\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _choiceColumnXml = "<Field ID=\"{0}\" Type=\"Choice\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\">{5}</Field>";
      private const string _lookupColumnXml = "<Field ID=\"{0}\" Type=\"Lookup\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _taxonomyColumnXml = "<Field ID=\"{0}\" Type=\"TaxonomyFieldType\" ShowField=\"Term1033\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _numberColumnXml = "<Field ID=\"{0}\" Type=\"Number\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";
      private const string _intColumnXml = "<Field ID=\"{0}\" Type=\"Integer\" Name=\"{1}\" StaticName=\"{1}\" DisplayName=\"{2}\" Description=\"{3}\" Group=\"{4}\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";

      public Guid Id { get; set; }
      public string InternalName { get; set; }
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public string ColumnChoices { get; set; }
      public SPFieldType Type { get; set; }
      public string TypeAsString { get; set; }
      public string Group { get; set; }



      public ColumnDetails(Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnChoices, string columnTypeAsString, string columnGroup)
      {
         Id = columnId;
         InternalName = columnInternalName;
         DisplayName = columnDisplayName;
         Description = columnDescription;
         ColumnChoices = columnChoices;
         Type = columnType;
         TypeAsString = columnTypeAsString;
         Group = columnGroup;
      }


      public ColumnDetails(Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnGroup)
         : this(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, string.Empty, string.Empty, columnGroup)
      {     
      }


      public ColumnDetails(ColumnDetails columnDetail)
         : this(columnDetail.Id, columnDetail.InternalName, columnDetail.DisplayName, columnDetail.Description, columnDetail.Type, columnDetail.ColumnChoices, columnDetail.TypeAsString, columnDetail.Group)
      {
      }


      public ColumnDetails(ReadOnlyColumnDetails columnDetail)
         : this(columnDetail.Id, columnDetail.InternalName, columnDetail.DisplayName, columnDetail.Description, columnDetail.Type, columnDetail.ColumnChoices, columnDetail.TypeAsString, columnDetail.Group)
      {
      }

      /// <summary>
      /// Gets the CAML required to create the column.
      /// </summary>
      /// <returns></returns>
      public string GetXml()
      {
          if (Type == SPFieldType.Choice)
          {
              return string.Format(GetXmlTemplate(Type, TypeAsString), Id.ToString("B"), InternalName, DisplayName, Description, Group, ColumnChoices);
          }
          else
          {
              return string.Format(GetXmlTemplate(Type, TypeAsString), Id.ToString("B"), InternalName, DisplayName, Description, Group);
          }
      }


      /// <summary>
      /// Gets the CAML template required to create a column.
      /// </summary>
      /// <param name="columnType">An SPFieldType object that specifies the type of column.</param>
      /// <param name="columnTypeAsString">A string specifying the type of column for special column types e.g. Publishing HTML column.</param>
      /// <returns></returns>
      public string GetXmlTemplate(SPFieldType columnType, string columnTypeAsString)
      {
         string xmlTemplate = string.Empty;
         switch (columnType)
         {
            case SPFieldType.Text:
               xmlTemplate = _textColumnXml;
               break;
            case SPFieldType.Number:
               xmlTemplate = _numberColumnXml;
               break;
            case SPFieldType.Integer:
               xmlTemplate = _intColumnXml;
               break;
            case SPFieldType.Note:
               xmlTemplate = _multiLineColumnXml;
               break;
            case SPFieldType.DateTime:
               xmlTemplate = _dateColumnXml;
               break;
            case SPFieldType.URL:
               xmlTemplate = _hyperlinkColumnXml;
               break;
            case SPFieldType.User:
               xmlTemplate = _personColumnXml;
               break;
            case SPFieldType.Boolean:
               xmlTemplate = _booleanXmlTemplate;
               break;
            case SPFieldType.Choice:
               xmlTemplate = _choiceColumnXml; 
               break;
            case SPFieldType.Lookup:
               xmlTemplate = _lookupColumnXml; 
               break;
            case SPFieldType.Invalid:
               switch (columnTypeAsString.ToLower())
               {
                  case "html":
                     xmlTemplate = _htmlXmlTemplate;
                     break;
                  case "taxonomyfieldtype":
                     xmlTemplate = _taxonomyColumnXml;
                     break;
                  default:
                     throw new ApplicationException("The specified type is not supported.");
               }
               break;
            default:
               throw new ApplicationException("The specified type is not supported.");
         }
         return xmlTemplate;
      }
   }
}
