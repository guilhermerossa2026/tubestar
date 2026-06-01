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

        // RPG Base Limpa: O personagem inicia careca, sem barba, de cueca, sem acessórios e sem tatuagens!
        private int _selectedHairStyle = 0; // 0=Careca/Nenhum
        private string _selectedHairColor = "#FF00FF"; // default Pink
        private string _selectedHairName = "Pink";

        private int _selectedBeard = 0; // 0=Sem Barba, 1=Barba Fechada, 2=Cavanhaque
        private int _selectedOutfit = -1; // -1=Só Cueca, 0=Tshirt, 1=Hoodie, 2=Jacket, 3=Suit
        private int _selectedAccessory = 0; // 0=Nenhum, 1=Headphones, 2=Glasses, 3=Chain
        private int _selectedTattoo = 0; // 0=Nenhuma, 1=Clover, 2=Tribal, 3=Geek

        // Neon color palette definitions
        private readonly Tuple<string, string>[] _hairColors = new Tuple<string, string>[]
        {
            Tuple.Create("#FF00FF", "Pink"),
            Tuple.Create("#00FFFF", "Cyan"),
            Tuple.Create("#00FF00", "Green"),
            Tuple.Create("#FF5500", "Orange"),
            Tuple.Create("#FFDD44", "Yellow"),
            Tuple.Create("#FF2222", "Red"),
            Tuple.Create("#8A2BE2", "Purple"),
            Tuple.Create("#FFFFFF", "White")
        };

        private readonly string[] _outfits = new string[] { "Camiseta Gamer", "Moletom de Rua", "Jaqueta Puffer Preta", "Terno Premium" };
        private readonly string[] _accessories = new string[] { "Nenhum", "Headphones Pro", "Óculos Escuros", "Corrente de Ouro $" };
        private readonly string[] _tattoos = new string[] { "Nenhuma", "Trevo no Pescoço", "Tribal Manga", "Símbolos Geek" };
        private readonly string[] _beards = new string[] { "Sem Barba", "Barba Fechada", "Cavanhaque" };
        private readonly string[] _hairNames = new string[] { "Careca", "Arrepiado", "Mohawk", "Topete", "Gamer", "Messy Pink", "Longos" };

        private List<Button> _colorButtons = new List<Button>();

        public CharacterCreationPage()
        {
            InitializeComponent();
            SetupHairColorPanel();
            
            // Set initial highlights on RPG grids to align with the "Naked/Clean" default state
            HighlightSelectedButton(btnHairNone);
            HighlightSelectedButton(btnBeardNone);
            HighlightSelectedButton(btnOutfitNone);
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

            SelectColorButton("#FF00FF"); // default Pink
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

        private void Beard_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                _selectedBeard = int.Parse(btn.Tag.ToString());
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

            // 1. Update Hair Style Visibilities & Neon Colors
            var neonColor = (Color)ColorConverter.ConvertFromString(_selectedHairColor);
            var hairBrush = new SolidColorBrush(neonColor);

            hairSpiky.Visibility = (_selectedHairStyle == 1) ? Visibility.Visible : Visibility.Collapsed;
            hairMohawk.Visibility = (_selectedHairStyle == 2) ? Visibility.Visible : Visibility.Collapsed;
            hairPompadour.Visibility = (_selectedHairStyle == 3) ? Visibility.Visible : Visibility.Collapsed;
            hairGamer.Visibility = (_selectedHairStyle == 4) ? Visibility.Visible : Visibility.Collapsed;
            hairMessy.Visibility = (_selectedHairStyle == 5) ? Visibility.Visible : Visibility.Collapsed;
            hairWavy.Visibility = (_selectedHairStyle == 6) ? Visibility.Visible : Visibility.Collapsed;

            // Apply active Neon color brush to hair styles
            hairSpiky.Fill = hairBrush;
            hairMohawk.Fill = hairBrush;
            hairPompadour.Fill = hairBrush;
            hairGamer.Fill = hairBrush;
            hairMessy.Fill = hairBrush;
            hairWavy.Stroke = hairBrush; // Wavy long hair uses thick stroke

            // Update Aura surrounding frame to sintonize with Neon hair color (if has hair, otherwise default red neon)
            if (glowShadow != null)
            {
                if (_selectedHairStyle == 0)
                {
                    avatarGlowBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2222"));
                    glowShadow.Color = (Color)ColorConverter.ConvertFromString("#FF2222");
                }
                else
                {
                    avatarGlowBorder.BorderBrush = hairBrush;
                    glowShadow.Color = neonColor;
                }
            }

            // 2. Update Beard Visibilities
            beardFull.Visibility = (_selectedBeard == 1) ? Visibility.Visible : Visibility.Collapsed;
            beardGoatee.Visibility = (_selectedBeard == 2) ? Visibility.Visible : Visibility.Collapsed;

            // 3. Update Clothing Visibilities
            clothingTshirt.Visibility = (_selectedOutfit == 0) ? Visibility.Visible : Visibility.Collapsed;
            clothingHoodie.Visibility = (_selectedOutfit == 1) ? Visibility.Visible : Visibility.Collapsed;
            clothingJacket.Visibility = (_selectedOutfit == 2) ? Visibility.Visible : Visibility.Collapsed;
            clothingSuit.Visibility = (_selectedOutfit == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 4. Update Accessories Visibilities
            accPhones.Visibility = (_selectedAccessory == 1) ? Visibility.Visible : Visibility.Collapsed;
            accGlasses.Visibility = (_selectedAccessory == 2) ? Visibility.Visible : Visibility.Collapsed;
            accChain.Visibility = (_selectedAccessory == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 5. Update Tattoos Visibilities
            tattooClover.Visibility = (_selectedTattoo == 1) ? Visibility.Visible : Visibility.Collapsed;
            tattooTribal.Visibility = (_selectedTattoo == 2) ? Visibility.Visible : Visibility.Collapsed;
            tattooGeek.Visibility = (_selectedTattoo == 3) ? Visibility.Visible : Visibility.Collapsed;

            // 6. Update Texts on the Summary Card
            if (txtSummaryChannel != null)
                txtSummaryChannel.Text = "CANAL: " + (string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text);
            
            if (txtSummaryOutfit != null)
                txtSummaryOutfit.Text = "👕 " + (_selectedOutfit == -1 ? "Só Cueca" : _outfits[_selectedOutfit]);
            
            if (txtSummaryAccessory != null)
                txtSummaryAccessory.Text = "💍 " + _accessories[_selectedAccessory];
            
            if (txtSummaryTattoo != null)
                txtSummaryTattoo.Text = "💉 " + _tattoos[_selectedTattoo];
            
            if (txtSummaryHair != null)
            {
                if (_selectedHairStyle == 0)
                {
                    txtSummaryHair.Text = "⚡ Careca";
                }
                else
                {
                    txtSummaryHair.Text = "⚡ Cabelo " + _hairNames[_selectedHairStyle] + " " + _selectedHairName;
                }
            }
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            // Apply customization details to current Player instance
            Player.Current.YoutuberName = string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text;
            
            Player.Current.YoutuberAvatarId = _selectedHairStyle;
            Player.Current.YoutuberHairColor = _selectedHairColor;
            Player.Current.YoutuberOutfit = (_selectedOutfit == -1 ? "Só Cueca" : _outfits[_selectedOutfit]);
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
