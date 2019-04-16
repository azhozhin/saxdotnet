
                SAX for .NET 2.0

This distribution contains not only the SAX API and
demo project assemblies, but also two actual SAX parser
implementations, AElfred (managed code ported from Java)
and SAXExpat (.NET wrapper for Expat parser, a C library).

Please consult the ReadMe files and other documentation
in the respective sub-directories.

All assemblies should work under Mono 1.1.17+ except for
SAXExpat, as this version of Mono still does not support
all .NET 2.0 features.

INSTALL

The "bin" directory can be considered the install location
and contains the release builds of all the included assemblies:

* SAX:
  - Sax.dll (strong named - can be installed in GAC)
  - TreeviewDemo.exe (demo)
  - XmlConf.exe (demo)
* Org.System.Xml
  - Org.System.Xml.dll (strong named - can be installed in GAC)
* AElfred:
  - AElfred.dll (strong named - can be installed in GAC)
* SAXExpat:
  - ExpatInterop.dll (requires the Expat C library to be installed)
  - KdsSax.dll (helper assembly)
  - KdsText.dll (helper assembly)
  - SaxExpat.dll (the actual SAX parser, strong named - can be installed in GAC)

There are no special installation steps needed except for
installing the Expat C library (as decribed in the respective Readme).

