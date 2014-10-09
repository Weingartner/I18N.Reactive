I18N.Reactive
======
Transform ResX files into strongly typed classes via T4 that can be updated at runtime,

####[Consult Documentation](https://github.com/weingartner/I18N.Reactive)

##Download
 * [Nuget Package](https://nuget.org/packages/I18N.Reactive)


#Overview

##Have a project that uses ResX files and need runtime I18N switching support ?

- Transform ResX files in a project into a strongly typed assembly
- Access your translations
   - From inside .cshtml & .aspx files
     - ``@Resources.User.Pseudo`` / ``<%= Resources.User.Pseudo %>``
- Bind your translations to a ViewModel
  - ``[Display(Name = "Pseudo", ResourceType = typeof(Resources.User))]``
- Use it again in your dlls to return localized error messages
  - ``return Resources.User.RegisterError;``

####Use variables inside your ResX files
- Format various messages
  - .resx: ``Welcome {0}``
     - .cs: ``return Resources.User.Welcome("Robert")``
     - result: ``Welcome Robert``
- Dynamically replace variables
  - .resx: ``Register with {DOMAIN}``
     - .cs: ``return Resources.Branding.Register;`` 
     - result: ``Register with www.i-technology.net`` 
  - .resx: ``{BRAND} announces new feature for {0}``
     - .cs: ``return Resources.Branding.Feature("T4")``
     - result: ``I-Technology announces new feature for T4``

##Implementation Details

    This nuget package automatically installs a VSIX based custom tool into visual studio. This
    VSIX is stored in the tools directory of the nuget package. It is not in the VS extensions
    gallery. The custom tool is for converting the ResX files into strongly typed cs files. 

    All __resx__ files found in the project after install of this nuget package will be converted
    to use __I18NReactive__ custom tool.
    
    This is better than dropping t4 templates into the folder which process the resx files. The
    main problem with the t4 template approach is that updates to the resx file are not automatically
    generated into the cs file. The user has to manually invoke the builder. With the custom
    tool approach the cs file is regenerated every time the resx file changes.

## TODO

    Extract the VSIX packaging logic into it's own nuget package