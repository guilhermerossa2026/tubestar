using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TubeStar
{
    public partial class CharacterCreationPage : UserControl
    {
        public event Action StartGameClicked;

        private int _selectedAvatarIndex = 0;

        private readonly string[] _avatarUris = new string[] {
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_1.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_2.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_3.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_mauricinho_4.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_gangstar_1.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_gangstar_2.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelolongo_1.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelolongo_2.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelocurto_1.png",
            "pack://application:,,,/TubeStarDesktop;component/Resources/avatar_cabelocurto_2.png"
        };

        private readonly string[] _avatarNames = new string[] {
            "Mauricinho Executivo",
            "Mauricinho Casual",
            "Empresário Jovem",
            "Empresário Maduro",
            "Trap Messy Pink",
            "Trap Hip-Hop",
            "Casual E-boy",
            "Casual Rocker",
            "Casual Cyberpunk",
            "Casual Contemporâneo"
        };

        private readonly string[] _avatarStyles = new string[] {
            "Terno Premium Cinza",
            "Suéter de Lã Azul Premium",
            "Terno Azul de Fino Trato",
            "Camisa Preta de Sucesso",
            "Jaqueta Puffer Preta de Grife",
            "Jaqueta Bomber Vermelha",
            "Jaqueta Jeans com Ovelha",
            "Jaqueta de Couro Rocker",
            "Windbreaker Neon Militar",
            "Moletom Oversized Branco"
        };

        private readonly string[] _avatarTattoos = new string[] {
            "Nenhuma (Barba Alinhada)",
            "Nenhuma (Cabelo Social)",
            "Nenhuma (Fade Degradê)",
            "Nenhuma (Barba Cerrada)",
            "Trevo no Pescoço & Face",
            "Tranças com Dreads Dourados",
            "Colar de Couro (Estilo Indie)",
            "Cavanhaque Moderno",
            "Riscos no Buzzcut Verde",
            "Barba de Linha Fina"
        };

        private readonly string[] _avatarAccessories = new string[] {
            "Óculos Escuros Italianos",
            "Colarinho de Lã Fina",
            "Gravata de Seda Fina",
            "Gola Aberta Confiante",
            "Corrente de Ouro com Pingente $",
            "Correntes de Prata de Grife",
            "Cabelo Longo Despojado",
            "Cabelo Longo Platinado",
            "Windbreaker Neon de Rua",
            "Corte Degradê Contemporâneo"
        };

        private readonly string[] _avatarProfiles = new string[] {
            "Empresário Social Moderno",
            "Empresário Jovem Confiante",
            "Empresário Altamente Focado",
            "Empresário de Sucesso Maduro",
            "Trap Star Ousado e Estiloso",
            "Trap Star Clássico de Grife",
            "Casual Moderno com Vibe Indie",
            "Casual com Atitude Alternativa",
            "Casual Streetwear de Vanguarda",
            "Casual Urbano Contemporâneo"
        };

        private readonly string[] _avatarGlowColors = new string[] {
            "#FFDD44", // Executivo - Dourado
            "#00FFFF", // Casual - Cyan
            "#FF2222", // Jovem - Vermelho Neon
            "#FF5500", // Maduro - Laranja de Sucesso
            "#FF00FF", // Trap Messy - Pink Neon
            "#FF2222", // Trap Hip-Hop - Vermelho Neon
            "#00FF00", // E-boy - Verde Neon
            "#FFFFFF", // Rocker - Branco Puro
            "#00FF00", // Cyberpunk - Verde Neon
            "#8A2BE2"  // Contemporâneo - Roxo Neon
        };

        public CharacterCreationPage()
        {
            InitializeComponent();
            SelectAvatar(0);
        }

        private void HighlightSelectedAvatarButton(int index)
        {
            Button[] buttons = new Button[] {
                btnAvatar1, btnAvatar2, btnAvatar3, btnAvatar4,
                btnAvatar5, btnAvatar6,
                btnAvatar7, btnAvatar8, btnAvatar9, btnAvatar10
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                var btn = buttons[i];
                if (btn != null)
                {
                    if (i == index)
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

        private void Avatar_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int index = int.Parse(btn.Tag.ToString());
                SelectAvatar(index);
            }
        }

        private void SelectAvatar(int index)
        {
            _selectedAvatarIndex = index;
            HighlightSelectedAvatarButton(index);
            UpdatePreview();
        }

        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (imgAvatarPreview == null) return;

            try
            {
                string packUri = _avatarUris[_selectedAvatarIndex];
                imgAvatarPreview.Source = new BitmapImage(new Uri(packUri, UriKind.Absolute));

                var glowColor = (Color)ColorConverter.ConvertFromString(_avatarGlowColors[_selectedAvatarIndex]);
                var glowBrush = new SolidColorBrush(glowColor);

                if (avatarGlowBorder != null)
                    avatarGlowBorder.BorderBrush = glowBrush;
                
                if (glowShadow != null)
                    glowShadow.Color = glowColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao carregar preview do avatar: " + ex.Message);
            }

            if (txtSummaryChannel != null)
                txtSummaryChannel.Text = "CANAL: " + (string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text);

            if (txtSummaryOutfit != null)
                txtSummaryOutfit.Text = "👕 Estilo: " + _avatarStyles[_selectedAvatarIndex];

            if (txtSummaryTattoo != null)
                txtSummaryTattoo.Text = "💉 Detalhes: " + _avatarTattoos[_selectedAvatarIndex];

            if (txtSummaryAccessory != null)
                txtSummaryAccessory.Text = "💍 Acessórios: " + _avatarAccessories[_selectedAvatarIndex];

            if (txtSummaryHair != null)
                txtSummaryHair.Text = "⚡ Perfil: " + _avatarProfiles[_selectedAvatarIndex];
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            Player.Current.YoutuberName = string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text;
            
            Player.Current.YoutuberAvatarId = _selectedAvatarIndex;
            Player.Current.YoutuberHairColor = _avatarGlowColors[_selectedAvatarIndex];
            Player.Current.YoutuberOutfit = _avatarStyles[_selectedAvatarIndex];
            Player.Current.YoutuberAccessories = _avatarAccessories[_selectedAvatarIndex];
            Player.Current.YoutuberTattoos = _avatarTattoos[_selectedAvatarIndex];

            Channel realChannel = new Channel()
            {
                Name = Player.Current.YoutuberName,
                Advertising = AdvertisingStrategy.Normal
            };

            if (Player.Current.Channels != null)
            {
                Player.Current.Channels.Clear(); // Limpar canais gerados incorretamente por fallbacks
                Player.Current.Channels.Add(realChannel);
            }

            if (StartGameClicked != null)
            {
                StartGameClicked();
            }
        }
    }
}
