using System.Windows.Input;

namespace WorldolioMauiPOC.Views.Partial;

public partial class FontIconButton : ContentView
{
    public static readonly BindableProperty FontNameProperty =
        BindableProperty.Create(
            nameof(FontName),
            typeof(string),
            typeof(FontIconButton),
            default(string));

    public string FontName
    {
        get => (string)GetValue(FontNameProperty);
        set => SetValue(FontNameProperty, value);
    }

    public static readonly BindableProperty IconGlyphProperty =
        BindableProperty.Create(
            nameof(IconGlyph),
            typeof(string),
            typeof(FontIconButton),
            default(string));

    public string IconGlyph
    {
        get => (string)GetValue(IconGlyphProperty);
        set => SetValue(IconGlyphProperty, value);
    }

    public static readonly BindableProperty IconGlyphSizeProperty =
        BindableProperty.Create(
            nameof(IconGlyphSize),
            typeof(double),
            typeof(FontIconButton),
            10.0);

    public double IconGlyphSize
    {
        get => (double)GetValue(IconGlyphSizeProperty);
        set => SetValue(IconGlyphSizeProperty, value);
    }

    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.Create(
            nameof(BorderSize),
            typeof(double),
            typeof(FontIconButton),
            15.0);

    public double BorderSize
    {
        get => (double)GetValue(BorderSizeProperty);
        set => SetValue(BorderSizeProperty, value);
    }

    public static readonly BindableProperty ClickCommandProperty =
            BindableProperty.Create(
                nameof(ClickCommand), 
                typeof(ICommand), 
                typeof(FontIconButton));

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }

    public FontIconButton()
	{
		InitializeComponent();
	}
}