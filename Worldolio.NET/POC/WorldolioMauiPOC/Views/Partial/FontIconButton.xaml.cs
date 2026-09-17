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