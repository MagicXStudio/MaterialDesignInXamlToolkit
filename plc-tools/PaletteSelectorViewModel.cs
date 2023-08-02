﻿using System.ComponentModel;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace ImageStudio;

public class PaletteSelectorViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;


    public PaletteSelectorViewModel()
    {
        Swatches = new SwatchesProvider().Swatches;

        PaletteHelper paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;
    }

    public ICommand ToggleStyleCommand { get; } = new AnotherCommandImplementation(o => ApplyStyle((bool)o!));

    public IEnumerable<Swatch> Swatches { get; }

    public ICommand ApplyPrimaryCommand { get; } = new AnotherCommandImplementation(o => ApplyPrimary((Swatch)o!));

    public ICommand ApplyAccentCommand { get; } = new AnotherCommandImplementation(o => ApplyAccent((Swatch)o!));

    private bool _isDarkTheme;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDarkTheme)));
                ApplyBase(value);
            }
        }
    }

    private static void ApplyStyle(bool alternate)
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri(@"pack://application:,,,/Dragablz;component/Themes/materialdesign.xaml")
        };

        var styleKey = alternate ? "MaterialDesignAlternateTabablzControlStyle" : "MaterialDesignTabablzControlStyle";
        var style = (Style)resourceDictionary[styleKey];

        foreach (var tabablzControl in Dragablz.TabablzControl.GetLoadedInstances())
        {
            tabablzControl.Style = style;
        }
    }

    private static void ApplyBase(bool isDark)
        => ModifyTheme(theme => theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light));

    private static void ApplyPrimary(Swatch swatch)
        => ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));

    private static void ApplyAccent(Swatch swatch)
    {
        if (swatch.AccentExemplarHue is Hue accentHue)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(accentHue.Color));
        }
    }

    private static void ModifyTheme(Action<Theme> modificationAction)
    {
        PaletteHelper paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        modificationAction?.Invoke(theme);

        paletteHelper.SetTheme(theme);
    }
}
