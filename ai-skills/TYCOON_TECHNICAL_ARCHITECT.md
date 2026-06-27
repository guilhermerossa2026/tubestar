# ARQUITETURA TÉCNICA EM TYCOONS

## MISSÃO
Garantir que a arquitetura do código em C# e WPF seja limpa, modular, de alta performance e desacoplada, facilitando a adição de novos recursos e a manutenção da estabilidade do estado do jogo durante a progressão do jogador.

---

## DIRETRIZES DE ARQUITETURA DE SIMULADORES

### 1. Desacoplamento via Eventos (Event-Driven Architecture)
A lógica do jogo (Models/Game Logic) **nunca** deve conhecer a UI (Views/MainPage). Toda comunicação da simulação para a interface deve ocorrer por meio de eventos:
- Crie eventos como `event Action MoneyChanged` ou `event Action ChannelCreated` no `Player` ou nos modelos.
- A View se inscreve nesses eventos ao inicializar e atualiza seus componentes visuais apenas quando notificada.
- Isso previne bugs onde o dinheiro é alterado em segundo plano mas a UI exibe o valor antigo (dessincronização).

### 2. Implementação Limpa de MVVM no WPF
- **View (XAML)**: Apenas define o layout visual, bindings de dados e recursos de estilo. O arquivo `.xaml.cs` deve conter apenas inicializações e gatilhos de UI muito específicos.
- **Model**: Classes puras que representam os dados do jogo (`Player`, `Channel`, `Video`, `Company`).
- **ViewModel**: A ponte que formata os dados do Model para consumo da UI e implementa `INotifyPropertyChanged` para atualizar automaticamente a tela em tempo real.

### 3. Ciclo de Simulação por Ticks (`NextDay` ou `AdvanceTurn`)
Toda a atualização do estado do jogo deve ser processada em uma única esteira sequencial e segura de simulação para evitar condições de corrida (race conditions) ou falhas parciais:

```csharp
public void ProcessNextDay()
{
    // 1. Processar canais e vídeos (Visualizações e Inscritos de ontem)
    UpdateVideoStats();

    // 2. Coletar e calcular receitas (AdSense, Patrocínios, Investimentos)
    CalculateRevenues();

    // 3. Processar despesas e manutenção (Custo de vida, Impostos, Juros)
    DeductExpensesAndTaxes();

    // 4. Executar eventos de simulação diários (Chance de eventos narrativos)
    TriggerRandomEvents();

    // 5. Atualizar tendências de mercado e expirações de contratos
    UpdateTrendsAndSponsors();

    // 6. Notificar UI das mudanças globais de estado
    NotifyStateChanged();
}
```

### 4. Salvamento Seguro (Serialization Guardrails)
Como tycoons envolvem muito tempo de jogo, manter compatibilidade de salvamento (`Save Game`) é crucial:
- Ao adicionar novas propriedades à classe `Player`, garanta que elas tenham valores padrão adequados para saves antigos (ex: `Reset()` atualizando novas propriedades se vierem nulas).
- Prefira serialização XML/JSON estruturada com tags explícitas e classes DTO (Data Transfer Object) se a lógica interna for complexa demais.

### 5. Arquivos de Dados Externos (Modding & Config)
Retire do código dados de tabelas como Cursos de Faculdade, Tipos de Câmera, Itens de Customização e Empresas. Armazene-os em arquivos de configuração (`JSON` ou `XML`) no diretório de recursos do jogo. Isso permite:
- Balanceamento rápido apenas alterando o arquivo JSON de dados.
- Suporte nativo para modding e traduções pela comunidade.

---

## CHECKLIST DE REVISÃO TÉCNICA

- [ ] **Desacoplamento**: O model proposto faz referência a algum componente UI (ex: `TextBlock` ou `MessageBox`)? Se sim, remova-o e substitua por eventos ou retorno de status.
- [ ] **Tratamento de Exceções**: Se um cálculo financeiro der overflow ou tentar processar valores nulos, o jogo continuará rodando ou fechará abruptamente?
- [ ] **Consistência de Estado**: Todas as coleções (`List<Channel>`, `List<Video>`) são inicializadas no construtor para evitar `NullReferenceException`?
- [ ] **Performance de Simulação**: O laço de atualização diária escala bem se o jogador tiver 100 canais e 10.000 vídeos cadastrados?
