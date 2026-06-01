using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace TubeStar
{
    public partial class CharacterCreationPage : UserControl
    {
        public event Action StartGameClicked;

        private int _selectedHairStyle = 1; // 1=Spiky, 2=Mohawk, 3=Pompadour, 4=Gamer, 5=Wavy
        private string _selectedHairColor = "#00FFFF"; // default Cyan
        private string _selectedHairName = "Cyan";

        private int _selectedOutfit = 0; // 0=Tshirt, 1=Hoodie, 2=Jacket, 3=Suit
        private int _selectedAccessory = 0; // 0=None, 1=Headphones, 2=Glasses, 3=Cap
        private int _selectedTattoo = 0; // 0=None, 1=Tribal, 2=Geek, 3=Gamer

        // Neon color palette definitions
        private readonly Tuple<string, string>[] _hairColors = new Tuple<string, string>[]
        {
            Tuple.Create("#00FFFF", "Cyan"),
            Tuple.Create("#FF00FF", "Pink"),
            Tuple.Create("#00FF00", "Green"),
            Tuple.Create("#FF5500", "Orange"),
            Tuple.Create("#FFDD44", "Yellow"),
            Tuple.Create("#FF2222", "Red"),
            Tuple.Create("#8A2BE2", "Purple"),
            Tuple.Create("#FFFFFF", "White")
        };

        private readonly string[] _outfits = new string[] { "Camiseta Gamer", "Moletom Gamer", "Jaqueta de Couro", "Terno Elegante" };
        private readonly string[] _accessories = new string[] { "Nenhum", "Headphone Pro Neon", "Óculos Gamer Escuros", "Boné Virado" };
        private readonly string[] _tattoos = new string[] { "Nenhuma", "Tribal Manga", "Símbolos Geek / Códigos", "Gamer Completa (Braço)" };
        private readonly string[] _hairNames = new string[] { "", "Arrepiado", "Mohawk", "Topete", "Gamer", "Longos" };

        private List<Button> _colorButtons = new List<Button>();

        public CharacterCreationPage()
        {
            InitializeComponent();
            SetupHairColorPanel();
            
            // Set initial highlights on RPG grids
            HighlightSelectedButton(btnHairSpiky);
            HighlightSelectedButton(btnOutfitTshirt);
            HighlightSelectedButton(btnAccNone);
            HighlightSelectedButton(btnTattooNone);

            UpdatePreview();
        }

        private void SetupHairColorPanel()
        {
            panelHairColors.Children.Clear();
            _colorButtons.Clear();

            for (int i = 0; i < _hairColors.Length; i++)
            {
                var colorHex = _hairColors[i].Item1;
                var colorName = _hairColors[i].Item2;

                Button btn = new Button
                {
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(4),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(2),
                    ToolTip = "Cabelo Neon " + colorName
                };

                btn.Click += (s, e) =>
                {
                    _selectedHairColor = colorHex;
                    _selectedHairName = colorName;
                    SelectColorButton(colorHex);
                    UpdatePreview();
                };

                panelHairColors.Children.Add(btn);
                _colorButtons.Add(btn);
            }

            SelectColorButton("#00FFFF");
        }

        private void SelectColorButton(string selectedHex)
        {
            for (int i = 0; i < _hairColors.Length; i++)
            {
                var btn = _colorButtons[i];
                var hex = _hairColors[i].Item1;

                if (hex == selectedHex)
                {
                    btn.BorderBrush = Brushes.White;
                }
                else
                {
                    btn.BorderBrush = Brushes.Transparent;
                }
            }
        }

        private void HighlightSelectedButton(Button selectedBtn)
        {
            if (selectedBtn == null) return;
            Panel container = selectedBtn.Parent as Panel;
            if (container == null) return;

            foreach (var child in container.Children)
            {
                Button btn = child as Button;
                if (btn != null)
                {
                    if (btn == selectedBtn)
                    {
                        btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2222"));
                        btn.BorderThickness = new Thickness(2);
                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A1212"));
                        btn.Foreground = Brushes.White;
                    }
                    else
                    {
                        btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E"));
                        btn.BorderThickness = new Thickness(1);
                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                        btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
                    }
                }
            }
        }

        private void HairStyle_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedHairStyle = int.Parse(btn.Tag.ToString());
                HighlightSelectedButton(btn);
                UpdatePreview();
            }
        }

        private void Outfit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedOutfit = int.Parse(btn.Tag.ToString());
                HighlightSelectedButton(btn);
                UpdatePreview();
            }
        }

        private void Accessory_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedAccessory = int.Parse(btn.Tag.ToString());
                HighlightSelectedButton(btn);
                UpdatePreview();
            }
        }

        private void Tattoo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedTattoo = int.Parse(btn.Tag.ToString());
                HighlightSelectedButton(btn);
                UpdatePreview();
            }
        }

        private void InputChanged(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            // Ensure visual elements exist
            if (avatarCanvas == null) return;

            // 1. Update Hair Style Visibilities & Colors
            var neonColor = (Color)ColorConverter.ConvertFromString(_selectedHairColor);
            var hairBrush = new SolidColorBrush(neonColor);

            hairSpiky.Visibility = (_selectedHairStyle == 1) ? Visibility.Visible : Visibility.Collapsed;
            hairMohawk.Visibility = (_selectedHairStyle == 2) ? Visibility.Visible : Visibility.Collapsed;
            hairPompadour.Visibility = (_selectedHairStyle == 3) ? Visibility.Visible : Visibility.Collapsed;
            hairGamer.Visibility = (_selectedHairStyle == 4) ? Visibility.Visible : Visibility.Collapsed;
            hairWavy.Visibility = (_selectedHairStyle == 5) ? Visibility.Visible : Visibility.Collapsed;

            // Apply Neon color to active hair
            hairSpiky.Fill = hairBrush;
            hairMohawk.Fill = hairBrush;
            hairPompadour.Fill = hairBrush;
            hairGamer.Fill = hairBrush;
            hairWavy.Stroke = hairBrush; // Wavy is drawn as thick stroke

            // Update Aura Color
            avatarGlowBorder.BorderBrush = hairBrush;
            if (glowShadow != null)
            {
                glowShadow.Color = neonColor;
            }

            // 2. Update Clothing Visibilities
            clothingTshirt.Visibility = (_selectedOutfit == 0) ? Visibility.Visible : Visibility.Collapsed;
            clothingHoodie.Visibility = (_selectedOutfit == 1) ? Visibility.Visible : Visibility.Collapsed;
            clothingJacket.Visibility = (_selectedOutfit == 2) ? Visibility.Visible : Visibility.Collapsed;
            clothingSuit.Visibility = (_selectedOutfit == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 3. Update Accessories Visibilities
            accHeadphonesCanvas.Visibility = (_selectedAccessory == 1) ? Visibility.Visible : Visibility.Collapsed;
            accGlassesCanvas.Visibility = (_selectedAccessory == 2) ? Visibility.Visible : Visibility.Collapsed;
            accCapCanvas.Visibility = (_selectedAccessory == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 4. Update Tattoos Visibilities
            tattooGeek.Visibility = (_selectedTattoo == 2) ? Visibility.Visible : Visibility.Collapsed;
            tattooTribal.Visibility = (_selectedTattoo == 1) ? Visibility.Visible : Visibility.Collapsed;
            tattooGamer.Visibility = (_selectedTattoo == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 5. Update Texts
            txtSummaryChannel.Text = "CANAL: " + (string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text);
            txtSummaryOutfit.Text = "👕 " + _outfits[_selectedOutfit];
            txtSummaryAccessory.Text = "🎧 " + _accessories[_selectedAccessory];
            txtSummaryTattoo.Text = "💉 " + _tattoos[_selectedTattoo];
            txtSummaryHair.Text = "⚡ Cabelo " + _hairNames[_selectedHairStyle] + " " + _selectedHairName;
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            // Apply customization details to current Player instance
            Player.Current.YoutuberName = string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text;
            
            // In Phase 2, AvatarId represents the chosen hair style index
            Player.Current.YoutuberAvatarId = _selectedHairStyle;
            Player.Current.YoutuberHairColor = _selectedHairColor;
            Player.Current.YoutuberOutfit = _outfits[_selectedOutfit];
            Player.Current.YoutuberAccessories = _accessories[_selectedAccessory];
            Player.Current.YoutuberTattoos = _tattoos[_selectedTattoo];

            // Create a real initial channel for the player using the chosen name!
            Channel realChannel = new Channel()
            {
                Name = Player.Current.YoutuberName,
                Advertising = AdvertisingStrategy.Normal
            };

            // Add the real channel to the list (position 1)
            if (Player.Current.Channels != null)
            {
                Player.Current.Channels.Add(realChannel);
            }

            // Raise StartGameClicked event to transition UI
            if (StartGameClicked != null)
            {
                StartGameClicked();
            }
        }
    }
}
