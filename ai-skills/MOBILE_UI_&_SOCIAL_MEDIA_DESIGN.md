# DESIGN DE SMARTPHONE E REDES SOCIAIS (INSTAFANS UI/UX)

## MISSÃO
Projetar e otimizar a réplica do smartphone e o feed do InstaFans (réplica do Instagram) em WPF/XAML, aplicando conceitos de design de interface móvel moderno, hierarquia de informações e micro-animações interativas para criar um simulador de smartphone que pareça vivo e premium.

---

## ESTRUTURAÇÃO DO CORPO DO SMARTPHONE (CELLPHONE MOCKUP)
Para que a moldura do celular pareça um hardware real em vez de uma caixa de layout simples:

### 1. Moldura Física e Tela (Bezel & Notch)
- **Borda do Celular**: Use uma `Border` externa com `CornerRadius="28"`, `BorderThickness="6"` cor `#2D2D30` e uma sombra suave (`DropShadowEffect` com opacidade `0.5`, Blur `15`).
- **Dynamic Island / Notch**: Um elemento centralizado no topo (`Grid` com largura `110`, altura `22`, `CornerRadius="11"` e fundo `#000000`). Pode exibir ícones críticos de notificação em miniatura.
- **Barra de Home**: Um indicador na base da tela (um `Border` horizontal com `Height="4"`, `Width="100"`, `Background="#666666"`, `CornerRadius="2"`, centralizado na base).

### 2. Barra de Status (Status Bar)
- **Informações do Topo**: Alinhada na altura do Notch.
  * *Esquerda*: Relógio digital atual do jogo (Ex: `09:41`).
  * *Direita*: Ícones de conectividade (`5G` ou `Wi-Fi` dependendo da moradia do jogador) e um ícone de Bateria (`🔋`) cuja cor muda dinamicamente (vermelho se bateria/energia <20, verde se cheia).

---

## DESIGN DO INSTAFANS (INSTAGRAM REPLICA)

### 1. Identidade Visual e Header
- **Gradiente do Topo**: Use um cabeçalho com gradiente característico da marca InstaFans (transição suave de roxo, rosa neon e laranja):
  ```xml
  <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
      <GradientStop Color="#833AB4" Offset="0.0"/>
      <GradientStop Color="#FD1D1D" Offset="0.5"/>
      <GradientStop Color="#FCAF45" Offset="1.0"/>
  </LinearGradientBrush>
  ```
- **Ícone do Perfil**: Uma foto circular com uma borda gradiente representando "Stories" ativos.

### 2. Cards de Feed de Postagens
Cada foto publicada pelo jogador deve renderizar um card completo no feed do InstaFans:
* **Cabeçalho do Card**: Nome do YouTuber com avatar pequeno ao lado e indicador de tempo relativo (Ex: `há 2h`).
* **Mídia**: Imagem simulada ou representação em ícone do post com dimensões quadradas (`Stretch="UniformToFill"`).
* **Barra de Interação**: Botões de ação rápida estilizados:
  - Botão de Curtir (Ícone de Coração `🤍` que vira `❤️` em vermelho com animação de pulso ao ser clicado).
  - Botão de Comentários (Bolinha de conversa `💬`).
* **Contador de Engajamento**: Texto em destaque "Curtido por **1.234 pessoas**".
* **Legenda**: Legenda escrita pelo jogador com hashtags destacadas em azul neon (Ex: `#GamerLife #Vlog`).

---

## MICRO-ANIMAÇÕES E TRANSIÇÕES DE INTERFACE

### 1. Transição de Abas
Ao alternar entre a aba do *TubeAnalyzer* e do *InstaFans* no dock inferior do celular:
- Não oculte de forma seca (`Visibility="Collapsed"`).
- Aplique uma animação de fade-in e deslize lateral (`TranslateTransform` animado via `Storyboard` no WPF) para simular a navegação suave do iOS/Android.

### 2. Notificações do Sistema (Push Alerts)
- Quando um vídeo novo viralizar no canal principal, exiba um banner de notificação estilo push descendo do topo do smartphone (`Slide-down Animation`), brilhando levemente para prender a atenção do jogador sem interromper seu fluxo atual.

---

## CHECKLIST DE REDES SOCIAIS E MOBILE UI

- [ ] **Mimetismo Realista**: O celular do jogo se comporta e se parece com um smartphone moderno em termos de proporção, bordas e detalhes?
- [ ] **Suco de Curtidas**: O botão de like do InstaFans aciona uma animação visual satisfatória de escala ao receber o clique do jogador?
- [ ] **Hierarquia das Fotos**: A imagem do post e o número de curtidas se destacam mais do que os botões de navegação secundários?
- [ ] **Design Responsivo**: Ao redimensionar a janela do Tube Star, a escala do smartphone permanece proporcional ao painel lateral do jogo sem distorcer as fotos?
