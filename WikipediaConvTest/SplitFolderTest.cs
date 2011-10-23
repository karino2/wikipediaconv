using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikipediaConvTest
{
    using NUnit.Framework;
    using WikipediaConv;
    using System.IO;
    using EPubLib;
    
    [TestFixture]
    public class SplitFolderTest
    {
#if false
        // NG
public const string _content = @"
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body><code>Dablink|Due to <a class=""pagelink"" href=""Wikipedia:Naming conventions (technical restrictions)#Forbidden characters"" title=""technical restrictions"">technical restrictions</a>, A# redirects here. For other uses, see <a class=""pagelink"" href=""A-sharp (disambiguation)"" title=""A-sharp (disambiguation)"">A-sharp (disambiguation)</a>.</code>
<code>pp-move-indef</code>
<code>Two other uses|the letter|the indefinite article|A and an</code>
<code>Latin alphabet navbox|uc=A|lc=a</code>
<b>A</b> (<code>IPAc-en|En-us-A.ogg|e?</code>; <a class=""pagelink"" href=""English_alphabet#Letter_names"" title=""named"">named</a> <i>a</i>, plural <i>aes</i>)<ref name=""OED""/> is the first <a class=""pagelink"" href=""Letter (alphabet)"" title=""letter"">letter</a> and a <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a> in the <a class=""pagelink"" href=""basic modern Latin alphabet"" title=""basic modern Latin alphabet"">basic modern Latin alphabet</a>. It is similar to the Ancient Greek letter <a class=""pagelink"" href=""Alpha"" title=""Alpha"">Alpha</a>, from which it derives.<br /><br /><a id=""Origins_0""></a><h1 class=""separator"">Origins</h1>
""A"" may have started as a <a class=""pagelink"" href=""pictogram"" title=""pictogram"">pictogram</a> of an <a class=""pagelink"" href=""ox"" title=""ox"">ox</a> head in <a class=""pagelink"" href=""Egyptian hieroglyph"" title=""Egyptian hieroglyph"">Egyptian hieroglyph</a>. It has stood at the head of every alphabet in which it has been found, the earliest of which being the Phoenician.<ref name=""Britannica""/><br /><br /><table class=""wikitable""><tr  style=""background-color:#EEEEEE; text-align:center;""><th> Egyptian</th><th> Phoenician <br /><i><a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a></i></th><th> Greek <br /><i><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></i></th><th> Etruscan <br />A</th><th> Roman/Cyrillic <br />A</th></tr><tr  style=""background-color:white; text-align:center;""><td <a class=""pagelink"" href=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"" title=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"">File:EgyptianA-01.svg>Egyptian hieroglyphic ox head</a></td><td <a class=""pagelink"" href=""File:PhoenicianA-01.svg>Phoenician aleph"" title=""File:PhoenicianA-01.svg>Phoenician aleph"">File:PhoenicianA-01.svg>Phoenician aleph</a></td><td <a class=""pagelink"" href=""File:Alpha uc lc.svg>65px"" title=""Greek alpha"">Greek alpha</a></td><td <a class=""pagelink"" href=""File:EtruscanA.svg>Etruscan A"" title=""File:EtruscanA.svg>Etruscan A"">File:EtruscanA.svg>Etruscan A</a></td><td <a class=""pagelink"" href=""File:RomanA-01.svg>Roman A"" title=""File:RomanA-01.svg>Roman A"">File:RomanA-01.svg>Roman A</a></td></tr></table><br />In 1600 B.C., the <a class=""pagelink"" href=""Phoenician alphabet"" title=""Phoenician alphabet"">Phoenician alphabet</a>'s letter had a linear form that served as the base for some later forms. Its name must have corresponded closely to the <a class=""pagelink"" href=""Hebrew alphabet"" title=""Hebrew"">Hebrew</a> or <a class=""pagelink"" href=""Arabic alphabet"" title=""Arabic"">Arabic</a> <a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a>.<br /><br /><table cellspacing=""10"" cellpadding=""0"" style=""background-color: white; float: right;""><tr  align=""center""><td <a class=""pagelink"" href=""File:BlackletterA-01.png>Blackletter A"" title=""File:BlackletterA-01.png>Blackletter A"">File:BlackletterA-01.png>Blackletter A</a><br /><a class=""pagelink"" href=""Blackletter"" title=""Blackletter"">Blackletter</a> A</td><td <a class=""pagelink"" href=""File:UncialA-01.svg>Uncial A"" title=""File:UncialA-01.svg>Uncial A"">File:UncialA-01.svg>Uncial A</a><br /><a class=""pagelink"" href=""Uncial"" title=""Uncial"">Uncial</a> A</td><td <a class=""pagelink"" href=""File:Acap.svg>Another Capital A"" title=""File:Acap.svg>Another Capital A"">File:Acap.svg>Another Capital A</a><br />Another Blackletter A&nbsp;</td></tr><tr  align=""center""><td <a class=""pagelink"" href=""File:ModernRomanA-01.svg>64 px"" title=""Modern Roman A"">Modern Roman A</a><br />Modern Roman A</td><td <a class=""pagelink"" href=""File:Modern Italic A.svg>64 px"" title=""Modern Italic A"">Modern Italic A</a><br />Modern Italic A</td><td <a class=""pagelink"" href=""File:Modern Script A.svg>64 px"" title=""Modern Script A"">Modern Script A</a><br />Modern Script A</td></tr></table>
When the <a class=""pagelink"" href=""Ancient Greece"" title=""Ancient Greeks"">Ancient Greeks</a> adopted the alphabet, they had no use for the <a class=""pagelink"" href=""glottal stop"" title=""glottal stop"">glottal stop</a> that the letter had denoted in <a class=""pagelink"" href=""Phoenician languages"" title=""Phoenician"">Phoenician</a> and other <a class=""pagelink"" href=""Semitic languages"" title=""Semitic languages"">Semitic languages</a>, so they used the sign to represent the vowel <code>IPA|/a/</code>, and kept its name with a minor change (<a class=""pagelink"" href=""alpha (letter)"" title=""alpha"">alpha</a>). In the earliest Greek inscriptions after the <a class=""pagelink"" href=""Greek Dark Ages"" title=""Greek Dark Ages"">Greek Dark Ages</a>, dating to the 8th century BC, the letter rests upon its side, but in the <a class=""pagelink"" href=""Greek alphabet"" title=""Greek alphabet"">Greek alphabet</a> of later times it generally resembles the modern capital letter, although many local varieties can be distinguished by the shortening of one leg, or by the angle at which the cross line is set.<br /><br />The <a class=""pagelink"" href=""Etruscans"" title=""Etruscans"">Etruscans</a> brought the Greek alphabet to their civilization in the <a class=""pagelink"" href=""Italian Peninsula"" title=""Italian Peninsula"">Italian Peninsula</a> and left the letter unchanged. The Romans later adopted the <a class=""pagelink"" href=""Old Italic alphabet"" title=""Etruscan alphabet"">Etruscan alphabet</a> to write the <a class=""pagelink"" href=""Latin language"" title=""Latin language"">Latin language</a>, and the resulting letter was preserved in the modern <a class=""pagelink"" href=""Latin alphabet"" title=""Latin alphabet"">Latin alphabet</a> used to write many languages, including <a class=""pagelink"" href=""English language"" title=""English"">English</a>.<br /><br />
The letter has two <a class=""pagelink"" href=""lower case"" title=""minuscule"">minuscule</a> (lower-case) forms. The form used in most current <a class=""pagelink"" href=""handwriting"" title=""handwriting"">handwriting</a> consists of a circle and vertical stroke (<code>Unicode|""?""</code>), called <a class=""pagelink"" href=""Latin alpha"" title=""Latin alpha"">Latin alpha</a> or ""script a"". This slowly developed from the fifth-century form resembling the Greek letter <a class=""pagelink"" href=""tau"" title=""tau"">tau</a> in the hands of dark-age Irish and English writers.<ref name=""Britannica""/> Most printed material uses a form consisting of a small loop with an arc over it (<code>Unicode|""a""</code>). Both derive from the <a class=""pagelink"" href=""majuscule"" title=""majuscule"">majuscule</a> (capital) form. In Greek handwriting, it was common to join the left leg and horizontal stroke into a single loop, as demonstrated by the Uncial version shown. Many fonts then made the right leg vertical. In some of these, the <a class=""pagelink"" href=""serif"" title=""serif"">serif</a> that began the right leg stroke developed into an arc, resulting in the printed form, while in others it was dropped, resulting in the modern handwritten form.<br /><br /><a id=""Usage_1""></a><h1 class=""separator"">Usage</h1>
<code>main|a (disambiguation)</code>
The letter A currently represents six different vowel sounds. In English, ""a"" by itself frequently denotes the <a class=""pagelink"" href=""near-open front unrounded vowel"" title=""near-open front unrounded vowel"">near-open front unrounded vowel</a> (<code>IPA|/a/</code>) as in <i>pad</i>; the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> (<code>IPA|/??/</code>) as in <i>father</i>, its original, Latin and Greek, sound; a closer, further fronted sound as in ""hare"", which developed as the sound progressed from ""father"" to ""ace"";<ref name=""Britannica""/> in concert with a later orthographic vowel, the diphthong <code>IPA|/e?/</code> as in <i>ace</i> and <i>major</i>, due to effects of the <a class=""pagelink"" href=""great vowel shift"" title=""great vowel shift"">great vowel shift</a>; the more rounded form in ""water"" or its closely-related cousin, found in ""was"".<ref name=""Britannica""/><br /><br />In most other languages that use the Latin alphabet, ""a"" denotes an <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a> (<code>IPA|/a/</code>). In the <a class=""pagelink"" href=""help:IPA"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a>, variants of ""a"" denote various <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a>s. In <a class=""pagelink"" href=""X-SAMPA"" title=""X-SAMPA"">X-SAMPA</a>, capital ""A"" denotes the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> and lowercase ""a"" denotes the <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a>.<br /><br />""A"" is the third most commonly used letter in English, and the second most common in <a class=""pagelink"" href=""Spanish language"" title=""Spanish"">Spanish</a> and <a class=""pagelink"" href=""French language"" title=""French"">French</a>. In one study, on average, about 3.68% of letters used in English tend to be ?a?s, while the number is 6.22% in Spanish and 3.95% in French.<ref name=""Trinity College"" /><br /><br />""A"" is often used to denote something or someone of a better or more prestigious quality or status: A-, A or A+, the best grade that can be assigned by teachers for students' schoolwork; A grade for clean restaurants; <a class=""pagelink"" href=""A-List"" title=""A-List"">A-List</a> celebrities, etc. Such associations can have a <a class=""pagelink"" href=""motivation"" title=""motivating"">motivating</a> effect as exposure to the letter A has been found to improve performance, when compared with other letters.<ref><code>Cite document |url=http://www.sciencealert.com.au/news/20100903-20689.html |title=Letters affect exam results |date=9 March 2010 |publisher=British Psychological Society |postscript=<!--None--></code></ref><br /><br />A <a class=""pagelink"" href=""turned a"" title=""turned ""a"""">turned ""a""</a>, <code>IPA|???</code> is used by the <a class=""pagelink"" href=""International Phonetic Alphabet"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a> for the <a class=""pagelink"" href=""near-open central vowel"" title=""near-open central vowel"">near-open central vowel</a>, while a turned capital ""A"" (""∀"") is used in <a class=""pagelink"" href=""predicate logic"" title=""predicate logic"">predicate logic</a> to specify <a class=""pagelink"" href=""universal quantification"" title=""universal quantification"">universal quantification</a>.<br /><br /><a id=""Computing_codes_2""></a><h1 class=""separator"">Computing codes</h1>
 of Unicode U+0061.]]<br /><br />In <a class=""pagelink"" href=""Unicode"" title=""Unicode"">Unicode</a>, the <a class=""pagelink"" href=""majuscule"" title=""capital"">capital</a> ""A"" is codepoint U+0041 and the <a class=""pagelink"" href=""lower case"" title=""lower case"">lower case</a> ""a"" is U+0061.<ref name=""unicode"" /><br />
The closed form (<code>Unicode|""?""</code>), which is related with the lowercase <a class=""pagelink"" href=""alpha"" title=""alpha"">alpha</a>, has codepoint U+0251 (from the Code Chart ""IPA Extensions"".<ref><a class=""externallink"" href=""http://www.unicode.org/charts/PDF/U0250.pdf "" title="""" target=""_blank"">Unicode.org</a></ref><br /><br />The <a class=""pagelink"" href=""ASCII"" title=""ASCII"">ASCII</a> code for capital ""A"" is 65 and for lower case ""a"" is 97; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 01000001 and 01100001, respectively.<br /><br />The <a class=""pagelink"" href=""EBCDIC"" title=""EBCDIC"">EBCDIC</a> code for capital ""A"" is 193 and for lowercase ""a"" is 129; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 11000001 and 10000001, respectively.<br /><br />The <a class=""pagelink"" href=""numeric character reference"" title=""numeric character reference"">numeric character reference</a>s in <a class=""pagelink"" href=""HTML"" title=""HTML"">HTML</a> and <a class=""pagelink"" href=""XML"" title=""XML"">XML</a> are ""<tt>&amp;#65;</tt>"" and ""<tt>&amp;#97;</tt>"" for upper and lower case, respectively.<br /><br /><a id=""Other_representations_3""></a><h1 class=""separator"">Other representations</h1>
<div style=""float:left"">
<code>Letter
|NATO=Alpha<!--don't change to official ""alfa"" until Commons images are moved to this spelling, or redirects are set up, as otherwise the table does not display the semaphore and flag images-->
|Morse=・?
|Character=A1
|Braille=??
</code>
</div>
{<div style=""clear: both;""></div>}<br /><br /><a id=""See_also_4""></a><h1 class=""separator"">See also</h1>
<code>Commons|A</code>
<ul><li><big><a class=""pagelink"" href=""a"" title=""a"">a</a></big></li><li><a class=""pagelink"" href=""A"" title=""A"">A</a></li><li><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></li><li><a class=""pagelink"" href=""A (Cyrillic)"" title=""Cyrillic A"">Cyrillic A</a></li></ul><br /><a id=""References_5""></a><h1 class=""separator"">References</h1><br /><br /><code>Reflist|refs=
<ref name=""OED"">""A"" (word), <i>Oxford English Dictionary,</i> 2nd edition, 1989. <i>Aes</i> is the plural of the name of the letter. The plural of the letter itself is: <i>A</i>s, A's, <i>a</i>s, or a's.</ref><br /><br /><ref name=""Trinity College""><a class=""externallink"" href=""http://starbase.trincoll.edu/~crypto/resources/LetFreq.html "" title="""" target=""_blank"">""Percentages of Letter frequencies per Thousand words""</a>, Trinity College, Retrieved 2006-05-01.</ref><br /><br /><ref name=""unicode""><a class=""externallink"" href=""http://macchiato.com/unicode/chart/ "" title="""" target=""_blank"">""Javascript Unicode Chart""</a>, macchiato.com, 2009, Retrieved 2009-03-08.</ref><br /><br /><ref name=""Britannica"">""A"", ""Encyclopaedia Britannica"", Volume 1, 1962. p.1.</ref> 
</code><br /><br /><a id=""_External_links__6""></a><h1 class=""separator""> External links </h1>
<code>Wiktionary|A|a</code>
<ul><li><a class=""externallink"" href=""http://members.peak.org/~jeremy/dictionaryclassic/chapters/pix/alphabet.gif "" title="""" target=""_blank"">History of the Alphabet</a></li><li><code>Wikisource-inline|list=<ul><li>“<a class=""pagelink"" href=""s:A Dictionary of the English Language/A"" title=""A"">A</a>” in <i><a class=""pagelink"" href=""s:A Dictionary of the English Language"" title=""A Dictionary of the English Language"">A Dictionary of the English Language</a></i> by <a class=""pagelink"" href=""Samuel Johnson"" title=""Samuel Johnson"">Samuel Johnson</a></li><li>Chisholm, Hugh, ed. (1911). <a class=""pagelink"" href=""wikisource:1911 Encyclopadia Britannica/A"" title=""""A"""">""A""</a> (entry). <i><a class=""pagelink"" href=""Encyclopadia Britannica"" title=""Encyclopadia Britannica"">Encyclopadia Britannica</a></i> (Eleventh ed.). Cambridge University Press.</li><li>""<a class=""pagelink"" href=""wikisource:The New Student's Reference Work/A"" title=""A"">A</a>"". <i>The New Student's Reference Work</i>. Chicago: F. E. Compton and Co. 1914.</li><li>""<a class=""pagelink"" href=""wikisource:Collier's New Encyclopedia (1921)/A"" title=""A"">A</a>"". <i><a class=""pagelink"" href=""Collier's Encyclopedia"" title=""Collier's New Encyclopedia"">Collier's New Encyclopedia</a></i>. 1921.</li></ul></li></ul>
</code><br /><br /><code>Latin alphabet|A|</code><br /><br /><a class=""pagelink"" href=""Category:ISO basic Latin letters"" title=""Category:ISO basic Latin letters"">Category:ISO basic Latin letters</a>
<a class=""pagelink"" href=""Category:Vowel letters"" title=""Category:Vowel letters"">Category:Vowel letters</a><br /><br /><code>Link GA|th</code><br /><br /><a class=""pagelink"" href=""ace:A"" title=""ace:A"">ace:A</a>
<a class=""pagelink"" href=""af:A"" title=""af:A"">af:A</a>
<a class=""pagelink"" href=""als:A"" title=""als:A"">als:A</a>
<a class=""pagelink"" href=""ar:A"" title=""ar:A"">ar:A</a>
<a class=""pagelink"" href=""an:A"" title=""an:A"">an:A</a>
<a class=""pagelink"" href=""arc:A"" title=""arc:A"">arc:A</a>
<a class=""pagelink"" href=""ast:A"" title=""ast:A"">ast:A</a>
<a class=""pagelink"" href=""az:A"" title=""az:A"">az:A</a>
<a class=""pagelink"" href=""zh-min-nan:A"" title=""zh-min-nan:A"">zh-min-nan:A</a>
<a class=""pagelink"" href=""ba:A (латин х?рефе)"" title=""ba:A (латин х?рефе)"">ba:A (латин х?рефе)</a>
<a class=""pagelink"" href=""be:A, л?тара"" title=""be:A, л?тара"">be:A, л?тара</a>
<a class=""pagelink"" href=""be-x-old:A (л?тара)"" title=""be-x-old:A (л?тара)"">be-x-old:A (л?тара)</a>
<a class=""pagelink"" href=""bar:A"" title=""bar:A"">bar:A</a>
<a class=""pagelink"" href=""bs:A"" title=""bs:A"">bs:A</a>
<a class=""pagelink"" href=""br:A (lizherenn)"" title=""br:A (lizherenn)"">br:A (lizherenn)</a>
<a class=""pagelink"" href=""bg:A"" title=""bg:A"">bg:A</a>
<a class=""pagelink"" href=""ca:A"" title=""ca:A"">ca:A</a>
<a class=""pagelink"" href=""cs:A"" title=""cs:A"">cs:A</a>
<a class=""pagelink"" href=""tum:A"" title=""tum:A"">tum:A</a>
<a class=""pagelink"" href=""co:A"" title=""co:A"">co:A</a>
<a class=""pagelink"" href=""cy:A"" title=""cy:A"">cy:A</a>
<a class=""pagelink"" href=""da:A"" title=""da:A"">da:A</a>
<a class=""pagelink"" href=""de:A"" title=""de:A"">de:A</a>
<a class=""pagelink"" href=""dv:????"" title=""dv:????"">dv:????</a>
<a class=""pagelink"" href=""et:A"" title=""et:A"">et:A</a>
<a class=""pagelink"" href=""el:A"" title=""el:A"">el:A</a>
<a class=""pagelink"" href=""eml:A"" title=""eml:A"">eml:A</a>
<a class=""pagelink"" href=""es:A"" title=""es:A"">es:A</a>
<a class=""pagelink"" href=""eo:A"" title=""eo:A"">eo:A</a>
<a class=""pagelink"" href=""eu:A"" title=""eu:A"">eu:A</a>
<a class=""pagelink"" href=""fa:A"" title=""fa:A"">fa:A</a>
<a class=""pagelink"" href=""fr:A (lettre)"" title=""fr:A (lettre)"">fr:A (lettre)</a>
<a class=""pagelink"" href=""fy:A"" title=""fy:A"">fy:A</a>
<a class=""pagelink"" href=""fur:A"" title=""fur:A"">fur:A</a>
<a class=""pagelink"" href=""gv:Aittyn"" title=""gv:Aittyn"">gv:Aittyn</a>
<a class=""pagelink"" href=""gd:A"" title=""gd:A"">gd:A</a>
<a class=""pagelink"" href=""gl:A"" title=""gl:A"">gl:A</a>
<a class=""pagelink"" href=""gan:A"" title=""gan:A"">gan:A</a>
<a class=""pagelink"" href=""xal:A ?зг"" title=""xal:A ?зг"">xal:A ?зг</a>
<a class=""pagelink"" href=""ko:A"" title=""ko:A"">ko:A</a>
<a class=""pagelink"" href=""hr:A"" title=""hr:A"">hr:A</a>
<a class=""pagelink"" href=""io:A"" title=""io:A"">io:A</a>
<a class=""pagelink"" href=""ilo:A"" title=""ilo:A"">ilo:A</a>
<a class=""pagelink"" href=""id:A"" title=""id:A"">id:A</a>
<a class=""pagelink"" href=""ia:A"" title=""ia:A"">ia:A</a>
<a class=""pagelink"" href=""is:A"" title=""is:A"">is:A</a>
<a class=""pagelink"" href=""it:A"" title=""it:A"">it:A</a>
<a class=""pagelink"" href=""he:A"" title=""he:A"">he:A</a>
<a class=""pagelink"" href=""jv:A"" title=""jv:A"">jv:A</a>
<a class=""pagelink"" href=""ka:A"" title=""ka:A"">ka:A</a>
<a class=""pagelink"" href=""kw:A"" title=""kw:A"">kw:A</a>
<a class=""pagelink"" href=""sw:A"" title=""sw:A"">sw:A</a>
<a class=""pagelink"" href=""ht:A"" title=""ht:A"">ht:A</a>
<a class=""pagelink"" href=""ku:A (tip)"" title=""ku:A (tip)"">ku:A (tip)</a>
<a class=""pagelink"" href=""la:A"" title=""la:A"">la:A</a>
<a class=""pagelink"" href=""lv:A"" title=""lv:A"">lv:A</a>
<a class=""pagelink"" href=""lb:A (Buschtaf)"" title=""lb:A (Buschtaf)"">lb:A (Buschtaf)</a>
<a class=""pagelink"" href=""lt:A"" title=""lt:A"">lt:A</a>
<a class=""pagelink"" href=""lmo:A"" title=""lmo:A"">lmo:A</a>
<a class=""pagelink"" href=""hu:A"" title=""hu:A"">hu:A</a>
<a class=""pagelink"" href=""mk:A (Латиница)"" title=""mk:A (Латиница)"">mk:A (Латиница)</a>
<a class=""pagelink"" href=""mg:A"" title=""mg:A"">mg:A</a>
<a class=""pagelink"" href=""ml:A"" title=""ml:A"">ml:A</a>
<a class=""pagelink"" href=""mr:A"" title=""mr:A"">mr:A</a>
<a class=""pagelink"" href=""mzn:A"" title=""mzn:A"">mzn:A</a>
<a class=""pagelink"" href=""ms:A"" title=""ms:A"">ms:A</a>
<a class=""pagelink"" href=""my:A"" title=""my:A"">my:A</a>
<a class=""pagelink"" href=""nah:A"" title=""nah:A"">nah:A</a>
<a class=""pagelink"" href=""nl:A (letter)"" title=""nl:A (letter)"">nl:A (letter)</a>
<a class=""pagelink"" href=""ja:A"" title=""ja:A"">ja:A</a>
<a class=""pagelink"" href=""nap:A"" title=""nap:A"">nap:A</a>
<a class=""pagelink"" href=""no:A"" title=""no:A"">no:A</a>
<a class=""pagelink"" href=""nn:A"" title=""nn:A"">nn:A</a>
<a class=""pagelink"" href=""nrm:A"" title=""nrm:A"">nrm:A</a>
<a class=""pagelink"" href=""uz:A (harf)"" title=""uz:A (harf)"">uz:A (harf)</a>
<a class=""pagelink"" href=""pl:A"" title=""pl:A"">pl:A</a>
<a class=""pagelink"" href=""pt:A"" title=""pt:A"">pt:A</a>
<a class=""pagelink"" href=""crh:A"" title=""crh:A"">crh:A</a>
<a class=""pagelink"" href=""ro:A"" title=""ro:A"">ro:A</a>
<a class=""pagelink"" href=""qu:A"" title=""qu:A"">qu:A</a>
<a class=""pagelink"" href=""ru:A (латиница)"" title=""ru:A (латиница)"">ru:A (латиница)</a>
<a class=""pagelink"" href=""se:A"" title=""se:A"">se:A</a>
<a class=""pagelink"" href=""stq:A"" title=""stq:A"">stq:A</a>
<a class=""pagelink"" href=""scn:A"" title=""scn:A"">scn:A</a>
<a class=""pagelink"" href=""simple:A"" title=""simple:A"">simple:A</a>
<a class=""pagelink"" href=""sk:A"" title=""sk:A"">sk:A</a>
<a class=""pagelink"" href=""sl:A"" title=""sl:A"">sl:A</a>
<a class=""pagelink"" href=""szl:A"" title=""szl:A"">szl:A</a>
<a class=""pagelink"" href=""srn:A"" title=""srn:A"">srn:A</a>
<a class=""pagelink"" href=""sr:A (слово латинице)"" title=""sr:A (слово латинице)"">sr:A (слово латинице)</a>
<a class=""pagelink"" href=""sh:A"" title=""sh:A"">sh:A</a>
<a class=""pagelink"" href=""su:A"" title=""su:A"">su:A</a>
<a class=""pagelink"" href=""fi:A"" title=""fi:A"">fi:A</a>
<a class=""pagelink"" href=""sv:A"" title=""sv:A"">sv:A</a>
<a class=""pagelink"" href=""tl:A"" title=""tl:A"">tl:A</a>
<a class=""pagelink"" href=""th:A"" title=""th:A"">th:A</a>
<a class=""pagelink"" href=""tr:A (harf)"" title=""tr:A (harf)"">tr:A (harf)</a>
<a class=""pagelink"" href=""tk:A"" title=""tk:A"">tk:A</a>
<a class=""pagelink"" href=""uk:A (латиниця)"" title=""uk:A (латиниця)"">uk:A (латиниця)</a>
<a class=""pagelink"" href=""vi:A"" title=""vi:A"">vi:A</a>
<a class=""pagelink"" href=""vo:A"" title=""vo:A"">vo:A</a>
<a class=""pagelink"" href=""war:A"" title=""war:A"">war:A</a>
<a class=""pagelink"" href=""yi:A"" title=""yi:A"">yi:A</a>
<a class=""pagelink"" href=""yo:A"" title=""yo:A"">yo:A</a>
<a class=""pagelink"" href=""zh-yue:A"" title=""zh-yue:A"">zh-yue:A</a>
<a class=""pagelink"" href=""diq:A"" title=""diq:A"">diq:A</a>
<a class=""pagelink"" href=""bat-smg:A"" title=""bat-smg:A"">bat-smg:A</a>
<a class=""pagelink"" href=""zh:A"" title=""zh:A"">zh:A</a>
</body></html>";
#endif
#if false
        // NG
        public const string _content = @"
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body>
hello </body></html>";
#endif
#if false
        // OK
        public const string _content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body>
hello </body></html>";
#endif
#if false
        // until roman/cyrillic
        public const string _content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body><code>Dablink|Due to <a class=""pagelink"" href=""Wikipedia:Naming conventions (technical restrictions)#Forbidden characters"" title=""technical restrictions"">technical restrictions</a>, A# redirects here. For other uses, see <a class=""pagelink"" href=""A-sharp (disambiguation)"" title=""A-sharp (disambiguation)"">A-sharp (disambiguation)</a>.</code>
<code>pp-move-indef</code>
<code>Two other uses|the letter|the indefinite article|A and an</code>
<code>Latin alphabet navbox|uc=A|lc=a</code>
<b>A</b> (<code>IPAc-en|En-us-A.ogg|e?</code>; <a class=""pagelink"" href=""English_alphabet#Letter_names"" title=""named"">named</a> <i>a</i>, plural <i>aes</i>)<ref name=""OED""/> is the first <a class=""pagelink"" href=""Letter (alphabet)"" title=""letter"">letter</a> and a <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a> in the <a class=""pagelink"" href=""basic modern Latin alphabet"" title=""basic modern Latin alphabet"">basic modern Latin alphabet</a>. It is similar to the Ancient Greek letter <a class=""pagelink"" href=""Alpha"" title=""Alpha"">Alpha</a>, from which it derives.<br /><br /><a id=""Origins_0""></a><h1 class=""separator"">Origins</h1>
""A"" may have started as a <a class=""pagelink"" href=""pictogram"" title=""pictogram"">pictogram</a> of an <a class=""pagelink"" href=""ox"" title=""ox"">ox</a> head in <a class=""pagelink"" href=""Egyptian hieroglyph"" title=""Egyptian hieroglyph"">Egyptian hieroglyph</a>. It has stood at the head of every alphabet in which it has been found, the earliest of which being the Phoenician.<ref name=""Britannica""/><br /><br /><table class=""wikitable""><tr  style=""background-color:#EEEEEE; text-align:center;""><th> Egyptian</th><th> Phoenician <br /><i><a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a></i></th><th> Greek <br /><i><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></i></th><th> Etruscan <br />A</th><th> Roman/Cyrillic <br />A</th></tr><tr  style=""background-color:white; text-align:center;""><td <a class=""pagelink"" href=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"" title=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"">File:EgyptianA-01.svg>Egyptian hieroglyphic ox head</a></td><td <a class=""pagelink"" href=""File:PhoenicianA-01.svg>Phoenician aleph"" title=""File:PhoenicianA-01.svg>Phoenician aleph"">File:PhoenicianA-01.svg>Phoenician aleph</a></td><td <a class=""pagelink"" href=""File:Alpha uc lc.svg>65px"" title=""Greek alpha"">Greek alpha</a></td><td <a class=""pagelink"" href=""File:EtruscanA.svg>Etruscan A"" title=""File:EtruscanA.svg>Etruscan A"">File:EtruscanA.svg>Etruscan A</a></td><td <a class=""pagelink"" href=""File:RomanA-01.svg>Roman A"" title=""File:RomanA-01.svg>Roman A"">File:RomanA-01.svg>Roman A</a></td></tr></table><br />In 1600 B.C., the <a class=""pagelink"" href=""Phoenician alphabet"" title=""Phoenician alphabet"">Phoenician alphabet</a>'s letter had a linear form that served as the base for some later forms. Its name must have corresponded closely to the <a class=""pagelink"" href=""Hebrew alphabet"" title=""Hebrew"">Hebrew</a> or <a class=""pagelink"" href=""Arabic alphabet"" title=""Arabic"">Arabic</a> <a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a>.<br /><br /><table cellspacing=""10"" cellpadding=""0"" style=""background-color: white; float: right;""><tr  align=""center""><td <a class=""pagelink"" href=""File:BlackletterA-01.png>Blackletter A"" title=""File:BlackletterA-01.png>Blackletter A"">File:BlackletterA-01.png>Blackletter A</a><br /><a class=""pagelink"" href=""Blackletter"" title=""Blackletter"">Blackletter</a> A</td><td <a class=""pagelink"" href=""File:UncialA-01.svg>Uncial A"" title=""File:UncialA-01.svg>Uncial A"">File:UncialA-01.svg>Uncial A</a><br /><a class=""pagelink"" href=""Uncial"" title=""Uncial"">Uncial</a> A</td><td <a class=""pagelink"" href=""File:Acap.svg>Another Capital A"" title=""File:Acap.svg>Another Capital A"">File:Acap.svg>Another Capital A</a><br />Another Blackletter A&nbsp;</td></tr><tr  align=""center""><td <a class=""pagelink"" href=""File:ModernRomanA-01.svg>64 px"" title=""Modern Roman A"">Modern Roman A</a><br />Modern Roman A</td><td <a class=""pagelink"" href=""File:Modern Italic A.svg>64 px"" title=""Modern Italic A"">Modern Italic A</a><br />Modern Italic A</td><td <a class=""pagelink"" href=""File:Modern Script A.svg>64 px"" title=""Modern Script A"">Modern Script A</a><br />Modern Script A</td></tr></table>
When the <a class=""pagelink"" href=""Ancient Greece"" title=""Ancient Greeks"">Ancient Greeks</a> adopted the alphabet, they had no use for the <a class=""pagelink"" href=""glottal stop"" title=""glottal stop"">glottal stop</a> that the letter had denoted in <a class=""pagelink"" href=""Phoenician languages"" title=""Phoenician"">Phoenician</a> and other <a class=""pagelink"" href=""Semitic languages"" title=""Semitic languages"">Semitic languages</a>, so they used the sign to represent the vowel <code>IPA|/a/</code>, and kept its name with a minor change (<a class=""pagelink"" href=""alpha (letter)"" title=""alpha"">alpha</a>). In the earliest Greek inscriptions after the <a class=""pagelink"" href=""Greek Dark Ages"" title=""Greek Dark Ages"">Greek Dark Ages</a>, dating to the 8th century BC, the letter rests upon its side, but in the <a class=""pagelink"" href=""Greek alphabet"" title=""Greek alphabet"">Greek alphabet</a> of later times it generally resembles the modern capital letter, although many local varieties can be distinguished by the shortening of one leg, or by the angle at which the cross line is set.<br /><br />The <a class=""pagelink"" href=""Etruscans"" title=""Etruscans"">Etruscans</a> brought the Greek alphabet to their civilization in the <a class=""pagelink"" href=""Italian Peninsula"" title=""Italian Peninsula"">Italian Peninsula</a> and left the letter unchanged. The Romans later adopted the <a class=""pagelink"" href=""Old Italic alphabet"" title=""Etruscan alphabet"">Etruscan alphabet</a> to write the <a class=""pagelink"" href=""Latin language"" title=""Latin language"">Latin language</a>, and the resulting letter was preserved in the modern <a class=""pagelink"" href=""Latin alphabet"" title=""Latin alphabet"">Latin alphabet</a> used to write many languages, including <a class=""pagelink"" href=""English language"" title=""English"">English</a>.<br /><br />
The letter has two <a class=""pagelink"" href=""lower case"" title=""minuscule"">minuscule</a> (lower-case) forms. The form used in most current <a class=""pagelink"" href=""handwriting"" title=""handwriting"">handwriting</a> consists of a circle and vertical stroke (<code>Unicode|""?""</code>), called <a class=""pagelink"" href=""Latin alpha"" title=""Latin alpha"">Latin alpha</a> or ""script a"". This slowly developed from the fifth-century form resembling the Greek letter <a class=""pagelink"" href=""tau"" title=""tau"">tau</a> in the hands of dark-age Irish and English writers.<ref name=""Britannica""/> Most printed material uses a form consisting of a small loop with an arc over it (<code>Unicode|""a""</code>). Both derive from the <a class=""pagelink"" href=""majuscule"" title=""majuscule"">majuscule</a> (capital) form. In Greek handwriting, it was common to join the left leg and horizontal stroke into a single loop, as demonstrated by the Uncial version shown. Many fonts then made the right leg vertical. In some of these, the <a class=""pagelink"" href=""serif"" title=""serif"">serif</a> that began the right leg stroke developed into an arc, resulting in the printed form, while in others it was dropped, resulting in the modern handwritten form.<br /><br /><a id=""Usage_1""></a><h1 class=""separator"">Usage</h1>
<code>main|a (disambiguation)</code>
The letter A currently represents six different vowel sounds. In English, ""a"" by itself frequently denotes the <a class=""pagelink"" href=""near-open front unrounded vowel"" title=""near-open front unrounded vowel"">near-open front unrounded vowel</a> (<code>IPA|/a/</code>) as in <i>pad</i>; the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> (<code>IPA|/??/</code>) as in <i>father</i>, its original, Latin and Greek, sound; a closer, further fronted sound as in ""hare"", which developed as the sound progressed from ""father"" to ""ace"";<ref name=""Britannica""/> in concert with a later orthographic vowel, the diphthong <code>IPA|/e?/</code> as in <i>ace</i> and <i>major</i>, due to effects of the <a class=""pagelink"" href=""great vowel shift"" title=""great vowel shift"">great vowel shift</a>; the more rounded form in ""water"" or its closely-related cousin, found in ""was"".<ref name=""Britannica""/><br /><br />In most other languages that use the Latin alphabet, ""a"" denotes an <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a> (<code>IPA|/a/</code>). In the <a class=""pagelink"" href=""help:IPA"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a>, variants of ""a"" denote various <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a>s. In <a class=""pagelink"" href=""X-SAMPA"" title=""X-SAMPA"">X-SAMPA</a>, capital ""A"" denotes the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> and lowercase ""a"" denotes the <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a>.<br /><br />""A"" is the third most commonly used letter in English, and the second most common in <a class=""pagelink"" href=""Spanish language"" title=""Spanish"">Spanish</a> and <a class=""pagelink"" href=""French language"" title=""French"">French</a>. In one study, on average, about 3.68% of letters used in English tend to be ?a?s, while the number is 6.22% in Spanish and 3.95% in French.<ref name=""Trinity College"" /><br /><br />""A"" is often used to denote something or someone of a better or more prestigious quality or status: A-, A or A+, the best grade that can be assigned by teachers for students' schoolwork; A grade for clean restaurants; <a class=""pagelink"" href=""A-List"" title=""A-List"">A-List</a> celebrities, etc. Such associations can have a <a class=""pagelink"" href=""motivation"" title=""motivating"">motivating</a> effect as exposure to the letter A has been found to improve performance, when compared with other letters.<ref><code>Cite document |url=http://www.sciencealert.com.au/news/20100903-20689.html |title=Letters affect exam results |date=9 March 2010 |publisher=British Psychological Society |postscript=<!--None--></code></ref><br /><br />A <a class=""pagelink"" href=""turned a"" title=""turned ""a"""">turned ""a""</a>, <code>IPA|???</code> is used by the <a class=""pagelink"" href=""International Phonetic Alphabet"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a> for the <a class=""pagelink"" href=""near-open central vowel"" title=""near-open central vowel"">near-open central vowel</a>, while a turned capital ""A"" (""∀"") is used in <a class=""pagelink"" href=""predicate logic"" title=""predicate logic"">predicate logic</a> to specify <a class=""pagelink"" href=""universal quantification"" title=""universal quantification"">universal quantification</a>.<br /><br /><a id=""Computing_codes_2""></a><h1 class=""separator"">Computing codes</h1>
 of Unicode U+0061.]]<br /><br />In <a class=""pagelink"" href=""Unicode"" title=""Unicode"">Unicode</a>, the <a class=""pagelink"" href=""majuscule"" title=""capital"">capital</a> ""A"" is codepoint U+0041 and the <a class=""pagelink"" href=""lower case"" title=""lower case"">lower case</a> ""a"" is U+0061.<ref name=""unicode"" /><br />
The closed form (<code>Unicode|""?""</code>), which is related with the lowercase <a class=""pagelink"" href=""alpha"" title=""alpha"">alpha</a>, has codepoint U+0251 (from the Code Chart ""IPA Extensions"".<ref><a class=""externallink"" href=""http://www.unicode.org/charts/PDF/U0250.pdf "" title="""" target=""_blank"">Unicode.org</a></ref><br /><br />The <a class=""pagelink"" href=""ASCII"" title=""ASCII"">ASCII</a> code for capital ""A"" is 65 and for lower case ""a"" is 97; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 01000001 and 01100001, respectively.<br /><br />The <a class=""pagelink"" href=""EBCDIC"" title=""EBCDIC"">EBCDIC</a> code for capital ""A"" is 193 and for lowercase ""a"" is 129; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 11000001 and 10000001, respectively.<br /><br />The <a class=""pagelink"" href=""numeric character reference"" title=""numeric character reference"">numeric character reference</a>s in <a class=""pagelink"" href=""HTML"" title=""HTML"">HTML</a> and <a class=""pagelink"" href=""XML"" title=""XML"">XML</a> are ""<tt>&amp;#65;</tt>"" and ""<tt>&amp;#97;</tt>"" for upper and lower case, respectively.<br /><br /><a id=""Other_representations_3""></a><h1 class=""separator"">Other representations</h1>
<div style=""float:left"">
<code>Letter
|NATO=Alpha<!--don't change to official ""alfa"" until Commons images are moved to this spelling, or redirects are set up, as otherwise the table does not display the semaphore and flag images-->
|Morse=・?
|Character=A1
|Braille=??
</code>
</div>
{<div style=""clear: both;""></div>}<br /><br /><a id=""See_also_4""></a><h1 class=""separator"">See also</h1>
<code>Commons|A</code>
<ul><li><big><a class=""pagelink"" href=""a"" title=""a"">a</a></big></li><li><a class=""pagelink"" href=""A"" title=""A"">A</a></li><li><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></li><li><a class=""pagelink"" href=""A (Cyrillic)"" title=""Cyrillic A"">Cyrillic A</a></li></ul><br /><a id=""References_5""></a><h1 class=""separator"">References</h1><br /><br /><code>Reflist|refs=
<ref name=""OED"">""A"" (word), <i>Oxford English Dictionary,</i> 2nd edition, 1989. <i>Aes</i> is the plural of the name of the letter. The plural of the letter itself is: <i>A</i>s, A's, <i>a</i>s, or a's.</ref><br /><br /><ref name=""Trinity College""><a class=""externallink"" href=""http://starbase.trincoll.edu/~crypto/resources/LetFreq.html "" title="""" target=""_blank"">""Percentages of Letter frequencies per Thousand words""</a>, Trinity College, Retrieved 2006-05-01.</ref><br /><br /><ref name=""unicode""><a class=""externallink"" href=""http://macchiato.com/unicode/chart/ "" title="""" target=""_blank"">""Javascript Unicode Chart""</a>, macchiato.com, 2009, Retrieved 2009-03-08.</ref><br /><br /><ref name=""Britannica"">""A"", ""Encyclopaedia Britannica"", Volume 1, 1962. p.1.</ref> 
</code><br /><br /><a id=""_External_links__6""></a><h1 class=""separator""> External links </h1>
<code>Wiktionary|A|a</code>
<ul><li><a class=""externallink"" href=""http://members.peak.org/~jeremy/dictionaryclassic/chapters/pix/alphabet.gif "" title="""" target=""_blank"">History of the Alphabet</a></li><li><code>Wikisource-inline|list=<ul><li>“<a class=""pagelink"" href=""s:A Dictionary of the English Language/A"" title=""A"">A</a>” in <i><a class=""pagelink"" href=""s:A Dictionary of the English Language"" title=""A Dictionary of the English Language"">A Dictionary of the English Language</a></i> by <a class=""pagelink"" href=""Samuel Johnson"" title=""Samuel Johnson"">Samuel Johnson</a></li><li>Chisholm, Hugh, ed. (1911). <a class=""pagelink"" href=""wikisource:1911 Encyclopadia Britannica/A"" title=""""A"""">""A""</a> (entry). <i><a class=""pagelink"" href=""Encyclopadia Britannica"" title=""Encyclopadia Britannica"">Encyclopadia Britannica</a></i> (Eleventh ed.). Cambridge University Press.</li><li>""<a class=""pagelink"" href=""wikisource:The New Student's Reference Work/A"" title=""A"">A</a>"". <i>The New Student's Reference Work</i>. Chicago: F. E. Compton and Co. 1914.</li><li>""<a class=""pagelink"" href=""wikisource:Collier's New Encyclopedia (1921)/A"" title=""A"">A</a>"". <i><a class=""pagelink"" href=""Collier's Encyclopedia"" title=""Collier's New Encyclopedia"">Collier's New Encyclopedia</a></i>. 1921.</li></ul></li></ul>
</code><br /><br /><code>Latin alphabet|A|</code><br /><br /><a class=""pagelink"" href=""Category:ISO basic Latin letters"" title=""Category:ISO basic Latin letters"">Category:ISO basic Latin letters</a>
<a class=""pagelink"" href=""Category:Vowel letters"" title=""Category:Vowel letters"">Category:Vowel letters</a><br /><br /><code>Link GA|th</code><br /><br /><a class=""pagelink"" href=""ace:A"" title=""ace:A"">ace:A</a>
<a class=""pagelink"" href=""af:A"" title=""af:A"">af:A</a>
<a class=""pagelink"" href=""als:A"" title=""als:A"">als:A</a>
<a class=""pagelink"" href=""ar:A"" title=""ar:A"">ar:A</a>
<a class=""pagelink"" href=""an:A"" title=""an:A"">an:A</a>
<a class=""pagelink"" href=""arc:A"" title=""arc:A"">arc:A</a>
<a class=""pagelink"" href=""ast:A"" title=""ast:A"">ast:A</a>
<a class=""pagelink"" href=""az:A"" title=""az:A"">az:A</a>
<a class=""pagelink"" href=""zh-min-nan:A"" title=""zh-min-nan:A"">zh-min-nan:A</a>
<a class=""pagelink"" href=""ba:A (латин х?рефе)"" title=""ba:A (латин х?рефе)"">ba:A (латин х?рефе)</a>
<a class=""pagelink"" href=""be:A, л?тара"" title=""be:A, л?тара"">be:A, л?тара</a>
<a class=""pagelink"" href=""be-x-old:A (л?тара)"" title=""be-x-old:A (л?тара)"">be-x-old:A (л?тара)</a>
<a class=""pagelink"" href=""bar:A"" title=""bar:A"">bar:A</a>
<a class=""pagelink"" href=""bs:A"" title=""bs:A"">bs:A</a>
<a class=""pagelink"" href=""br:A (lizherenn)"" title=""br:A (lizherenn)"">br:A (lizherenn)</a>
<a class=""pagelink"" href=""bg:A"" title=""bg:A"">bg:A</a>
<a class=""pagelink"" href=""ca:A"" title=""ca:A"">ca:A</a>
<a class=""pagelink"" href=""cs:A"" title=""cs:A"">cs:A</a>
<a class=""pagelink"" href=""tum:A"" title=""tum:A"">tum:A</a>
<a class=""pagelink"" href=""co:A"" title=""co:A"">co:A</a>
<a class=""pagelink"" href=""cy:A"" title=""cy:A"">cy:A</a>
<a class=""pagelink"" href=""da:A"" title=""da:A"">da:A</a>
<a class=""pagelink"" href=""de:A"" title=""de:A"">de:A</a>
<a class=""pagelink"" href=""dv:????"" title=""dv:????"">dv:????</a>
<a class=""pagelink"" href=""et:A"" title=""et:A"">et:A</a>
<a class=""pagelink"" href=""el:A"" title=""el:A"">el:A</a>
<a class=""pagelink"" href=""eml:A"" title=""eml:A"">eml:A</a>
<a class=""pagelink"" href=""es:A"" title=""es:A"">es:A</a>
<a class=""pagelink"" href=""eo:A"" title=""eo:A"">eo:A</a>
<a class=""pagelink"" href=""eu:A"" title=""eu:A"">eu:A</a>
<a class=""pagelink"" href=""fa:A"" title=""fa:A"">fa:A</a>
<a class=""pagelink"" href=""fr:A (lettre)"" title=""fr:A (lettre)"">fr:A (lettre)</a>
<a class=""pagelink"" href=""fy:A"" title=""fy:A"">fy:A</a>
<a class=""pagelink"" href=""fur:A"" title=""fur:A"">fur:A</a>
<a class=""pagelink"" href=""gv:Aittyn"" title=""gv:Aittyn"">gv:Aittyn</a>
<a class=""pagelink"" href=""gd:A"" title=""gd:A"">gd:A</a>
<a class=""pagelink"" href=""gl:A"" title=""gl:A"">gl:A</a>
<a class=""pagelink"" href=""gan:A"" title=""gan:A"">gan:A</a>
<a class=""pagelink"" href=""xal:A ?зг"" title=""xal:A ?зг"">xal:A ?зг</a>
<a class=""pagelink"" href=""ko:A"" title=""ko:A"">ko:A</a>
<a class=""pagelink"" href=""hr:A"" title=""hr:A"">hr:A</a>
<a class=""pagelink"" href=""io:A"" title=""io:A"">io:A</a>
<a class=""pagelink"" href=""ilo:A"" title=""ilo:A"">ilo:A</a>
<a class=""pagelink"" href=""id:A"" title=""id:A"">id:A</a>
<a class=""pagelink"" href=""ia:A"" title=""ia:A"">ia:A</a>
<a class=""pagelink"" href=""is:A"" title=""is:A"">is:A</a>
<a class=""pagelink"" href=""it:A"" title=""it:A"">it:A</a>
<a class=""pagelink"" href=""he:A"" title=""he:A"">he:A</a>
<a class=""pagelink"" href=""jv:A"" title=""jv:A"">jv:A</a>
<a class=""pagelink"" href=""ka:A"" title=""ka:A"">ka:A</a>
<a class=""pagelink"" href=""kw:A"" title=""kw:A"">kw:A</a>
<a class=""pagelink"" href=""sw:A"" title=""sw:A"">sw:A</a>
<a class=""pagelink"" href=""ht:A"" title=""ht:A"">ht:A</a>
<a class=""pagelink"" href=""ku:A (tip)"" title=""ku:A (tip)"">ku:A (tip)</a>
<a class=""pagelink"" href=""la:A"" title=""la:A"">la:A</a>
<a class=""pagelink"" href=""lv:A"" title=""lv:A"">lv:A</a>
<a class=""pagelink"" href=""lb:A (Buschtaf)"" title=""lb:A (Buschtaf)"">lb:A (Buschtaf)</a>
<a class=""pagelink"" href=""lt:A"" title=""lt:A"">lt:A</a>
<a class=""pagelink"" href=""lmo:A"" title=""lmo:A"">lmo:A</a>
<a class=""pagelink"" href=""hu:A"" title=""hu:A"">hu:A</a>
<a class=""pagelink"" href=""mk:A (Латиница)"" title=""mk:A (Латиница)"">mk:A (Латиница)</a>
<a class=""pagelink"" href=""mg:A"" title=""mg:A"">mg:A</a>
<a class=""pagelink"" href=""ml:A"" title=""ml:A"">ml:A</a>
<a class=""pagelink"" href=""mr:A"" title=""mr:A"">mr:A</a>
<a class=""pagelink"" href=""mzn:A"" title=""mzn:A"">mzn:A</a>
<a class=""pagelink"" href=""ms:A"" title=""ms:A"">ms:A</a>
<a class=""pagelink"" href=""my:A"" title=""my:A"">my:A</a>
<a class=""pagelink"" href=""nah:A"" title=""nah:A"">nah:A</a>
<a class=""pagelink"" href=""nl:A (letter)"" title=""nl:A (letter)"">nl:A (letter)</a>
<a class=""pagelink"" href=""ja:A"" title=""ja:A"">ja:A</a>
<a class=""pagelink"" href=""nap:A"" title=""nap:A"">nap:A</a>
<a class=""pagelink"" href=""no:A"" title=""no:A"">no:A</a>
<a class=""pagelink"" href=""nn:A"" title=""nn:A"">nn:A</a>
<a class=""pagelink"" href=""nrm:A"" title=""nrm:A"">nrm:A</a>
<a class=""pagelink"" href=""uz:A (harf)"" title=""uz:A (harf)"">uz:A (harf)</a>
<a class=""pagelink"" href=""pl:A"" title=""pl:A"">pl:A</a>
<a class=""pagelink"" href=""pt:A"" title=""pt:A"">pt:A</a>
<a class=""pagelink"" href=""crh:A"" title=""crh:A"">crh:A</a>
<a class=""pagelink"" href=""ro:A"" title=""ro:A"">ro:A</a>
<a class=""pagelink"" href=""qu:A"" title=""qu:A"">qu:A</a>
<a class=""pagelink"" href=""ru:A (латиница)"" title=""ru:A (латиница)"">ru:A (латиница)</a>
<a class=""pagelink"" href=""se:A"" title=""se:A"">se:A</a>
<a class=""pagelink"" href=""stq:A"" title=""stq:A"">stq:A</a>
<a class=""pagelink"" href=""scn:A"" title=""scn:A"">scn:A</a>
<a class=""pagelink"" href=""simple:A"" title=""simple:A"">simple:A</a>
<a class=""pagelink"" href=""sk:A"" title=""sk:A"">sk:A</a>
<a class=""pagelink"" href=""sl:A"" title=""sl:A"">sl:A</a>
<a class=""pagelink"" href=""szl:A"" title=""szl:A"">szl:A</a>
<a class=""pagelink"" href=""srn:A"" title=""srn:A"">srn:A</a>
<a class=""pagelink"" href=""sr:A (слово латинице)"" title=""sr:A (слово латинице)"">sr:A (слово латинице)</a>
<a class=""pagelink"" href=""sh:A"" title=""sh:A"">sh:A</a>
<a class=""pagelink"" href=""su:A"" title=""su:A"">su:A</a>
<a class=""pagelink"" href=""fi:A"" title=""fi:A"">fi:A</a>
<a class=""pagelink"" href=""sv:A"" title=""sv:A"">sv:A</a>
<a class=""pagelink"" href=""tl:A"" title=""tl:A"">tl:A</a>
<a class=""pagelink"" href=""th:A"" title=""th:A"">th:A</a>
<a class=""pagelink"" href=""tr:A (harf)"" title=""tr:A (harf)"">tr:A (harf)</a>
<a class=""pagelink"" href=""tk:A"" title=""tk:A"">tk:A</a>
<a class=""pagelink"" href=""uk:A (латиниця)"" title=""uk:A (латиниця)"">uk:A (латиниця)</a>
<a class=""pagelink"" href=""vi:A"" title=""vi:A"">vi:A</a>
<a class=""pagelink"" href=""vo:A"" title=""vo:A"">vo:A</a>
<a class=""pagelink"" href=""war:A"" title=""war:A"">war:A</a>
<a class=""pagelink"" href=""yi:A"" title=""yi:A"">yi:A</a>
<a class=""pagelink"" href=""yo:A"" title=""yo:A"">yo:A</a>
<a class=""pagelink"" href=""zh-yue:A"" title=""zh-yue:A"">zh-yue:A</a>
<a class=""pagelink"" href=""diq:A"" title=""diq:A"">diq:A</a>
<a class=""pagelink"" href=""bat-smg:A"" title=""bat-smg:A"">bat-smg:A</a>
<a class=""pagelink"" href=""zh:A"" title=""zh:A"">zh:A</a>
</body></html>";
#endif
#if false
        // header with close TD
        // until cite
        public const string _content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body><code>Dablink|Due to <a class=""pagelink"" href=""Wikipedia:Naming conventions (technical restrictions)#Forbidden characters"" title=""technical restrictions"">technical restrictions</a>, A# redirects here. For other uses, see <a class=""pagelink"" href=""A-sharp (disambiguation)"" title=""A-sharp (disambiguation)"">A-sharp (disambiguation)</a>.</code>
<code>pp-move-indef</code>
<code>Two other uses|the letter|the indefinite article|A and an</code>
<code>Latin alphabet navbox|uc=A|lc=a</code>
<b>A</b> (<code>IPAc-en|En-us-A.ogg|e?</code>; <a class=""pagelink"" href=""English_alphabet#Letter_names"" title=""named"">named</a> <i>a</i>, plural <i>aes</i>)<ref name=""OED""/> is the first <a class=""pagelink"" href=""Letter (alphabet)"" title=""letter"">letter</a> and a <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a> in the <a class=""pagelink"" href=""basic modern Latin alphabet"" title=""basic modern Latin alphabet"">basic modern Latin alphabet</a>. It is similar to the Ancient Greek letter <a class=""pagelink"" href=""Alpha"" title=""Alpha"">Alpha</a>, from which it derives.<br /><br /><a id=""Origins_0""></a><h1 class=""separator"">Origins</h1>
""A"" may have started as a <a class=""pagelink"" href=""pictogram"" title=""pictogram"">pictogram</a> of an <a class=""pagelink"" href=""ox"" title=""ox"">ox</a> head in <a class=""pagelink"" href=""Egyptian hieroglyph"" title=""Egyptian hieroglyph"">Egyptian hieroglyph</a>. It has stood at the head of every alphabet in which it has been found, the earliest of which being the Phoenician.<ref name=""Britannica""/><br /><br /><table class=""wikitable""><tr  style=""background-color:#EEEEEE; text-align:center;""><th> Egyptian</th><th> Phoenician <br /><i><a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a></i></th><th> Greek <br /><i><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></i></th><th> Etruscan <br />A</th><th> Roman/Cyrillic <br />A</th></tr><tr  style=""background-color:white; text-align:center;""><td> <a class=""pagelink"" href=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"" title=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"">File:EgyptianA-01.svg>Egyptian hieroglyphic ox head</a></td><td> <a class=""pagelink"" href=""File:PhoenicianA-01.svg>Phoenician aleph"" title=""File:PhoenicianA-01.svg>Phoenician aleph"">File:PhoenicianA-01.svg>Phoenician aleph</a></td><td> <a class=""pagelink"" href=""File:Alpha uc lc.svg>65px"" title=""Greek alpha"">Greek alpha</a></td><td> <a class=""pagelink"" href=""File:EtruscanA.svg>Etruscan A"" title=""File:EtruscanA.svg>Etruscan A"">File:EtruscanA.svg>Etruscan A</a></td><td> <a class=""pagelink"" href=""File:RomanA-01.svg>Roman A"" title=""File:RomanA-01.svg>Roman A"">File:RomanA-01.svg>Roman A</a></td></tr></table><br />In 1600 B.C., the <a class=""pagelink"" href=""Phoenician alphabet"" title=""Phoenician alphabet"">Phoenician alphabet</a>'s letter had a linear form that served as the base for some later forms. Its name must have corresponded closely to the <a class=""pagelink"" href=""Hebrew alphabet"" title=""Hebrew"">Hebrew</a> or <a class=""pagelink"" href=""Arabic alphabet"" title=""Arabic"">Arabic</a> <a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a>.<br /><br /><table cellspacing=""10"" cellpadding=""0"" style=""background-color: white; float: right;""><tr  align=""center""><td> <a class=""pagelink"" href=""File:BlackletterA-01.png>Blackletter A"" title=""File:BlackletterA-01.png>Blackletter A"">File:BlackletterA-01.png>Blackletter A</a><br /><a class=""pagelink"" href=""Blackletter"" title=""Blackletter"">Blackletter</a> A</td><td> <a class=""pagelink"" href=""File:UncialA-01.svg>Uncial A"" title=""File:UncialA-01.svg>Uncial A"">File:UncialA-01.svg>Uncial A</a><br /><a class=""pagelink"" href=""Uncial"" title=""Uncial"">Uncial</a> A</td><td> <a class=""pagelink"" href=""File:Acap.svg>Another Capital A"" title=""File:Acap.svg>Another Capital A"">File:Acap.svg>Another Capital A</a><br />Another Blackletter A&nbsp;</td></tr><tr  align=""center""><td> <a class=""pagelink"" href=""File:ModernRomanA-01.svg>64 px"" title=""Modern Roman A"">Modern Roman A</a><br />Modern Roman A</td><td> <a class=""pagelink"" href=""File:Modern Italic A.svg>64 px"" title=""Modern Italic A"">Modern Italic A</a><br />Modern Italic A</td><td> <a class=""pagelink"" href=""File:Modern Script A.svg>64 px"" title=""Modern Script A"">Modern Script A</a><br />Modern Script A</td></tr></table>
When the <a class=""pagelink"" href=""Ancient Greece"" title=""Ancient Greeks"">Ancient Greeks</a> adopted the alphabet, they had no use for the <a class=""pagelink"" href=""glottal stop"" title=""glottal stop"">glottal stop</a> that the letter had denoted in <a class=""pagelink"" href=""Phoenician languages"" title=""Phoenician"">Phoenician</a> and other <a class=""pagelink"" href=""Semitic languages"" title=""Semitic languages"">Semitic languages</a>, so they used the sign to represent the vowel <code>IPA|/a/</code>, and kept its name with a minor change (<a class=""pagelink"" href=""alpha (letter)"" title=""alpha"">alpha</a>). In the earliest Greek inscriptions after the <a class=""pagelink"" href=""Greek Dark Ages"" title=""Greek Dark Ages"">Greek Dark Ages</a>, dating to the 8th century BC, the letter rests upon its side, but in the <a class=""pagelink"" href=""Greek alphabet"" title=""Greek alphabet"">Greek alphabet</a> of later times it generally resembles the modern capital letter, although many local varieties can be distinguished by the shortening of one leg, or by the angle at which the cross line is set.<br /><br />The <a class=""pagelink"" href=""Etruscans"" title=""Etruscans"">Etruscans</a> brought the Greek alphabet to their civilization in the <a class=""pagelink"" href=""Italian Peninsula"" title=""Italian Peninsula"">Italian Peninsula</a> and left the letter unchanged. The Romans later adopted the <a class=""pagelink"" href=""Old Italic alphabet"" title=""Etruscan alphabet"">Etruscan alphabet</a> to write the <a class=""pagelink"" href=""Latin language"" title=""Latin language"">Latin language</a>, and the resulting letter was preserved in the modern <a class=""pagelink"" href=""Latin alphabet"" title=""Latin alphabet"">Latin alphabet</a> used to write many languages, including <a class=""pagelink"" href=""English language"" title=""English"">English</a>.<br /><br />
The letter has two <a class=""pagelink"" href=""lower case"" title=""minuscule"">minuscule</a> (lower-case) forms. The form used in most current <a class=""pagelink"" href=""handwriting"" title=""handwriting"">handwriting</a> consists of a circle and vertical stroke (<code>Unicode|""?""</code>), called <a class=""pagelink"" href=""Latin alpha"" title=""Latin alpha"">Latin alpha</a> or ""script a"". This slowly developed from the fifth-century form resembling the Greek letter <a class=""pagelink"" href=""tau"" title=""tau"">tau</a> in the hands of dark-age Irish and English writers.<ref name=""Britannica""/> Most printed material uses a form consisting of a small loop with an arc over it (<code>Unicode|""a""</code>). Both derive from the <a class=""pagelink"" href=""majuscule"" title=""majuscule"">majuscule</a> (capital) form. In Greek handwriting, it was common to join the left leg and horizontal stroke into a single loop, as demonstrated by the Uncial version shown. Many fonts then made the right leg vertical. In some of these, the <a class=""pagelink"" href=""serif"" title=""serif"">serif</a> that began the right leg stroke developed into an arc, resulting in the printed form, while in others it was dropped, resulting in the modern handwritten form.<br /><br /><a id=""Usage_1""></a><h1 class=""separator"">Usage</h1>
<code>main|a (disambiguation)</code>
The letter A currently represents six different vowel sounds. In English, ""a"" by itself frequently denotes the <a class=""pagelink"" href=""near-open front unrounded vowel"" title=""near-open front unrounded vowel"">near-open front unrounded vowel</a> (<code>IPA|/a/</code>) as in <i>pad</i>; the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> (<code>IPA|/??/</code>) as in <i>father</i>, its original, Latin and Greek, sound; a closer, further fronted sound as in ""hare"", which developed as the sound progressed from ""father"" to ""ace"";<ref name=""Britannica""/> in concert with a later orthographic vowel, the diphthong <code>IPA|/e?/</code> as in <i>ace</i> and <i>major</i>, due to effects of the <a class=""pagelink"" href=""great vowel shift"" title=""great vowel shift"">great vowel shift</a>; the more rounded form in ""water"" or its closely-related cousin, found in ""was"".<ref name=""Britannica""/><br /><br />In most other languages that use the Latin alphabet, ""a"" denotes an <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a> (<code>IPA|/a/</code>). In the <a class=""pagelink"" href=""help:IPA"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a>, variants of ""a"" denote various <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a>s. In <a class=""pagelink"" href=""X-SAMPA"" title=""X-SAMPA"">X-SAMPA</a>, capital ""A"" denotes the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> and lowercase ""a"" denotes the <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a>.<br /><br />""A"" is the third most commonly used letter in English, and the second most common in <a class=""pagelink"" href=""Spanish language"" title=""Spanish"">Spanish</a> and <a class=""pagelink"" href=""French language"" title=""French"">French</a>. In one study, on average, about 3.68% of letters used in English tend to be ?a?s, while the number is 6.22% in Spanish and 3.95% in French.<ref name=""Trinity College"" /><br /><br />""A"" is often used to denote something or someone of a better or more prestigious quality or status: A-, A or A+, the best grade that can be assigned by teachers for students' schoolwork; A grade for clean restaurants; <a class=""pagelink"" href=""A-List"" title=""A-List"">A-List</a> celebrities, etc. Such associations can have a <a class=""pagelink"" href=""motivation"" title=""motivating"">motivating</a> effect as exposure to the letter A has been found to improve performance, when compared with other letters.<ref><code>Cite document |url=http://www.sciencealert.com.au/news/20100903-20689.html |title=Letters affect exam results |date=9 March 2010 |publisher=British Psychological Society |postscript=<!--None--></code></ref><br /><br />A <a class=""pagelink"" href=""turned a"" title=""turned ""a"""">turned ""a""</a>, <code>IPA|???</code> is used by the <a class=""pagelink"" href=""International Phonetic Alphabet"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a> for the <a class=""pagelink"" href=""near-open central vowel"" title=""near-open central vowel"">near-open central vowel</a>, while a turned capital ""A"" (""∀"") is used in <a class=""pagelink"" href=""predicate logic"" title=""predicate logic"">predicate logic</a> to specify <a class=""pagelink"" href=""universal quantification"" title=""universal quantification"">universal quantification</a>.<br /><br /><a id=""Computing_codes_2""></a><h1 class=""separator"">Computing codes</h1>
 of Unicode U+0061.]]<br /><br />In <a class=""pagelink"" href=""Unicode"" title=""Unicode"">Unicode</a>, the <a class=""pagelink"" href=""majuscule"" title=""capital"">capital</a> ""A"" is codepoint U+0041 and the <a class=""pagelink"" href=""lower case"" title=""lower case"">lower case</a> ""a"" is U+0061.<ref name=""unicode"" /><br />
The closed form (<code>Unicode|""?""</code>), which is related with the lowercase <a class=""pagelink"" href=""alpha"" title=""alpha"">alpha</a>, has codepoint U+0251 (from the Code Chart ""IPA Extensions"".<ref><a class=""externallink"" href=""http://www.unicode.org/charts/PDF/U0250.pdf "" title="""" target=""_blank"">Unicode.org</a></ref><br /><br />The <a class=""pagelink"" href=""ASCII"" title=""ASCII"">ASCII</a> code for capital ""A"" is 65 and for lower case ""a"" is 97; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 01000001 and 01100001, respectively.<br /><br />The <a class=""pagelink"" href=""EBCDIC"" title=""EBCDIC"">EBCDIC</a> code for capital ""A"" is 193 and for lowercase ""a"" is 129; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 11000001 and 10000001, respectively.<br /><br />The <a class=""pagelink"" href=""numeric character reference"" title=""numeric character reference"">numeric character reference</a>s in <a class=""pagelink"" href=""HTML"" title=""HTML"">HTML</a> and <a class=""pagelink"" href=""XML"" title=""XML"">XML</a> are ""<tt>&amp;#65;</tt>"" and ""<tt>&amp;#97;</tt>"" for upper and lower case, respectively.<br /><br /><a id=""Other_representations_3""></a><h1 class=""separator"">Other representations</h1>
<div style=""float:left"">
<code>Letter
|NATO=Alpha<!--don't change to official ""alfa"" until Commons images are moved to this spelling, or redirects are set up, as otherwise the table does not display the semaphore and flag images-->
|Morse=・?
|Character=A1
|Braille=??
</code>
</div>
{<div style=""clear: both;""></div>}<br /><br /><a id=""See_also_4""></a><h1 class=""separator"">See also</h1>
<code>Commons|A</code>
<ul><li><big><a class=""pagelink"" href=""a"" title=""a"">a</a></big></li><li><a class=""pagelink"" href=""A"" title=""A"">A</a></li><li><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></li><li><a class=""pagelink"" href=""A (Cyrillic)"" title=""Cyrillic A"">Cyrillic A</a></li></ul><br /><a id=""References_5""></a><h1 class=""separator"">References</h1><br /><br /><code>Reflist|refs=
<ref name=""OED"">""A"" (word), <i>Oxford English Dictionary,</i> 2nd edition, 1989. <i>Aes</i> is the plural of the name of the letter. The plural of the letter itself is: <i>A</i>s, A's, <i>a</i>s, or a's.</ref><br /><br /><ref name=""Trinity College""><a class=""externallink"" href=""http://starbase.trincoll.edu/~crypto/resources/LetFreq.html "" title="""" target=""_blank"">""Percentages of Letter frequencies per Thousand words""</a>, Trinity College, Retrieved 2006-05-01.</ref><br /><br /><ref name=""unicode""><a class=""externallink"" href=""http://macchiato.com/unicode/chart/ "" title="""" target=""_blank"">""Javascript Unicode Chart""</a>, macchiato.com, 2009, Retrieved 2009-03-08.</ref><br /><br /><ref name=""Britannica"">""A"", ""Encyclopaedia Britannica"", Volume 1, 1962. p.1.</ref> 
</code><br /><br /><a id=""_External_links__6""></a><h1 class=""separator""> External links </h1>
<code>Wiktionary|A|a</code>
<ul><li><a class=""externallink"" href=""http://members.peak.org/~jeremy/dictionaryclassic/chapters/pix/alphabet.gif "" title="""" target=""_blank"">History of the Alphabet</a></li><li><code>Wikisource-inline|list=<ul><li>“<a class=""pagelink"" href=""s:A Dictionary of the English Language/A"" title=""A"">A</a>” in <i><a class=""pagelink"" href=""s:A Dictionary of the English Language"" title=""A Dictionary of the English Language"">A Dictionary of the English Language</a></i> by <a class=""pagelink"" href=""Samuel Johnson"" title=""Samuel Johnson"">Samuel Johnson</a></li><li>Chisholm, Hugh, ed. (1911). <a class=""pagelink"" href=""wikisource:1911 Encyclopadia Britannica/A"" title=""""A"""">""A""</a> (entry). <i><a class=""pagelink"" href=""Encyclopadia Britannica"" title=""Encyclopadia Britannica"">Encyclopadia Britannica</a></i> (Eleventh ed.). Cambridge University Press.</li><li>""<a class=""pagelink"" href=""wikisource:The New Student's Reference Work/A"" title=""A"">A</a>"". <i>The New Student's Reference Work</i>. Chicago: F. E. Compton and Co. 1914.</li><li>""<a class=""pagelink"" href=""wikisource:Collier's New Encyclopedia (1921)/A"" title=""A"">A</a>"". <i><a class=""pagelink"" href=""Collier's Encyclopedia"" title=""Collier's New Encyclopedia"">Collier's New Encyclopedia</a></i>. 1921.</li></ul></li></ul>
</code><br /><br /><code>Latin alphabet|A|</code><br /><br /><a class=""pagelink"" href=""Category:ISO basic Latin letters"" title=""Category:ISO basic Latin letters"">Category:ISO basic Latin letters</a>
<a class=""pagelink"" href=""Category:Vowel letters"" title=""Category:Vowel letters"">Category:Vowel letters</a><br /><br /><code>Link GA|th</code><br /><br /><a class=""pagelink"" href=""ace:A"" title=""ace:A"">ace:A</a>
<a class=""pagelink"" href=""af:A"" title=""af:A"">af:A</a>
<a class=""pagelink"" href=""als:A"" title=""als:A"">als:A</a>
<a class=""pagelink"" href=""ar:A"" title=""ar:A"">ar:A</a>
<a class=""pagelink"" href=""an:A"" title=""an:A"">an:A</a>
<a class=""pagelink"" href=""arc:A"" title=""arc:A"">arc:A</a>
<a class=""pagelink"" href=""ast:A"" title=""ast:A"">ast:A</a>
<a class=""pagelink"" href=""az:A"" title=""az:A"">az:A</a>
<a class=""pagelink"" href=""zh-min-nan:A"" title=""zh-min-nan:A"">zh-min-nan:A</a>
<a class=""pagelink"" href=""ba:A (латин х?рефе)"" title=""ba:A (латин х?рефе)"">ba:A (латин х?рефе)</a>
<a class=""pagelink"" href=""be:A, л?тара"" title=""be:A, л?тара"">be:A, л?тара</a>
<a class=""pagelink"" href=""be-x-old:A (л?тара)"" title=""be-x-old:A (л?тара)"">be-x-old:A (л?тара)</a>
<a class=""pagelink"" href=""bar:A"" title=""bar:A"">bar:A</a>
<a class=""pagelink"" href=""bs:A"" title=""bs:A"">bs:A</a>
<a class=""pagelink"" href=""br:A (lizherenn)"" title=""br:A (lizherenn)"">br:A (lizherenn)</a>
<a class=""pagelink"" href=""bg:A"" title=""bg:A"">bg:A</a>
<a class=""pagelink"" href=""ca:A"" title=""ca:A"">ca:A</a>
<a class=""pagelink"" href=""cs:A"" title=""cs:A"">cs:A</a>
<a class=""pagelink"" href=""tum:A"" title=""tum:A"">tum:A</a>
<a class=""pagelink"" href=""co:A"" title=""co:A"">co:A</a>
<a class=""pagelink"" href=""cy:A"" title=""cy:A"">cy:A</a>
<a class=""pagelink"" href=""da:A"" title=""da:A"">da:A</a>
<a class=""pagelink"" href=""de:A"" title=""de:A"">de:A</a>
<a class=""pagelink"" href=""dv:????"" title=""dv:????"">dv:????</a>
<a class=""pagelink"" href=""et:A"" title=""et:A"">et:A</a>
<a class=""pagelink"" href=""el:A"" title=""el:A"">el:A</a>
<a class=""pagelink"" href=""eml:A"" title=""eml:A"">eml:A</a>
<a class=""pagelink"" href=""es:A"" title=""es:A"">es:A</a>
<a class=""pagelink"" href=""eo:A"" title=""eo:A"">eo:A</a>
<a class=""pagelink"" href=""eu:A"" title=""eu:A"">eu:A</a>
<a class=""pagelink"" href=""fa:A"" title=""fa:A"">fa:A</a>
<a class=""pagelink"" href=""fr:A (lettre)"" title=""fr:A (lettre)"">fr:A (lettre)</a>
<a class=""pagelink"" href=""fy:A"" title=""fy:A"">fy:A</a>
<a class=""pagelink"" href=""fur:A"" title=""fur:A"">fur:A</a>
<a class=""pagelink"" href=""gv:Aittyn"" title=""gv:Aittyn"">gv:Aittyn</a>
<a class=""pagelink"" href=""gd:A"" title=""gd:A"">gd:A</a>
<a class=""pagelink"" href=""gl:A"" title=""gl:A"">gl:A</a>
<a class=""pagelink"" href=""gan:A"" title=""gan:A"">gan:A</a>
<a class=""pagelink"" href=""xal:A ?зг"" title=""xal:A ?зг"">xal:A ?зг</a>
<a class=""pagelink"" href=""ko:A"" title=""ko:A"">ko:A</a>
<a class=""pagelink"" href=""hr:A"" title=""hr:A"">hr:A</a>
<a class=""pagelink"" href=""io:A"" title=""io:A"">io:A</a>
<a class=""pagelink"" href=""ilo:A"" title=""ilo:A"">ilo:A</a>
<a class=""pagelink"" href=""id:A"" title=""id:A"">id:A</a>
<a class=""pagelink"" href=""ia:A"" title=""ia:A"">ia:A</a>
<a class=""pagelink"" href=""is:A"" title=""is:A"">is:A</a>
<a class=""pagelink"" href=""it:A"" title=""it:A"">it:A</a>
<a class=""pagelink"" href=""he:A"" title=""he:A"">he:A</a>
<a class=""pagelink"" href=""jv:A"" title=""jv:A"">jv:A</a>
<a class=""pagelink"" href=""ka:A"" title=""ka:A"">ka:A</a>
<a class=""pagelink"" href=""kw:A"" title=""kw:A"">kw:A</a>
<a class=""pagelink"" href=""sw:A"" title=""sw:A"">sw:A</a>
<a class=""pagelink"" href=""ht:A"" title=""ht:A"">ht:A</a>
<a class=""pagelink"" href=""ku:A (tip)"" title=""ku:A (tip)"">ku:A (tip)</a>
<a class=""pagelink"" href=""la:A"" title=""la:A"">la:A</a>
<a class=""pagelink"" href=""lv:A"" title=""lv:A"">lv:A</a>
<a class=""pagelink"" href=""lb:A (Buschtaf)"" title=""lb:A (Buschtaf)"">lb:A (Buschtaf)</a>
<a class=""pagelink"" href=""lt:A"" title=""lt:A"">lt:A</a>
<a class=""pagelink"" href=""lmo:A"" title=""lmo:A"">lmo:A</a>
<a class=""pagelink"" href=""hu:A"" title=""hu:A"">hu:A</a>
<a class=""pagelink"" href=""mk:A (Латиница)"" title=""mk:A (Латиница)"">mk:A (Латиница)</a>
<a class=""pagelink"" href=""mg:A"" title=""mg:A"">mg:A</a>
<a class=""pagelink"" href=""ml:A"" title=""ml:A"">ml:A</a>
<a class=""pagelink"" href=""mr:A"" title=""mr:A"">mr:A</a>
<a class=""pagelink"" href=""mzn:A"" title=""mzn:A"">mzn:A</a>
<a class=""pagelink"" href=""ms:A"" title=""ms:A"">ms:A</a>
<a class=""pagelink"" href=""my:A"" title=""my:A"">my:A</a>
<a class=""pagelink"" href=""nah:A"" title=""nah:A"">nah:A</a>
<a class=""pagelink"" href=""nl:A (letter)"" title=""nl:A (letter)"">nl:A (letter)</a>
<a class=""pagelink"" href=""ja:A"" title=""ja:A"">ja:A</a>
<a class=""pagelink"" href=""nap:A"" title=""nap:A"">nap:A</a>
<a class=""pagelink"" href=""no:A"" title=""no:A"">no:A</a>
<a class=""pagelink"" href=""nn:A"" title=""nn:A"">nn:A</a>
<a class=""pagelink"" href=""nrm:A"" title=""nrm:A"">nrm:A</a>
<a class=""pagelink"" href=""uz:A (harf)"" title=""uz:A (harf)"">uz:A (harf)</a>
<a class=""pagelink"" href=""pl:A"" title=""pl:A"">pl:A</a>
<a class=""pagelink"" href=""pt:A"" title=""pt:A"">pt:A</a>
<a class=""pagelink"" href=""crh:A"" title=""crh:A"">crh:A</a>
<a class=""pagelink"" href=""ro:A"" title=""ro:A"">ro:A</a>
<a class=""pagelink"" href=""qu:A"" title=""qu:A"">qu:A</a>
<a class=""pagelink"" href=""ru:A (латиница)"" title=""ru:A (латиница)"">ru:A (латиница)</a>
<a class=""pagelink"" href=""se:A"" title=""se:A"">se:A</a>
<a class=""pagelink"" href=""stq:A"" title=""stq:A"">stq:A</a>
<a class=""pagelink"" href=""scn:A"" title=""scn:A"">scn:A</a>
<a class=""pagelink"" href=""simple:A"" title=""simple:A"">simple:A</a>
<a class=""pagelink"" href=""sk:A"" title=""sk:A"">sk:A</a>
<a class=""pagelink"" href=""sl:A"" title=""sl:A"">sl:A</a>
<a class=""pagelink"" href=""szl:A"" title=""szl:A"">szl:A</a>
<a class=""pagelink"" href=""srn:A"" title=""srn:A"">srn:A</a>
<a class=""pagelink"" href=""sr:A (слово латинице)"" title=""sr:A (слово латинице)"">sr:A (слово латинице)</a>
<a class=""pagelink"" href=""sh:A"" title=""sh:A"">sh:A</a>
<a class=""pagelink"" href=""su:A"" title=""su:A"">su:A</a>
<a class=""pagelink"" href=""fi:A"" title=""fi:A"">fi:A</a>
<a class=""pagelink"" href=""sv:A"" title=""sv:A"">sv:A</a>
<a class=""pagelink"" href=""tl:A"" title=""tl:A"">tl:A</a>
<a class=""pagelink"" href=""th:A"" title=""th:A"">th:A</a>
<a class=""pagelink"" href=""tr:A (harf)"" title=""tr:A (harf)"">tr:A (harf)</a>
<a class=""pagelink"" href=""tk:A"" title=""tk:A"">tk:A</a>
<a class=""pagelink"" href=""uk:A (латиниця)"" title=""uk:A (латиниця)"">uk:A (латиниця)</a>
<a class=""pagelink"" href=""vi:A"" title=""vi:A"">vi:A</a>
<a class=""pagelink"" href=""vo:A"" title=""vo:A"">vo:A</a>
<a class=""pagelink"" href=""war:A"" title=""war:A"">war:A</a>
<a class=""pagelink"" href=""yi:A"" title=""yi:A"">yi:A</a>
<a class=""pagelink"" href=""yo:A"" title=""yo:A"">yo:A</a>
<a class=""pagelink"" href=""zh-yue:A"" title=""zh-yue:A"">zh-yue:A</a>
<a class=""pagelink"" href=""diq:A"" title=""diq:A"">diq:A</a>
<a class=""pagelink"" href=""bat-smg:A"" title=""bat-smg:A"">bat-smg:A</a>
<a class=""pagelink"" href=""zh:A"" title=""zh:A"">zh:A</a>
</body></html>";
#endif
#if true
        // header with close TD with turn a double quote fix.
        // until "while a turned Capital "A" ("
        public const string _content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/><title>A</title></head><body><code>Dablink|Due to <a class=""pagelink"" href=""Wikipedia:Naming conventions (technical restrictions)#Forbidden characters"" title=""technical restrictions"">technical restrictions</a>, A# redirects here. For other uses, see <a class=""pagelink"" href=""A-sharp (disambiguation)"" title=""A-sharp (disambiguation)"">A-sharp (disambiguation)</a>.</code>
<code>pp-move-indef</code>
<code>Two other uses|the letter|the indefinite article|A and an</code>
<code>Latin alphabet navbox|uc=A|lc=a</code>
<b>A</b> (<code>IPAc-en|En-us-A.ogg|e?</code>; <a class=""pagelink"" href=""English_alphabet#Letter_names"" title=""named"">named</a> <i>a</i>, plural <i>aes</i>)<ref name=""OED""/> is the first <a class=""pagelink"" href=""Letter (alphabet)"" title=""letter"">letter</a> and a <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a> in the <a class=""pagelink"" href=""basic modern Latin alphabet"" title=""basic modern Latin alphabet"">basic modern Latin alphabet</a>. It is similar to the Ancient Greek letter <a class=""pagelink"" href=""Alpha"" title=""Alpha"">Alpha</a>, from which it derives.<br /><br /><a id=""Origins_0""></a><h1 class=""separator"">Origins</h1>
""A"" may have started as a <a class=""pagelink"" href=""pictogram"" title=""pictogram"">pictogram</a> of an <a class=""pagelink"" href=""ox"" title=""ox"">ox</a> head in <a class=""pagelink"" href=""Egyptian hieroglyph"" title=""Egyptian hieroglyph"">Egyptian hieroglyph</a>. It has stood at the head of every alphabet in which it has been found, the earliest of which being the Phoenician.<ref name=""Britannica""/><br /><br /><table class=""wikitable""><tr  style=""background-color:#EEEEEE; text-align:center;""><th> Egyptian</th><th> Phoenician <br /><i><a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a></i></th><th> Greek <br /><i><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></i></th><th> Etruscan <br />A</th><th> Roman/Cyrillic <br />A</th></tr><tr  style=""background-color:white; text-align:center;""><td> <a class=""pagelink"" href=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"" title=""File:EgyptianA-01.svg>Egyptian hieroglyphic ox head"">File:EgyptianA-01.svg>Egyptian hieroglyphic ox head</a></td><td> <a class=""pagelink"" href=""File:PhoenicianA-01.svg>Phoenician aleph"" title=""File:PhoenicianA-01.svg>Phoenician aleph"">File:PhoenicianA-01.svg>Phoenician aleph</a></td><td> <a class=""pagelink"" href=""File:Alpha uc lc.svg>65px"" title=""Greek alpha"">Greek alpha</a></td><td> <a class=""pagelink"" href=""File:EtruscanA.svg>Etruscan A"" title=""File:EtruscanA.svg>Etruscan A"">File:EtruscanA.svg>Etruscan A</a></td><td> <a class=""pagelink"" href=""File:RomanA-01.svg>Roman A"" title=""File:RomanA-01.svg>Roman A"">File:RomanA-01.svg>Roman A</a></td></tr></table><br />In 1600 B.C., the <a class=""pagelink"" href=""Phoenician alphabet"" title=""Phoenician alphabet"">Phoenician alphabet</a>'s letter had a linear form that served as the base for some later forms. Its name must have corresponded closely to the <a class=""pagelink"" href=""Hebrew alphabet"" title=""Hebrew"">Hebrew</a> or <a class=""pagelink"" href=""Arabic alphabet"" title=""Arabic"">Arabic</a> <a class=""pagelink"" href=""aleph"" title=""aleph"">aleph</a>.<br /><br /><table cellspacing=""10"" cellpadding=""0"" style=""background-color: white; float: right;""><tr  align=""center""><td> <a class=""pagelink"" href=""File:BlackletterA-01.png>Blackletter A"" title=""File:BlackletterA-01.png>Blackletter A"">File:BlackletterA-01.png>Blackletter A</a><br /><a class=""pagelink"" href=""Blackletter"" title=""Blackletter"">Blackletter</a> A</td><td> <a class=""pagelink"" href=""File:UncialA-01.svg>Uncial A"" title=""File:UncialA-01.svg>Uncial A"">File:UncialA-01.svg>Uncial A</a><br /><a class=""pagelink"" href=""Uncial"" title=""Uncial"">Uncial</a> A</td><td> <a class=""pagelink"" href=""File:Acap.svg>Another Capital A"" title=""File:Acap.svg>Another Capital A"">File:Acap.svg>Another Capital A</a><br />Another Blackletter A&nbsp;</td></tr><tr  align=""center""><td> <a class=""pagelink"" href=""File:ModernRomanA-01.svg>64 px"" title=""Modern Roman A"">Modern Roman A</a><br />Modern Roman A</td><td> <a class=""pagelink"" href=""File:Modern Italic A.svg>64 px"" title=""Modern Italic A"">Modern Italic A</a><br />Modern Italic A</td><td> <a class=""pagelink"" href=""File:Modern Script A.svg>64 px"" title=""Modern Script A"">Modern Script A</a><br />Modern Script A</td></tr></table>
When the <a class=""pagelink"" href=""Ancient Greece"" title=""Ancient Greeks"">Ancient Greeks</a> adopted the alphabet, they had no use for the <a class=""pagelink"" href=""glottal stop"" title=""glottal stop"">glottal stop</a> that the letter had denoted in <a class=""pagelink"" href=""Phoenician languages"" title=""Phoenician"">Phoenician</a> and other <a class=""pagelink"" href=""Semitic languages"" title=""Semitic languages"">Semitic languages</a>, so they used the sign to represent the vowel <code>IPA|/a/</code>, and kept its name with a minor change (<a class=""pagelink"" href=""alpha (letter)"" title=""alpha"">alpha</a>). In the earliest Greek inscriptions after the <a class=""pagelink"" href=""Greek Dark Ages"" title=""Greek Dark Ages"">Greek Dark Ages</a>, dating to the 8th century BC, the letter rests upon its side, but in the <a class=""pagelink"" href=""Greek alphabet"" title=""Greek alphabet"">Greek alphabet</a> of later times it generally resembles the modern capital letter, although many local varieties can be distinguished by the shortening of one leg, or by the angle at which the cross line is set.<br /><br />The <a class=""pagelink"" href=""Etruscans"" title=""Etruscans"">Etruscans</a> brought the Greek alphabet to their civilization in the <a class=""pagelink"" href=""Italian Peninsula"" title=""Italian Peninsula"">Italian Peninsula</a> and left the letter unchanged. The Romans later adopted the <a class=""pagelink"" href=""Old Italic alphabet"" title=""Etruscan alphabet"">Etruscan alphabet</a> to write the <a class=""pagelink"" href=""Latin language"" title=""Latin language"">Latin language</a>, and the resulting letter was preserved in the modern <a class=""pagelink"" href=""Latin alphabet"" title=""Latin alphabet"">Latin alphabet</a> used to write many languages, including <a class=""pagelink"" href=""English language"" title=""English"">English</a>.<br /><br />
The letter has two <a class=""pagelink"" href=""lower case"" title=""minuscule"">minuscule</a> (lower-case) forms. The form used in most current <a class=""pagelink"" href=""handwriting"" title=""handwriting"">handwriting</a> consists of a circle and vertical stroke (<code>Unicode|""?""</code>), called <a class=""pagelink"" href=""Latin alpha"" title=""Latin alpha"">Latin alpha</a> or ""script a"". This slowly developed from the fifth-century form resembling the Greek letter <a class=""pagelink"" href=""tau"" title=""tau"">tau</a> in the hands of dark-age Irish and English writers.<ref name=""Britannica""/> Most printed material uses a form consisting of a small loop with an arc over it (<code>Unicode|""a""</code>). Both derive from the <a class=""pagelink"" href=""majuscule"" title=""majuscule"">majuscule</a> (capital) form. In Greek handwriting, it was common to join the left leg and horizontal stroke into a single loop, as demonstrated by the Uncial version shown. Many fonts then made the right leg vertical. In some of these, the <a class=""pagelink"" href=""serif"" title=""serif"">serif</a> that began the right leg stroke developed into an arc, resulting in the printed form, while in others it was dropped, resulting in the modern handwritten form.<br /><br /><a id=""Usage_1""></a><h1 class=""separator"">Usage</h1>
<code>main|a (disambiguation)</code>
The letter A currently represents six different vowel sounds. In English, ""a"" by itself frequently denotes the <a class=""pagelink"" href=""near-open front unrounded vowel"" title=""near-open front unrounded vowel"">near-open front unrounded vowel</a> (<code>IPA|/a/</code>) as in <i>pad</i>; the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> (<code>IPA|/??/</code>) as in <i>father</i>, its original, Latin and Greek, sound; a closer, further fronted sound as in ""hare"", which developed as the sound progressed from ""father"" to ""ace"";<ref name=""Britannica""/> in concert with a later orthographic vowel, the diphthong <code>IPA|/e?/</code> as in <i>ace</i> and <i>major</i>, due to effects of the <a class=""pagelink"" href=""great vowel shift"" title=""great vowel shift"">great vowel shift</a>; the more rounded form in ""water"" or its closely-related cousin, found in ""was"".<ref name=""Britannica""/><br /><br />In most other languages that use the Latin alphabet, ""a"" denotes an <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a> (<code>IPA|/a/</code>). In the <a class=""pagelink"" href=""help:IPA"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a>, variants of ""a"" denote various <a class=""pagelink"" href=""vowel"" title=""vowel"">vowel</a>s. In <a class=""pagelink"" href=""X-SAMPA"" title=""X-SAMPA"">X-SAMPA</a>, capital ""A"" denotes the <a class=""pagelink"" href=""open back unrounded vowel"" title=""open back unrounded vowel"">open back unrounded vowel</a> and lowercase ""a"" denotes the <a class=""pagelink"" href=""open front unrounded vowel"" title=""open front unrounded vowel"">open front unrounded vowel</a>.<br /><br />""A"" is the third most commonly used letter in English, and the second most common in <a class=""pagelink"" href=""Spanish language"" title=""Spanish"">Spanish</a> and <a class=""pagelink"" href=""French language"" title=""French"">French</a>. In one study, on average, about 3.68% of letters used in English tend to be ?a?s, while the number is 6.22% in Spanish and 3.95% in French.<ref name=""Trinity College"" /><br /><br />""A"" is often used to denote something or someone of a better or more prestigious quality or status: A-, A or A+, the best grade that can be assigned by teachers for students' schoolwork; A grade for clean restaurants; <a class=""pagelink"" href=""A-List"" title=""A-List"">A-List</a> celebrities, etc. Such associations can have a <a class=""pagelink"" href=""motivation"" title=""motivating"">motivating</a> effect as exposure to the letter A has been found to improve performance, when compared with other letters.<ref><code>Cite document |url=http://www.sciencealert.com.au/news/20100903-20689.html |title=Letters affect exam results |date=9 March 2010 |publisher=British Psychological Society |postscript=<!--None--></code></ref><br /><br />A <a class=""pagelink"" href=""turned a"" title=""turned &quote;a&quote;"">turned ""a""</a>, <code>IPA|???</code> is used by the <a class=""pagelink"" href=""International Phonetic Alphabet"" title=""International Phonetic Alphabet"">International Phonetic Alphabet</a> for the <a class=""pagelink"" href=""near-open central vowel"" title=""near-open central vowel"">near-open central vowel</a>, while a turned capital ""A"" (""∀"") is used in <a class=""pagelink"" href=""predicate logic"" title=""predicate logic"">predicate logic</a> to specify <a class=""pagelink"" href=""universal quantification"" title=""universal quantification"">universal quantification</a>.<br /><br /><a id=""Computing_codes_2""></a><h1 class=""separator"">Computing codes</h1>
 of Unicode U+0061.]]<br /><br />In <a class=""pagelink"" href=""Unicode"" title=""Unicode"">Unicode</a>, the <a class=""pagelink"" href=""majuscule"" title=""capital"">capital</a> ""A"" is codepoint U+0041 and the <a class=""pagelink"" href=""lower case"" title=""lower case"">lower case</a> ""a"" is U+0061.<ref name=""unicode"" /><br />
The closed form (<code>Unicode|""?""</code>), which is related with the lowercase <a class=""pagelink"" href=""alpha"" title=""alpha"">alpha</a>, has codepoint U+0251 (from the Code Chart ""IPA Extensions"".<ref><a class=""externallink"" href=""http://www.unicode.org/charts/PDF/U0250.pdf "" title="""" target=""_blank"">Unicode.org</a></ref><br /><br />The <a class=""pagelink"" href=""ASCII"" title=""ASCII"">ASCII</a> code for capital ""A"" is 65 and for lower case ""a"" is 97; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 01000001 and 01100001, respectively.<br /><br />The <a class=""pagelink"" href=""EBCDIC"" title=""EBCDIC"">EBCDIC</a> code for capital ""A"" is 193 and for lowercase ""a"" is 129; or in <a class=""pagelink"" href=""Binary numeral system"" title=""binary"">binary</a> 11000001 and 10000001, respectively.<br /><br />The <a class=""pagelink"" href=""numeric character reference"" title=""numeric character reference"">numeric character reference</a>s in <a class=""pagelink"" href=""HTML"" title=""HTML"">HTML</a> and <a class=""pagelink"" href=""XML"" title=""XML"">XML</a> are ""<tt>&amp;#65;</tt>"" and ""<tt>&amp;#97;</tt>"" for upper and lower case, respectively.<br /><br /><a id=""Other_representations_3""></a><h1 class=""separator"">Other representations</h1>
<div style=""float:left"">
<code>Letter
|NATO=Alpha<!--don't change to official ""alfa"" until Commons images are moved to this spelling, or redirects are set up, as otherwise the table does not display the semaphore and flag images-->
|Morse=・?
|Character=A1
|Braille=??
</code>
</div>
{<div style=""clear: both;""></div>}<br /><br /><a id=""See_also_4""></a><h1 class=""separator"">See also</h1>
<code>Commons|A</code>
<ul><li><big><a class=""pagelink"" href=""a"" title=""a"">a</a></big></li><li><a class=""pagelink"" href=""A"" title=""A"">A</a></li><li><a class=""pagelink"" href=""Alpha (letter)"" title=""Alpha"">Alpha</a></li><li><a class=""pagelink"" href=""A (Cyrillic)"" title=""Cyrillic A"">Cyrillic A</a></li></ul><br /><a id=""References_5""></a><h1 class=""separator"">References</h1><br /><br /><code>Reflist|refs=
<ref name=""OED"">""A"" (word), <i>Oxford English Dictionary,</i> 2nd edition, 1989. <i>Aes</i> is the plural of the name of the letter. The plural of the letter itself is: <i>A</i>s, A's, <i>a</i>s, or a's.</ref><br /><br /><ref name=""Trinity College""><a class=""externallink"" href=""http://starbase.trincoll.edu/~crypto/resources/LetFreq.html "" title="""" target=""_blank"">""Percentages of Letter frequencies per Thousand words""</a>, Trinity College, Retrieved 2006-05-01.</ref><br /><br /><ref name=""unicode""><a class=""externallink"" href=""http://macchiato.com/unicode/chart/ "" title="""" target=""_blank"">""Javascript Unicode Chart""</a>, macchiato.com, 2009, Retrieved 2009-03-08.</ref><br /><br /><ref name=""Britannica"">""A"", ""Encyclopaedia Britannica"", Volume 1, 1962. p.1.</ref> 
</code><br /><br /><a id=""_External_links__6""></a><h1 class=""separator""> External links </h1>
<code>Wiktionary|A|a</code>
<ul><li><a class=""externallink"" href=""http://members.peak.org/~jeremy/dictionaryclassic/chapters/pix/alphabet.gif "" title="""" target=""_blank"">History of the Alphabet</a></li><li><code>Wikisource-inline|list=<ul><li>“<a class=""pagelink"" href=""s:A Dictionary of the English Language/A"" title=""A"">A</a>” in <i><a class=""pagelink"" href=""s:A Dictionary of the English Language"" title=""A Dictionary of the English Language"">A Dictionary of the English Language</a></i> by <a class=""pagelink"" href=""Samuel Johnson"" title=""Samuel Johnson"">Samuel Johnson</a></li><li>Chisholm, Hugh, ed. (1911). <a class=""pagelink"" href=""wikisource:1911 Encyclopadia Britannica/A"" title=""""A"""">""A""</a> (entry). <i><a class=""pagelink"" href=""Encyclopadia Britannica"" title=""Encyclopadia Britannica"">Encyclopadia Britannica</a></i> (Eleventh ed.). Cambridge University Press.</li><li>""<a class=""pagelink"" href=""wikisource:The New Student's Reference Work/A"" title=""A"">A</a>"". <i>The New Student's Reference Work</i>. Chicago: F. E. Compton and Co. 1914.</li><li>""<a class=""pagelink"" href=""wikisource:Collier's New Encyclopedia (1921)/A"" title=""A"">A</a>"". <i><a class=""pagelink"" href=""Collier's Encyclopedia"" title=""Collier's New Encyclopedia"">Collier's New Encyclopedia</a></i>. 1921.</li></ul></li></ul>
</code><br /><br /><code>Latin alphabet|A|</code><br /><br /><a class=""pagelink"" href=""Category:ISO basic Latin letters"" title=""Category:ISO basic Latin letters"">Category:ISO basic Latin letters</a>
<a class=""pagelink"" href=""Category:Vowel letters"" title=""Category:Vowel letters"">Category:Vowel letters</a><br /><br /><code>Link GA|th</code><br /><br /><a class=""pagelink"" href=""ace:A"" title=""ace:A"">ace:A</a>
<a class=""pagelink"" href=""af:A"" title=""af:A"">af:A</a>
<a class=""pagelink"" href=""als:A"" title=""als:A"">als:A</a>
<a class=""pagelink"" href=""ar:A"" title=""ar:A"">ar:A</a>
<a class=""pagelink"" href=""an:A"" title=""an:A"">an:A</a>
<a class=""pagelink"" href=""arc:A"" title=""arc:A"">arc:A</a>
<a class=""pagelink"" href=""ast:A"" title=""ast:A"">ast:A</a>
<a class=""pagelink"" href=""az:A"" title=""az:A"">az:A</a>
<a class=""pagelink"" href=""zh-min-nan:A"" title=""zh-min-nan:A"">zh-min-nan:A</a>
<a class=""pagelink"" href=""ba:A (латин х?рефе)"" title=""ba:A (латин х?рефе)"">ba:A (латин х?рефе)</a>
<a class=""pagelink"" href=""be:A, л?тара"" title=""be:A, л?тара"">be:A, л?тара</a>
<a class=""pagelink"" href=""be-x-old:A (л?тара)"" title=""be-x-old:A (л?тара)"">be-x-old:A (л?тара)</a>
<a class=""pagelink"" href=""bar:A"" title=""bar:A"">bar:A</a>
<a class=""pagelink"" href=""bs:A"" title=""bs:A"">bs:A</a>
<a class=""pagelink"" href=""br:A (lizherenn)"" title=""br:A (lizherenn)"">br:A (lizherenn)</a>
<a class=""pagelink"" href=""bg:A"" title=""bg:A"">bg:A</a>
<a class=""pagelink"" href=""ca:A"" title=""ca:A"">ca:A</a>
<a class=""pagelink"" href=""cs:A"" title=""cs:A"">cs:A</a>
<a class=""pagelink"" href=""tum:A"" title=""tum:A"">tum:A</a>
<a class=""pagelink"" href=""co:A"" title=""co:A"">co:A</a>
<a class=""pagelink"" href=""cy:A"" title=""cy:A"">cy:A</a>
<a class=""pagelink"" href=""da:A"" title=""da:A"">da:A</a>
<a class=""pagelink"" href=""de:A"" title=""de:A"">de:A</a>
<a class=""pagelink"" href=""dv:????"" title=""dv:????"">dv:????</a>
<a class=""pagelink"" href=""et:A"" title=""et:A"">et:A</a>
<a class=""pagelink"" href=""el:A"" title=""el:A"">el:A</a>
<a class=""pagelink"" href=""eml:A"" title=""eml:A"">eml:A</a>
<a class=""pagelink"" href=""es:A"" title=""es:A"">es:A</a>
<a class=""pagelink"" href=""eo:A"" title=""eo:A"">eo:A</a>
<a class=""pagelink"" href=""eu:A"" title=""eu:A"">eu:A</a>
<a class=""pagelink"" href=""fa:A"" title=""fa:A"">fa:A</a>
<a class=""pagelink"" href=""fr:A (lettre)"" title=""fr:A (lettre)"">fr:A (lettre)</a>
<a class=""pagelink"" href=""fy:A"" title=""fy:A"">fy:A</a>
<a class=""pagelink"" href=""fur:A"" title=""fur:A"">fur:A</a>
<a class=""pagelink"" href=""gv:Aittyn"" title=""gv:Aittyn"">gv:Aittyn</a>
<a class=""pagelink"" href=""gd:A"" title=""gd:A"">gd:A</a>
<a class=""pagelink"" href=""gl:A"" title=""gl:A"">gl:A</a>
<a class=""pagelink"" href=""gan:A"" title=""gan:A"">gan:A</a>
<a class=""pagelink"" href=""xal:A ?зг"" title=""xal:A ?зг"">xal:A ?зг</a>
<a class=""pagelink"" href=""ko:A"" title=""ko:A"">ko:A</a>
<a class=""pagelink"" href=""hr:A"" title=""hr:A"">hr:A</a>
<a class=""pagelink"" href=""io:A"" title=""io:A"">io:A</a>
<a class=""pagelink"" href=""ilo:A"" title=""ilo:A"">ilo:A</a>
<a class=""pagelink"" href=""id:A"" title=""id:A"">id:A</a>
<a class=""pagelink"" href=""ia:A"" title=""ia:A"">ia:A</a>
<a class=""pagelink"" href=""is:A"" title=""is:A"">is:A</a>
<a class=""pagelink"" href=""it:A"" title=""it:A"">it:A</a>
<a class=""pagelink"" href=""he:A"" title=""he:A"">he:A</a>
<a class=""pagelink"" href=""jv:A"" title=""jv:A"">jv:A</a>
<a class=""pagelink"" href=""ka:A"" title=""ka:A"">ka:A</a>
<a class=""pagelink"" href=""kw:A"" title=""kw:A"">kw:A</a>
<a class=""pagelink"" href=""sw:A"" title=""sw:A"">sw:A</a>
<a class=""pagelink"" href=""ht:A"" title=""ht:A"">ht:A</a>
<a class=""pagelink"" href=""ku:A (tip)"" title=""ku:A (tip)"">ku:A (tip)</a>
<a class=""pagelink"" href=""la:A"" title=""la:A"">la:A</a>
<a class=""pagelink"" href=""lv:A"" title=""lv:A"">lv:A</a>
<a class=""pagelink"" href=""lb:A (Buschtaf)"" title=""lb:A (Buschtaf)"">lb:A (Buschtaf)</a>
<a class=""pagelink"" href=""lt:A"" title=""lt:A"">lt:A</a>
<a class=""pagelink"" href=""lmo:A"" title=""lmo:A"">lmo:A</a>
<a class=""pagelink"" href=""hu:A"" title=""hu:A"">hu:A</a>
<a class=""pagelink"" href=""mk:A (Латиница)"" title=""mk:A (Латиница)"">mk:A (Латиница)</a>
<a class=""pagelink"" href=""mg:A"" title=""mg:A"">mg:A</a>
<a class=""pagelink"" href=""ml:A"" title=""ml:A"">ml:A</a>
<a class=""pagelink"" href=""mr:A"" title=""mr:A"">mr:A</a>
<a class=""pagelink"" href=""mzn:A"" title=""mzn:A"">mzn:A</a>
<a class=""pagelink"" href=""ms:A"" title=""ms:A"">ms:A</a>
<a class=""pagelink"" href=""my:A"" title=""my:A"">my:A</a>
<a class=""pagelink"" href=""nah:A"" title=""nah:A"">nah:A</a>
<a class=""pagelink"" href=""nl:A (letter)"" title=""nl:A (letter)"">nl:A (letter)</a>
<a class=""pagelink"" href=""ja:A"" title=""ja:A"">ja:A</a>
<a class=""pagelink"" href=""nap:A"" title=""nap:A"">nap:A</a>
<a class=""pagelink"" href=""no:A"" title=""no:A"">no:A</a>
<a class=""pagelink"" href=""nn:A"" title=""nn:A"">nn:A</a>
<a class=""pagelink"" href=""nrm:A"" title=""nrm:A"">nrm:A</a>
<a class=""pagelink"" href=""uz:A (harf)"" title=""uz:A (harf)"">uz:A (harf)</a>
<a class=""pagelink"" href=""pl:A"" title=""pl:A"">pl:A</a>
<a class=""pagelink"" href=""pt:A"" title=""pt:A"">pt:A</a>
<a class=""pagelink"" href=""crh:A"" title=""crh:A"">crh:A</a>
<a class=""pagelink"" href=""ro:A"" title=""ro:A"">ro:A</a>
<a class=""pagelink"" href=""qu:A"" title=""qu:A"">qu:A</a>
<a class=""pagelink"" href=""ru:A (латиница)"" title=""ru:A (латиница)"">ru:A (латиница)</a>
<a class=""pagelink"" href=""se:A"" title=""se:A"">se:A</a>
<a class=""pagelink"" href=""stq:A"" title=""stq:A"">stq:A</a>
<a class=""pagelink"" href=""scn:A"" title=""scn:A"">scn:A</a>
<a class=""pagelink"" href=""simple:A"" title=""simple:A"">simple:A</a>
<a class=""pagelink"" href=""sk:A"" title=""sk:A"">sk:A</a>
<a class=""pagelink"" href=""sl:A"" title=""sl:A"">sl:A</a>
<a class=""pagelink"" href=""szl:A"" title=""szl:A"">szl:A</a>
<a class=""pagelink"" href=""srn:A"" title=""srn:A"">srn:A</a>
<a class=""pagelink"" href=""sr:A (слово латинице)"" title=""sr:A (слово латинице)"">sr:A (слово латинице)</a>
<a class=""pagelink"" href=""sh:A"" title=""sh:A"">sh:A</a>
<a class=""pagelink"" href=""su:A"" title=""su:A"">su:A</a>
<a class=""pagelink"" href=""fi:A"" title=""fi:A"">fi:A</a>
<a class=""pagelink"" href=""sv:A"" title=""sv:A"">sv:A</a>
<a class=""pagelink"" href=""tl:A"" title=""tl:A"">tl:A</a>
<a class=""pagelink"" href=""th:A"" title=""th:A"">th:A</a>
<a class=""pagelink"" href=""tr:A (harf)"" title=""tr:A (harf)"">tr:A (harf)</a>
<a class=""pagelink"" href=""tk:A"" title=""tk:A"">tk:A</a>
<a class=""pagelink"" href=""uk:A (латиниця)"" title=""uk:A (латиниця)"">uk:A (латиниця)</a>
<a class=""pagelink"" href=""vi:A"" title=""vi:A"">vi:A</a>
<a class=""pagelink"" href=""vo:A"" title=""vo:A"">vo:A</a>
<a class=""pagelink"" href=""war:A"" title=""war:A"">war:A</a>
<a class=""pagelink"" href=""yi:A"" title=""yi:A"">yi:A</a>
<a class=""pagelink"" href=""yo:A"" title=""yo:A"">yo:A</a>
<a class=""pagelink"" href=""zh-yue:A"" title=""zh-yue:A"">zh-yue:A</a>
<a class=""pagelink"" href=""diq:A"" title=""diq:A"">diq:A</a>
<a class=""pagelink"" href=""bat-smg:A"" title=""bat-smg:A"">bat-smg:A</a>
<a class=""pagelink"" href=""zh:A"" title=""zh:A"">zh:A</a>
</body></html>";
#endif

        #region epub hello
        [Test]
        public void TestHelloEPub()
        {
            Book book = new Book();
            book.Language = "ja";
            //You can also use the factory, FileItem.Create(FileItemType.XHTML).
            //For images, you must use the factory, FileItem.Create(FileItemType.JPEG), etc
            Chapter chapter1 = new Chapter()
            {
                Title = "Chapter 1!",

                //You are responsible for setting chapter numbers appropriately, starting with 1.
                Number = 1,

                //Be sure your Content is XHTML 1.1 compliant!
                /*
                Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html><head><title>Chapter 1</title></head>
<body><h1>Episode 1</h1><h2>The Phantom Menace</h2>
<p>Well look here, it's content in an epub!</p></body></html>"
                 * */
                Content = _content
            };

            //Book.FileItems holds all of the files in the package: chapters, images, CSS, etc. 
            //They all inherit from the abstract type FileItem and so it's easily extensible. 
            book.FileItems.Add(chapter1);

            //Repeat as necessary for each chapter
            //And then save to a file.
            //EPubLib will create a table of contents with an entry for each chapter,
            //and then package/zip everything into the proper format. 
            book.Save(@"AHeaderTDCloseDblquote.epub");
        }
        #endregion

        [Test]
        public void TestFileNameHeadUntilCurrent_Root()
        {
            string expected = "";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            string actual = sf.FileNameHeadUntilCurrent;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestFileNameHeadUntilCurrent_TwoLevel()
        {
            string expected = "ika";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"), new DirectoryInfo(@"I:\hoge\i\k\a"));
            string actual = sf.FileNameHeadUntilCurrent;
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void TestFileNameHeadUntilCurrent_RootEndWithBackSlash()
        {
            string expected = "ika";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge\"), new DirectoryInfo(@"I:\hoge\i\k\a\"));
            string actual = sf.FileNameHeadUntilCurrent;
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void TestGetMatchedSubdirectoryPath_Root()
        {
            string expected = @"I:\hoge\i";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            string actual = sf.GetMatchedSubdirectoryPath(new FileInfo(@"I:hoge\ika.txt"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetMatchedSubdirectoryPath_SecondLevel()
        {
            string expected = @"I:\hoge\i\k";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            sf.Current = new DirectoryInfo(@"I:\hoge\i");
            string actual = sf.GetMatchedSubdirectoryPath(new FileInfo(@"I:\hoge\ika.txt"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetMatchedSubdirectoryPath_CantMoveMore()
        {
            string expected = @"I:\hoge\a";
            SplitFolder sf = new SplitFolder(new DirectoryInfo(@"I:\hoge"));
            sf.Current = new DirectoryInfo(@"I:\hoge\a");
            string actual = sf.GetMatchedSubdirectoryPath(new FileInfo(@"I:\hoge\a.html"));

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void TestFileNameToSortKey_Number()
        {
            string expected = @"1903worldseries";
            string input = @"I:hoge\\1903 World Series.html";
            VerifyFileNameToSortKey(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Dot()
        {
            string expected = @"babyonemoretime";
            string input = @"I:h\\...Baby One More Time.html";
            VerifyFileNameToSortKey(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Space()
        {
            string expected = @"acappella";
            string input = @"I:hoge\\A cappella.html";
            VerifyFileNameToSortKey(input, expected);
        }


        [Test]
        public void TestFileNameToSortKey_Quote()
        {
            string input = @"I:hoge\\&quot;Love and Theft&quot;.html";
            string expected = @"loveandtheft";
            VerifyFileNameToSortKey(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Brace()
        {
            string input = @"I:hoge\\Liberal Party (UK).html";
            string expected = @"liberalpartyuk";
            VerifyFileNameToSortKey(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Japanese()
        {
            string input = @"I:hoge\\にほん_日本.html";
            string expected = @"にほん";
            VerifyFileNameToSortKeyJP(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Japanese_Katakana()
        {
            string input = @"I:hoge\\ニホン_日本.html";
            string expected = @"ニホン";
            VerifyFileNameToSortKeyJP(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Japanese_KatakanaDakuten()
        {
            string input = @"I:hoge\\ニホンゴ_日本.html";
            string expected = @"ニホンゴ";
            VerifyFileNameToSortKeyJP(input, expected);
        }

        // [Test]
        public void PrintCodeTable()
        {
            int count = 0;
            Console.WriteLine();
            for (char kata = 'ぁ'; kata < 'ン' + 5; kata++)
            {
                Console.Write(kata + " ");
                if (count++ % 10 == 0)
                    Console.WriteLine();
            }
        }

        [Test]
        public void TestFileNameToSortKey_Japanese_Katakana_Dakuten_Vu()
        {
            string input = @"I:hoge\\ヴァンパイア_吸血鬼.html";
            string expected = @"ヴァンパイア";
            VerifyFileNameToSortKeyJP(input, expected);
        }

        [Test]
        public void TestFileNameToSortKey_Japanese_RemoveSymbol()
        {
            string input = @"I:hoge\\らき☆すた_らきすたの星ってこれだっけ？.html";
            string expected = @"らきすた";
            VerifyFileNameToSortKeyJP(input, expected);
        }


        [Test]
        public void TestLookupSortChar_Japanese_Katakana_Dakuten_Vu()
        {
            string input = @"ヴァンパイア";
            string expected = @"あ";
            VerifyLookupSortCharJP(input, 0, expected);
        }

        [Test]
        public void TestLookupSortChar_Japanese_Katakana_SmallA()
        {
            string input = @"ヴァンパイア";
            string expected = @"あ";
            VerifyLookupSortCharJP(input, 1, expected);
        }

        [Test]
        public void TestLookupSortChar_Japanese_Katakana_SmallTu()
        {
            string input = @"いっかしょ";
            string expected = @"た";
            VerifyLookupSortCharJP(input, 1, expected);
        }

        [Test]
        public void TestLookupSortChar_Japanese_Katakana_Hatsuon()
        {
            string input = @"ヴァンパイア";
            string expected = @"は";
            VerifyLookupSortCharJP(input, 3, expected);
        }

        private static void VerifyFileNameToSortKeyJP(string input, string expected)
        {
            VerifyFileNameToSortKeyGeneric(input, expected, true);
        }

        private static void VerifyLookupSortCharJP(string input, int start, string expected)
        {
            var di = new DirectoryInfo("./");
            var folder = new SplitFolder(di, di, new JapaneseTactics());
            string actual = folder.LookupSortChar(input, start);
            Assert.AreEqual(expected, actual);
        }

        private static void VerifyFileNameToSortKeyGeneric(string input, string expected, bool japanese)
        {
            var di = new DirectoryInfo("./");
            SplitFolder folder;
            if (japanese)
                folder = new SplitFolder(di, di, new JapaneseTactics());
            else
                folder = new SplitFolder(di);
            string actual = folder.FileNameToSortKey(new FileInfo(input));
            Assert.AreEqual(expected, actual);
        }

        private static void VerifyFileNameToSortKey(string input, string expected)
        {
            VerifyFileNameToSortKeyGeneric(input, expected, false);
        }


        // below here is not test, some experiment.
        // [Test]
        public void RunSplitFolder()
        {
            /*
            EPubArchiver archiver = new EPubArchiver();
            DirectoryInfo di = new DirectoryInfo(@"../../../../test");
            archiver.Archive(di.GetFiles("*.html"), Path.Combine(di.FullName, "test.epub"));
             * */
            /*
            SplitFolder spliter = new SplitFolder(new DirectoryInfo(@"../../../../ePub2"));
            spliter.Split();
             * */
            var di = new DirectoryInfo(@"../../../../pdf");
            SplitFolder spliter = new SplitFolder(di, di, new JapaneseTactics());
            spliter.Extension = ".wiki";
            spliter.Split();
        }


    }
}
