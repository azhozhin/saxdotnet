
                         SAX for .NET 2.0

This release is an update to the .NET 2.0 framework. It should also work under
Mono 1.1.17 as long as the generics capable compiler - gmcs - is used.

As there are no legacy SAX applications to consider in the .NET environment,
none of the SAX 1 portions of the API have been ported.


- WHAT IS INCLUDED

This distribution consists of the source for Sax.ll, Org.System.Xml.dll and the
demo applications as well as documentation, build scripts and VS.NET project files.
Sax.dll contains the SAX API, Org.System.Xml.dll provides general XML specific
types and namespace processing functionality independent of, and not necessarily
required by SAX. The pre-built assemblies are located in the distribution's 
"bin" directory.


- INSTALL

Other than unzipping the distribution, nothing special must be done.
If Sax.dll and Org.System.Xml.dll are to be shared among applications,
it is recommended to install them into the GAC (Global Assembly Cache)
using gacutil.exe:

     gacutil –I Sax.dll
and
     gacutil -I Org.System.Xml.dll


- REBUILDING FROM SOURCE

For how to build, consult Build.txt.
Note: Rebuilding will produce strong namend assemblies signed with
a different key than the ones included, as this distribution contains
only a dummy key required by the source code. This may cause applications
that require the original assemblies to stop working.


- DOCUMENTATION

Source documentation was created using  an alpha version NDoc 2.0 (http://ndoc.sf.net)
and can be found in the "doc" subdirectory. It mostly refers back to the
original Java documentation, except where the .NET port is different.
There is also a very basic "How To" included (HowTo.html).


- DEMO APPLICATIONS

Two demo applications are included in the Demo subdirectory.
The Treeview demo shows how to load a tree view component from an XML document.
The Conformance demo runs a given parser through the test cases of the
W3C XML test suite (http://w3.org/xml/test) and additional SAX conformance tests.
The precompiled executables depend on the included Sax.dll assembly that
is signed with the release key (this key is not part of the distribution).


- DIFFERENCES TO JAVA - C# SPECIFICS

This port intends to follow C# coding and naming conventions - as much as 
I am aware of them, but tries to stay close to the original Java source semantics.
I have strayed away from them when I believed there was a better choice for C#, 
or when I absolutely didn't like the way it was done in the original.
Some of these choices are based on discussions I had when porting SAX to Delphi.

Here are some of the differences:

1) SAX core and extensions have been unified (no "version2" interfaces or classes).
   IAttributes gets the members of IAttributes2 (except for "IsDeclared").
   IEntityResolver gets its only member replaced by the members of IEntityResolver2.
   ILocator gets the ILocator2 members and a new EntityType property.
   IXmlReader gets a Lexicalhandler and DeclHandler property, as well
   as the members of the IXmlReaderControl interface (which is now obsolete).
   The helper classes have been adjusted accordingly.

2) Usage of exceptions has been streamlined to give preference to
   pre-defined .NET framework classes. The SaxNotSupportedException
   and SaxNotRecognizedException have been removed.

3) IXmlReader.GetProperty(name) returns a reference to a generic IProperty<T> interface
   instead of an object representing the property value. Two reasons:
   - Properties can potentially be accessed frequently, and this approach
     avoids the cost of a frequent name lookup, as IProperty<T>.GetValue() directly
     connects to the property.
   - Property access can be made type-safe using the appropriate type parameter T.

4) IAttributes.GetType() may also return the value "UNDECLARED", making the
   IAttributes2.IsDeclared member unnecessary.

5) The AttributesImpl class does not follow the Java implementation,
   to take advantage of C# structs (similar to the Delphi implementation).

6) There is no port of the Java NamespaceSupport class, instead the corresponding
   Delphi classes were ported to C#. The source can be found in Namespaces.cs
   as part of the Org.System.Xml assembly. The main class's name is XmlNamespaces. 
   It should be possible to write a wrapper that exposes an API similar to the
   Java NamespaceSupport class.

7) The error handlers do not have arguments derived from Exception,
   as Exception instances cannot be re-used because their properties
   can only be set in the constructor. Instead we use ParseError objects.

8) The set of standard features and properties is not exactly
   the same as in the original Java specs.

I am hoping for feedback to make this a generally accepted SAX API
specification for C#, and more generally, for .NET.


Karl Waclawek (karl@waclawek.net)