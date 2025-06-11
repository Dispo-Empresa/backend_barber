namespace Dispo.Barber.Domain.Integration.SubscriptionClient.Models
{
    public enum SubscriptionStatus // Vai ser usado para o IoS também
    {
        /// <summary>
        /// Estado de assinatura não especificado.
        /// </summary>
        Unspecified,

        /// <summary>
        /// A assinatura foi criada, mas aguarda pagamento durante a inscrição. Neste estado, todos os itens aguardam pagamento.
        /// </summary>
        Pending,

        /// <summary>
        /// A assinatura está ativa. — (1) Se a assinatura for um plano com renovação automática, pelo menos um item será "autoRenewEnabled" e válido. — (2) Se a assinatura for um plano pré-pago, pelo menos um item será válido.
        /// </summary>
        Active,

        /// <summary>
        /// A assinatura está pausada. Este estado só fica disponível quando a assinatura é um plano com renovação automática. Neste estado, todos os itens também estão pausados.
        /// </summary>
        Paused,

        /// <summary>
        /// A assinatura está no período de carência. Este estado só fica disponível quando a assinatura é um plano com renovação automática. Neste estado, todos os itens também estão no período de carência.
        /// </summary>
        InGracePeriod,

        /// <summary>
        /// A assinatura está pausada (suspensa). Este estado só fica disponível quando a assinatura é um plano com renovação automática. Neste estado, todos os itens também estão pausados.
        /// </summary>
        OnHold,

        /// <summary>
        /// A assinatura foi cancelada, mas ainda não expirou. Este estado só fica disponível quando a assinatura é um plano com renovação automática. Todos os itens têm "autoRenewEnabled" definido como "false".
        /// </summary>
        Canceled,

        /// <summary>
        /// A assinatura expirou. O "expiryTime" de todos os itens está no passado.
        /// </summary>
        Expired,

        /// <summary>
        /// Uma transação pendente para a assinatura foi cancelada. Se essa compra pendente era para uma assinatura existente, use "linkedPurchaseToken" para receber o estado atual da assinatura.
        /// </summary>
        PendingPurchaseCanceled
    }

    public class SubscriptionValidationResponse // Vai ser usado para o IoS também
    {
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }

        public SubscriptionStatus StatusEnum => Status switch
        {
            "SUBSCRIPTION_STATE_PENDING" => SubscriptionStatus.Pending,
            "SUBSCRIPTION_STATE_ACTIVE" => SubscriptionStatus.Active,
            "SUBSCRIPTION_STATE_PAUSED" => SubscriptionStatus.Paused,
            "SUBSCRIPTION_STATE_IN_GRACE_PERIOD" => SubscriptionStatus.InGracePeriod,
            "SUBSCRIPTION_STATE_ON_HOLD" => SubscriptionStatus.OnHold,
            "SUBSCRIPTION_STATE_CANCELED" => SubscriptionStatus.Canceled,
            "SUBSCRIPTION_STATE_EXPIRED" => SubscriptionStatus.Expired,
            "SUBSCRIPTION_STATE_PENDING_PURCHASE_CANCELED" => SubscriptionStatus.PendingPurchaseCanceled,
            _ => SubscriptionStatus.Unspecified,
        };

        public bool IsSubscriptionValid => StatusEnum is SubscriptionStatus.Active or SubscriptionStatus.InGracePeriod;
    }
}
