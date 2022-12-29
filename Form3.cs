using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Globalization;

namespace GOR_Launcher
{
    public partial class SettingsForm : MaterialForm
    {
        public SettingsForm()
        {
            InitializeComponent();

            List<string> langList = CLocalization.GetLanguageList();
            foreach(string str in langList)
                languageSelectBox.Items.Add(str);

            languageSelectBox.SelectedIndex = languageSelectBox.FindString(CLocalization.GetLanguageByCode(CLocalization.GetLanguage()));
            Translate();

            // Updating info
            if (CGameConfig.GetValue("zStartupWindowed") == "1")
                windowedModeSwitch.Checked = true;

            resolutionBoxX.Text = CGameConfig.GetValue("zVidResFullscreenX");
            resolutionBoxY.Text = CGameConfig.GetValue("zVidResFullscreenY");

            int musicValue = Convert.ToInt16(Convert.ToDouble(CGameConfig.GetValue("musicVolume"), CultureInfo.InvariantCulture) * 100);
            int soundValue = Convert.ToInt16(Convert.ToDouble(CGameConfig.GetValue("soundVolume"), CultureInfo.InvariantCulture) * 100);
            int mouseValue = Convert.ToInt16((Convert.ToDouble(CGameConfig.GetValue("mouseSensitivity"), CultureInfo.InvariantCulture) * 100) / 4);

            musicValue = musicValue > 100 ? 100 : musicValue;
            soundValue = soundValue > 100 ? 100 : soundValue;
            mouseValue = mouseValue > 100 ? 100 : mouseValue;

            musicSlider.Value = musicValue;
            soundSlider.Value = soundValue;
            mouseSlider.Value = mouseValue;

            altConnectionSwitch.Checked = Convert.ToBoolean(CSettings.Get("retranslatorEnabled"));
        }

        private async void validateFilesButton_Click(object sender, EventArgs e)
        {
            if (CFileValidation.IsReady())
                Task.Run(()=> CFileValidation.StartFileValidation());

            await Task.Delay(1);
        }

        private void languageSelectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string languageCode = CLocalization.GetLanguageByName((string)languageSelectBox.Text);
            if (languageCode != "none")
                CLocalization.Translate(languageCode);
        }

        public void Translate()
        {
            this.Text                   = CLocalization.Get("settings_header");
            languageSelectLabel.Text    = CLocalization.Get("settings_language");
            resolutionLabel.Text        = CLocalization.Get("settings_resolution");
            windowedModeLabel.Text      = CLocalization.Get("settings_windowed");
            musicVolumeLabel.Text       = CLocalization.Get("settings_musvolume");
            soundVolumeLabel.Text       = CLocalization.Get("settings_sndvolume");
            mouseSensitivityLabel.Text  = CLocalization.Get("settings_sensitivity");
            validateFilesButton.Text    = CLocalization.Get("settings_validation");
            altConnectionLabel.Text     = CLocalization.Get("settings_altConnection");

        }

        private void resolutionBoxX_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void resolutionBoxY_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (resolutionBoxX.Text.Length != 0) CGameConfig.SetValue("zVidResFullscreenX", resolutionBoxX.Text);
            if (resolutionBoxY.Text.Length != 0) CGameConfig.SetValue("zVidResFullscreenY", resolutionBoxY.Text);

            if (windowedModeSwitch.Checked)
                CGameConfig.SetValue("zStartupWindowed", "1");
            else
                CGameConfig.SetValue("zStartupWindowed", "0");

            CGameConfig.SetValue("musicVolume", Convert.ToString(Math.Round(musicSlider.Value / 100.0, 1)));
            CGameConfig.SetValue("soundVolume", Convert.ToString(Math.Round(soundSlider.Value / 100.0, 1)));
            CGameConfig.SetValue("mouseSensitivity", Convert.ToString(Math.Round((mouseSlider.Value / 100.0) * 4, 1)));

            CGameConfig.SaveConfig();
        }

        private void altConnectionSwitch_CheckedChanged(object sender, EventArgs e)
        {
            CConfig.SetRetranslator(altConnectionSwitch.Checked);
            CSettings.Set("retranslatorEnabled", altConnectionSwitch.Checked);
        }
    }
}
