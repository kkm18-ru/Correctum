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
        {0}        }}
        
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

    const string ViewTemplate = """
        using System.Windows.Controls;

        namespace Correctum.Views.{0};

        public sealed class {1}View : global::System.Windows.Controls.StackPanel
        {{
            public {1}View()
            {{
                Orientation = Orientation.Vertical;
                var invertBoolConverter = (global::System.Windows.Data.IValueConverter)
                    global::System.Windows.Application.Current.FindResource("invertBoolConverter");
        {2}    }}
        }}
        """;

    const string Separator = "        Children.Add(new global::System.Windows.Controls.Separator { Height = 5, Background = global::System.Windows.Media.Brushes.White });";
    const string Label = "        Children.Add(new global::System.Windows.Controls.TextBlock {{ Text = \"{0}\" }});";
    const string TextBox = """
                {{
                    var tb = new global::System.Windows.Controls.TextBox();
                    var bind = new global::System.Windows.Data.Binding("{0}");
                    if(!string.IsNullOrEmpty("{3}"))
                        bind.Converter = (global::System.Windows.Data.IValueConverter)
                            global::System.Windows.Application.Current.FindResource("{3}");
                    tb.SetBinding(global::System.Windows.Controls.TextBox.TextProperty, bind);
                    if({1})
                    {{
                        global::System.Windows.Data.Binding b;
                        if({2})
                        {{
                            b = new global::System.Windows.Data.Binding("Authorizing");
                            b.Converter = invertBoolConverter;
                        }}
                        else
                        {{
                            b = new global::System.Windows.Data.Binding("Authorized");
                        }}
                        tb.SetBinding(global::System.Windows.Controls.TextBox.IsEnabledProperty, b);
                    }}
                    Children.Add(tb);
                }}
        """;
    const string DateRange = """
                {{
                    var dr = new global::Correctum.UI.Controls.DateRangePicker();
                    dr.SetBinding(global::Correctum.UI.Controls.DateRangePicker.DateRangeProperty, "{0}");
                    if({1})
                    {{
                        global::System.Windows.Data.Binding b;
                        if({2})
                        {{
                            b = new global::System.Windows.Data.Binding("Authorizing");
                            b.Converter = invertBoolConverter;
                        }}
                        else
                        {{
                            b = new global::System.Windows.Data.Binding("Authorized");
                        }}
                        dr.SetBinding(global::Correctum.UI.Controls.DateRangePicker.IsEnabledProperty, b);
                    }}
                    Children.Add(dr);
                }}
        """;
    const string CheckComboBox = """
                {{
                    var ccb = new global::Xceed.Wpf.Toolkit.CheckComboBox();
                    ccb.SetBinding(global::Xceed.Wpf.Toolkit.CheckComboBox.ItemsSourceProperty, "{0}");
                    ccb.SetBinding(global::Xceed.Wpf.Toolkit.CheckComboBox.SelectedItemsOverrideProperty, "{1}");
                    if({2})
                    {{
                        global::System.Windows.Data.Binding b;
                        if({3})
                        {{
                            b = new global::System.Windows.Data.Binding("Authorizing");
                            b.Converter = invertBoolConverter;
                        }}
                        else
                        {{
                            b = new global::System.Windows.Data.Binding("Authorized");
                        }}
                        ccb.SetBinding(global::Xceed.Wpf.Toolkit.CheckComboBox.IsEnabledProperty, b);
                    }}
                    Children.Add(ccb);
                }}
        """;
    const string AuthButton = """
                {
                    var btn = new global::System.Windows.Controls.Button();
                    btn.Content = "Авторизоваться";
                    var bind = new global::System.Windows.Data.Binding("Authorizing");
                    bind.Converter = invertBoolConverter;
                    btn.SetBinding(global::System.Windows.Controls.Button.IsEnabledProperty, bind);
                    btn.SetBinding(global::System.Windows.Controls.Button.CommandProperty, "AuthCommand");
                    Children.Add(btn);
                }
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(FilterSyntaxNode, Transform)
            .Where(t => t.@using)
            .Select((t, _) => t.Item1);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateViews(ctx, t.Left, t.Right));

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateModuleViews(ctx, t.Left, t.Right));
    }

    private static bool FilterSyntaxNode(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ClassDeclarationSyntax cds) return false;
        if (cds.AttributeLists is not { Count: > 0 } attrLists) return false;
        if (!attrLists.Any(list => list.Attributes.Any(a => a.ToString().Contains("ImportInfo"))))
            return false;
        if (cds.BaseList is not BaseListSyntax baseList) return false;
        if (!baseList.Types.Any(t => t.ToString().Contains("Import"))) return false;
        return true;
    }

    private static (ClassDeclarationSyntax, bool @using) Transform
        (GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var cds = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in cds.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Correctum.Attributes.ImportInfoAttribute")
                    continue;

                return (cds, true);
            }
        }

        return (cds, false);
    }

    private sealed class ViewProperty(string type, string display, object?[] args)
    {
        public string Type { get; } = type;
        public bool ForAuth { get; set; } = false;
        public string DisplayName { get; } = display;
        public object?[] Arguments { get; } = args;
    }

    private static void GenerateViews(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        foreach (var cds in classDeclarations)
        {
            var builder = new StringBuilder();
            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            if (model.GetDeclaredSymbol(cds) is not INamedTypeSymbol symbol)
                continue;
            if (!symbol.AllInterfaces.Any(i => i.ConstructedFrom.ToDisplayString() == "Correctum.Abstractions.IImport"))
                continue;

            var authorizable = symbol.BaseType?.ToDisplayString().StartsWith("Correctum.Abstractions.OfdImport") ?? false;

            var dict = new Dictionary<string, ViewProperty>();

            foreach (var attribute in symbol.GetAttributes())
            {
                switch (attribute.AttributeClass?.ToDisplayString())
                {
                    case "Correctum.Attributes.ModuleViewProperty.SecretBoxAttribute":
                    case "Correctum.Attributes.ModuleViewProperty.TextBoxAttribute":
                    case "Correctum.Attributes.ModuleViewProperty.DateRangeAttribute":
                    case "Correctum.Attributes.ModuleViewProperty.CheckComboBoxAttribute":
                        var propName = (string)attribute.ConstructorArguments[1].Value!;
                        var displayName = (string)attribute.ConstructorArguments[0].Value!;
                        var attrClass = attribute.AttributeClass!.ToDisplayString();
                        var startIndex = "Correctum.Attributes.ModuleViewProperty.".Length;
                        var len = attrClass.Length - "Attribute".Length - startIndex;
                        var type = attrClass.Substring(startIndex, len);
                        var args = attribute.ConstructorArguments.Skip(2).Select(a => a.Value).ToArray();
                        dict.Add(propName, new(type, displayName, args));
                        break;
                    default: continue;
                }
            }

            if (authorizable)
            {
                var authAttr = symbol.GetAttributes().FirstOrDefault
                    (a => a.AttributeClass?.ToDisplayString() == "Correctum.Attributes.ModuleViewProperty.AuthorizationPropertiesAttribute");
                if (authAttr is not null)
                    foreach (var prop in authAttr.ConstructorArguments[0].Values)
                        dict[(string)prop.Value!].ForAuth = true;
            }

            foreach (var item in dict.Where(i => i.Value.ForAuth))
                AppendBuilder(item, authorizable, builder);

            if (authorizable)
            {
                builder.AppendLine();
                builder.AppendLine(AuthButton);
                builder.AppendLine(Separator);
            }

            foreach (var item in dict.Where(i => !i.Value.ForAuth))
                AppendBuilder(item, authorizable, builder);

            var text = string.Format(ViewTemplate, "Imports", cds.Identifier.Text, builder.ToString());
            var source = SourceText.From(text, Encoding.UTF8);
            context.AddSource($"{cds.Identifier.Text}View.g.cs", source);
        }
    }

    private static void AppendBuilder(KeyValuePair<string, ViewProperty> pair, bool authorizable, StringBuilder builder)
    {
        switch (pair.Value.Type)
        {
            case "SecretBox":
            case "TextBox":
                builder.AppendLine();
                builder.AppendLine(string.Format(Label, pair.Value.DisplayName));
                builder.AppendLine(string.Format(TextBox, pair.Key, 
                    authorizable.ToString().ToLowerInvariant(), 
                    pair.Value.ForAuth.ToString().ToLowerInvariant(),
                    pair.Value.Arguments.Length > 0 ? pair.Value.Arguments[0] : string.Empty));
                builder.AppendLine(Separator);
                break;
            case "DateRange":
                builder.AppendLine();
                builder.AppendLine(string.Format(Label, pair.Value.DisplayName));
                builder.AppendLine(string.Format(DateRange, pair.Key, authorizable.ToString().ToLowerInvariant(), pair.Value.ForAuth.ToString().ToLowerInvariant()));
                builder.AppendLine(Separator);
                break;
            case "CheckComboBox":
                builder.AppendLine();
                builder.AppendLine(string.Format(Label, pair.Value.DisplayName));
                builder.AppendLine(string.Format(CheckComboBox, pair.Key, pair.Value.Arguments[0], authorizable.ToString().ToLowerInvariant(), pair.Value.ForAuth.ToString().ToLowerInvariant()));
                builder.AppendLine(Separator);
                break;
            default: break;
        }
    }

    private static void GenerateModuleViews(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        var builder = new StringBuilder();
        foreach(var cds in classDeclarations)
        {
            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            if (model.GetDeclaredSymbol(cds) is not INamedTypeSymbol symbol)
                continue;
            var typePath = $"{symbol.ContainingNamespace.ToDisplayString()}.{cds.Identifier.Text}";

            if (!symbol.AllInterfaces.Any(i => i.ConstructedFrom.ToDisplayString() == "Correctum.Abstractions.IImport"))
                continue;
            builder.AppendLine(@$"            Add<{typePath}, Correctum.Views.Imports.{cds.Identifier.Text}View>();");
        }
        var text = string.Format(Template, builder.ToString());
        var source = SourceText.From(text, Encoding.UTF8);
        context.AddSource($"ModuleViews.g.cs", source);
    }
}
