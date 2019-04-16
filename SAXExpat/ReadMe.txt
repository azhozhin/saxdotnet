                          SAXExpat.NET 2.0

SAXExpat is a wrapper class library for the popular Expat parser,
making it possible to use it through the SAX API for .NET.

It works under MS.NET 2.0 and should work under Mono once it supports
.NET 2.0 more completely. Currently - as of Mono 1.1.17 - it still
fails due to some unsupported features.

Contents:
- 1) What is included
- 2) Install
- 3) Rebuilding from source
- 4) Using the SAX demo application
- 5) License


1) WHAT IS INCLUDED

- Directory SAXExpat:
  Source for SAX2 adapter for Expat.
- Directory SAX:
  Source for SAX extensions and helpers, supporting the SAXExpat implementation.
- Directory ExpatInterop:
  Source for interface to unmanaged Expat library.
- Directory Util:
  Source for general support libraries.
- Directory ExpatLib:
  Expat shared libraries for Windows and Linux.


2) INSTALL

Just unzip the SAXExpat directory, if it is distributed on its own.
Otherwise, follow the distribution's instructions.
There is no need for a special relationship to the SAX directory,
since the Sax.dll assembly (release 2.0), on which SaxExpat.NET depends,
is included.

If SaxExpat.dll (and the other assemblies it depends) on are to be shared
among applications, it is recommended to install them into the GAC
(Global Assembly Cache) using gacutil.exe, like this:

     gacutil –I SaxExpat.dll

Installing Expat on Windows:
Just copy the libexpatw.dll file to a directory on the path.
This file can be found in the ExpatLib directory of the SAXExpat distribution.
It is built from CVS after the Expat 2.0 release.

Installing Expat on Linux:
For building and installing from source, consult the file ReadMe_Linux.txt
in the ExpatLib directory. Pre-compiled libraries (for Intel) are included
as well and can be found in the same directory.
After installing, add a <dllmap dll="libexpatw.dll" target="libexpaw.so" />
entry to the file /etc/mono/config, which makes libexpatw.dll an alias
for libexpatw.so. It might also work to create a symlink called libexpatw.dll
in the install directory, pointing to libexpatw.so.

Setting up SAXExpat as default SAX for .NET parser:
Open the machine.config file and add the following entries to the appSettings section,
uncommenting it if necessary:

(<path> denotes the install location of SAXExpat.dll)

    <appSettings>
            <add key="Org.System.Xml.Sax.ReaderClass" value="Kds.Xml.Expat.ExpatReader" />
            <add key="Org.System.Xml.Sax.ReaderAssembly" value="<path>\SAXExpat.dll" />
    </appSettings>

On Windows, machine.config is located in the <WinDir>\Microsoft.NET\Framework\CONFIG
directory, on Linux it is usually located in /etc/mono.

Using Expat 1.95.7:
Remove the EXPAT_1_95_8_UP symbol definition from the build instructions
(both batch file and IDE), then rebuild SAXExpat.


3) REBUILDING FROM SOURCE

Please consult Build.txt.


4) USING THE SAX DEMO APPLICATIONS

These are supplied with the SAX distribution.

A tree view demo is supplied with SAX for .NET 2.0 and can be found in
the ..\SAX\Demo\Treeview\ directory. It should work with a Mono release
that has sufficient support for System.Windows.Forms.
Note:
  To load/change the parser, one needs to specify only the assembly and one can leave
  the class name blank - the SaxReaderFactory will instantiate the first implementation
  of IXmlReader it finds in the assembly. Alternatively - if the assembly is on the
  path or in the GAC - one needs to specify only the assembly name (without .dll), and
  optionally, the class name.

A conformance test application is supplied with SAX for .NET 2.0 and can be found
in the ..\SAX\Demo\Conformance\ directory.

Note:
  It requires the XML-Test-Suite to run, and should be placed directly in the
  xmlconf directory. The test suite can be obtained from http://www.w3.org/XML/Test/.


5) LICENSE

This software is released under the modified BSD license, which can be obtained from
http://opensource.org/licenses/bsd-license.html. It applies to all files that contain
a reference to it. This reference will contain a file-specific substitution of the fields
<OWNER>, <ORGANIZATION> and <YEAR> in the license template (a copy of which is included
with the file BSD-License.txt). If no substitution is present, assume the following:
<OWNER>=Karl Waclawek
<ORGANIZATION>=Karl Waclawek
<YEAR>=2004, 2005, 2006

Some files may fall under a different license, in which case the license is described or
referenced in the file itself.


Karl Waclawek
(karl@waclawek.net)





