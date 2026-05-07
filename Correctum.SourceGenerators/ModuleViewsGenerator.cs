namespace Correctum.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class ModuleViewsGenerator : IIncrementalGenerator
{
    const string Template = """
        using System.Windows;
        
        namespace Correctum.ResourceDictionaries
        {{
            public sealed class ModuleViews : ResourceDictionary
            {{
                public ModuleViews()
                {{
                    {0}
                }}
        
                private void Add<TModule, TView>()
                    where TView : FrameworkElement
                {{
                    var template = new DataTemplate
                    {{
                        DataType = typeof(TModule),
                        VisualTree = new FrameworkElementFactory(typeof(TView))
                    }};
        
                    template.Seal();
        
                    Add(template.DataTemplateKey, template);
                }}
            }}
        }}
        
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, GenerateModuleViews);
    }

    private static void GenerateModuleViews(SourceProductionContext context, Compilation compilation)
    {
        var builder = new StringBuilder();
        // todo
        var text = string.Format(Template, builder.ToString());
        var source = SourceText.From(text, Encoding.UTF8);
        context.AddSource($"ModuleViews.g.cs", source);
    }
}
