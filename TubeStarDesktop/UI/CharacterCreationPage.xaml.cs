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
            "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_1.png",
            "pack://application:,,,/TubeStar;component/Resources/avatar_mauricinho_2.png",
            "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_1.png",
            "pack://application:,,,/TubeStar;component/Resources/avatar_gangstar_2.png",
            "pack://application:,,,/TubeStar;component/Resources/avatar_casual_3.png",
            "pack://application:,,,/TubeStar;component/Resources/avatar_casual_4.png"
        };

        private readonly string[] _avatarNames = new string[] {
            "Empresário Jovem",
            "Empresária Jovem",
            "Trap Estilo Pink",
            "Trap Streetwear",
            "Casual Gamer Boy",
            "Casual Indie Girl"
        };

        private readonly string[] _avatarStyles = new string[] {
            "Blazer Slim Azul Marinho",
            "Blazer Executivo & Óculos",
            "Jaqueta Puffer & Óculos de Sol",
            "Moletom Streetwear & Correntes",
            "Moletom Gamer & Headset",
            "Jaqueta Retro & Cabelo Colorido"
        };

        private readonly string[] _avatarTattoos = new string[] {
            "Nenhuma (Social Alinhado)",
            "Nenhuma (Look Intelectual)",
            "Tatuagem de Trevo no Pescoço",
            "Estilo Buzzcut & Tranças",
            "Visual Confortável",
            "Visual Indie Despojado"
        };

        private readonly string[] _avatarAccessories = new string[] {
            "Visual Limpo & Confiante",
            "Armação de Grau Elegante",
            "Correntes de Ouro Rosé",
            "Correntes e Anel de Ouro",
            "Fones de Ouvido Estéreo",
            "Brincos e Anel Moderno"
        };

        private readonly string[] _avatarProfiles = new string[] {
            "Empreendedor Tecnológico Jovem",
            "Fundadora de Startup focada",
            "Trap Star ousado com Puffer",
            "Estrela de Trap com Cap e Corrente",
            "Gamer Focado com Headset",
            "Streamer Casual com Estilo Alternativo"
        };

        private readonly string[] _avatarGlowColors = new string[] {
            "#FFDD44", // Executivo - Dourado
            "#FFDD44", // Executivo - Dourado
            "#FF00FF", // Trap - Pink Neon
            "#FF2222", // Trap - Vermelho Neon
            "#00FFFF", // Casual - Ciano Neon
            "#00FF00"  // Casual - Verde Neon
        };

        public CharacterCreationPage()
        {
            InitializeComponent();
            SelectAvatar(0);
        }

        private void HighlightSelectedAvatarButton(int index)
        {
            Button[] buttons = new Button[] {
                btnAvatar1, btnAvatar2,
                btnAvatar3, btnAvatar4,
                btnAvatar5, btnAvatar6
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

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            var activeTab = sender as System.Windows.Controls.Primitives.ToggleButton;
            if (activeTab == null) return;

            // Mantém apenas a aba clicada selecionada
            tabBusiness.IsChecked = activeTab == tabBusiness;
            tabTrap.IsChecked = activeTab == tabTrap;
            tabCasual.IsChecked = activeTab == tabCasual;

            // Ajusta a visibilidade das grades
            gridBusinessAvatars.Visibility = (activeTab == tabBusiness) ? Visibility.Visible : Visibility.Collapsed;
            gridTrapAvatars.Visibility = (activeTab == tabTrap) ? Visibility.Visible : Visibility.Collapsed;
            gridCasualAvatars.Visibility = (activeTab == tabCasual) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Perk_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (imgAvatarPreview == null) return;

            try
            {
                string packUri = _avatarUris[_selectedAvatarIndex];
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(packUri, UriKind.Absolute);
                bitmap.DecodePixelWidth = 512;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgAvatarPreview.Source = bitmap;

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
                txtSummaryChannel.Text = "YOUTUBER: " + (string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text);

            if (txtSummaryOutfit != null)
                txtSummaryOutfit.Text = "👕 Estilo: " + _avatarStyles[_selectedAvatarIndex];

            if (txtSummaryTattoo != null)
                txtSummaryTattoo.Text = "💉 Detalhes: " + _avatarTattoos[_selectedAvatarIndex];

            if (txtSummaryAccessory != null)
                txtSummaryAccessory.Text = "💍 Acessórios: " + _avatarAccessories[_selectedAvatarIndex];

            if (txtSummaryHair != null)
                txtSummaryHair.Text = "⚡ Perfil: " + _avatarProfiles[_selectedAvatarIndex];

            // Atualiza os valores dinâmicos dos atributos iniciais conforme o Perk selecionado
            if (txtSummaryMoney != null && txtSummaryShooting != null && txtSummaryEditing != null && txtSummarySpecial != null)
            {
                if (rbPerkStudent != null && rbPerkStudent.IsChecked == true)
                {
                    txtSummaryMoney.Text = "💵 Dinheiro: R$ 950";
                    txtSummaryShooting.Text = "🎥 Gravação: 30";
                    txtSummaryEditing.Text = "✂️ Edição: 40";
                    txtSummarySpecial.Text = "🔍 Analítico: Não";
                }
                else if (rbPerkGamer != null && rbPerkGamer.IsChecked == true)
                {
                    txtSummaryMoney.Text = "💵 Dinheiro: R$ 950";
                    txtSummaryShooting.Text = "🎥 Gravação: 50";
                    txtSummaryEditing.Text = "✂️ Edição: 20";
                    txtSummarySpecial.Text = "🔍 Analítico: Não";
                }
                else if (rbPerkHeir != null && rbPerkHeir.IsChecked == true)
                {
                    txtSummaryMoney.Text = "💵 Dinheiro: R$ 2.450";
                    txtSummaryShooting.Text = "🎥 Gravação: 30";
                    txtSummaryEditing.Text = "✂️ Edição: 20";
                    txtSummarySpecial.Text = "🔍 Analítico: Não";
                }
                else if (rbPerkAnalytic != null && rbPerkAnalytic.IsChecked == true)
                {
                    txtSummaryMoney.Text = "💵 Dinheiro: R$ 950";
                    txtSummaryShooting.Text = "🎥 Gravação: 30";
                    txtSummaryEditing.Text = "✂️ Edição: 20";
                    txtSummarySpecial.Text = "🔍 Analítico: Sim (Ativo)";
                }
            }
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            Player.Current.YoutuberName = string.IsNullOrEmpty(txtYoutuberName.Text) ? "Gamer Pro" : txtYoutuberName.Text;
            
            Player.Current.YoutuberAvatarId = _selectedAvatarIndex;
            Player.Current.YoutuberHairColor = _avatarGlowColors[_selectedAvatarIndex];
            Player.Current.YoutuberOutfit = _avatarStyles[_selectedAvatarIndex];
            Player.Current.YoutuberAccessories = _avatarAccessories[_selectedAvatarIndex];
            Player.Current.YoutuberTattoos = _avatarTattoos[_selectedAvatarIndex];

            // Aplica os bônus iniciais baseados no Perk selecionado
            if (rbPerkStudent != null && rbPerkStudent.IsChecked == true)
            {
                Player.Current.PostProductionSkill = 40; // +20 Edição
            }
            else if (rbPerkGamer != null && rbPerkGamer.IsChecked == true)
            {
                Player.Current.ShootingSkill = 50; // +20 Gravação
            }
            else if (rbPerkHeir != null && rbPerkHeir.IsChecked == true)
            {
                Player.Current.Money = 2450; // +R$ 1500
            }
            else if (rbPerkAnalytic != null && rbPerkAnalytic.IsChecked == true)
            {
                Player.Current.CanViewQualityBeforeUpload = true; // Analítico grátis
            }

            // Garantimos que a lista de canais criados comece 100% vazia (nenhum canal criado automaticamente),
            // mas mantemos obrigatoriamente o canal técnico interno UnreleasedVideos na lista para que o fluxo de rascunhos de vídeos funcione perfeitamente.
            if (Player.Current.Channels != null)
            {
                Player.Current.Channels.Clear(); 
                Player.Current.Channels.Add(Channel.UnreleasedVideos);
            }

            if (StartGameClicked != null)
            {
                StartGameClicked();
            }
        }
    }
}
