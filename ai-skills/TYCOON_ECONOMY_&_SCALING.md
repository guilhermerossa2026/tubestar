# TYCOON ECONOMY & SCALING

## MISSÃO
Controlar a inflação, criar dinâmicas saudáveis de "Money Sources" (fontes de renda) e "Money Sinks" (ralos de dinheiro), e garantir que a progressão financeira do jogador seja desafiadora e realista em todas as fases da sua carreira.

---

## MODELO ECONÔMICO DE TUBE STAR

### 1. Money Sources (Fontes de Receita)
* **AdSense (Visualizações)**: CPM (Custo por Mil Visualizações). A fórmula deve considerar a demografia do canal e se o conteúdo é monetizável:
  $$\text{Receita} = \left(\frac{\text{Views}}{1000}\right) \times \text{CPM} \times \text{Fator de Qualidade}$$
* **Sponsorships (Patrocínios)**: Contratos temporários com base no tamanho do canal. Exigem entrega de vídeos patrocinados com frequência específica.
* **Merchandising (Produtos)**: Renda passiva baseada em lealdade e tamanho da comunidade.
* **Investimentos Corporativos**: Retorno sobre ações compradas (STB, PEAR, RVG, GDR, WHP) e lucros de empresas adquiridas.

### 2. Money Sinks (Gastos e Manutenção)
* **Custo de Vida**: Custos diários automáticos que aumentam conforme o jogador melhora seu estilo de vida (lifestyle inflation).
* **Upgrades de Equipamento**: Compras únicas com valores exponencialmente maiores.
* **Taxas e Impostos**: Declaração diária de impostos de renda baseado em `TaxDeclarationRate`. Evasão fiscal aumenta a grana imediata mas acumula `UnpaidEvadedTaxes` e gera risco de auditoria do fisco com multas pesadíssimas.
* **Folha de Pagamento**: Salários de editores, roteiristas e assessores.

---

## REGRAS DE BALANCEAMENTO FINANCEIRO

### A Regra de Ouro da Taxa Source/Sink
A proporção ideal entre geração e queima de dinheiro varia conforme a fase do jogador:
- **Early Game (Iniciante)**: Ratio $\approx 1.2$ (Jogador mal consegue sobreviver, cada centavo conta).
- **Mid Game (Profissional)**: Ratio $\approx 1.5$ (Início da acumulação, compra de ativos como ações).
- **Late Game (Empresário/Império)**: Ratio $\ge 2.0$ (Grande acúmulo, mas com custos fixos brutais de empresas e servidores para forçar reinvestimento).

### Fórmulas de Investimento e Risco
O mercado de ações e investimentos em imóveis devem possuir volatilidade guiada por eventos mundiais.
- Se o jogador investe tudo em ações de uma única empresa (ex: GDR), um evento de crise tecnológica aleatório deve ter potencial de desvalorizar o ativo em 30% a 50% em um único dia.

---

## CHECKLIST DA ECONOMIA

- [ ] **Relação de Esforço**: O retorno financeiro reflete a dificuldade e o tempo investidos no conteúdo?
- [ ] **Curva de Lifestyle Inflation**: O jogador é punido se comprar uma mansão antes de ter uma receita de AdSense estável?
- [ ] **Prevenção de Exploits**: Existe alguma ação repetível que gere lucro infinito em curto tempo (ex: spam de vídeos curtos)? Se sim, aplique penalidades de algoritmo de busca.
- [ ] **Risco de Evasão Fiscal**: Os benefícios de sonegar impostos são tentadores, mas o risco matemático de falência ou multas é alto o suficiente?
