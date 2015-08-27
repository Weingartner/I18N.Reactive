I18N.Reactive
======
Transform ResX files into strongly typed classes via T4 that can be updated at runtime,

##Download
 * [Nuget Package](https://nuget.org/packages/I18N.Reactive)

## Getting started

* Create a new *.resx file
* In the properties window, set `CustomTool` to `I18NReactive`
* Right-click on the *.resx file and click "Run custom tool"
* In your *.xaml file, create a new resource:  
```<ObjectDataProvider x:Key="T9N" ObjectType="p:Resources" MethodName="GetInstance" />```
* Bind an XAML property to a translation:  
```<TextBlock Text="{Binding Source={StaticResource T9N}, Path=Greetings}"/>```
