using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Glyma.Powershell.Update.v1_5_0_r4
{
    public class OldHtmlDescriptionUpdater
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public string Description { get; private set; }

        public OldHtmlDescriptionUpdater(string description, PSCmdlet callingCmdlet)
        {
            try
            {
                var settings = new XmlReaderSettings();
                settings.ProhibitDtd = false;
                callingCmdlet.WriteWarning("Reading HTML descrption");
                using (XmlReader reader = XmlReader.Create(new StringReader(XmlDeclaration + description), settings))
                {
                    reader.ReadToFollowing("div");
                    var id = reader.GetAttribute("id");
                    if (id == "GlymaNodeDescriptionDiv")
                    {
                        callingCmdlet.WriteWarning("GlymaNodeDescriptionDiv div tag found");

                        var widthString = reader.GetAttribute("width");
                        int width;
                        if (widthString != null && int.TryParse(widthString, out width) && width > 0)
                        {
                            Width = width;
                        }

                        var heightString = reader.GetAttribute("height");
                        int height;
                        if (heightString != null && int.TryParse(heightString, out height) && height > 0)
                        {
                            Height = height;
                        }

                        Description = reader.ReadInnerXml();
                    }
                    else
                    {
                        Description = description;
                    }
                }
            }
            catch
            {
                Description = description;
            }
        }

        public const string XmlDeclaration = "<!DOCTYPE HTML [" +
                                     "<!ENTITY nbsp    \"&#160;\">" +
                                     "<!ENTITY quot    \"&#34;\">" +
                                     "<!ENTITY amp     \"&#38;#38;\">" +
                                     "<!ENTITY lt      \"&#38;#60;\">" +
                                     "<!ENTITY gt      \"&#62;\">" +
                                     "<!ENTITY apos	 \"&#39;\">" +
                                     "<!ENTITY OElig   \"&#338;\">" +
                                     "<!ENTITY oelig   \"&#339;\">" +
                                     "<!ENTITY Scaron  \"&#352;\">" +
                                     "<!ENTITY scaron  \"&#353;\">" +
                                     "<!ENTITY Yuml    \"&#376;\">" +
                                     "<!ENTITY circ    \"&#710;\">" +
                                     "<!ENTITY tilde   \"&#732;\">" +
                                     "<!ENTITY ensp    \"&#8194;\">" +
                                     "<!ENTITY emsp    \"&#8195;\">" +
                                     "<!ENTITY thinsp  \"&#8201;\">" +
                                     "<!ENTITY zwnj    \"&#8204;\">" +
                                     "<!ENTITY zwj     \"&#8205;\">" +
                                     "<!ENTITY lrm     \"&#8206;\">" +
                                     "<!ENTITY rlm     \"&#8207;\">" +
                                     "<!ENTITY ndash   \"&#8211;\">" +
                                     "<!ENTITY mdash   \"&#8212;\">" +
                                     "<!ENTITY lsquo   \"&#8216;\">" +
                                     "<!ENTITY rsquo   \"&#8217;\">" +
                                     "<!ENTITY sbquo   \"&#8218;\">" +
                                     "<!ENTITY ldquo   \"&#8220;\">" +
                                     "<!ENTITY rdquo   \"&#8221;\">" +
                                     "<!ENTITY bdquo   \"&#8222;\">" +
                                     "<!ENTITY dagger  \"&#8224;\">" +
                                     "<!ENTITY Dagger  \"&#8225;\">" +
                                     "<!ENTITY permil  \"&#8240;\">" +
                                     "<!ENTITY lsaquo  \"&#8249;\">" +
                                     "<!ENTITY rsaquo  \"&#8250;\">" +
                                     "<!ENTITY euro   \"&#8364;\">" +
                                     "<!ENTITY fnof     \"&#402;\">" +
                                     "<!ENTITY Alpha    \"&#913;\">" +
                                     "<!ENTITY Beta     \"&#914;\">" +
                                     "<!ENTITY Gamma    \"&#915;\">" +
                                     "<!ENTITY Delta    \"&#916;\">" +
                                     "<!ENTITY Epsilon  \"&#917;\">" +
                                     "<!ENTITY Zeta     \"&#918;\">" +
                                     "<!ENTITY Eta      \"&#919;\">" +
                                     "<!ENTITY Theta    \"&#920;\">" +
                                     "<!ENTITY Iota     \"&#921;\">" +
                                     "<!ENTITY Kappa    \"&#922;\">" +
                                     "<!ENTITY Lambda   \"&#923;\">" +
                                     "<!ENTITY Mu       \"&#924;\">" +
                                     "<!ENTITY Nu       \"&#925;\">" +
                                     "<!ENTITY Xi       \"&#926;\">" +
                                     "<!ENTITY Omicron  \"&#927;\">" +
                                     "<!ENTITY Pi       \"&#928;\">" +
                                     "<!ENTITY Rho      \"&#929;\">" +
                                     "<!ENTITY Sigma    \"&#931;\">" +
                                     "<!ENTITY Tau      \"&#932;\">" +
                                     "<!ENTITY Upsilon  \"&#933;\">" +
                                     "<!ENTITY Phi      \"&#934;\">" +
                                     "<!ENTITY Chi      \"&#935;\">" +
                                     "<!ENTITY Psi      \"&#936;\">" +
                                     "<!ENTITY Omega    \"&#937;\">" +
                                     "<!ENTITY alpha    \"&#945;\">" +
                                     "<!ENTITY beta     \"&#946;\">" +
                                     "<!ENTITY gamma    \"&#947;\">" +
                                     "<!ENTITY delta    \"&#948;\">" +
                                     "<!ENTITY epsilon  \"&#949;\">" +
                                     "<!ENTITY zeta     \"&#950;\">" +
                                     "<!ENTITY eta      \"&#951;\">" +
                                     "<!ENTITY theta    \"&#952;\">" +
                                     "<!ENTITY iota     \"&#953;\">" +
                                     "<!ENTITY kappa    \"&#954;\">" +
                                     "<!ENTITY lambda   \"&#955;\">" +
                                     "<!ENTITY mu       \"&#956;\">" +
                                     "<!ENTITY nu       \"&#957;\">" +
                                     "<!ENTITY xi       \"&#958;\">" +
                                     "<!ENTITY omicron  \"&#959;\">" +
                                     "<!ENTITY pi       \"&#960;\">" +
                                     "<!ENTITY rho      \"&#961;\">" +
                                     "<!ENTITY sigmaf   \"&#962;\">" +
                                     "<!ENTITY sigma    \"&#963;\">" +
                                     "<!ENTITY tau      \"&#964;\">" +
                                     "<!ENTITY upsilon  \"&#965;\">" +
                                     "<!ENTITY phi      \"&#966;\">" +
                                     "<!ENTITY chi      \"&#967;\">" +
                                     "<!ENTITY psi      \"&#968;\">" +
                                     "<!ENTITY omega    \"&#969;\">" +
                                     "<!ENTITY thetasym \"&#977;\">" +
                                     "<!ENTITY upsih    \"&#978;\">" +
                                     "<!ENTITY piv      \"&#982;\">" +
                                     "<!ENTITY bull     \"&#8226;\">" +
                                     "<!ENTITY hellip   \"&#8230;\">" +
                                     "<!ENTITY prime    \"&#8242;\">" +
                                     "<!ENTITY Prime    \"&#8243;\">" +
                                     "<!ENTITY oline    \"&#8254;\">" +
                                     "<!ENTITY frasl    \"&#8260;\">" +
                                     "<!ENTITY weierp   \"&#8472;\">" +
                                     "<!ENTITY image    \"&#8465;\">" +
                                     "<!ENTITY real     \"&#8476;\">" +
                                     "<!ENTITY trade    \"&#8482;\">" +
                                     "<!ENTITY alefsym  \"&#8501;\">" +
                                     "<!ENTITY larr     \"&#8592;\">" +
                                     "<!ENTITY uarr     \"&#8593;\">" +
                                     "<!ENTITY rarr     \"&#8594;\">" +
                                     "<!ENTITY darr     \"&#8595;\">" +
                                     "<!ENTITY harr     \"&#8596;\">" +
                                     "<!ENTITY crarr    \"&#8629;\">" +
                                     "<!ENTITY lArr     \"&#8656;\">" +
                                     "<!ENTITY uArr     \"&#8657;\">" +
                                     "<!ENTITY rArr     \"&#8658;\">" +
                                     "<!ENTITY dArr     \"&#8659;\">" +
                                     "<!ENTITY hArr     \"&#8660;\">" +
                                     "<!ENTITY forall   \"&#8704;\">" +
                                     "<!ENTITY part     \"&#8706;\">" +
                                     "<!ENTITY exist    \"&#8707;\">" +
                                     "<!ENTITY empty    \"&#8709;\">" +
                                     "<!ENTITY nabla    \"&#8711;\">" +
                                     "<!ENTITY isin     \"&#8712;\">" +
                                     "<!ENTITY notin    \"&#8713;\">" +
                                     "<!ENTITY ni       \"&#8715;\">" +
                                     "<!ENTITY prod     \"&#8719;\">" +
                                     "<!ENTITY sum      \"&#8721;\">" +
                                     "<!ENTITY minus    \"&#8722;\">" +
                                     "<!ENTITY lowast   \"&#8727;\">" +
                                     "<!ENTITY radic    \"&#8730;\">" +
                                     "<!ENTITY prop     \"&#8733;\">" +
                                     "<!ENTITY infin    \"&#8734;\">" +
                                     "<!ENTITY ang      \"&#8736;\">" +
                                     "<!ENTITY and      \"&#8743;\">" +
                                     "<!ENTITY or       \"&#8744;\">" +
                                     "<!ENTITY cap      \"&#8745;\">" +
                                     "<!ENTITY cup      \"&#8746;\">" +
                                     "<!ENTITY int      \"&#8747;\">" +
                                     "<!ENTITY there4   \"&#8756;\">" +
                                     "<!ENTITY sim      \"&#8764;\">" +
                                     "<!ENTITY cong     \"&#8773;\">" +
                                     "<!ENTITY asymp    \"&#8776;\">" +
                                     "<!ENTITY ne       \"&#8800;\">" +
                                     "<!ENTITY equiv    \"&#8801;\">" +
                                     "<!ENTITY le       \"&#8804;\">" +
                                     "<!ENTITY ge       \"&#8805;\">" +
                                     "<!ENTITY sub      \"&#8834;\">" +
                                     "<!ENTITY sup      \"&#8835;\">" +
                                     "<!ENTITY nsub     \"&#8836;\">" +
                                     "<!ENTITY sube     \"&#8838;\">" +
                                     "<!ENTITY supe     \"&#8839;\">" +
                                     "<!ENTITY oplus    \"&#8853;\">" +
                                     "<!ENTITY otimes   \"&#8855;\">" +
                                     "<!ENTITY perp     \"&#8869;\">" +
                                     "<!ENTITY sdot     \"&#8901;\">" +
                                     "<!ENTITY lceil    \"&#8968;\">" +
                                     "<!ENTITY rceil    \"&#8969;\">" +
                                     "<!ENTITY lfloor   \"&#8970;\">" +
                                     "<!ENTITY rfloor   \"&#8971;\">" +
                                     "<!ENTITY lang     \"&#9001;\">" +
                                     "<!ENTITY rang     \"&#9002;\">" +
                                     "<!ENTITY loz      \"&#9674;\">" +
                                     "<!ENTITY spades   \"&#9824;\">" +
                                     "<!ENTITY clubs    \"&#9827;\">" +
                                     "<!ENTITY hearts   \"&#9829;\">" +
                                     "<!ENTITY diams    \"&#9830;\">" +
                                     "]>";

    }
}
