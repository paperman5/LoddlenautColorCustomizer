using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorCustomizer
{
    public class ColorMenuEntry : MonoBehaviour
    {
        public string settingKey;
        public string settingName;
        
        public BetterButton colorButton;
        public BetterButton resetButton;
        public TextMeshProUGUI settingText;

        public void Initialize(string settingKey, string settingName)
        {
            settingText.text = settingName;
            Color col = Color.magenta;
            if (CustomizerPlugin.allSettings.ContainsKey(settingKey))
            {
                col = CustomizerPlugin.allSettings[settingKey].Value;
                CustomizerPlugin.allSettings[settingKey].SettingChanged += (e, args) => SetColor(CustomizerPlugin.allSettings[settingKey].Value);
            }
            colorButton.GetComponent<Image>().color = col;
            this.settingKey = settingKey;
            this.settingName = settingName;
        }

        public Color GetColor()
        {
            return CustomizerPlugin.allSettings[settingKey].Value;
        }

        public Color GetDefaultColor()
        {
            return (Color)CustomizerPlugin.allSettings[settingKey].DefaultValue;
        }

        public void ResetColor()
        {
            CustomizerPlugin.allSettings[settingKey].Value = (Color)CustomizerPlugin.allSettings[settingKey].DefaultValue;
            UpdateColorButtonColor(CustomizerPlugin.allSettings[settingKey].Value);
        }

        public void SetColor(Color color)
        {
            CustomizerPlugin.allSettings[settingKey].Value = color;
            UpdateColorButtonColor(color);
        }

        private void UpdateColorButtonColor(Color color)
        {
            // Sets the button color without updating the config setting
            colorButton.GetComponent<Image>().color = color;
        }
    }
}
