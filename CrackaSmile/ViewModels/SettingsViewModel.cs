using CrackaSmile.Tools;
using REghZyFramework.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackaSmile.ViewModels
{
    public class SettingsViewModel: BaseViewModel
    {
        #region commands
        public CustomCommand SetColourfulDarkTheme { get; set; }
        public CustomCommand SetColourfulLightTheme { get; set; }
        public CustomCommand SetDarkTheme { get; set; }
        public CustomCommand SetLightTheme { get; set; }
        #endregion

        #region properties

        string q = "Light";
        string qw = "Dark";

        public ObservableCollection<string> Themes { get; set; }

        private object _selectedTheme;
        public object SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                if(SelectedTheme.ToString() == "Light")
                {
                    ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
                }
                if(SelectedTheme.ToString() == "Dark")
                {
                    ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
                }
                SignalChanged("SelectedTheme");
            }
        }
        #endregion
        public CustomCommand DarkTheme { get; set; }
        public CustomCommand LightTheme { get; set; }
        public SettingsViewModel()
        {
            Themes = new ObservableCollection<string>();
            Themes.Add(q);
            Themes.Add(qw);
            SetColourfulDarkTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulDark);
            });
            SetColourfulLightTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulLight);
            });
            SetDarkTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            });
            SetLightTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            });



            DarkTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulDark);
            });
            LightTheme = new CustomCommand(() =>
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.ColourfulLight);
            });


        }
    }
}
