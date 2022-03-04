using CrackaSmile.Tools;
using REghZyFramework.Themes;
using System;
using System.Collections.Generic;
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
        private object _selectedTheme;
        public object SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                SignalChanged("SelectedTheme");
            }
        }
        #endregion

        public SettingsViewModel()
        {
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

        }
    }
}
