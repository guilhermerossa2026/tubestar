# SISTEMA DE IMPOSTOS E RISCO LEGAL (LEGAL & TAX RUN)

## MISSÃO
Projetar e balancear a mecânica de evasão fiscal e riscos legais. Este sistema adiciona tensão dramática à simulação financeira do Tube Star, forçando o jogador a gerenciar o trade-off de reter mais dinheiro imediato através de fraudes fiscais versus o risco de falência devido a auditorias da Receita Federal.

---

## MECÂNICA DE DECLARAÇÃO E EVASÃO FISCAL

### 1. Declaração Diária
A cada turno, o jogador escolhe sua taxa de integridade fiscal (`TaxDeclarationRate` de `0.0` a `1.0`):
- **Declarado**: `YesterdayDeclaredRevenue = YesterdayRevenue * TaxDeclarationRate`
- **Imposto Pago**: `YesterdayDeclaredRevenue * TaxRate` (Alíquota de imposto, ex: `15%`)
- **Sonegado**: `UnpaidEvadedTaxes` acumula o montante sonegado não pago:
  $$\Delta \text{Sonegado} = (\text{YesterdayRevenue} - \text{YesterdayDeclaredRevenue}) \times \text{TaxRate}$$

---

## O ALGORITMO DA AUDITORIA FISCAL (TAX AUDIT)

### 1. Risco Temido de Auditoria
O risco diário da Receita Federal iniciar uma investigação aumenta com o acúmulo de sonegação acumulada:

$$P_{\text{auditoria}} = \min\left(1.0, \frac{\text{UnpaidEvadedTaxes}}{15.000} \times (1.0 - \text{FatorSegurança})\right)$$

* Se o jogador contratar um **Contador (`IsAccountantHired`)**: `FatorSegurança = 0.60` (60% de proteção contra auditoria) e reduz em 10% a alíquota legal de impostos através de elisão fiscal.
* Se contratar um **Advogado Tributário (`IsTaxAttorneyHired`)**: O jogador tem proteção especial durante a auditoria (evita bloqueios de conta imediatos).

### 2. Consequências da Auditoria
Caso ocorra uma auditoria bem-sucedida:
* **Multa de Evasão**: O jogador é condenado a pagar o valor total de `UnpaidEvadedTaxes` acrescido de uma multa punitiva de 50%.
* **Dívida Ativa (`TaxDebtAmount`)**: Se o jogador não tiver saldo líquido para quitar a multa imediatamente:
  - Todo o saldo diário é penhorado (zerando o dinheiro disponível).
  - Imóveis e ações são liquidados compulsoriamente a preço de banana (50% do valor de mercado) até saldar a dívida.
  - Taxa de juros de mora de 3% por turno é aplicada sobre o `TaxDebtAmount` restante.

---

## SUBSÍDIOS CULTURAIS (TAX SHELTERS)
Para jogadores que desejam jogar de forma 100% legal e ética, o governo oferece incentivos:
- **Subsídio Governamental**: Paga uma quantia diária extra fixa (ex: $200 por dia) para canais com foco estrito em educação, ciência ou cultura.
- **Contrapartida**: O jogador é obrigado a manter `TaxDeclarationRate = 1.0` (100% de transparência) e perde o subsídio caso publique conteúdos de clickbait ou polêmicos.

---

## CHECKLIST DO SISTEMA FISCAL

- [ ] **Alinhamento do Risco**: A sonegação no início do jogo deve ser tentadora devido à escassez de recursos, mas punitiva caso o jogador abuse de forma contínua?
- [ ] **Visibilidade de Ameaça**: A interface mostra uma estimativa do risco de auditoria de forma clara (ex: um medidor de risco "Baixo/Médio/Crítico") ou mantém o risco oculto para criar suspense? (Recomenda-se um medidor "Alerta do Contador").
- [ ] **Recuperação de Dívida**: Há uma mecânica viável para o jogador declarar reabilitação fiscal (parcelar dívidas) em vez de ser forçado à falência sem saída?
- [ ] **Sinergia Financeira**: O mercado de ações e as propriedades imobiliárias reagem se o nome do jogador for envolvido em escândalos de evasão fiscal divulgados na rede social?
