# UI/UX & GAME FEEL EM TYCOONS

## MISSÃO
Projetar e criticar interfaces do usuário (UI) construídas em WPF/XAML para garantir que sejam esteticamente impressionantes, extremamente legíveis, organizadas de forma intuitiva e repletas de "suco" (game feel) — o feedback dinâmico que faz o jogador sentir cada interação.

---

## DESIGN DE INTERFACE EM WPF/XAML
Ao propor ou editar telas no WPF do Tube Star, adote os seguintes padrões de design moderno:

### 1. Paleta de Cores & Estilo Premium
- **Fundo Escuro Principal (Dark Mode)**: `#121214` ou `#18181B`. Evita fadiga ocular durante longas sessões de jogo.
- **Cores de Destaque (Accent)**: Neon Cyan (`#00FFFF`), Neon Emerald (`#10B981`) ou Deep Violet (`#8B5CF6`).
- **Estados Visuais**: Botões desabilitados devem ser acinzentados, botões em hover devem ter uma leve elevação de brilho ou escala (ex: `ScaleTransform` animado).

### 2. Layouts Flexíveis
- **Nunca use posicionamento absoluto** (ex: `Canvas` com coordenadas fixas ou margens gigantescas para empurrar elementos).
- **Use `Grid` e `StackPanel`** de forma inteligente, configurando `Width="*"` e `Height="Auto"` para suporte a redimensionamentos.
- **Margens e Paddings Padronizados**: Grid de espaçamento baseado em potências de 2 (4px, 8px, 16px, 24px, 32px).

---

## GAME FEEL (JUICE / RETORNO VISUAL)
Um tycoon estático parece uma planilha do Excel. Devemos transformá-lo em um jogo vivo:

### 1. Indicadores Flutuantes e Mudanças de Status
- **Ganho de Inscritos/Dinheiro**: Adicionar textos flutuantes rápidos (`+520 Subscribers` em verde, `-$1,200` em vermelho) toda vez que um turno passa ou um vídeo é postado.
- **Cores Semânticas**: Dinheiro e ganhos devem usar verde limão/esmeralda; custos e multas devem usar vermelho coral/carmim.

### 2. Barras de Progresso Dinâmicas
- Barras de progresso de edição/gravação/estudo devem ter animações suaves de transição (usando `DoubleAnimation` no WPF) em vez de saltos bruscos.

### 3. Detalhamento via Tooltips
- Ao passar o mouse sobre qualquer métrica (ex: Receita Diária ou Qualidade do Vídeo), exiba um Tooltip estilizado detalhando a fórmula de cálculo (Ex: `Visualizações: 125.000 | CPM: $2.40 | Taxa de Declaração: 80% | Receita Líquida: $240`).

---

## CHECKLIST DE CRÍTICA DE INTERFACE (UX CRITIC)

- [ ] **Hierarquia Visual**: O elemento mais importante da tela (ex: botão de gravar ou avançar dia) é o mais chamativo?
- [ ] **Legibilidade**: O contraste entre o texto e o fundo respeita os padrões de acessibilidade (ex: texto claro em fundo escuro)?
- [ ] **Feedback de Cliques**: Cada botão clicável emite um feedback visual instantâneo (mudança de cor de borda, opacidade ou som)?
- [ ] **Poluição de Informações**: Há muitos números na tela de uma vez só? Se sim, utilize abas ou menus colapsáveis para organizar a informação.
- [ ] **Consistência**: As fontes e tamanhos seguem um padrão coeso por todo o projeto?
